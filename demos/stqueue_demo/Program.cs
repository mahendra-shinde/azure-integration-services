using Azure.Storage.Queues;
using System;

string connectionString = "DefaultEndpointsProtocol=https;AccountName=appgroupbf0e;AccountKey=hBWC8sFnEN5pPAYF1gMj2WcICcYdGLejP+d2w3LyeW1793bH0sW/5gTEqjoOiEK8h2Qt8iEPubYC+ASt6gmzbg==;EndpointSuffix=core.windows.net";
string queueName = "orders";

// Create a QueueClient
QueueClient queueClient = new QueueClient(connectionString, queueName);
Console.WriteLine($"Queue Client created for queue: {queueName}");

// Create the queue if it doesn't exist
queueClient.CreateIfNotExists();

for(int i=1; i<=5; i++)
{
    // Send a message to the queue
    string message = $"Hello, Azure Queue Storage! Message {i}";
    queueClient.SendMessage(message);
    Console.WriteLine($"Message sent: {message}");
}
