# Publishing APIs through API Management

## Lab Overview

In this lab, you'll publish an API using Azure API Management (APIM), secure it, apply policies, and automate testing using Java.

## Prerequisites

- Azure subscription
- API Management instance
- Sample backend API (e.g., HTTP endpoint)
- Java JDK (for SDK/test code)
- Visual Studio Code or Azure Portal

## Step 1: Import an API into APIM

1. In the Azure Portal, open your API Management instance.
2. Select **APIs** > **+ Add API**.
3. Choose **OpenAPI**, **WSDL**, or **HTTP**.
4. Provide the backend API URL and import the API.

## Step 2: Secure the API

- Add an **API key** or **JWT validation** policy.
    ```xml
    <!-- Validate JWT token in inbound policy -->
    <inbound>
      <validate-jwt header-name="Authorization" failed-validation-httpcode="401" />
    </inbound>
    ```

## Step 3: Apply Transformation Policies

- Modify request or response using policies.
    ```xml
    <!-- Add a custom header -->
    <inbound>
      <set-header name="X-Request-ID" exists-action="override">
        <value>@(Guid.NewGuid().ToString())</value>
      </set-header>
    </inbound>
    ```

## Step 4: Set Rate Limiting

- Protect your API from abuse.
    ```xml
    <inbound>
      <rate-limit calls="5" renewal-period="60" />
    </inbound>
    ```

## Step 5: Test the API Using Java

Automate API calls and validate responses using Java.

**Example:**
```java
// filepath: d:\git\azure-app-integration\Lab-05.md
// Java pseudocode for testing APIM-published API
HttpClient client = HttpClient.newHttpClient();
HttpRequest request = HttpRequest.newBuilder()
    .uri(URI.create("https://<apim-instance>.azure-api.net/myapi/resource"))
    .header("Ocp-Apim-Subscription-Key", "<your-subscription-key>")
    .header("Authorization", "Bearer <jwt-token>")
    .build();

HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());
assert response.statusCode() == 200;
System.out.println("Response: " + response.body());
```

## Step 6: Monitor and Debug

- Use APIM analytics and logs to monitor API usage and troubleshoot issues.

## Clean Up

- Remove test APIs and policies to avoid unnecessary charges.


## Summary

You published an API through Azure API Management, secured it, applied policies, and validated its behavior using Java.