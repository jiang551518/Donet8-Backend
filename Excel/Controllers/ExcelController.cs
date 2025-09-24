using Excel.AppService;
using Excel.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Excel.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ExcelController
    {
        public ExcelController()
        {
        }

        /// <summary>
        /// 测试导出
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpGet(nameof(ExportData))]
        public FileStreamResult ExportData([FromBody] List<ExcelVM> vm)
        {
            MemoryStream stream;
            stream = new MemoryStream(ExcelAppService.ExportData(vm));
            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "测试导入.xlsx"
            };
        }

        /// <summary>
        /// 测试导入
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
