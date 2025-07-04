using Excel.AppService;
using Excel.VM;
using Microsoft.AspNetCore.Mvc;

namespace Excel.Controllers
{
    [Route("api/[controller]")]
    public class TestController
    {
        public TestController()
        {
        }

        /// <summary>
        /// 测试导出
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpGet(nameof(ExportData))]
        public FileStreamResult ExportData([FromBody] List<TestExcelVM> vm)
        {
            MemoryStream stream;
            stream = new MemoryStream(ExcelUtil.ExportData(vm));
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
        public List<TestExcelVM> ImportData(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var data = ExcelUtil.ImportData<TestExcelVM>(stream);
            return data;
        }
    }
}
