# AI/ML Integration in Azure Integration Services

## Overview of Azure AI and ML Services

Azure provides a broad set of managed AI and ML offerings to accelerate building intelligent applications and automation.

- **Cognitive Services**
    - Pre-built, domain-specific APIs for Vision, Speech, Language, and Decision (e.g., Computer Vision, Form Recognizer, Speech-to-Text, Text Analytics).
    - Key capabilities: OCR, entity extraction, sentiment analysis, translation, anomaly detection, content moderation.
    - Typical use cases: document processing, chat enrichment, image/video analysis, accessibility features.
    - When to choose: fast time-to-value, limited model training needs, consistent API surface across languages.
    - Integration: Logic Apps, Functions, API Management, Key Vault for keys.

- **Azure OpenAI Service**
    - Managed access to large language models for generation, summarization, Q&A, and code assistance.
    - Key capabilities: text completions, embeddings, chat-style interactions, semantic search.
    - Typical use cases: conversational agents, knowledge augmentation, prompt-driven automation, content generation.
    - When to choose: advanced natural language tasks requiring flexible prompt control and contextual generation.
    - Integration: REST/SDKs, embeddings + vector stores for retrieval-augmented generation (RAG).

- **Azure Machine Learning**
    - End-to-end MLOps platform for data prep, model training, hyperparameter tuning, model registry, and deployment.
    - Key capabilities: automated ML, managed compute, pipelines, model explainability, MLflow-like tracking.
    - Typical use cases: custom supervised/unsupervised models, production model lifecycle, large-scale training.
    - When to choose: custom model development, reproducible experiments, governance and deployment at scale.
    - Integration: Containerized deployments, Azure Kubernetes Service (AKS), Azure Functions, CI/CD pipelines.

- **Complementary services**
    - Bot Service / Bot Framework for conversational UX.
    - Cognitive Search for semantic search over documents + embeddings.
    - Azure Databricks and Synapse for large-scale data engineering and model training.

Getting started
- Identify whether you need pre-built APIs (Cognitive Services), hosted LLM capabilities (Azure OpenAI), or full custom ML workflows (Azure ML).
- Prototype with Cognitive Services or Azure OpenAI, then move to Azure ML for custom models or production MLOps.

Security, compliance, and cost
- Secure keys and credentials with Azure Key Vault; use managed identities where possible.
- Apply network controls (private endpoints, VNet integration) and monitor usage/quotas.
- Choose appropriate service tiers and batching strategies to control costs.

Best practices
- Start with pre-built APIs to validate scenarios, then evaluate custom models if accuracy or control is insufficient.
- Use prompt engineering, context windows, and safety filters for LLMs.
- Instrument telemetry and audit model outputs for drift, bias, and cost.
- Reuse embeddings and cache results for repeated queries to reduce calls and costs.
- Implement CI/CD and model governance for production deployments.

**Best Practices:**
- Use Cognitive Services for common AI tasks to save development time.
- Use Azure ML for custom model training and deployment.
- Monitor usage and costs for all AI services.

---

## Integrating Azure Cognitive Services with Logic Apps and Functions

You can call Cognitive Services from Logic Apps and Azure Functions to add AI capabilities (e.g., sentiment analysis, OCR).

**Example: Calling Text Analytics API**

**Java:**
```java
HttpClient client = HttpClient.newHttpClient();
HttpRequest request = HttpRequest.newBuilder()
		.uri(URI.create("https://<region>.api.cognitive.microsoft.com/text/analytics/v3.0/sentiment"))
		.header("Ocp-Apim-Subscription-Key", "<api-key>")
		.header("Content-Type", "application/json")
		.POST(HttpRequest.BodyPublishers.ofString("{\"documents\":[{\"id\":\"1\",\"text\":\"Azure is great!\"}]}"))
		.build();
HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());
```

**C#:**
```csharp
var client = new HttpClient();
client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "<api-key>");
var content = new StringContent("{\"documents\":[{\"id\":\"1\",\"text\":\"Azure is great!\"}]}", Encoding.UTF8, "application/json");
var response = await client.PostAsync("https://<region>.api.cognitive.microsoft.com/text/analytics/v3.0/sentiment", content);
string result = await response.Content.ReadAsStringAsync();
```

**Node.js:**
```javascript
const axios = require('axios');
const response = await axios.post(
	'https://<region>.api.cognitive.microsoft.com/text/analytics/v3.0/sentiment',
	{ documents: [{ id: '1', text: 'Azure is great!' }] },
	{ headers: { 'Ocp-Apim-Subscription-Key': '<api-key>' } }
);
```

**Best Practices:**
- Secure API keys using Key Vault or environment variables.
- Handle API rate limits and errors gracefully.

---

## Azure OpenAI Service Integration for Intelligent Automation

Azure OpenAI enables advanced language understanding and generation in your workflows.

**Example: Calling Azure OpenAI Completion API**

**Java:**
```java
HttpRequest request = HttpRequest.newBuilder()
		.uri(URI.create("https://<resource-name>.openai.azure.com/openai/deployments/<deployment-id>/completions?api-version=2023-03-15-preview"))
		.header("api-key", "<api-key>")
		.header("Content-Type", "application/json")
		.POST(HttpRequest.BodyPublishers.ofString("{\"prompt\":\"Hello, world!\",\"max_tokens\":10}"))
		.build();
HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());
```

**C#:**
```csharp
var client = new HttpClient();
client.DefaultRequestHeaders.Add("api-key", "<api-key>");
var content = new StringContent("{\"prompt\":\"Hello, world!\",\"max_tokens\":10}", Encoding.UTF8, "application/json");
var response = await client.PostAsync("https://<resource-name>.openai.azure.com/openai/deployments/<deployment-id>/completions?api-version=2023-03-15-preview", content);
string result = await response.Content.ReadAsStringAsync();
```

**Node.js:**
```javascript
const axios = require('axios');
const response = await axios.post(
	'https://<resource-name>.openai.azure.com/openai/deployments/<deployment-id>/completions?api-version=2023-03-15-preview',
	{ prompt: 'Hello, world!', max_tokens: 10 },
	{ headers: { 'api-key': '<api-key>' } }
);
```

**Best Practices:**
- Use prompt engineering to optimize model responses.
- Monitor usage and set quotas to control costs.

---

## Building Intelligent Workflows with AI-Powered Decision Making

Combine Logic Apps, Functions, and AI services to automate decisions (e.g., route emails based on sentiment).

**Example Workflow:**
1. Logic App receives an email.
2. Calls Cognitive Services for sentiment analysis.
3. Routes the email based on sentiment score.

**Best Practices:**
- Use AI for tasks like classification, extraction, and summarization.
- Log AI decisions for audit and improvement.

---

## Prompt Engineering for Integration Scenarios

Prompt engineering is the process of designing effective prompts for language models.

**Tips:**
- Be explicit and clear in your instructions.
- Provide context and examples in the prompt.
- Test and iterate to improve results.

**Example Prompt:**
```
Summarize the following support ticket in one sentence: "The user cannot access the portal after the latest update."
```

---

## ML Model Deployment and Consumption via REST APIs

Deploy custom ML models to Azure ML and consume them via REST APIs.

**Example: Consuming a Deployed Model**

**Java:**
```java
HttpRequest request = HttpRequest.newBuilder()
		.uri(URI.create("<scoring-uri>"))
		.header("Authorization", "Bearer <access-token>")
		.header("Content-Type", "application/json")
		.POST(HttpRequest.BodyPublishers.ofString("{\"data\": [1, 2, 3]}"))
		.build();
HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());
```

**C#:**
```csharp
var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "<access-token>");
var content = new StringContent("{\"data\": [1, 2, 3]}", Encoding.UTF8, "application/json");
var response = await client.PostAsync("<scoring-uri>", content);
string result = await response.Content.ReadAsStringAsync();
```

**Node.js:**
```javascript
const axios = require('axios');
const response = await axios.post(
	'<scoring-uri>',
	{ data: [1, 2, 3] },
	{ headers: { Authorization: 'Bearer <access-token>' } }
);
```

**Best Practices:**
- Secure endpoints with authentication.
- Validate and sanitize input data.

---

## Cost Considerations for AI Services in Integration Solutions

- Monitor usage and set budgets/alerts.
- Use the right service tier for your workload.
- Optimize calls (batch requests, cache results where possible).

**Best Practices:**
- Regularly review billing and adjust usage.
- Use quotas and rate limits to prevent unexpected costs.
