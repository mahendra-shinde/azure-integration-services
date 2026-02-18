# Lab 1: Azure SDK Integration with C#

## Objective
Learn how to connect to Azure services using the .NET SDK.

## Prerequisites
- .NET 8.0 SDK or later
- Azure subscription
- Visual Studio or VS Code

## Steps

### 1. Create a new Console App

```bash
dotnet new console -n AzureLab1
cd AzureLab1
```

### 2. Add Azure SDK NuGet Package

```bash
dotnet add package Azure.Identity
dotnet add package Azure.Storage.Blobs
```

### 3. Sample Code: Connect to Azure Blob Storage

```csharp
using Azure.Identity;
using Azure.Storage.Blobs;

string connectionString = "<your_connection_string>";
string containerName = "sample-container";

var blobServiceClient = new BlobServiceClient(connectionString);
var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

Console.WriteLine($"Connected to container: {containerClient.Name}");
```

### 4. Run the Application

```bash
dotnet run
```

## Next Steps

- Try uploading and downloading blobs.
- Explore authentication with DefaultAzureCredential.

