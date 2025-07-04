using Excel.Enums;

namespace Excel.VM
{
    public class TestExcelVM
    {
        /// <summary>
        /// 用户名
        ///</summary>
        [ExcelColumn("用户名", Order = 1, Type = ExcelColumnEnum.自动类型)]
        public String user_name { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [ExcelColumn("真实姓名", Order = 2, Type = ExcelColumnEnum.自动类型)]
        public String real_name { get; set; }
    }
}
