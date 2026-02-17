# Implementing queue-based communication patterns (C#)

## Lab Overview

In this hands-on lab, you will learn how to implement queue-based communication patterns using C# and Azure Service Bus. You will create a simple producer and consumer application that sends and receives messages via an Azure Service Bus queue.

## Prerequisites

- .NET 8 SDK or later
- Azure Subscription
- Azure Service Bus namespace and queue
- Visual Studio or VS Code

## Step 1: Set Up Azure Service Bus

1. Log in to the [Azure Portal](https://portal.azure.com).
2. Create a new Service Bus namespace.
3. Within the namespace, create a queue (e.g., `demoqueue`).
4. Obtain the connection string for the namespace.

## Step 2: Create a Console Project

```sh
dotnet new console -n ServiceBusDemo
cd ServiceBusDemo
dotnet add package Azure.Messaging.ServiceBus
```

## Step 3: Implement the Queue Producer

Create `QueueProducer.cs`:

```csharp
using Azure.Messaging.ServiceBus;

string connectionString = "<YOUR_SERVICE_BUS_CONNECTION_STRING>";
string queueName = "demoqueue";

await using var client = new ServiceBusClient(connectionString);
ServiceBusSender sender = client.CreateSender(queueName);

for (int i = 1; i <= 5; i++)
{
    await sender.SendMessageAsync(new ServiceBusMessage($"Message {i}"));
    Console.WriteLine($"Sent: Message {i}");
}
```

## Step 4: Implement the Queue Consumer

Create `QueueConsumer.cs`:

```csharp
using Azure.Messaging.ServiceBus;

string connectionString = "<YOUR_SERVICE_BUS_CONNECTION_STRING>";
string queueName = "demoqueue";

await using var client = new ServiceBusClient(connectionString);
ServiceBusReceiver receiver = client.CreateReceiver(queueName);

var messages = await receiver.ReceiveMessagesAsync(maxMessages: 5);
foreach (var message in messages)
{
    Console.WriteLine($"Received: {message.Body}");
    await receiver.CompleteMessageAsync(message);
}
```

## Step 5: Run the Applications

1. Replace `<YOUR_SERVICE_BUS_CONNECTION_STRING>` with your actual connection string in both files.
2. Run the producer:

   ```sh
   dotnet run --project QueueProducer.csproj
   ```

3. Run the consumer:

   ```sh
   dotnet run --project QueueConsumer.csproj
   ```

## Step 6: Clean Up Resources

- Delete the Service Bus namespace from the Azure portal to avoid charges.

## Summary

You have implemented a simple queue-based communication pattern using C# and Azure Service Bus. This pattern is useful for decoupling application components and enabling reliable message delivery.
