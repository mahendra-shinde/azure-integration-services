# Setting up Rate Limiting and Quota Policies (C#)

## Lab Overview

In this lab, you'll configure rate limiting and quota policies in Azure API Management (APIM) and automate API testing using C#.

## Prerequisites

- Azure subscription
- API Management instance
- Sample API published in APIM
- .NET 6 SDK (for test code)
- Visual Studio Code or Azure Portal

## Step 1: Configure Rate Limiting Policy

1. In the Azure Portal, open your API Management instance.
2. Go to **APIs** and select your API.
3. In the **Design** tab, select the **Inbound processing** section.
4. Add the following policy to limit requests.

## Step 2: Configure Quota Policy

- Add a quota policy to restrict total calls per day.

## Step 3: Combine Rate Limiting and Quota Policies

- You can combine both policies in the inbound section.

## Step 4: Test Policies Using C#

Automate API calls to verify rate limiting and quota enforcement.

```csharp
// C# pseudocode for testing APIM rate limiting and quota
using System.Net.Http;

var client = new HttpClient();
string apiUrl = "https://<apim-instance>.azure-api.net/myapi/resource";
string subscriptionKey = "<your-subscription-key>";

for (int i = 1; i <= 15; i++)
{
    var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
    request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
    var response = await client.SendAsync(request);
    Console.WriteLine($"Call {i}: Status {response.StatusCode}");
    // Expect status 429 (Too Many Requests) after exceeding rate limit
}
```

## Step 5: Monitor and Debug

- Use APIM analytics to view throttled requests and quota usage.
- Check response headers for rate limit and quota information.

## Clean Up

- Remove or adjust policies as needed to avoid blocking legitimate traffic.

## Summary

You configured rate limiting and quota policies in APIM and validated their enforcement using C#.
