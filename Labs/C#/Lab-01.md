# Creating HTTP-triggered functions with dependency injection (C#)

## Overview

In this lab, you will create an Azure Function in C# that is triggered by HTTP requests. You will also learn how to use dependency injection to manage services within your function.

## Prerequisites

- .NET 8 SDK
- [Azure Functions Core Tools](https://docs.microsoft.com/azure/azure-functions/functions-run-local)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)
- An Azure subscription


## Step 1: Create a new Azure Functions project

```sh
func init lab01  --worker-runtime dotnet
cd lab01
dotnet add package Microsoft.Azure.Functions.Extensions  
```

## Step 2: Add a service class

Create `GreetingService.cs`:

```csharp
public class GreetingService
{
    public string Greet(string name) => $"Hello, {name}!";
}
```

## Step 3: Register the service for dependency injection

Create `Startup.cs` (Overwrite if present):

```csharp
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton<GreetingService>();
    }
}
```

## Step 4: Update the Function to use dependency injection

Edit `HttpExample.cs`:

```csharp
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class HttpExample
{
    private readonly GreetingService _greetingService;

    public HttpExample(GreetingService greetingService)
    {
        _greetingService = greetingService;
    }

    [FunctionName("HttpExample")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        string name = req.Query["name"];
        if (string.IsNullOrEmpty(name))
        {
            name = "world";
        }
        string responseMessage = _greetingService.Greet(name);
        return new OkObjectResult(responseMessage);
    }
}
```

---

## Step 5: Run and test the function locally

```sh
func start
```

Test with:

```sh
curl http://localhost:7071/api/HttpExample?name=Azure
```

---

## Step 6: Deploy to Azure

```sh
## Replace the variable values 
$RG_NAME="csharp-func-lab-rg"
$STORAGE_NAME="mahen0101xyz"
##

az group create --name $RG_NAME --location westus

az storage account create -n $STORAGE_NAME -l westus -g $RG_NAME

az functionapp create --resource-group $RG_NAME --consumption-plan-location westus --runtime dotnet --functions-version 4 --name httpfun1 --storage-account $STORAGE_NAME
func azure functionapp publish httpfun1
```
