using System.IO.Compression;
using Serilog;
using OfficeOpenXml;

namespace RaptorUtilLib
{
    public static class Utils
    {
        public static void UnzipFile(string zipFilePath, string extractPath)
        {
            try
            {
                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }
                ZipFile.ExtractToDirectory(zipFilePath, extractPath, true);
                Log.Information($"Unzipped file: {zipFilePath} to {extractPath}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error unzipping file: {zipFilePath} to {extractPath}.",ex);
            }
        }


        public static void MoveFile(string sourceFilePath, string destinationFilePath)
        {
            try
            {
                string targetPath = destinationFilePath;
                if (File.Exists(destinationFilePath))
                {
                    string directory = Path.GetDirectoryName(destinationFilePath)!;
                    string filename = Path.GetFileNameWithoutExtension(destinationFilePath);
                    string extension = Path.GetExtension(destinationFilePath);
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    targetPath = Path.Combine(directory, $"{filename}_{timestamp}{extension}");
                }

                File.Move(sourceFilePath, targetPath);
                Log.Information($"File moved from {sourceFilePath} to {targetPath}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error moving file from {sourceFilePath} to {destinationFilePath}", ex);
                throw new Exception($"Error moving file: {ex.Message}", ex);
            }
        }

        public static void UpdateExcelCell(string excelFilePath, string rangeAddress, object value)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(new FileInfo(excelFilePath));
                var worksheet = package.Workbook.Worksheets[0];
                var range = worksheet.Cells[rangeAddress];
                range.Value = value;
                package.Save();
                Log.Information($"Updated Excel file '{excelFilePath}' at range '{rangeAddress}' with value '{value}'");
            }
            catch (Exception ex)
            {
                Log.Error($"Error updating Excel file: {excelFilePath} at range: {rangeAddress} with value: {value}", ex);
                throw new Exception($"Error updating Excel file: {ex.Message}", ex);
            }
        }
    }
}
