using Azure.Messaging.EventGrid;

string topicName = "news";
string endpoint = "<GRID_HTTP_ENDPOINT>";
string key = "<ACCESSKEY>";

var client = new Azure.Messaging.EventGrid.EventGridPublisherClient(
    new Uri($"https://{endpoint}/api/events?topic={topicName}"),
    new Azure.AzureKeyCredential(key));

var event1 =  new EventGridEvent(
        subject: "New Article Published",
        eventType: "News.ArticlePublished",
        dataVersion: "1.0",
        data: new
        {
            Title = "Azure Event Grid Simplifies Event-Driven Architectures",
            Author = "John Doe",
            PublishedDate = DateTime.UtcNow
        });

await client.SendEventAsync(event1);
Console.WriteLine("Event sent successfully.");

