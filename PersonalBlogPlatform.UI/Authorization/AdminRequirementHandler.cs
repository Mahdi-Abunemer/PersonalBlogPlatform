using Microsoft.AspNetCore.Authorization;
using PersonalBlogPlatform.Core.Exceptions;

namespace PersonalBlogPlatform.UI.Authorization
{
    public class AdminRequirementHandler : AuthorizationHandler<AdminRequirement>
    {
        protected override Task HandleRequirementAsync(
          AuthorizationHandlerContext context,
          AdminRequirement requirement)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }
            else
            {
                throw new ForbiddenException("Admin role required");
            }

            return Task.CompletedTask;
        }
    }
}
