using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace sbdemo;

public class MessageProcess
{
    private readonly ILogger<MessageProcess> _logger;

    public MessageProcess(ILogger<MessageProcess> logger)
    {
        _logger = logger;
    }

    [Function(nameof(MessageProcess))]
    public async Task Run(
        [ServiceBusTrigger("news", "sub1", Connection = "AzureWebJobsServiceBus")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}