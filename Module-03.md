# Azure Functions

Azure Functions is a serverless compute service that enables you to run event-driven code without managing infrastructure. It allows you to focus on your application logic, while Azure handles scaling, patching, and resource management.

## Azure Functions Architecture and Hosting Plans

### Architecture Overview
- Function app: a logical container for one or more functions that share configuration, runtime and deployment.
- Functions runtime: executes function code, loads bindings/triggers, manages worker processes (in-process or isolated).
- Triggers & bindings: declarative plumbing that connects functions to events and external services (HTTP, queues, blobs, Event Grid, Service Bus, etc.).
- Scale controller: monitors trigger load and controls instance scale-out/scale-in (platform-managed for Azure-hosted plans; custom via KEDA for Kubernetes).
- Storage account: required for host metadata, triggers (storage queues/blobs), and scale state (Consumption/Premium); not a replacement for app data.
- Ephemeral file system: function instances have a transient file system; persist state externally (Cosmos DB, Storage, Durable Entities).
- Stateless by default: design functions to be idempotent and externalize state. Use Durable Functions for orchestrations and long-running workflows.

### Hosting Plans (summary)
- Consumption Plan
    - Serverless, pay-per-execution model.
    - Automatic scaling out to handle bursts; platform manages instance lifecycle.
    - Cold starts possible after idle periods.
    - Minimal operational overhead; storage account required.
    - Best for unpredictable or spiky workloads and cost-sensitive scenarios.

- Premium Plan
    - Pre-warmed instances eliminate or greatly reduce cold starts.
    - VNet integration and enhanced networking/security options.
    - Predictable performance, advanced CPU/memory sizes, and longer execution durations.
    - Ideal for latency-sensitive, enterprise, or VNet-bound workloads.

- Dedicated (App Service) Plan
    - Runs on App Service VMs you provision; functions behave like web apps.
    - Always-on, supports App Service features (slots, scaling rules you control).
    - Predictable billing based on reserved VM instances.
    - Good for legacy, high-resource, or integrated App Service scenarios.

- Kubernetes / Container-hosted
    - Deploy Functions as container images to AKS or other Kubernetes clusters.
    - Use KEDA to provide event-driven autoscaling.
    - Full infrastructure control, bring-your-own runtime, and consistent container tooling.
    - Best when you need custom runtimes, isolation, or complex orchestration across workloads.

- Other deployment options
    - Deploy via Zip/Run-From-Package, container images, or CI/CD pipelines (GitHub Actions, Azure DevOps).
    - Use Slots, App Settings, and feature flags for staged rollouts on App Service / Premium / Dedicated.

### Networking, Security & Integrations
- VNet Integration: available in Premium and Dedicated plans for private resource access.
- Managed Identity: enable system-assigned or user-assigned identities for secure service access.
- Secrets: use Key Vault and managed identities; avoid committing secrets to repo or local.settings.json.
- TLS and IP restrictions: configure at function app/App Service level for hardened endpoints.

### Scaling and Performance Considerations
- Cold starts: mitigate with Premium pre-warmed instances, Always On on Dedicated, or warm-up techniques.
- Concurrency: design functions to handle concurrent invocations; external resources (databases, APIs) may become bottlenecks.
- Throttling and quotas: platform enforces limits; plan for retries/backoff and idempotency. Check Azure docs for current quotas.
- Observability: instrument with Application Insights or OpenTelemetry for traces, metrics and logs.

### When to choose which plan (guidance)
- Consumption: most event-driven, cost-sensitive apps with variable traffic.
- Premium: low-latency, VNet-required, predictable performance, or enterprise-grade needs.
- Dedicated: apps requiring dedicated VMs, App Service features, or long-running processes.
- Kubernetes: full control, custom runtimes, or heterogeneous workloads already on Kubernetes.

Best practice: design functions to be stateless and idempotent, externalize state, secure access with managed identities, and pick the hosting plan that matches latency, networking, and cost requirements for your workload.

### 1. Consumption Plan

- **Auto-scaling:** Azure automatically adds or removes compute resources to match the number of incoming events, ensuring your function app scales efficiently without manual intervention.
- **Billing:** You are charged only for the actual execution time and resources consumed by your functions, making it cost-effective for workloads with variable or unpredictable traffic.
- **Cold start:** In the Consumption plan, when a function app has been idle, Azure may need to allocate resources before running your code. This can cause a short delay (cold start) on the first request, as the platform initializes the function environment.

### 2. Premium Plan

- **No cold starts:** Pre-warmed instances ensure your functions are always ready to run, minimizing startup delays.
- **VNET integration:** Allows secure connectivity to resources in your Azure Virtual Network, supporting advanced networking scenarios.
- **Advanced scaling:** Automatically scales based on demand, with pre-warmed instances for consistent performance during traffic spikes.
- **Billing:** Charges are based on the number of pre-warmed and running instances, offering predictable pricing for enterprise workloads.
- **Enterprise features:** Supports custom scaling, larger memory and CPU options, and enhanced security for mission-critical applications.

### 3. Dedicated (App Service) Plan

- **Always-on:** Runs on dedicated VMs, suitable for long-running or resource-intensive functions.
- **Predictable pricing:** Shares resources with other App Service apps.

**Best Practice:**  
Choose the Consumption plan for most event-driven workloads. Use Premium or Dedicated plans for enterprise scenarios requiring VNET integration, no cold starts, or predictable performance.

## Supported Languages and Runtime Versions

Azure Functions supports multiple languages and runtime versions:

- **C#** (in-process and isolated process)
- **JavaScript/TypeScript** (Node.js)
- **Python**
- **Java**
- **PowerShell**
- **Custom handlers** (for other languages)

**Runtime Versions:**
- Version 4.x (latest, recommended)
- Version 3.x (legacy support)

**Best Practice:**  
Always use the latest runtime and language versions for security and new features.

## Triggers and Bindings Overview

Azure Functions are event-driven. Triggers define how a function is invoked, and bindings provide a declarative way to connect to other services.

### Common Triggers

- **HTTP Trigger:** Responds to HTTP requests (REST APIs, webhooks).
- **Timer Trigger:** Runs on a schedule (CRON expressions).
- **Queue Trigger:** Responds to messages in Azure Storage Queues.
- **Blob Trigger:** Runs when a blob is added or modified in Azure Storage.
- **Event Hub Trigger:** Processes events from Azure Event Hub.
- **Service Bus Trigger:** Responds to messages in Azure Service Bus queues or topics.
- **Event Grid Trigger:** Handles events from Event Grid.
- **Cosmos DB Trigger:** Responds to changes in Cosmos DB collections.

### Bindings

- **Input Bindings:** Read data from external sources (e.g., Storage, Cosmos DB).
- **Output Bindings:** Write data to external sources.

**Example:**

```csharp
[FunctionName("QueueTriggerFunction")]
public void Run(
    [QueueTrigger("myqueue-items")] string myQueueItem,
    ILogger log)
{
    log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
}
```

---

## Durable Functions for Stateful Workflows and Orchestrations

Durable Functions is an extension of Azure Functions that enables you to write stateful workflows in a serverless environment.

### Key Concepts

- **Orchestrator functions:** Define workflows using code.
- **Activity functions:** Perform tasks.
- **Entity functions:** Manage state.

**Use Cases:**
- Chaining functions
- Fan-out/fan-in patterns
- Human interaction workflows
- Monitoring and timeout scenarios

**Example:**

```csharp
[FunctionName("E1_HelloSequence")]
public static async Task<List<string>> Run(
    [OrchestrationTrigger] IDurableOrchestrationContext context)
{
    var outputs = new List<string>();
    outputs.Add(await context.CallActivityAsync<string>("E1_SayHello", "Tokyo"));
    outputs.Add(await context.CallActivityAsync<string>("E1_SayHello", "Seattle"));
    outputs.Add(await context.CallActivityAsync<string>("E1_SayHello", "London"));
    return outputs;
}
```

---

## Local Development and Debugging with Azure Functions Core Tools

Azure Functions Core Tools allow you to develop and test functions locally before deploying to Azure.

### Steps:

1. **Install Core Tools:**  
   Use npm or download from the official site.
   ```
   npm install -g azure-functions-core-tools@4 --unsafe-perm true
   ```

2. **Initialize a Project:**  
   ```
   func init MyFunctionProj --worker-runtime dotnet
   ```

3. **Create a Function:**  
   ```
   func new --name MyHttpFunction --template "HTTP trigger"
   ```

4. **Run Locally:**  
   ```
   func start
   ```

5. **Debug:**  
   Use breakpoints in your IDE (e.g., Visual Studio Code).

**Best Practice:**  
Use `local.settings.json` for local configuration and secrets (do not commit this file).

---

## Use OpenTelemetry with Functions

OpenTelemetry provides observability (tracing, metrics, logs) for distributed systems.

### Integrating OpenTelemetry

- **.NET Functions:**  
  Add the `OpenTelemetry` NuGet packages and configure tracing in your function app.

**Example:**

```csharp
// Program.cs or Startup.cs
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddAzureMonitorTraceExporter());
```

- **Exporters:**  
  Send telemetry to Azure Monitor, Application Insights, or other backends.

**Best Practice:**  
Instrument your functions to monitor performance, diagnose issues, and trace requests across distributed systems.

---

## Summary

Azure Functions provides a powerful, flexible, and scalable platform for building event-driven applications. By understanding hosting plans, supported languages, triggers, bindings, stateful workflows, local development, and observability, you can build robust serverless solutions on Azure.
