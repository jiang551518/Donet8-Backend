using Excel.Enums;

namespace Excel.VM
{
    /// <summary>
    ///  Excel单元格设置
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelColumnAttribute : Attribute
    {

        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 单元格类型：常规、超链接、本地图片、数值、小数
        /// </summary>
        public ExcelColumnEnum Type { get; set; }

        /// <summary>
        /// 列排序：默认按属性的上下抒写顺序
        /// </summary>
        public int Order { get; set; } = 999999999;


        /// <summary>
        /// 初始化单元格设置
        /// </summary>
        /// <param name="name"></param>
        public ExcelColumnAttribute(string name)
        {
            Name = name;
        }
    }
}
