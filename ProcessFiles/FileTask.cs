namespace ConsoleApp3
{
    public class FileTask
    {
        public int FileId { get; set; }
        public required string Name { get; set; }
        public required string Path { get; set; }
        public long Size { get; set; }
        public string? Status { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
    }
}
