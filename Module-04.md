# Azure API Management

## API Management Architecture and Components

Azure API Management (APIM) is a turnkey solution for publishing, securing, transforming, maintaining, and monitoring APIs. It acts as a gateway between backend services and API consumers.

**Key Components:**
- **Gateway (API Gateway):** The runtime proxy that receives client calls, enforces policies (auth, rate limits, transformations), routes to backends, and returns responses.
- **Developer Portal:** Customizable, documentation-driven site where consumers discover APIs, view samples, try APIs (interactive console), and obtain subscription keys.
- **Publisher Portal (Azure Portal):** Management plane UI for publishers to create/configure APIs, products, policies, backends, named values, and monitor usage.
- **Management API:** REST API for automating APIM tasks (create/update APIs, products, users, policies) and integrating with CI/CD pipelines.
- **Products:** Logical groupings of one or more APIs with associated quotas, rate limits, and approval flows used to package and expose APIs to consumers.
- **Subscriptions:** Keys issued to consumers (per product) that control access, quota enforcement, and metering for billing/monitoring.
- **Policies:** XML-based, pipeline-executing statements (inbound/backend/outbound/on-error) for security, transformation, routing, caching, and rate/quota enforcement.
- **Backends:** Configured service endpoints (HTTP, Function App, Logic App, App Service, service URI) with settings for credentials, managed identity, and certificate authentication.
- **Revisions & Versions:** Mechanisms for safe change management—revisions for non-breaking updates and versioning strategies (path, query, header) for breaking changes.
- **Named Values:** Reusable key/value pairs (secrets, endpoints) referenced in policies and APIs; can be marked secret and linked to Key Vault.
- **Loggers & Diagnostics:** Integrations (e.g., Application Insights, Azure Monitor) for request/response logging, traceability, dashboards, and alerting.
- **Caching & Throttling:** Response caching and throttling policies to improve performance and protect backends under load.
- **Security & Identity:** Support for OAuth2/OpenID Connect, JWT validation, client certificates, subscription keys, and managed identities for backend auth.
- **Importers & Formats:** Import from OpenAPI/Swagger, WSDL, and other specs; support for SOAP-to-REST transformations and schema validation.
- **Network & Deployment:** VNet integration, custom domains, SSL certificates, multi-region deployment options, and Git-backed configuration for CI/CD.
- **Analytics & Monitoring:** Built-in analytics for usage, latency, and errors; exportable telemetry for chargeback and capacity planning.
- **Best Practices (brief):**
  - Model APIs as products for lifecycle control.
  - Use policies for cross-cutting concerns, keep them simple and version-controlled.
  - Store secrets in Named Values or Key Vault and use managed identities for backends.
  - Enable diagnostics and integrate with Application Insights for observability.

**Best Practices:**
- Use the Developer Portal to onboard and support API consumers.
- Automate APIM configuration using the Management API and infrastructure-as-code tools.

## API Versioning and Revision Strategies

Managing changes to APIs is crucial for backward compatibility and smooth consumer experience.

**Versioning Strategies:**

- **Path-based**
  - Pattern: /v1/orders, /v2/orders
  - Pros: Simple to understand, easy to cache, easy to route and document, works with browsers and proxies.
  - Cons: URL changes for clients when upgrading; can clutter routes.
  - When to use: Breaking changes where explicit client opt-in is desired.

- **Query-string**
  - Pattern: /orders?api-version=1.0
  - Pros: Easy for clients to pass version; simple to implement.
  - Cons: Less cache friendly by intermediaries; some tools/clients may ignore query parameters; less RESTful semantics.
  - When to use: Minor or experimental versions where path changes are undesirable.

- **Header-based**
  - Pattern: Header "api-version: 1.0" (or custom Accept media type)
  - Pros: Clean URLs, expressive (can use content negotiation), no route proliferation.
  - Cons: Harder to test from browsers, can be invisible in simple logs, some intermediaries may strip headers.
  - When to use: When you need clean URLs and clients can set headers reliably.

- **Media-type (Content Negotiation)**
  - Pattern: Accept: application/vnd.myapi.v1+json
  - Pros: Encodes version in media type; aligns with content negotiation semantics.
  - Cons: More complex for clients; tooling support varies.

- **Semantic vs. Calendar Versioning**
  - Use semantic versioning for API feature/compatibility expectations (major = breaking).
  - Keep version numbers simple (major.minor) for public API readability.

- **Revisions vs Versions**
  - Use APIM revisions for non-breaking, internal updates (safe to swap traffic).
  - Use versions for breaking changes that require explicit client migration.

- **Routing & Implementation in APIM**
  - APIM natively supports path, query, and header versioning—choose based on the pros/cons above.
  - Example approaches:
    - Path: create separate API versions with different URL suffixes.
    - Query/Header: use APIM versioning settings or policies to route to the correct backend.
  - Consider using Named Values to map versioned backends and avoid duplicating policies.

- **Best Practices**
  - Decide on a single versioning strategy and document it clearly for consumers.
  - Communicate deprecation timelines and provide migration guides and changelogs.
  - Use revisions for quick rollbacks and non-breaking fixes; reserve versions for breaking changes.
  - Expose current version info in responses (e.g., X-API-Version) and document supported versions in the Developer Portal.
  - Automate tests per API version and include version checks in CI/CD.
  - Group versions into APIM Products to manage quotas, subscriptions, and visibility.

- **Example policy snippets**
  - Header-based routing (conceptual):
    ```xml
    <inbound>
      <choose>
        <when condition="@(context.Request.Headers.GetValueOrDefault("api-version") == "2.0")">
          <set-backend-service base-url="https://backend-v2.example.com" />
        </when>
        <otherwise>
          <set-backend-service base-url="https://backend-v1.example.com" />
        </otherwise>
      </choose>
    </inbound>
    ```
  - Query string extract:
    ```xml
    <inbound>
      <set-variable name="ver" value="@(context.Request.Url.Query.GetValueOrDefault("api-version","1.0"))" />
    </inbound>
    ```

- **Migration guidance**
  - Provide clear migration steps, sample requests, and SDK updates.
  - Deprecate older versions gradually: announce → monitor usage → disable new subscriptions → remove after retirement date.
  - Preserve backward compatibility where feasible and use feature flags for gradual rollouts.

**Revisions:**
- Allow you to make non-breaking changes and test them before making them current.
- Each revision can be tested and rolled back if needed.

**Example:**
- Create a new version of an API for breaking changes.
- Use revisions for minor, non-breaking updates.

**Best Practices:**
- Communicate versioning strategy to API consumers.
- Deprecate old versions with clear timelines.


## Policy Expressions and Transformations

Policies are XML-based statements that execute at different stages of the API request/response pipeline.

**Policy Types:**
- **Inbound:** Applied before the request is forwarded to the backend.
- **Backend:** Applied when communicating with the backend service.
- **Outbound:** Applied before the response is sent to the client.
- **On-error:** Applied when an error occurs.

**Common Policy Examples:**
- **Transformation:** Modify request/response formats (e.g., XML to JSON).
    ```xml
    <!-- Inbound policy to convert XML to JSON -->
    <inbound>
      <xml-to-json apply="always" consider-accept-header="false" />
    </inbound>
    ```
- **Set headers:**
    ```xml
    <inbound>
      <set-header name="X-Request-ID" exists-action="override">
        <value>@(Guid.NewGuid().ToString())</value>
      </set-header>
    </inbound>
    ```
- **Rewrite URLs:**
    ```xml
    <inbound>
      <rewrite-uri template="/newpath/{param}" />
    </inbound>
    ```

**Best Practices:**
- Keep policies simple and maintainable.
- Use policy expressions for dynamic behavior (e.g., context variables).

## Rate Limiting, Quota Management, and Throttling Policies

Protect your APIs from abuse and ensure fair usage.

**Rate Limiting:**
- Restricts the number of calls per time interval.
    ```xml
    <inbound>
      <rate-limit calls="10" renewal-period="60" />
    </inbound>
    ```

**Quota Management:**
- Limits the total number of calls over a longer period (e.g., per day).
    ```xml
    <inbound>
      <quota calls="1000" renewal-period="86400" />
    </inbound>
    ```

**Throttling:**
- Temporarily slows down or rejects requests when limits are exceeded.

**Best Practices:**
- Apply rate limits and quotas at product or API level.
- Communicate limits to consumers via response headers.

## Integration with Azure Functions and Logic Apps

APIM can expose Azure Functions and Logic Apps as APIs, providing a unified endpoint and applying policies.

**Integration Steps:**
1. Import the Azure Function or Logic App as an API in APIM.
2. Secure the backend with function keys or managed identities.
3. Apply policies for transformation, authentication, or throttling.

**Example:**
- Import a Logic App HTTP endpoint and add a policy to validate JWT tokens.
    ```xml
    <inbound>
      <validate-jwt header-name="Authorization" failed-validation-httpcode="401" />
    </inbound>
    ```

**Best Practices:**
- Use APIM to centralize security and monitoring for serverless endpoints.
- Hide backend URLs from consumers by exposing only the APIM endpoint.

## Summary

Azure API Management provides a robust platform for API publishing, security, transformation, and monitoring. Use versioning, policies, and integration features to deliver reliable and scalable APIs.
