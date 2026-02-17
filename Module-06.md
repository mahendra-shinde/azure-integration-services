# Azure Event Grid

## Event-Driven Architecture Principles

Event-driven architecture (EDA) is a design paradigm where components communicate by producing, publishing, and consuming discrete events that represent state changes or significant occurrences. This decouples producers from consumers, enabling scalable, resilient, and evolvable systems.

Key benefits
- Loose coupling: producers don't need knowledge of consumers or their implementation.
- Scalability: consumers can be scaled independently to meet demand.
- Responsiveness: asynchronous processing improves perceived and actual responsiveness.
- Extensibility: new consumers can subscribe to existing events without changing producers.

Core principles
- Events are immutable facts about something that happened (who, what, when, where, why).
- Communication is asynchronous and typically push-based (publish/subscribe).
- Components own their data and emit events when that data changes.
- Consumers act on events and should be designed to be idempotent.
- Use explicit contracts (schemas) for event payloads and evolve them with versioning/compatibility strategies.

Common patterns
- Pub/Sub: decoupled distribution of events from one or many publishers to many subscribers.
- Event Sourcing: state is derived from a sequence of events; the event log is the system of record.
- CQRS (Command Query Responsibility Segregation): separate models for updates (commands/events) and reads.
- Event Notification vs. Event-Carried State: events either signal that “something happened” or carry the full state needed by consumers.

Design considerations
- Delivery semantics: decide between at-most-once, at-least-once, or exactly-once delivery and design consumers accordingly.
- Idempotency: make handlers safe to run multiple times for the same event.
- Ordering: determine whether consumers require ordered delivery and choose infrastructure that supports ordering or partitioning.
- Schema evolution: version events, use tolerant parsers, and provide backward/forward compatibility guarantees.
- Correlation and tracing: include correlation IDs, causation IDs, and timestamps to trace flows across services.
- Filtering and routing: reduce noise by filtering events close to the source or at the broker.
- Security and governance: authenticate publishers/subscribers, authorize actions, and audit event flows.
- Observability: emit metrics, logs, and distributed traces for latency, failures, and throughput.
- Error handling: implement retries, exponential backoff, dead-lettering, and compensating actions for failed processing.

When to choose EDA
- Suitable for highly scalable, distributed, or loosely coupled systems.
- Useful when many consumers need to react to the same occurrences or when asynchronous workflows improve responsiveness.
- Avoid if the domain requires strong synchronous consistency or simple request/response interactions.

Short checklist
- Define event schemas and versioning rules.
- Design producers to publish meaningful, minimal events.
- Implement idempotent, fast consumers with proper retry and dead-letter strategies.
- Ensure observability, security, and governance are in place before production rollout.
- Choose the appropriate event distribution model (system topics, custom topics, domains) based on scale and tenancy.

**Best Practices:**
- Design for loose coupling between producers and consumers.
- Use events to trigger business logic asynchronously.
- Ensure idempotency in event handlers to handle duplicate events.

## Event Grid Concepts: Topics, Event Subscriptions, Event Handlers

### Topic
- Definition: An HTTP endpoint that accepts published events; can be a system topic (Azure service), a custom topic you create, or a domain for multi-tenant scenarios.
- Envelope fields: id, eventType, subject, eventTime, data, dataVersion, topic (for domain).
- Schemas: EventGridEvent, CloudEvent, or custom payload — choose consistently for producers/consumers.
- Authentication: Shared access keys (aeg-sas-key) for custom topics, or Azure AD for more secure scenarios.
- Best practices: Keep events small and meaningful; include correlation/cause IDs and minimal required state.

### Event Subscription
- Purpose: Registers an endpoint to receive events from a topic (or system topic) and configures filtering, delivery, and retry behavior.
- Delivery targets: Webhook, Azure Function, Event Hubs, Storage Queue, Service Bus (Queue/Topic), Logic App, Hybrid Connection, etc.
- Filtering:
    - Subject filters: begins-with / ends-with for subject path matching.
    - Advanced filters: numeric, string, boolean, and array filters on event data or metadata.
    - Include / exclude event types for coarse filtering.
- Delivery schema & transformations:
    - Select EventGridEvent vs CloudEvent delivery.
    - Optionally perform event mapping or enrichment at subscription creation (transforms).
- Reliability:
    - Built-in retries with exponential backoff (up to configured/max window).
    - Dead-lettering: undeliverable events can be sent to a storage account or dead-letter endpoint.
- Security & validation:
    - Webhook handshake: endpoint must echo validationCode for initial validation.
    - Use HTTPS and validate authentication tokens/keys on the handler.
- Management:
    - Create/manage via Azure Portal, CLI, PowerShell, ARM templates, or SDKs.
    - Example CLI snippet:
        ```bash
        az eventgrid event-subscription create \
            --name mySub \
            --source-resource-id /subscriptions/<sub>/resourceGroups/<rg>/providers/Microsoft.EventGrid/topics/<topic> \
            --endpoint https://contoso.com/handler \
            --subject-begins-with "orders/" \
            --included-event-types "order.created" "order.updated"
        ```

### Event Handler
- Definition: The consumer that receives and processes events (e.g., Azure Function, Webhook API, Logic App).
- Handler responsibilities:
    - Acknowledge success with HTTP 2xx.
    - Implement idempotency to handle retries and duplicate deliveries.
    - Validate and authenticate incoming events (verify aeg-sas-key or AAD tokens, check signatures if used).
    - Perform fast, non-blocking acceptance then enqueue longer processing work as needed.
- Best practices:
    - Log correlation IDs and emit metrics/traces for observability.
    - Use dead-lettering and compensating actions for failed processing.
    - Handle batching: parse arrays of events and process each item safely.
    - Respect rate limits and implement backpressure where applicable.

### Quick Checklist
- Choose the right topic type (system/custom/domain).
- Decide delivery schema and auth model (SAS key vs Azure AD).
- Apply subject/advanced filters to minimize noise.
- Configure retries and dead-lettering for reliability.
- Implement idempotent, authenticated, and observable handlers.


**Example: Creating a Custom Topic and Subscribing**

**Java (using REST API):**
```java
// Publish an event to Event Grid topic (pseudocode)
HttpClient client = HttpClient.newHttpClient();
String eventJson = "[{ \"id\": \"1\", \"eventType\": \"recordInserted\", \"subject\": \"new/record\", \"eventTime\": \"2023-01-01T00:00:00Z\", \"data\": { \"field\": \"value\" }, \"dataVersion\": \"1.0\" }]";
HttpRequest request = HttpRequest.newBuilder()
		.uri(URI.create("https://<topic-name>.<region>-1.eventgrid.azure.net/api/events"))
		.header("aeg-sas-key", "<access-key>")
		.header("Content-Type", "application/json")
		.POST(HttpRequest.BodyPublishers.ofString(eventJson))
		.build();
client.send(request, HttpResponse.BodyHandlers.ofString());
```

**C#:**
```csharp
// Publish an event to Event Grid topic
var topicHostname = "<topic-name>.<region>-1.eventgrid.azure.net";
var topicKey = "<access-key>";
var topicEndpoint = $"https://{topicHostname}/api/events";
var credentials = new AzureKeyCredential(topicKey);
var client = new EventGridPublisherClient(new Uri(topicEndpoint), credentials);

EventGridEvent egEvent = new EventGridEvent(
		subject: "new/record",
		eventType: "recordInserted",
		dataVersion: "1.0",
		data: new { field = "value" }
);
await client.SendEventAsync(egEvent);
```

**Node.js:**
```javascript
// Publish an event to Event Grid topic
const { EventGridPublisherClient, AzureKeyCredential } = require("@azure/eventgrid");
const endpoint = "https://<topic-name>.<region>-1.eventgrid.azure.net/api/events";
const key = "<access-key>";
const client = new EventGridPublisherClient(endpoint, new AzureKeyCredential(key));

const egEvent = {
	subject: "new/record",
	eventType: "recordInserted",
	dataVersion: "1.0",
	data: { field: "value" }
};
await client.sendEvents([egEvent]);
```

**Best Practices:**
- Use custom topics for application-specific events.
- Use system topics for Azure resource events.
- Secure topics with authentication keys or Azure AD.

---

## System Topics vs Custom Topics vs Domains

- **System Topics:** Built-in topics for Azure services (e.g., Storage, Event Hubs).
- **Custom Topics:** User-created topics for custom applications.
- **Domains:** Used for multi-tenant event publishing and management at scale.

**Best Practices:**
- Use system topics for Azure resource events to reduce management overhead.
- Use domains for SaaS or multi-tenant scenarios.

---

## Event Filtering (Subject, Advanced Filters)

Event Grid supports filtering events at the subscription level to ensure only relevant events are delivered.

- **Subject Filtering:** Filter events based on the subject field.
- **Advanced Filters:** Filter on event data, number, string, or boolean values.

**Example: Subscription with Filters (Azure CLI):**
```bash
az eventgrid event-subscription create \
	--name mySub \
	--source-resource-id <topic-or-system-topic-id> \
	--endpoint <handler-endpoint> \
	--subject-begins-with "/blobServices/default/containers/images/"
```

**Best Practices:**
- Use filters to minimize unnecessary event delivery.
- Combine subject and advanced filters for precise targeting.

---

## Event Delivery and Retry Policies

Event Grid ensures reliable delivery with built-in retry policies.

- **Delivery:** Pushes events to handlers via HTTP POST.
- **Retry Policy:** Retries delivery with exponential backoff for up to 24 hours.
- **Dead-lettering:** Optionally store undeliverable events in a storage account.

**Best Practices:**
- Ensure event handlers return 2xx status codes on success.
- Implement idempotency in handlers to handle retries.
- Configure dead-lettering for critical event loss scenarios.

---

## Performance and Throughput Considerations

- **Throughput:** Event Grid can handle millions of events per second.
- **Batching:** Events may be delivered in batches for efficiency.
- **Latency:** Typically sub-second end-to-end.

**Best Practices:**
- Design handlers to process events quickly and asynchronously.
- Monitor Event Grid metrics for throttling or delivery failures.
- Scale event handlers to match expected event volume.
