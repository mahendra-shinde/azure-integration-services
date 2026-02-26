# .NET Function App and Test Setup

This document explains how the .NET Azure Function App and its test project were created using the `dotnet` CLI and Azure Functions Core Tools (`func` CLI).

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Azure Functions Core Tools](https://docs.microsoft.com/azure/azure-functions/functions-run-local)
- [Visual Studio Code](https://code.visualstudio.com/) (optional)

## Steps

### 1. Create a Function App

```bash
func init fn-test1 --worker-runtime dotnet
cd fn-test1
func new --name HttpExample --template "Http Trigger" 
```

### 2. Create a Test Project

```bash
cd .. 
dotnet new xunit -o fn-test1.Tests
cd fn-test1.Tests
dotnet add reference ../fn-test1/fn-test1.csproj
```

### 3. Add Test Code

- Implement your test cases in the `fn-test1.Tests` project, referencing the function app code.

```csharp
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
```


### 4. Run Tests

```bash
cd fn-test1.Tests
dotnet test
```

### 5. Run the Function App Locally

```bash
cd ../fn-test1
func start
```

## Summary

- The function app was scaffolded using `func init` and `func new`.
- Tests were set up with `dotnet new xunit` and linked to the function app.
- Use `dotnet test` to run tests and `func start` to run the app locally.