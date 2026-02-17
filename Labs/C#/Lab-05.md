# Publishing APIs through API Management (C#)

## Lab Overview

In this lab, you'll publish an API using Azure API Management (APIM), secure it, apply policies, and automate testing using C#.

## Prerequisites

- Azure subscription
- API Management instance
- Sample backend API (e.g., HTTP endpoint)
- .NET 6 SDK (for test code)
- Visual Studio Code or Azure Portal

## Step 1: Import an API into APIM

1. In the Azure Portal, open your API Management instance.
2. Select **APIs** > **+ Add API**.
3. Choose **OpenAPI**, **WSDL**, or **HTTP**.
4. Provide the backend API URL and import the API.

## Step 2: Secure the API

- Add an **API key** or **JWT validation** policy.

## Step 3: Apply Transformation Policies

- Modify request or response using policies.

## Step 4: Set Rate Limiting

- Protect your API from abuse.

## Step 5: Test the API Using C#

Automate API calls and validate responses using C#.

```csharp
// C# pseudocode for testing APIM-published API
using System.Net.Http;
using System.Net.Http.Headers;

var client = new HttpClient();
var request = new HttpRequestMessage(HttpMethod.Get, "https://<apim-instance>.azure-api.net/myapi/resource");
request.Headers.Add("Ocp-Apim-Subscription-Key", "<your-subscription-key>");
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "<jwt-token>");

var response = await client.SendAsync(request);
response.EnsureSuccessStatusCode();
Console.WriteLine("Response: " + await response.Content.ReadAsStringAsync());
```

## Step 6: Monitor and Debug

- Use APIM analytics and logs to monitor API usage and troubleshoot issues.

## Clean Up

- Remove test APIs and policies to avoid unnecessary charges.

## Summary

You published an API through Azure API Management, secured it, applied policies, and validated its behavior using C#.
