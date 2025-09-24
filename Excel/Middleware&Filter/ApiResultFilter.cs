using Excel.VM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Excel.TateFilter
{
    public class ApiResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            // 已经是ApiResult的就不包了
            if (context.Result is ObjectResult objResult && objResult.Value is not ApiResult)
            {
                var apiResult = new ApiResult
                {
                    Code = 0,
                    Msg = "success",
                    Data = objResult.Value
                };
                context.Result = new ObjectResult(apiResult)
                {
                    StatusCode = objResult.StatusCode
                };
            }
            await next();
        }
    }
}
