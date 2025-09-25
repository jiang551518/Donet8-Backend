using Excel.AppService;
using Excel.EfCoreDb;
using Excel.IService;
using Excel.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Excel.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ExcelController : ControllerBase
    {
        private readonly IRabbitMqService _rabbitMqService;
        public ExcelController(IRabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        /// <summary>
        /// excel导出（ExcelVM是excel的导出模板，参考这个可以自定义导出模板）
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost(nameof(ExportData))]
        public async Task<FileStreamResult> ExportData([FromBody] List<ExcelVM> vm)
        {
            var userId = User.FindFirst("uid")?.Value;

            var stream = new MemoryStream(ExcelAppService.ExportData(vm));

            // 构造通知 DTO
            var notification = new
            {
                UserId = userId,
                FileName = "测试导入.xlsx",
                Timestamp = DateTime.UtcNow
            };
            await _rabbitMqService.PublishAsync(notification, "excel.export.completed");

            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "测试导入.xlsx"
            };
        }

        /// <summary>
        /// excel导入（ExcelVM是excel的导出模板，参考这个可以自定义导出模板）
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost(nameof(ImportData))]
        public List<ExcelVM> ImportData(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var data = ExcelAppService.ImportData<ExcelVM>(stream);
            return data;
        }
    }
}
