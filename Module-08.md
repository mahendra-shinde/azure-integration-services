# Azure DevOps for Integration Solutions
## Azure DevOps Overview: Repos, Pipelines, Boards, Artifacts

Azure DevOps is an integrated suite for planning, building, testing, and delivering software. It brings source control, CI/CD, work tracking, and package management into a single platform with enterprise security and integrations.

- Repos
    - Git repositories with pull requests, branch policies, code search, and file-level permissions.
    - Use branch strategies (main/prod, develop, feature/*, release/*) and enforce PR reviews + build validation.
    - Link commits and PRs to work items for traceability.

- Pipelines
    - YAML-first CI/CD for reproducible, versioned pipeline definitions; classic pipelines available for visual editing.
    - Build -> test -> publish artifacts -> deploy to environments (dev/stage/prod) with approvals, gates, and deployment conditions.
    - Use pipeline templates, task groups, and reusable stages to enforce consistency across projects.
    - Integrate test frameworks, code analysis (SonarQube), and security scans into pipeline stages.

- Boards
    - Agile planning: work items, backlogs, sprints, Kanban boards, queries, and dashboards.
    - Link user stories, tasks, bugs to commits/PRs and pipeline runs to provide end-to-end traceability.
    - Use area/iteration paths and automation rules to reflect team processes.

- Artifacts
    - Private package feeds (NuGet, npm, Maven, Python) for hosting and sharing build outputs and dependencies.
    - Promote packages across feeds/environments, control retention and upstream sources, and integrate package publishing into pipelines.

Typical workflow
- Developer creates a feature branch and pushes code to Repos.
- A PR triggers CI (lint/build/test). Branch policies block merging until checks pass and reviewers approve.
- Successful build publishes packages/artifacts to Artifacts feed.
- A release pipeline deploys to environments with infrastructure deployed via IaC (ARM/Bicep/Terraform), applying approvals and gates before production.
- Boards track work items and link to commits/PRs and deployment history.

Security and integrations
- Use service connections and managed identities for secure access to Azure subscriptions and external services.
- Enforce least privilege, branch protection, required reviewers, and pipeline permissions.
- Integrate Azure Key Vault for secret management; avoid storing secrets in repos or pipeline variables.
- Connect Azure DevOps to GitHub, Azure AD, and external monitoring/alerting systems for end-to-end traceability.

Best Practices
- Prefer YAML pipelines checked into source control and use templates for reuse.
- Enforce branch policies (CI, required reviewers, work item linking).
- Automate IaC deployment before app deployment and run automated tests and security scans in CI.
- Use environments, approvals, and deployment gates for safe releases.
- Publish and version packages in Artifacts; set retention and upstream rules.
- Link Boards items to code and deployments for auditability and metrics.
- Regularly review service connections, feed permissions, and pipeline secrets.
- Monitor pipeline run times and failures; keep pipelines fast and deterministic.
- Document pipeline inputs/outputs and standardize naming and folder structures across repos.
- Use branches and pull requests for code quality.
- Automate builds and deployments with pipelines.
- Track work items and bugs in Boards.
- Store and version packages in Artifacts.


## Source Control Strategies for Serverless and Integration Projects

Source control is essential for managing code, configuration, and infrastructure as code (IaC) for Logic Apps, Azure Functions, and API Management.

**Strategies:**
- Use Git for versioning all code and configuration.
- Organize repositories by service or solution.
- Use branch policies to enforce code reviews and build validation.

**Example: .gitignore for Azure Functions**
```
# Ignore build output and local settings
bin/
obj/
local.settings.json
```

**Best Practices:**
- Store ARM/Bicep templates or Terraform scripts in source control.
- Use feature branches for new work.
- Protect main/master branch with policies.

---

## CI/CD Pipeline Design for Logic Apps, Functions, and API Management

Continuous Integration/Continuous Deployment (CI/CD) automates building, testing, and deploying integration solutions.

**Example: Azure Pipeline YAML for Deploying a Function App**

**Java:**
```yaml
trigger:
- main

pool:
	vmImage: 'ubuntu-latest'

steps:
- task: Maven@3
	inputs:
		mavenPomFile: 'pom.xml'
		goals: 'package'
- task: AzureFunctionApp@1
	inputs:
		appType: 'java'
		appName: 'my-function-app'
		package: '$(System.DefaultWorkingDirectory)/target/*.jar'
```

**C#:**
```yaml
trigger:
- main

pool:
	vmImage: 'windows-latest'

steps:
- task: DotNetCoreCLI@2
	inputs:
		command: 'publish'
		publishWebProjects: true
		arguments: '--configuration Release --output $(Build.ArtifactStagingDirectory)'
- task: AzureFunctionApp@1
	inputs:
		appType: 'functionApp'
		appName: 'my-function-app'
		package: '$(Build.ArtifactStagingDirectory)/**/*.zip'
```

**Node.js:**
```yaml
trigger:
- main

pool:
	vmImage: 'ubuntu-latest'

steps:
- task: NodeTool@0
	inputs:
		versionSpec: '18.x'
- script: npm install
- script: npm run build
- task: AzureFunctionApp@1
	inputs:
		appType: 'functionApp'
		appName: 'my-function-app'
		package: '$(System.DefaultWorkingDirectory)/**/*.zip'
```

**Best Practices:**
- Use pipeline templates for consistency.
- Automate infrastructure deployment (IaC) before app deployment.
- Run tests as part of the pipeline.

---

## Variable Groups and Secure Secrets Management with Key Vault

Variable groups in Azure Pipelines allow you to manage and reuse values across pipelines. For sensitive data, integrate with Azure Key Vault.

**Example: Linking Variable Group to Key Vault**
```yaml
variables:
- group: my-variable-group

steps:
- task: AzureKeyVault@2
	inputs:
		azureSubscription: 'MyServiceConnection'
		KeyVaultName: 'my-keyvault'
		SecretsFilter: '*'
```

**Accessing Secrets in Code:**

**Java:**
```java
// Using Azure Identity and Key Vault SDK
SecretClient secretClient = new SecretClientBuilder()
		.vaultUrl("<key-vault-url>")
		.credential(new DefaultAzureCredentialBuilder().build())
		.buildClient();
String secret = secretClient.getSecret("my-secret").getValue();
```

**C#:**
```csharp
var client = new SecretClient(new Uri("<key-vault-url>"), new DefaultAzureCredential());
KeyVaultSecret secret = await client.GetSecretAsync("my-secret");
string value = secret.Value;
```

**Node.js:**
```javascript
const { SecretClient } = require("@azure/keyvault-secrets");
const { DefaultAzureCredential } = require("@azure/identity");
const client = new SecretClient("<key-vault-url>", new DefaultAzureCredential());
const secret = await client.getSecret("my-secret");
```

**Best Practices:**
- Never store secrets in source control.
- Use Key Vault for all sensitive configuration.
- Limit access to Key Vault using RBAC.
