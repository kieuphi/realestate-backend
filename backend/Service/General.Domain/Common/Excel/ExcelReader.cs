using ExcelDataReader;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace General.Domain.Common.Excel
{
    public static class ExcelReader
    {
        /// <summary>
        /// Reads the first sheet of an Excel file from a stream, and writes it into a List of rows with a List of columns
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="includesHeader"></param>
        /// <returns></returns>
        public static List<List<string>> ReadExcelWorksheet(Stream stream, bool includesHeader = true, bool subHeader = false)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
            DataTableCollection dataTables = reader.AsDataSet().Tables;

            var rows = new List<List<string>>();
            if (dataTables.Count == 0) return rows;

            DataTable table = dataTables[0];
            for (int rowIndex = includesHeader ? (subHeader ? 2 : 1) : 0; rowIndex < table.Rows.Count; rowIndex++)
            {
                //IF NULL THEN CONTINUE
                //if (string.IsNullOrWhiteSpace(table.Rows[rowIndex][0].ToString()))
                //{
                //    continue;
                //}

                //VALID DATA
                var row = new List<string>();
                for (var columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                {
                    row.Add(table.Rows[rowIndex][columnIndex].ToString());
                }

                rows.Add(row);
            }

            return rows;
        }

        /// <summary>
        /// Reads an entire Excel file from a stream, and writes it into a List of Worksheets with SheetName and List rows with a List of columns
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="includesHeader"></param>
        /// <returns></returns>
        public static List<(string, List<List<string>>)> ReadExcelWorkbook(Stream stream, bool includesHeader = true)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
            DataTableCollection dataTables = reader.AsDataSet().Tables;

            var sheets = new List<(string, List<List<string>>)>();
            if (dataTables.Count == 0) return sheets;

            foreach (DataTable table in dataTables)
            {
                var rows = new List<List<string>>();

                for (int rowIndex = includesHeader ? 1 : 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    var row = new List<string>();
                    for (var columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                    {
                        row.Add(table.Rows[rowIndex][columnIndex].ToString());
                    }

                    rows.Add(row);
                }

                sheets.Add((table.TableName, rows));
            }

            return sheets;
        } 
    }
}
