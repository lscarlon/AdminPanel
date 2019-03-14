using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace AdminPanel.Attributes
{
    internal class CommandAuthorizeAttribute : AuthorizeAttribute
    {
        const string POLICY_PREFIX = "Command";
        public CommandAuthorizeAttribute(string command) => Command = command;
        // Get or set the Command property by manipulating the underlying Policy property
        public string Command
        {
            get
            {
                return Policy.Substring(POLICY_PREFIX.Length);
            }
            set
            {
                Policy = $"{POLICY_PREFIX}{value}";
            }
        }
    }

    internal class CommandPolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "Command";

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
            Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.RequireClaim("CommandAuthorize", policyName.Substring(POLICY_PREFIX.Length));
                return Task.FromResult(policy.Build());
            }

            return Task.FromResult<AuthorizationPolicy>(null);
        }
    }

    public static class CustomAuthorize
    {
        public static bool HasCommandClaim (this IPrincipal User, string Controller, string Action)
        {
            var claims = (ClaimsIdentity)User.Identity;

            AppDbContext db = Database.dbContext;
            string CommandName=db.Commands.FirstOrDefault(c => c.Controller == Controller && c.Action == Action).CommandName;

            return claims.HasClaim("CommandAuthorize", CommandName);
        }
    }
}
