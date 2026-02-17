# Setting up Rate Limiting and Quota Policies

## Lab Overview

In this lab, you'll configure rate limiting and quota policies in Azure API Management (APIM) and automate API testing using Java.

## Prerequisites

- Azure subscription
- API Management instance
- Sample API published in APIM
- Java JDK (for SDK/test code)
- Visual Studio Code or Azure Portal

## Step 1: Configure Rate Limiting Policy

1. In the Azure Portal, open your API Management instance.
2. Go to **APIs** and select your API.
3. In the **Design** tab, select the **Inbound processing** section.
4. Add the following policy to limit requests:
    ```xml
    <inbound>
      <rate-limit calls="10" renewal-period="60" />
    </inbound>
    ```

## Step 2: Configure Quota Policy

- Add a quota policy to restrict total calls per day:
    ```xml
    <inbound>
      <quota calls="100" renewal-period="86400" />
    </inbound>
    ```

## Step 3: Combine Rate Limiting and Quota Policies

- You can combine both policies in the inbound section:
    ```xml
    <inbound>
      <rate-limit calls="10" renewal-period="60" />
      <quota calls="100" renewal-period="86400" />
    </inbound>
    ```

## Step 4: Test Policies Using Java

Automate API calls to verify rate limiting and quota enforcement.

**Example:**
```java
// filepath: d:\git\azure-app-integration\Lab-06.md
// Java pseudocode for testing APIM rate limiting and quota
HttpClient client = HttpClient.newHttpClient();
String apiUrl = "https://<apim-instance>.azure-api.net/myapi/resource";
String subscriptionKey = "<your-subscription-key>";

for (int i = 1; i <= 15; i++) {
    HttpRequest request = HttpRequest.newBuilder()
        .uri(URI.create(apiUrl))
        .header("Ocp-Apim-Subscription-Key", subscriptionKey)
        .build();
    HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());
    System.out.println("Call " + i + ": Status " + response.statusCode());
    // Expect status 429 (Too Many Requests) after exceeding rate limit
}
```

## Step 5: Monitor and Debug

- Use APIM analytics to view throttled requests and quota usage.
- Check response headers for rate limit and quota information.


## Clean Up

- Remove or adjust policies as needed to avoid blocking legitimate traffic.

## Summary

You configured rate limiting and quota policies in APIM and validated their enforcement using Java.
