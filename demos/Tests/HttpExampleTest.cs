using Xunit;
using System.Net.Http;
using Moq;
using HttpExample = fn_test1.HttpExample;

public class HttpExampleTest
{
    [Fact]
    public void TestHttpClient()
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<HttpExample>>();
        var httpExample = new fn_test1.HttpExample(loggerMock.Object);
        var httpRequestMock = new Mock<Microsoft.AspNetCore.Http.HttpRequest>();
        var result = httpExample.Run(httpRequestMock.Object);
        Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var okResult = result as Microsoft.AspNetCore.Mvc.OkObjectResult;
        Assert.Equal("Welcome to Azure Functions!", okResult.Value);
    }
}
