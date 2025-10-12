using System.Data;
using OfficeOpenXml;
using System.Text;

namespace RaptorUtilLib
{
    public static class ExcelManager
    {
        public static void RunExcelMacros()
        {
            // Implementation for running Excel macros
        }

        public static void UpdateExcelFileCells(string filename, string cells, string value)
        {
            if (string.IsNullOrEmpty(filename)) {
                throw new ArgumentException("Filename cannot be null or empty", nameof(filename));
            }
        }

        /// <summary>
        /// Reads a specified range from an Excel sheet into a DataTable.
        /// </summary>
        /// <param name="filename">Path to the Excel file.</param>
        /// <param name="sheetName">Name of the worksheet.</param>
        /// <param name="rangeAddress">Excel range address (e.g., "A1:C10").</param>
        /// <returns>DataTable containing the range data.</returns>
        public static DataTable ReadExcelRangeToDataTable(string filename, string sheetName, string rangeAddress)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("Filename cannot be null or empty", nameof(filename));
            if (string.IsNullOrEmpty(sheetName))
                throw new ArgumentException("Sheet name cannot be null or empty", nameof(sheetName));
            if (string.IsNullOrEmpty(rangeAddress))
                throw new ArgumentException("Range address cannot be null or empty", nameof(rangeAddress));

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filename));
            var worksheet = package.Workbook.Worksheets[sheetName];
            if (worksheet == null)
                throw new ArgumentException($"Sheet '{sheetName}' not found in file '{filename}'.");

            var range = worksheet.Cells[rangeAddress];
            var dataTable = new DataTable();

            // Add columns
            for (int col = range.Start.Column; col <= range.End.Column; col++)
            {
                var columnName = worksheet.Cells[range.Start.Row, col].Text;
                if (string.IsNullOrWhiteSpace(columnName))
                    columnName = $"Column{col}";
                dataTable.Columns.Add(columnName);
            }

            // Add rows
            for (int row = range.Start.Row + 1; row <= range.End.Row; row++)
            {
                var dataRow = dataTable.NewRow();
                for (int col = range.Start.Column; col <= range.End.Column; col++)
                {
                    dataRow[col - range.Start.Column] = worksheet.Cells[row, col].Text;
                }
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        /// <summary>
        /// Exports a DataTable to a CSV file.
        /// </summary>
        /// <param name="dataTable">The DataTable to export.</param>
        /// <param name="csvFilePath">The path to the CSV file to create.</param>
        /// <param name="delimiter">The delimiter to use (default is comma).</param>
        public static void ExportDataTableToCsv(DataTable dataTable, string csvFilePath, char delimiter = ',')
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));
            if (string.IsNullOrEmpty(csvFilePath))
                throw new ArgumentException("CSV file path cannot be null or empty", nameof(csvFilePath));

            using var writer = new StreamWriter(csvFilePath, false, Encoding.UTF8);

            // Write header
            var columnNames = dataTable.Columns.Cast<DataColumn>()
                .Select(column => EscapeCsvValue(column.ColumnName, delimiter));
            writer.WriteLine(string.Join(delimiter, columnNames));

            // Write rows
            foreach (DataRow row in dataTable.Rows)
            {
                var fields = row.ItemArray.Select(field => EscapeCsvValue(field?.ToString() ?? string.Empty, delimiter));
                writer.WriteLine(string.Join(delimiter, fields));
            }
        }

        private static string EscapeCsvValue(string value, char delimiter)
        {
            if (value.Contains(delimiter) || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }
            return value;
        }

        /// <summary>
        /// Writes a DataTable to a specified range in an Excel worksheet.
        /// </summary>
        /// <param name="filename">Path to the Excel file.</param>
        /// <param name="sheetName">Name of the worksheet.</param>
        /// <param name="startCellAddress">Top-left cell address for the range (e.g., "A1").</param>
        /// <param name="dataTable">The DataTable to write.</param>
        public static void WriteDataTableToExcelRange(string filename, string sheetName, string startCellAddress, DataTable dataTable)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("Filename cannot be null or empty", nameof(filename));
            if (string.IsNullOrEmpty(sheetName))
                throw new ArgumentException("Sheet name cannot be null or empty", nameof(sheetName));
            if (string.IsNullOrEmpty(startCellAddress))
                throw new ArgumentException("Start cell address cannot be null or empty", nameof(startCellAddress));
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filename));
            var worksheet = package.Workbook.Worksheets[sheetName];
            if (worksheet == null)
                throw new ArgumentException($"Sheet '{sheetName}' not found in file '{filename}'.");

            var startCell = worksheet.Cells[startCellAddress];
            int startRow = startCell.Start.Row;
            int startCol = startCell.Start.Column;

            // Write column headers
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                worksheet.Cells[startRow, startCol + col].Value = dataTable.Columns[col].ColumnName;
            }

            // Write data rows
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    worksheet.Cells[startRow + 1 + row, startCol + col].Value = dataTable.Rows[row][col];
                }
            }

            package.Save();
        }
    }
}