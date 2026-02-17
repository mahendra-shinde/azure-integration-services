# Implementing queue-triggered batch processing (C#)

## Overview

In this lab, you will create an Azure Function in C# that is triggered by messages in an Azure Storage Queue and processes messages in batches.

## Prerequisites

- .NET 8
- [Azure Functions Core Tools](https://docs.microsoft.com/azure/azure-functions/functions-run-local)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)
- An Azure subscription
- [Azurite](https://docs.microsoft.com/azure/storage/common/storage-use-azurite) (for local Storage Emulator) or an Azure Storage Account

---

## Step 1: Create a new Azure Functions project

```sh
func init lab02 --worker-runtime dotnet
cd lab02
func new --name QueueBatchProcessor --template "Queue trigger"
```


## Step 2: Implement batch processing

Edit `QueueBatchProcessor.cs`:

```csharp
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

public static class QueueBatchProcessor
{
    [FunctionName("QueueBatchProcessor")]
    public static void Run(
        [QueueTrigger("batch-items", Connection = "AzureWebJobsStorage")] List<string> messages,
        ILogger log)
    {
        log.LogInformation($"Batch size: {messages.Count}");
        foreach (var message in messages)
        {
            log.LogInformation($"Processing message: {message}");
            // Add your processing logic here
        }
    }
}
```

---

## Step 3: Configure local.settings.json

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  }
}
```

## Step 4: Create and populate the queue

Use Azure Storage Explorer or Azure CLI to create a queue named `batch-items` and add messages.

```sh
$STORAGE_NAME="mahen0101xyz"
$Q_NAME="batch-items"

az storage account create --name $STORAGE_NAME --location westus --resource-group csharp-queue-batch-rg --sku Standard_LRS

az storage queue create --name $Q_NAME --account-name $STORAGE_NAME

az storage message put --queue-name batch-items --account-name $STORAGE_NAME --content "Message 1"
az storage message put --queue-name batch-items --account-name $STORAGE_NAME --content "Message 2"
az storage message put --queue-name batch-items --account-name $STORAGE_NAME --content "Message 3"
```

## Step 5: Run and test the function locally

```sh
func start
```


## Step 6: Deploy to Azure

```sh
$STORAGE_NAME="mahen0101xyz"
$FN_NAME="fun101"
$RG_NAME="csharp-queue-batch-rg"

az functionapp create --resource-group $RG_NAME --consumption-plan-location westus --runtime dotnet --functions-version 4 --name $FN_NAME --storage-account $STORAGE_NAME

```sh
$CONN_STR=$(az storage account show-connection-string --name $STORAGE_NAME --resource-group $RG_NAME --query connectionString --output tsv)
```

az functionapp config appsettings set --name $FN_NAME --resource-group $RG_NAME --settings AzureWebJobsStorage=""

func azure functionapp publish $FN_NAME
```

## Conclusion

You have implemented a queue-triggered Azure Function in C# that processes messages in batches.
