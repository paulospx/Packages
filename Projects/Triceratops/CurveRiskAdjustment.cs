namespace Triceratops
{
    public class CurveRiskAdjustment
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required string Currency { get; set; }
        public string? Description { get; set; }
        public string? CurveName { get; set; }
        public DateTime? From { get; set; }
        public DateTime? Until { get; set; }
        public double Value { get; set; }
    }
}
