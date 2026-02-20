using Azure.Storage.Queues;
using System;

string connectionString = "<storage_account_connection_string>";
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
