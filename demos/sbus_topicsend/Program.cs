using System.Diagnostics.Contracts;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;


string connString = "<your_connection_string>";
string topicName = "news";

ServiceBusClient client = new ServiceBusClient(connString);
ServiceBusSender sender = client.CreateSender(topicName);


async Task sendMessage()
{
    ServiceBusMessage message = new ServiceBusMessage("Breaking news: Azure Service Bus is awesome!");
    message.ApplicationProperties.Add("Category", "Breaking");
    message.ApplicationProperties.Add("Priority", "High");
    message.ApplicationProperties.Add("Timestamp", DateTime.UtcNow);
    try
    {
        await sender.SendMessageAsync(message);
        Console.WriteLine("Message sent successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending message: {ex.Message}");
    }
    finally
    {
        await sender.DisposeAsync();
    }
}
async Task recieveMessage()
{
    ServiceBusProcessor processor = client.CreateProcessor(topicName, "sub1", new ServiceBusProcessorOptions());

    processor.ProcessMessageAsync += async (ProcessMessageEventArgs args) =>
    {
        string body = args.Message.Body.ToString();
        Console.WriteLine($"Received message: {body}");
        Console.WriteLine($"Category: {args.Message.ApplicationProperties["Category"]}");
        Console.WriteLine($"Priority: {args.Message.ApplicationProperties["Priority"]}");
        Console.WriteLine($"Timestamp: {args.Message.ApplicationProperties["Timestamp"]}");
        await args.CompleteMessageAsync(args.Message);
    };

    processor.ProcessErrorAsync += async (ProcessErrorEventArgs args) =>
    {
        Console.WriteLine($"Error receiving message: {args.Exception.Message}");
    };

    await processor.StartProcessingAsync();
}

Console.WriteLine("Press 's' to send a message or 'r' to receive messages. Press 'q' to quit.");
while (true)
{
    var key = Console.ReadKey(true);
    if (key.KeyChar == 's')
    {
        await sendMessage();
    }
    else if (key.KeyChar == 'r')
    {
        await recieveMessage();
    }
    else if (key.KeyChar == 'q')
    {
        break;
    }
}