using Azure.Messaging.EventGrid;

// Replace with your Event Grid topic endpoint and access key
// Topic Endpoint format: https://<topic-name>.<region>-1.eventgrid.azure.net/api/events
string endpoint = "topic-endpoint";
string key = "<ACESS_KEY>";

var client = new Azure.Messaging.EventGrid.EventGridPublisherClient(
    new Uri(endpoint),
    new Azure.AzureKeyCredential(key));

var event1 = new EventGridEvent(
    subject: "NewArticle",
    eventType: "News.ArticlePublished",
    dataVersion: "1.0",
    data: new
    {
        Title = "Breaking News: New Event Grid Features Released",
        Author = "John Doe",
        PublishedDate = DateTime.UtcNow
    });

await client.SendEventAsync(event1);
Console.WriteLine("Event sent successfully.");