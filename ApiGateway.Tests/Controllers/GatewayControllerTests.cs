using ApiGateway.Controllers;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.AspNetCore.Mvc;
namespace ApiGateway.Tests.Controllers
{
    public class GatewayControllerTests
    {
        [Fact]
        public async Task UploadFile_ReturnsResult()
        {
            // Arrange
            var mockFactory = new Mock<IHttpClientFactory>();
            var handler = new FakeHttpMessageHandler();
            var mockClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost")
            };
            mockFactory.Setup(f => f.CreateClient("FileStore")).Returns(mockClient);

            var controller = new GatewayController(mockFactory.Object);
            var fileMock = new Mock<IFormFile>();
            var contentBytes = "content"u8.ToArray();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(contentBytes));
            fileMock.Setup(f => f.Length).Returns(contentBytes.Length);

            // Act
            var result = await controller.UploadFile(fileMock.Object);

            // Assert
            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objResult.StatusCode);
            Assert.Contains("fileId", objResult.Value?.ToString());
        }

        private class FakeHttpMessageHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        "{\"fileId\":\"00000000-0000-0000-0000-000000000000\",\"fileName\":\"test.txt\"}")
                };
                return Task.FromResult(resp);
            }
        }
    }
}
