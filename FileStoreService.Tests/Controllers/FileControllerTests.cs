using FileStoreService.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Moq;

namespace FileStoreService.Tests.Controllers
{
    public class FileControllerTests
    {
        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenNoFileProvided()
        {
            // Arrange
            var controller = new FileController();

            // Act
            var result = await controller.Upload(null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file provided.", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsOk_WithValidFile()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            const string content = "test file content";
            const string fileName = "test.txt";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            await writer.WriteAsync(content);
            await writer.FlushAsync();
            ms.Position = 0;
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);

            var controller = new FileController();

            // Act
            var result = await controller.Upload(fileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<FileUploadResponse>(okResult.Value);
            Assert.False(response.FileId == Guid.Empty);
            Assert.Equal(fileName, response.FileName);

            var filePath = Path.Combine(AppContext.BaseDirectory, "Files", $"{response.FileId}.txt");
            Assert.True(File.Exists(filePath));
            var fileContent = await File.ReadAllTextAsync(filePath);
            Assert.Equal(content, fileContent);
            File.Delete(filePath);
        }

        [Fact]
        public Task Download_ReturnsNotFound_WhenFileDoesNotExist()
        {
            // Arrange
            var controller = new FileController();

            // Act
            var result = controller.Download(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Download_ReturnsFile_WhenFileExists()
        {
            // Arrange
            var controller = new FileController();
            var testId = Guid.NewGuid();
            var testContent = "hello";
            var storagePath = Path.Combine(AppContext.BaseDirectory, "Files");
            Directory.CreateDirectory(storagePath);
            var filePath = Path.Combine(storagePath, $"{testId}.txt");
            await File.WriteAllTextAsync(filePath, testContent);

            // Act
            var result = controller.Download(testId);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/plain", fileResult.ContentType);
            Assert.Equal($"{testId}.txt", fileResult.FileDownloadName);
            Assert.Equal(System.Text.Encoding.UTF8.GetBytes(testContent), fileResult.FileContents);

            // Чистим тестовый файл
            File.Delete(filePath);
        }
    }
}
