namespace Excel.VM
{
    /// <summary>
    /// Excel表格设置
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExcelSheetAttribute : Attribute
    {
        /// <summary>
        /// 默认值sheet1
        /// </summary>
        public string Name { get; set; } = "sheet1";


        /// <summary>
        /// 列宽：默认值15
        /// </summary>
        public int ColumnWidth { get; set; } = 15;


        /// <summary>
        ///  自动列宽：默认值false，设置为true时，将根据内容自动调整列宽，固定列宽会失效
        /// </summary>
        public bool AutoColumnWidth { get; set; }
    }
}
