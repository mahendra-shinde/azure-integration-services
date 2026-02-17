# Building a Multi-Step Approval Workflow in Logic App

## Lab Overview

In this lab, you'll create a multi-step approval workflow using Azure Logic Apps. You'll integrate with external systems, use control flow actions, and automate testing using Java code snippets.

## Prerequisites

- Azure subscription
- Logic Apps resource (Standard or Consumption)
- Visual Studio Code with Azure Logic Apps extension
- Java JDK (for SDK/test code)
- Sample API endpoint for approval

## Step 1: Create a Logic App Workflow

1. In the Azure Portal, create a new Logic App (Standard).
2. Open the Logic App Designer.

## Step 2: Add a Trigger

- Choose the **HTTP Request** trigger.
- Define the JSON schema for the approval request:
    ```json
    {
      "type": "object",
      "properties": {
        "requestId": { "type": "string" },
        "amount": { "type": "number" },
        "approverEmail": { "type": "string" }
      }
    }
    ```

## Step 3: Add Approval Steps

- Add an **Outlook 365 - Send Email** action to notify the approver.
- Add a **Condition** to check approval status:
    ```json
    "actions": {
      "Condition": {
        "type": "If",
        "expression": { "equals": [ "@triggerBody()?['status']", "Approved" ] },
        "actions": { ... }
      }
    }
    ```
- Add a **ForEach** loop for multi-level approval:
    ```json
    "actions": {
      "ForEach": {
        "type": "Foreach",
        "foreach": "@triggerBody()?['approvers']",
        "actions": { ... }
      }
    }
    ```

## Step 4: Integrate with External API

- Use the **HTTP** built-in connector to call an external approval API.
- Map workflow data to API request body.

## Step 5: Add Error Handling

- Use **Scope** actions to group steps.
- Configure **Run After** for error handling.

## Step 6: Instrument Telemetry

- Enable Application Insights in Logic App settings.
- Add custom tracking IDs to actions.

## Step 7: Automated Testing with Java

Use Java to automate workflow testing with mocked inputs and outputs.

**Example:**
```java
// filepath: d:\git\azure-app-integration\Lab-03.md
// Java pseudocode for Logic App workflow test
LogicAppTestRunner testRunner = new LogicAppTestRunner();
Map<String, Object> testInput = new HashMap<>();
testInput.put("requestId", "REQ123");
testInput.put("amount", 1500);
testInput.put("approverEmail", "manager@contoso.com");

// Mock the approval API response
Map<String, Object> mockResponse = new HashMap<>();
mockResponse.put("status", "Approved");
testRunner.mockOperation("ApprovalAPI", mockResponse);

// Run the workflow and validate output
WorkflowResult result = testRunner.runWorkflow("ApprovalWorkflow", testInput);
assert result.getOutput().get("finalStatus").equals("Approved");
```

## Step 8: Deploy and Monitor

- Deploy the workflow using VS Code or Azure Portal.
- Monitor workflow runs and approval status in Application Insights.

## Clean Up

- Delete Logic App resources to avoid charges.

## Summary

You built a multi-step approval workflow, integrated with external APIs, added error handling and telemetry, and automated tests using Java.