using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace FileStoreService.Controllers;

[ApiController]
[Route("api/files")]
public class FileController : ControllerBase
{
    private static readonly string StoragePath = Path.Combine(AppContext.BaseDirectory, "Files");

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided.");

        Directory.CreateDirectory(StoragePath);
        var id = Guid.NewGuid();
        var filePath = Path.Combine(StoragePath, $"{id}.txt");
        await using (var stream = System.IO.File.Create(filePath))
            await file.CopyToAsync(stream);

        return Ok(new FileUploadResponse { FileId = id, FileName = file.FileName });
    }

    [HttpGet("{id}")]
    public IActionResult Download(Guid id)
    {
        var filePath = Path.Combine(StoragePath, $"{id}.txt");
        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, "text/plain", $"{id}.txt");
    }
}