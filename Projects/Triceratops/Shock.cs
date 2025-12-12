namespace Triceratops
{
    public class Shock
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string Type { get; set; }
        public double Size { get; set; }
        public DateTime From { get; set; }
        public DateTime Until { get; set; }

    }
}
