using Azure.Messaging.ServiceBus;

string connString = "<your_connection_string>";
string queueName = "orderqueue";

var clientOptions = new ServiceBusClientOptions()
{ 
    TransportType = ServiceBusTransportType.AmqpWebSockets
};

ServiceBusClient client = new ServiceBusClient(connString, clientOptions);
ServiceBusSender sender = client.CreateSender(queueName);

using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

for (int i = 0; i < 10; i++)
{
    ServiceBusMessage message = new ServiceBusMessage($"Order {i}");
    message.ApplicationProperties.Add("OrderId", i);
    message.ApplicationProperties.Add("Priority", i % 2 == 0 ? "High" : "Low");
    message.ApplicationProperties.Add("Timestamp", DateTime.UtcNow);
    message.ApplicationProperties.Add("CustomerId", $"Customer{i % 5}");

    if(!messageBatch.TryAddMessage(message))
    {
        throw new Exception($"Message {i} is too large to fit in the batch.");
    }
}

try
{
    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine("Messages sent successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error sending messages: {ex.Message}");
}
finally
{
    await sender.DisposeAsync();
    await client.DisposeAsync();
}