using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Identity;
using Azure.Storage.Blobs;

namespace fn_test1;

public class FileUploadProcess
{
    private readonly ILogger<FileUploadProcess> _logger;

    public FileUploadProcess(ILogger<FileUploadProcess> logger)
    {
        _logger = logger;
    }

    // NOTE: Kindly assign Azure role "Storage Blob Data Contributor" to the Managed Identity of this Function App to allow it to upload files to Blob Storage.
    // You can do this via the Azure Portal by navigating to your Function App, selecting "Identity" under the "Settings" section, and then assigning the appropriate role to the Managed Identity.
    [Function("FileUploadProcess")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        var credentials = new DefaultAzureCredential(); // Use Managed Identity
        try{
        BlobServiceClient blobClient = new BlobServiceClient(new Uri("https://day89fec.blob.core.windows.net/"), credentials);
        blobClient.GetBlobContainerClient("images").GetBlobClient("test.txt").Upload(new BinaryData("Hello World!"));
        }catch(Exception ex){
            _logger.LogError(ex, "Error uploading file to Blob Storage");
            _logger.LogError("Error uploading file to Blob Storage: {Message}", ex.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        return new OkObjectResult("File uploaded !");
    }
}
