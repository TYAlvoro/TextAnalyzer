using FileAnalysisService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
namespace FileAnalysisService.Tests.Controllers
{
    public class AnalysisControllerTests
    {
        [Fact]
        public async Task Analyze_ReturnsNotFound_WhenFileDoesNotExist()
        {
            var controller = new AnalysisController();
            var req = new AnalysisRequest { FileId = Guid.NewGuid() };

            var result = await controller.Analyze(req);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("File not found.", notFound.Value);
        }

        [Fact]
        public async Task Analyze_ReturnsCorrectStats_WhenFileExists()
        {
            var testId = Guid.NewGuid();
            var storagePath = Path.Combine(AppContext.BaseDirectory, "Files");
            Directory.CreateDirectory(storagePath);

            const string content = "Hello world!\n\nNew paragraph.\nHello again.";
            var filePath = Path.Combine(storagePath, $"{testId}.txt");
            await File.WriteAllTextAsync(filePath, content);

            var controller = new AnalysisController();
            var req = new AnalysisRequest { FileId = testId };

            var result = await controller.Analyze(req);

            var ok = Assert.IsType<OkObjectResult>(result);
            var analysis = Assert.IsType<AnalysisResult>(ok.Value);
            Assert.Equal(testId, analysis.FileId);
            Assert.Equal(2, analysis.Paragraphs);
            Assert.Equal(6, analysis.Words);
            Assert.Equal(content.Length, analysis.Characters);

            // Clean up
            File.Delete(filePath);
        }
    }
}