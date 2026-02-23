using System;
using Azure.Storage.Blobs;

string connectionString = "<CONNECTION_STRING>";
string containerName = "files";

BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
string blobName = "product.csv";

BlobClient blobClient = containerClient.GetBlobClient(blobName);
string localFilePath = Path.Combine(Environment.CurrentDirectory, blobName);
Console.WriteLine($"Downloading blob to {localFilePath}");
await blobClient.DownloadToAsync(localFilePath);