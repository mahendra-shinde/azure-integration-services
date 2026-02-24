using System;
using Azure.Messaging.ServiceBus;

string connectionString = "<connection_string>";
string queueName = "<queue_name>";
// Create a Service Bus client
await using var client = new ServiceBusClient(connectionString);
// Create a sender for the queue
ServiceBusSender sender = client.CreateSender(queueName);
// Create a message to send
int productId, quantity;
string name;
double price;
Console.WriteLine("Enter product details:");
Console.Write("Product ID: ");
productId = int.Parse(Console.ReadLine());
Console.Write("Product Name: ");
name = Console.ReadLine();
Console.Write("Quantity: ");
quantity = int.Parse(Console.ReadLine());
Console.Write("Price: ");
price = double.Parse(Console.ReadLine());

ServiceBusMessage message = new ServiceBusMessage("Add new product");
message.ApplicationProperties.Add("Id", productId);
message.ApplicationProperties.Add("Name", name);
message.ApplicationProperties.Add("Quantity", quantity);
message.ApplicationProperties.Add("Price", price);
// Send the message to the queue
await sender.SendMessageAsync(message);
Console.WriteLine("Message sent successfully.");