using Excel.Enums;
using Excel.VM;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System.ComponentModel;
using System.Reflection;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace Excel.AppService
{
    public class ExcelAppService
    {
        static ExcelAppService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ExportData<T>(List<T> data) where T : class, new()
        {
            ExcelSheetAttribute excelSheetAttribute = typeof(T).GetCustomAttribute<ExcelSheetAttribute>();
            if (excelSheetAttribute == null)
            {
                excelSheetAttribute = new ExcelSheetAttribute();
            }

            Dictionary<string, IEnumerable<Attribute>> attributes = GetAttributes<T>();
            List<PropertyInfo> properties = GetProperties<T>();
            using ExcelPackage excelPackage = new ExcelPackage();
            ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add(excelSheetAttribute.Name);
            for (int i = 0; i < properties.Count; i++)
            {
                int row = 1;
                int col = i + 1;
                ExcelRange excelRange = excelWorksheet.Cells[row, col];
                PropertyInfo propertyInfo = properties[i];
                if (attributes.TryGetValue(propertyInfo.Name, out var value))
                {
                    foreach (Attribute item in value)
                    {
                        if (item is ExcelColumnAttribute excelColumnAttribute)
                        {
                            excelRange.Value = excelColumnAttribute.Name ?? propertyInfo.Name;
                        }
                        else if (item is ExcelHeaderStyleAttribute excelHeaderStyleAttribute)
                        {
                            excelRange.Style.Font.Color.SetColor(excelHeaderStyleAttribute.FontColor);
                            excelRange.Style.Font.Size = excelHeaderStyleAttribute.FontSize;
                        }
                    }
                }

                if (!excelSheetAttribute.AutoColumnWidth)
                {
                    excelWorksheet.Column(col).AutoFit(excelSheetAttribute.ColumnWidth);
                }
            }

            for (int j = 0; j < data.Count; j++)
            {
                for (int k = 0; k < properties.Count; k++)
                {
                    int num = j + 2;
                    int num2 = k + 1;
                    ExcelRange excelRange2 = excelWorksheet.Cells[num, num2];
                    PropertyInfo propertyInfo2 = properties[k];
                    object value2 = propertyInfo2.GetValue(data[j]);
                    if (value2 == null || string.IsNullOrEmpty(value2.ToString()) || !attributes.TryGetValue(propertyInfo2.Name, out var value3))
                    {
                        continue;
                    }

                    foreach (Attribute item2 in value3)
                    {
                        if (item2 is ExcelColumnAttribute excelColumnAttribute2)
                        {
                            switch (excelColumnAttribute2.Type)
                            {
                                case ExcelColumnEnum.常规:
                                    excelRange2.Value = value2;
                                    break;
                                case ExcelColumnEnum.超链接:
                                    excelRange2.Hyperlink = new Uri(value2.ToString());
                                    break;
                                case ExcelColumnEnum.本地图片:
                                    {
                                        string path = value2.ToString();
                                        if (File.Exists(path))
                                        {
                                            using Image image = Image.Load(path);
                                            using MemoryStream memoryStream = new MemoryStream();
                                            image.Save(memoryStream, new PngEncoder());
                                            SetPicture(excelWorksheet, num, num2, memoryStream);
                                        }
                                        else
                                        {
                                            excelRange2.Value = $"图片路径无效：{value2}";
                                        }

                                        break;
                                    }
                                case ExcelColumnEnum.网络图片:
                                    {
                                        if (Uri.TryCreate(value2.ToString(), UriKind.Absolute, out Uri result))
                                        {
                                            using HttpClient httpClient = new HttpClient();
                                            using MemoryStream ms = new MemoryStream(httpClient.GetByteArrayAsync(result).Result);
                                            SetPicture(excelWorksheet, num, num2, ms);
                                        }
                                        else
                                        {
                                            excelRange2.Value = $"图片地址无效：{value2}";
                                        }

                                        break;
                                    }
                                case ExcelColumnEnum.自动类型:
                                    if (propertyInfo2.PropertyType == typeof(string))
                                    {
                                        excelRange2.Value = value2.ToString();
                                    }
                                    else if (propertyInfo2.PropertyType == typeof(int) || propertyInfo2.PropertyType == typeof(int?))
                                    {
                                        excelRange2.Value = int.Parse(value2.ToString());
                                    }
                                    else if (propertyInfo2.PropertyType == typeof(long) || propertyInfo2.PropertyType == typeof(long?))
                                    {
                                        excelRange2.Value = long.Parse(value2.ToString());
                                    }
                                    else if (propertyInfo2.PropertyType == typeof(short) || propertyInfo2.PropertyType == typeof(short?))
                                    {
                                        excelRange2.Value = short.Parse(value2.ToString());
                                    }
                                    else if (propertyInfo2.PropertyType == typeof(decimal) || propertyInfo2.PropertyType == typeof(decimal?))
                                    {
                                        excelRange2.Value = decimal.Parse(value2.ToString());
                                    }
                                    else if (propertyInfo2.PropertyType == typeof(double) || propertyInfo2.PropertyType == typeof(double?))
                                    {
                                        excelRange2.Value = double.Parse(value2.ToString());
                                    }
                                    else if (propertyInfo2.PropertyType == typeof(float) || propertyInfo2.PropertyType == typeof(float?))
                                    {
                                        excelRange2.Value = float.Parse(value2.ToString());
                                    }
                                    else if (propertyInfo2.PropertyType == typeof(DateTime) || propertyInfo2.PropertyType == typeof(DateTime?))
                                    {
                                        excelRange2.Value = value2.ToString();
                                    }
                                    else
                                    {
                                        excelRange2.Value = value2;
                                    }

                                    break;
                            }
                        }
                        else if (item2 is ExcelColumnStyleAttribute excelColumnStyleAttribute)
                        {
                            excelRange2.Style.Font.Color.SetColor(excelColumnStyleAttribute.FontColor);
                            excelRange2.Style.Font.Size = excelColumnStyleAttribute.FontSize;
                        }
                    }
                }
            }

            if (excelSheetAttribute.AutoColumnWidth)
            {
                excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();
            }

            return excelPackage.GetAsByteArray();
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="sheetIndex"></param>
        /// <returns></returns>
        public static List<T> ImportData<T>(Stream stream, int sheetIndex = 0) where T : class, new()
        {
            using ExcelPackage excel = new ExcelPackage(stream);
            return ImportData<T>(excel, sheetIndex);
        }

        private static List<T> ImportData<T>(ExcelPackage excel, int sheetIndex) where T : class, new()
        {
            var source = (from a in typeof(T).GetProperties()
                          where a.GetCustomAttribute<ExcelColumnAttribute>() != null
                          select new
                          {
                              Property = a,
                              ColumnName = a.GetCustomAttribute<ExcelColumnAttribute>().Name
                          }).ToList();
            ExcelWorksheet excelWorksheet = excel.Workbook.Worksheets[sheetIndex];
            Dictionary<int, PropertyInfo> dictionary = new Dictionary<int, PropertyInfo>();
            for (int i = 1; i <= excelWorksheet.Dimension.Columns; i++)
            {
                ExcelRange cell = excelWorksheet.Cells[1, i];
                if (cell != null && cell.Value != null)
                {
                    var anon = source.FirstOrDefault(a => a.ColumnName == cell.Value.ToString());
                    if (anon != null)
                    {
                        dictionary.Add(i, anon.Property);
                    }
                }
            }

            List<T> list = new List<T>();
            for (int j = 2; j <= excelWorksheet.Dimension.Rows; j++)
            {
                T val = new T();
                for (int k = 1; k <= excelWorksheet.Dimension.Columns; k++)
                {
                    try
                    {
                        ExcelRange excelRange = excelWorksheet.Cells[j, k];
                        if (excelRange != null && excelRange.Value != null)
                        {
                            object value = excelRange.Value;
                            if (dictionary.TryGetValue(k, out var value2))
                            {
                                Type conversionType = Nullable.GetUnderlyingType(value2.PropertyType) ?? value2.PropertyType;
                                object value3 = Convert.ChangeType(value, conversionType);
                                value2.SetValue(val, value3);
                            }
                        }
                    }
                    catch (Exception innerException)
                    {
                        throw new Exception($"导入失败：第{j}行，第{k}列数据类型转换失败！", innerException);
                    }
                }

                list.Add(val);
            }

            return list;
        }


        private static List<PropertyInfo> GetProperties<T>() where T : class, new()
        {
            return (from a in typeof(T).GetProperties()
                    where a.GetCustomAttribute<ExcelColumnAttribute>() != null
                    orderby a.GetCustomAttribute<ExcelColumnAttribute>().Order
                    select a).ToList();
        }

        private static Dictionary<string, IEnumerable<Attribute>> GetAttributes<T>() where T : class, new()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            Dictionary<string, IEnumerable<Attribute>> dictionary = new Dictionary<string, IEnumerable<Attribute>>();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                IEnumerable<Attribute> customAttributes = propertyInfo.GetCustomAttributes();
                dictionary.Add(propertyInfo.Name, customAttributes);
            }

            return dictionary;
        }

        private static void SetPicture(ExcelWorksheet worksheet, int rowNum, int columnNum, Stream ms)
        {
            ExcelPicture excelPicture = worksheet.Drawings.AddPicture($"img_{DateTime.Now.Ticks}", ms);
            excelPicture.SetSize(20, 20);
            excelPicture.SetPosition(rowNum - 1, 0, columnNum - 1, 5);
        }
    }
}
