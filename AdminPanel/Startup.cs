using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AdminPanel.Models;
using AdminPanel.Identity;
using AdminPanel.Attributes;
using AdminPanel.Common;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AdminPanel
{
    public class Startup
    {
        private const string ExceptionsOnStartup = "Startup";
        private const string ExceptionsOnConfigureServices = "ConfigureServices";

        private readonly Dictionary<string, List<Exception>> _exceptions;

        public Startup(IHostingEnvironment env)
        {
            this._exceptions = new Dictionary<string, List<Exception>>
                           {
                             { ExceptionsOnStartup, new List<Exception>() },
                             { ExceptionsOnConfigureServices, new List<Exception>() },
                           };
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables()
                    .AddUserSecrets<Startup>();
                Configuration = builder.Build();
            }
            catch (Exception ex)
            {
                this._exceptions[ExceptionsOnStartup].Add(ex);
            }
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMemoryCache();

                services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();
                });

                services.Configure<GzipCompressionProviderOptions>
                    (options => options.Level = CompressionLevel.Fastest);
                services.AddResponseCompression(options =>
                {
                    options.Providers.Add<GzipCompressionProvider>();
                });

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

                services.AddIdentity<User, Role>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

                services.AddAuthentication().AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });

                services.Configure<IdentityOptions>(o =>
                {
                    // Password Settings
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequiredLength = 6;

                    // Lockout settings
                    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                    o.Lockout.MaxFailedAccessAttempts = 10;
                    o.Lockout.AllowedForNewUsers = true;

                    // User settings
                    o.User.RequireUniqueEmail = true;
                    o.SignIn.RequireConfirmedEmail = true;
                });

                services.ConfigureApplicationCookie(o =>
                {
                    o.Cookie.Expiration = TimeSpan.FromDays(150);
                    o.LoginPath = "/Login/Login";
                    o.LogoutPath = "/Login/LockScreen";
                    o.AccessDeniedPath = "/Error/403";
                    o.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToAccessDenied = context =>
                        {
                            context.Response.StatusCode = 403;
                            return Task.FromResult(0);
                        },
                        OnRedirectToLogin = context =>
                        {
                            context.Response.StatusCode = 401;
                            //context.Response.Redirect(context.RedirectUri);
                            return Task.FromResult(0);
                        }
                    };
                });

                services.AddMvc(config =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                });

                services.Configure<MvcOptions>(options =>
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                });

                services.AddDistributedMemoryCache();
                services.AddSession();

                services.AddSingleton<IAuthorizationPolicyProvider, CommandPolicyProvider>();

                services.AddSingleton<IEmailSettings>(Configuration.GetSection("EmailSettings").Get<EmailSettings>());
                services.AddTransient<IEmailService, EmailService>();
            }
            catch (Exception ex)
            {
                this._exceptions[ExceptionsOnConfigureServices].Add(ex);
            }
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (this._exceptions.Any(p => p.Value.Any()))
            {
                app.Run(
                  async context =>
                  {
                      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                      context.Response.ContentType = "text/plain";

                      foreach (var ex in this._exceptions)
                      {
                          foreach (var val in ex.Value)
                          {
                              //log.LogError($"{ex.Key}:::{val.Message}");
                              await context.Response.WriteAsync($"Error on {ex.Key}: {val.Message}").ConfigureAwait(false);
                          }
                      }
                  });
                return;
            }


            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseBrowserLink();
                }
                else
                {
                    app.UseExceptionHandler("/Error/E500");
                    app.UseStatusCodePagesWithReExecute("/Error/{0}");
                }

                app.UseAuthentication();

                app.UseStaticFiles();

                app.UseResponseCompression();

                app.UseSession();

                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Default}/{id?}");
                });

                app.UseRewriter(new RewriteOptions().AddRedirectToHttps());

                //Inizializzo la classe statica da usare per accesso al DB
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                Database.InizializeDbContext(optionsBuilder);

                StartupMethods.RunAll();
            }
            catch (Exception ex)
            {
                app.Run(
                    async context =>
                    {
                        //log.LogError($"{ex.Message}");

                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync(ex.Message).ConfigureAwait(false);
                        await context.Response.WriteAsync(ex.StackTrace).ConfigureAwait(false);
                    });
            }
        }
    }
}
