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


namespace AdminPanel
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
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

            //services.Configure<IISServerOptions>(options =>
            //{
            //    options.AutomaticAuthentication = false;
            //    options.AuthenticationDisplayName = "AdminPanel";
            //});

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
            });

            services.ConfigureApplicationCookie(o => {
                o.Cookie.Expiration = TimeSpan.FromDays(150);
                o.LoginPath = "/Login/Login";
                o.LogoutPath = "/Login/LockScreen";
                o.AccessDeniedPath = "/Login/AccessDenied";
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

            services.AddSingleton<IControllerInformationRepository, ControllerInformationRepository>();

        }

        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/CustomErrors/Error");
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
                routes.MapRoute(
                    "E404-PageNotFound",
                    "{*url}",
                    new { controller = "CustomErrors", action = "E404" });
            });

            app.UseRewriter(new RewriteOptions().AddRedirectToHttps());
        }
    }
}
