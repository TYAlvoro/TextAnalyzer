namespace Shared.Models;

public class AnalysisResult
{
    public Guid FileId { get; set; }
    public int Paragraphs { get; set; }
    public int Words { get; set; }
    public int Characters { get; set; }
}