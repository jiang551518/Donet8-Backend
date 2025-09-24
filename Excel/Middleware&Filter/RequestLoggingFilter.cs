using Microsoft.AspNetCore.Mvc.Filters;

namespace Excel.Middleware_Filter
{
    public class RequestLoggingFilter : IAsyncActionFilter
    {
        private readonly ILogger<RequestLoggingFilter> _logger;
        public RequestLoggingFilter(ILogger<RequestLoggingFilter> logger) => _logger = logger;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var args = context.ActionArguments;
            if (args.Any())
            {
                _logger.LogInformation(
                    "请求 {Method} {Path} 入参：{@Arguments}",
                    context.HttpContext.Request.Method,
                    context.HttpContext.Request.Path,
                    args
                );
            }
            await next();
        }
    }

}
