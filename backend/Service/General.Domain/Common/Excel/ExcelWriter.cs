using OfficeOpenXml;
using System;
using System.IO;

namespace General.Domain.Common.Excel
{
    public class ExcelWriter : IDisposable
    {
        public static string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private static string Extension => "xlsx";
        protected internal readonly ExcelPackage Package;
        private readonly string _fileName;

        public string FileName => $"{_fileName}.{Extension}";

        /// <summary>
        /// Create a new Excel Writer with the specified filename
        /// </summary>
        /// <param name="fileName"></param>
        public ExcelWriter(string fileName)
        {
            Package = new ExcelPackage();
            _fileName = fileName;
        }

        /// <summary>
        /// Creates a new Excel Worksheet within the existing package with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ExcelSheet CreateWorksheet(string name)
        {
            return new ExcelSheet(Package.Workbook.Worksheets.Add(name));
        }

        /// <summary>
        /// Returns the package as a byte array
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes() => Package.GetAsByteArray();

        /// <summary>
        /// Returns the package as a memory stream
        /// </summary>
        /// <returns></returns>
        public MemoryStream GetStream()
        {
            var outputStream = new MemoryStream();
            Package.SaveAs(outputStream);
            return outputStream;
        }

        /// <summary>
        /// Creates a new Named Range at the WorkBook level
        /// </summary>
        /// <param name="rangeName"></param>
        /// <param name="sourceSheet"></param>
        /// <param name="cellAddress"></param>
        /// <returns></returns>
        public void CreateNamedRange(string rangeName, string sourceSheet, string cellAddress)
        {
            var range = Package.Workbook.Worksheets[sourceSheet].Cells[cellAddress];
            Package.Workbook.Names.Add(rangeName, range);
        }

        public void Dispose()
        {
            Package?.Dispose();
        }
    }
}
