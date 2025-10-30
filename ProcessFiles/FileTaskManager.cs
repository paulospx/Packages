namespace ConsoleApp3
{
    public static class FileTaskManager
    {
        public static FileTask ProcessFile(FileTask file)
        {
            // Simulate work here (IO, transformations, uploads, etc.)
            // Update metadata after successful processing

            file.Status = "Completed";
            file.LastModifiedAt = DateTime.Now;
            file.Comment = string.IsNullOrWhiteSpace(file.Comment)
                ? $"Processed on {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
                : file.Comment + $" | Processed on {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            return file;
        }

        public static List<FileTask> GetFilesToProcess(IEnumerable<FileTask>? tasks, DateTime? date = null)
        {
            if (tasks is null)
            {
                return new List<FileTask>();
            }

            var targetDate = (date ?? DateTime.Today).Date;

            return tasks
                .Where(t => t.CreatedAt.Date == targetDate
                            && !string.Equals(t.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Generate a list of all dates in the specified year formatted as "yyyyMMdd".
        /// </summary>
        /// <param name="year">The year for which to generate date stamps (1..9999).</param>
        /// <returns>List of date strings formatted as "yyyyMMdd".</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if year is outside 1..9999.</exception>
        public static List<string> GenerateDateTimestamps(int year)
        {
            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(year), $"Year must be between {DateTime.MinValue.Year} and {DateTime.MaxValue.Year}.");
            }

            var start = new DateTime(year, 1, 1);
            var end = new DateTime(year, 12, 31);

            var result = new List<string>((end - start).Days + 1);

            for (var current = start; current <= end; current = current.AddDays(1))
            {
                result.Add(current.ToString("yyyyMMdd"));
            }

            return result;
        }


        public static List<string> GenerateFilenames(IEnumerable<string> templateNames)
        {
            var result = new List<string>();
            var yearlyTimestamps = GenerateDateTimestamps(DateTime.Today.Year);
            foreach (var templateName in templateNames)
            {
                foreach (var dateStamp in yearlyTimestamps)
                {
                    result.Add(templateName.Replace("yyyyMMdd", dateStamp));
                }
            }
            return result;
        }


        public static void ExportFileTasksToCsv(IEnumerable<FileTask> tasks, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("FileId,Name,Path,Size,Status,Comment,CreatedAt,LastModifiedAt");
            foreach (var task in tasks)
            {
                var line = $"{task.FileId},{task.Name},{task.Path},{task.Size},{task.Status},{task.Comment},{task.CreatedAt:O},{task.LastModifiedAt:O}";
                writer.WriteLine(line);
            }
        }

    }
}
