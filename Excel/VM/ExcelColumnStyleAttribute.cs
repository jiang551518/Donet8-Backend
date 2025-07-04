using System.Drawing;

namespace Excel.VM
{
    /// <summary>
    /// Excel单元格数据列设置
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelColumnStyleAttribute : Attribute
    {
        /// <summary>
        /// 字体颜色
        /// </summary>
        public Color FontColor { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize { get; set; }

        public ExcelColumnStyleAttribute(int r = 0, int g = 0, int b = 0, int fontSize = 11)
        {
            FontColor = Color.FromArgb(r, g, b);
            FontSize = fontSize;
        }
    }
}
