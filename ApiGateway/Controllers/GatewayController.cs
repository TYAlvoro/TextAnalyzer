using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/gateway")]
public class GatewayController(IHttpClientFactory httpFactory) : ControllerBase
{
    [HttpPost("files")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
    {
        var client = httpFactory.CreateClient("FileStore");
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(file.OpenReadStream());
        content.Add(fileContent, "file", file.FileName);

        var resp = await client.PostAsync("/api/files", content);
        var respBody = await resp.Content.ReadAsStringAsync();
        return StatusCode((int)resp.StatusCode, respBody);
    }

    [HttpGet("files/{id}")]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var client = httpFactory.CreateClient("FileStore");
        var resp = await client.GetAsync($"/api/files/{id}");
        var data = await resp.Content.ReadAsByteArrayAsync();
        return File(data, "text/plain", $"{id}.txt");
    }

    [HttpPost("analysis")]
    public async Task<IActionResult> AnalyzeFile([FromBody] AnalysisRequest req)
    {
        var client = httpFactory.CreateClient("FileAnalysis");
        var resp = await client.PostAsJsonAsync("/api/analysis", req);
        var respBody = await resp.Content.ReadAsStringAsync();
        return StatusCode((int)resp.StatusCode, respBody);
    }
}