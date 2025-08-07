using Microsoft.AspNetCore.Mvc.Filters;

namespace PersonalBlogPlatform.API.Filters
{
    public class LoggingActionFilter : IActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;
        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.ActionDescriptor.RouteValues["controller"];
            var method = context.ActionDescriptor.RouteValues["action"];

            var sensitiveNames = new[] { "password", "token" };

            var parameters = context.ActionArguments
                .Where(kv =>
                    !sensitiveNames.Contains(kv.Key, StringComparer.OrdinalIgnoreCase))
                .Select(kv => $"{kv.Key}: {kv.Value}")
                .ToList();

            var parameterString = parameters.Count > 0
                ? string.Join(", ", parameters)
                : "(without parameters)";

            _logger.LogInformation("Executing {Controller}.{Action} with parameters: {Parameters}",
           controller, method, parameterString);
        }
    }
}
