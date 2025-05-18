using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace FileAnalysisService.Controllers;

[ApiController]
[Route("api/analysis")]
public class AnalysisController : ControllerBase
{
    private static readonly string StoragePath = Path.Combine(AppContext.BaseDirectory, "Files");

    [HttpPost]
    public async Task<IActionResult> Analyze([FromBody] AnalysisRequest req)
    {
        var filePath = Path.Combine(StoragePath, $"{req.FileId}.txt");
        if (!System.IO.File.Exists(filePath))
            return NotFound("File not found.");

        var text = await System.IO.File.ReadAllTextAsync(filePath);

        var paragraphs = text.Split(["\r\n\r\n", "\n\n"], StringSplitOptions.RemoveEmptyEntries).Length;
        var words = text.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries).Length;
        var chars = text.Length;

        var result = new AnalysisResult
        {
            FileId = req.FileId,
            Paragraphs = paragraphs,
            Words = words,
            Characters = chars
        };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        return NotFound("Not implemented");
    }
}