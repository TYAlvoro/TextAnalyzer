namespace Shared.Models;

public class FileUploadResponse
{
    public Guid FileId { get; set; }
    public string FileName { get; set; } = null!;
}