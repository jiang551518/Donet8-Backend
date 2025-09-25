using Excel.EfCoreDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Excel.TateFilter
{
    public class UserStateFilter : IAsyncActionFilter
    {
        public UserStateFilter()
        {
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(meta => meta is AllowAnonymousAttribute);
            if (allowAnonymous)
            {
                await next();
                return;
            }

            var user = new Users()
            {
                id = Guid.Parse("fd008695-fdae-46d8-a630-88086e3eb048"),
                username = "admin",
                password = "123456",
                isenabled = true,
            };

            var users = new List<Users>();
            users.Add(user);

            var uid = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;
            if (string.IsNullOrEmpty(uid))
            {
                context.Result = new UnauthorizedObjectResult("Token无效");
                return;
            }

            var userDetail = users.Where(x => x.id == Guid.Parse(uid)).FirstOrDefault();
            if (user == null)
            {
                context.Result = new UnauthorizedObjectResult("用户已被删除");
                return;
            }
            if (!user.isenabled)
            {
                context.Result = new ForbidResult("用户已禁用");
                return;
            }


            await next();
        }
    }

}
