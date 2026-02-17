# Azure Service Bus

## Service Bus Architecture

Azure Service Bus is a fully managed enterprise message broker with message queues and publish-subscribe topics.
**Key Components:**
- **Namespace:** Global container and DNS endpoint for messaging entities. Choose SKU (Basic/Standard/Premium) for features and isolation. Use separate namespaces per environment or tenant.
- **Queue:** Point-to-point entity for single-consumer processing. Supports TTL, duplicate detection, dead-lettering, sessions (ordered processing), auto-forwarding, and partitioning.
- **Topic:** Publish-subscribe entry point where senders post messages for distribution to multiple subscriptions.
- **Subscription:** Virtual queue attached to a topic. Each subscription can have its own filters/rules, TTL and dead-letter queue. Multiple subscriptions receive independent copies of topic messages.
- **Dead-Letter Queue (DLQ):** System sub-queue for messages that exceed delivery attempts or are explicitly dead-lettered; inspect, fix and resubmit as needed.
- **Rules & Filters:** SQL-like (and correlation) filters on subscriptions to route/select messages based on system/application properties (e.g., "priority = 'high'").
- **Message Properties:** System and application properties used for routing, filtering, correlation and diagnostics (e.g., messageId, correlationId, custom applicationProperties).
- **Sessions & Correlation:** Sessions group related messages for ordered processing; correlationId links messages across workflows for tracing.
- **Auto-forwarding / ForwardTo:** Chain entities by forwarding messages automatically from one queue/subscription to another for modular workflows.
- **Partitioning & Throughput:** Partitioned entities distribute load across brokers for scale. Use Premium for predictable throughput and advanced features.
- **Security & Access Control:** SAS policies, Azure RBAC and managed identities for secure access; always use TLS and least-privilege policies.
- **Monitoring & Operations:** Use Azure Monitor, metrics, logs and Service Bus Explorer (or SDK) to monitor throughput, dead-letter counts, delivery failures and retry behavior.

Examples:
- SQL filter example:
```sql
color = 'red' AND priority = 'high'
```
- Best practices: isolate environments by namespace, enable DLQ and retry policies, use sessions for ordered workflows, prefer managed identities for access.

**Best Practices:**
- Use separate namespaces for isolation between environments (dev, test, prod).
- Organize queues and topics based on business domains.


## Messaging Patterns: Point-to-Point and Publish-Subscribe

**Point-to-Point (Queue):**
- One sender, one receiver.
- Messages are processed in order and removed from the queue once received.

**Publish-Subscribe (Topic/Subscription):**
- One sender, multiple receivers.
- Sender posts messages to a topic; each subscription receives a copy based on filters.

**Example:**
- Order processing: Use a queue for single consumer.
- Event notification: Use a topic with multiple subscriptions for different consumers.

**Best Practices:**
- Use queues for tasks that require exactly-once processing.
- Use topics for broadcasting events to multiple systems.


## Message Properties, Sessions, and Correlation

**Message Properties:**
- Custom metadata attached to messages (e.g., priority, type).
- Used for filtering and routing.

**Sessions:**
- Enable ordered message processing for related messages.
- Each session represents a group of related messages.

**Correlation:**
- Use correlation IDs to track related messages across systems.


**Example:**
```java
// Java pseudocode for sending a message with properties and session
ServiceBusMessage message = new ServiceBusMessage("Order Created");
message.setSessionId("order-123");
message.getApplicationProperties().put("priority", "high");
message.getApplicationProperties().put("correlationId", "corr-456");
// ...existing code...
```

```csharp
// C# example for sending a message with properties and session
var message = new ServiceBusMessage("Order Created")
{
		SessionId = "order-123"
};
message.ApplicationProperties["priority"] = "high";
message.ApplicationProperties["correlationId"] = "corr-456";
// ...existing code...
```

```javascript
// Node.js example for sending a message with properties and session
const { ServiceBusMessage } = require("@azure/service-bus");
const message = {
	body: "Order Created",
	sessionId: "order-123",
	applicationProperties: {
		priority: "high",
		correlationId: "corr-456"
	}
};
// ...existing code...
```

**Best Practices:**
- Use sessions for workflows requiring message ordering.
- Set correlation IDs for distributed tracing.

## Dead-Letter Queues and Poison Message Handling

**Dead-Letter Queue (DLQ):**
- Automatically stores messages that cannot be delivered or processed.
- Each queue and subscription has an associated DLQ.

**Poison Messages:**
- Messages that repeatedly fail processing.

**Handling:**
- Monitor DLQs for failed messages.
- Implement retry logic and alerting for poison messages.

**Example:**
- Move messages to DLQ after exceeding max delivery count.
- Use Service Bus Explorer or SDK to inspect and resubmit DLQ messages.

**Best Practices:**
- Set appropriate max delivery count.
- Regularly monitor and process DLQs.


## Auto-Forwarding and Message Deferral

**Auto-Forwarding:**
- Automatically forwards messages from one queue or subscription to another.
- Useful for chaining workflows or routing messages.

**Message Deferral:**
- Temporarily postpones message processing.
- Deferred messages can be retrieved later using sequence numbers.


**Example:**
```java
// Java pseudocode for deferring a message
ServiceBusReceivedMessage receivedMessage = receiver.receiveMessages(1).get(0);
receiver.defer(receivedMessage);
// Later, retrieve deferred message
ServiceBusReceivedMessage deferredMessage = receiver.receiveDeferredMessage(receivedMessage.getSequenceNumber());
// ...existing code...
```

```csharp
// C# example for deferring a message
ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
await receiver.DeferMessageAsync(receivedMessage);
// Later, retrieve deferred message
ServiceBusReceivedMessage deferredMessage = await receiver.ReceiveDeferredMessageAsync(receivedMessage.SequenceNumber);
// ...existing code...
```

```javascript
// Node.js example for deferring a message
const receivedMessages = await receiver.receiveMessages(1);
const receivedMessage = receivedMessages[0];
await receiver.deferMessage(receivedMessage);
// Later, retrieve deferred message
const deferredMessage = await receiver.receiveDeferredMessages([receivedMessage.sequenceNumber]);
// ...existing code...
```

**Best Practices:**
- Use auto-forwarding for modular workflow design.
- Use deferral for messages that require delayed processing.


## Integration with Other Azure Services

Azure Service Bus integrates with many Azure services:

- **Logic Apps:** Automate workflows triggered by Service Bus messages.
- **Azure Functions:** Process messages using serverless compute.
- **Event Grid:** Route Service Bus events to other services.
- **Azure Stream Analytics:** Real-time analytics on Service Bus data.

**Example:**
- Trigger a Logic App when a message arrives in a queue.
- Use Azure Function with Service Bus trigger for message processing.

**Best Practices:**
- Use managed identities for secure integration.
- Monitor integrations using Azure Monitor and Application Insights.


## Summary

Azure Service Bus provides robust messaging capabilities for distributed applications. Use queues and topics for flexible patterns, leverage sessions and properties for advanced scenarios, and integrate with other Azure services for end-to-end solutions.
