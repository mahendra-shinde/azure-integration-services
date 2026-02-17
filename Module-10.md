# Azure Security Best Practices
## Azure Security Fundamentals and Shared Responsibility Model

Azure operates on a shared responsibility model where security duties are split between Microsoft and the customer.

- Microsoft responsibilities
    - Physical datacenter security, host infrastructure, hypervisor, and foundational platform services.
    - Global network, physical connectivity, and underlying control plane.
    - Patching and securing managed platform components for PaaS/SaaS offerings.

- Customer responsibilities (examples)
    - Data: classification, encryption, and lifecycle management.
    - Identity and access: account management, RBAC, conditional access, MFA.
    - Applications: secure design, secure coding, dependency management.
    - Endpoints and OS (for IaaS): OS patching, host hardening, configuration.
    - Network controls: NSGs, firewalls, private endpoints, VNet design.
    - Monitoring, logging, alerting, and incident response for resources you control.
    - Backup, retention, and recovery of your application data.

- Shared responsibilities
    - Secure configurations, vulnerability management, and threat detection require collaboration and clear operational processes.
    - Compliance evidence and reporting often involve both parties' artifacts.

Responsibility by service type
- IaaS: Customer manages VMs, OS, apps, data, and network configs; Microsoft manages physical hosts, storage, and network fabric.
- PaaS: Microsoft manages OS and platform runtime; customer manages app code, data, identities, and configuration.
- SaaS: Microsoft manages the application platform and infrastructure; customer manages data, user access, and endpoint security.

Best practices
- Apply least privilege and role-based access.
- Require MFA and protect privileged accounts.
- Encrypt data at rest and in transit; use Key Vault for secrets and keys.
- Patch and harden OS and application dependencies promptly.
- Use network segmentation, private endpoints, and NSGs.
- Enable centralized logging, Azure Monitor/Defender, and automated alerts.
- Enforce policies with Azure Policy and governance controls.
- Integrate security scanning and CI/CD security checks.
- Regularly review and document responsibilities, run tabletop exercises, and validate compliance.

Understand these boundaries early when designing solutions and codify responsibilities in architecture, runbooks, and operational playbooks.

---

## Identity and Access Management with Azure AD/Entra ID

Azure Active Directory (Azure AD, now Entra ID) provides identity services for authentication and authorization.

**Key Features:**
- Single sign-on (SSO)
- Multi-factor authentication (MFA)
- Conditional access policies
- Role-based access control (RBAC)

**Example: Assigning a Role to a User (Azure CLI)**
```bash
az role assignment create --assignee <user-email> --role "Contributor" --scope /subscriptions/<sub-id>/resourceGroups/<rg-name>
```

**Best Practices:**
- Use least privilege principle.
- Require MFA for all users.
- Regularly review access assignments.

---

## Managed Identities for Azure Resources

Managed Identities allow Azure resources to authenticate to services without storing credentials.

**Example: Accessing Azure Resource with Managed Identity**

**Java:**
```java
DefaultAzureCredential credential = new DefaultAzureCredentialBuilder().build();
SecretClient client = new SecretClientBuilder()
	.vaultUrl("<key-vault-url>")
	.credential(credential)
	.buildClient();
String secret = client.getSecret("my-secret").getValue();
```

**C#:**
```csharp
var credential = new DefaultAzureCredential();
var client = new SecretClient(new Uri("<key-vault-url>"), credential);
KeyVaultSecret secret = await client.GetSecretAsync("my-secret");
```

**Node.js:**
```javascript
const { DefaultAzureCredential } = require("@azure/identity");
const { SecretClient } = require("@azure/keyvault-secrets");
const credential = new DefaultAzureCredential();
const client = new SecretClient("<key-vault-url>", credential);
const secret = await client.getSecret("my-secret");
```

**Best Practices:**
- Prefer managed identities over hardcoded credentials.
- Grant only necessary permissions to managed identities.

---

## Azure Key Vault for Secrets, Keys, and Certificates Management

Azure Key Vault securely stores secrets, keys, and certificates.

**Example: Storing and Retrieving a Secret**

**Java:**
```java
SecretClient client = new SecretClientBuilder()
	.vaultUrl("<key-vault-url>")
	.credential(new DefaultAzureCredentialBuilder().build())
	.buildClient();
client.setSecret("my-secret", "value");
String value = client.getSecret("my-secret").getValue();
```

**C#:**
```csharp
var client = new SecretClient(new Uri("<key-vault-url>"), new DefaultAzureCredential());
await client.SetSecretAsync("my-secret", "value");
KeyVaultSecret secret = await client.GetSecretAsync("my-secret");
string value = secret.Value;
```

**Node.js:**
```javascript
const { SecretClient } = require("@azure/keyvault-secrets");
const { DefaultAzureCredential } = require("@azure/identity");
const client = new SecretClient("<key-vault-url>", new DefaultAzureCredential());
await client.setSecret("my-secret", "value");
const secret = await client.getSecret("my-secret");
```

**Best Practices:**
- Never store secrets in code or config files.
- Use Key Vault references in app settings.

---

## Network Security: VNet Integration, Private Endpoints, NSGs

- **VNet Integration:** Connects resources securely within a virtual network.
- **Private Endpoints:** Provides private IP addresses for Azure services.
- **Network Security Groups (NSGs):** Control inbound/outbound traffic.

**Best Practices:**
- Restrict public access to resources.
- Use NSGs to allow only required traffic.
- Use private endpoints for sensitive services.

---

## Security Scanning and Vulnerability Management

Regularly scan your code and resources for vulnerabilities.

**Tools:**
- Azure Security Center
- Microsoft Defender for Cloud
- Static code analysis tools (e.g., SonarQube)

**Best Practices:**
- Integrate security scanning into CI/CD pipelines.
- Remediate vulnerabilities promptly.
- Monitor security recommendations in Azure Portal.

---

## Azure Policy and Compliance Frameworks

Azure Policy enforces organizational standards and compliance.

**Example: Assigning a Built-in Policy (Azure CLI)**
```bash
az policy assignment create --policy <policy-definition-id> --scope /subscriptions/<sub-id>/resourceGroups/<rg-name>
```

**Best Practices:**
- Use built-in policies for common requirements.
- Create custom policies for specific needs.
- Regularly audit compliance in the Azure Portal.
