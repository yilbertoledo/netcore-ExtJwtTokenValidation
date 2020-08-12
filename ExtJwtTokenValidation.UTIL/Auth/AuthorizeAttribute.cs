using ExtJwtTokenValidation.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace ExtJwtTokenValidation.Util.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// List of groups or roles comma separated
        /// </summary>
        public string Groups { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (UserDto)context.HttpContext.Items["User"];
            if (user == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            else if (!string.IsNullOrEmpty(Groups))
            {
                var gruopsToCheck = Groups.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(g => g.Trim().ToLower())
                    .Where(g => !string.IsNullOrWhiteSpace(g));

                if (!user.Groups.Any(g => gruopsToCheck.Contains(g.Trim().ToLower())))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
        }
    }
}