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
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockClient = new HttpClient(new FakeHttpMessageHandler());
            mockFactory.Setup(f => f.CreateClient("FileStore")).Returns(mockClient);

            var controller = new GatewayController(mockFactory.Object);
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream("content"u8.ToArray()));
            fileMock.Setup(f => f.Length).Returns(7);

            var result = await controller.UploadFile(fileMock.Object);

            Assert.IsType<ObjectResult>(result);
        }

        private class FakeHttpMessageHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"fileId\": \"00000000-0000-0000-0000-000000000000\", \"fileName\": \"test.txt\"}")
                };
                return Task.FromResult(resp);
            }
        }
    }
}