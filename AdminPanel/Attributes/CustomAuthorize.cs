using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
}
