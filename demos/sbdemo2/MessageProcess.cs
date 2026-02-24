using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;

namespace sbdemo2;

public class MessageProcess
{
    private readonly ILogger<MessageProcess> _logger;

    public MessageProcess(ILogger<MessageProcess> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Azure Function triggered by a Service Bus message from the "products" queue.
    /// Processes the message by extracting product details from its application properties,
    /// logs the information, and stores the product entity in an Azure Storage Table.
    /// Completes the Service Bus message after successful processing.
    /// </summary>
    /// <param name="message">The received Service Bus message containing product information.</param>
    /// <param name="messageActions">Actions for managing the Service Bus message lifecycle.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Function(nameof(MessageProcess))]
    public async Task Run(
        [ServiceBusTrigger("products", Connection = "AzureWebJobsServiceBus")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
        int productId = (int)message.ApplicationProperties["Id"];
        string name = (string)message.ApplicationProperties["Name"];
        int quantity = (int)message.ApplicationProperties["Quantity"];
        double price = (double)message.ApplicationProperties["Price"];
        _logger.LogInformation("Product ID: {productId}", productId);
        _logger.LogInformation("Product Name: {name}", name);
        _logger.LogInformation("Quantity: {quantity}", quantity);
        _logger.LogInformation("Price: {price}", price);

        // Connect to Azure Storage Tables
        string tableConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        string tableName = "products";
        var tableClient = new TableClient(tableConnectionString, tableName);
        await tableClient.CreateIfNotExistsAsync();

        var productEntity = new TableEntity(productId.ToString(), name)
        {
            { "Quantity", quantity },
            { "Price", price }
        };
        await tableClient.AddEntityAsync(productEntity);
        _logger.LogInformation("Product added to Azure Storage Table successfully.");
        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}