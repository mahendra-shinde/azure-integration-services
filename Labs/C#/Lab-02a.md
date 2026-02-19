# Azure function to process Storage Blobs

1. Create azure resources: resource-group, storage-account, blob container in storage account.

```pwsh
###### Kindly use different values #######
$RG_NAME="azfun_rg"
$ST_NAME="str3624876"

#############
# Create resource group in eastus
az group create --name $RG_NAME --location eastus

# Create storage account
az storage account create --name $ST_NAME --resource-group $RG_NAME --location eastus --sku Standard_LRS

# Create blob container "uploads"
az storage container create --name uploads --account-name $ST_NAME

# Create Message queue 'queue1' in same storage account
az storage queue create --name queue1 --account-name $ST_NAME
```
2. Create a new local azure function project using `func` cli. Also add new `BlobTrigger` function

```pwsh
func init demo3 --worker-runtime dotnet-isolated -l csharp  
cd demo3
# To support BlobTriggers 
dotnet add package Microsoft.Azure.Functions.Worker.Extensions.Storage
# Add new function for Blob Trigger
func new --name BlobTriggerProcess  --template BlobTrigger 
```

3. Update the code inside `BlobTriggerProcess.cs` file.

```csharp
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace demo3;

public class BlobTriggerProcess
{
    private readonly ILogger<BlobTriggerProcess> _logger;

    public BlobTriggerProcess(ILogger<BlobTriggerProcess> logger)
    {
        _logger = logger;
    }

    [Function("BlobTriggerProcess")]
    public void Run([
        BlobTrigger("uploads/{name}", Connection = "AzureWebJobsStorage")
    ] Stream stream, string name)
    {
        long size = stream.Length;
        _logger.LogInformation("C# Blob trigger function Processed blob\n Name: {name} \n Size: {size} Bytes", name, size);
        string message = $"Blob Name: {name}, Size: {size} Bytes";
        var queueClient = new Azure.Storage.Queues.QueueClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage"),
            "queue1"
        );
        queueClient.CreateIfNotExists();
        if (queueClient.Exists())
        {
            queueClient.SendMessage(message);
        }
    }
}
```

4. Deploy the function to Azure using the existing storage account and resource group.

```pwsh
# Log in to Azure if not already logged in
az login
## Use exact same values from previous section

$RG_NAME="azfun_rg"
$ST_NAME="str3624876"
# Create a Function App in the existing resource group and storage account
$FUNC_APP_NAME="demo2-funcapp-$(Get-Random)"

az functionapp create -g $RG_NAME --consumption-plan-location eastus --runtime dotnet-isolated --functions-version 4 -n $FUNC_APP_NAME -s $ST_NAME

# Publish your function to Azure
func azure functionapp publish $FUNC_APP_NAME
```

> **Note:**  
> - Make sure the storage account and resource group names match those you created earlier.
> - The function app name must be globally unique.

5. Test the function by uploading few files to storage account. 

> Use `Log Stream` from Azure portal to view live logs from function!

```pwsh
# Create a sample text file
echo "Hello from Azure Blob Trigger!" > sample.txt

# Upload the file to the 'uploads' container
az storage blob upload --account-name $ST_NAME --container-name uploads --name sample.txt --file sample.txt
```

6. Delete the resource group which should auto-delete function and storage account as well.

```pwsh
az group delete -n $RG_NAME --no-wait -y
```