# Data transformation using Liquid templates (C#)

## Lab Overview

In this lab, you'll use Azure Logic Apps to perform data transformation with Liquid templates. You'll also automate testing of your transformation logic using C#.

## Prerequisites

- Azure subscription
- Logic Apps resource (Standard or Consumption)
- Visual Studio  Code with Logic Apps extension
- .NET 6 SDK (for test code)
- Sample JSON data

## Step 1: Create a Logic App Workflow

1. In the Azure Portal, create a new Logic App (Standard).
2. Open the Logic App Designer.

## Step 2: Add a Trigger

- Use the **HTTP Request** trigger.
- Define a sample JSON schema for incoming data.

## Step 3: Add a Data Transformation Action

- Add a **Transform JSON to JSON** action.
- Select **Liquid** as the transformation language.
- Create a new Liquid template.

## Step 4: Test the Transformation

- Use the Logic App run history to view the transformed output.

## Step 5: Automated Testing with C#

Automate the transformation test using C#.

```csharp
// C# pseudocode for testing Liquid transformation in Logic App
var testRunner = new LogicAppTestRunner();
var testInput = new Dictionary<string, object>
{
    { "orderId", "ORD1001" },
    { "customer", "Alice" },
    { "items", new List<Dictionary<string, object>>
        {
            new Dictionary<string, object> { { "sku", "A123" }, { "quantity", 2 } }
        }
    }
};

// Run the workflow and validate transformation output
var result = testRunner.RunWorkflow("DataTransformWorkflow", testInput);
var output = result.Output;
if ((string)output["OrderId"] != "ORD1001" || (string)output["CustomerName"] != "Alice")
    throw new Exception("Transformation failed");
```

## Step 6: Monitor and Debug

- Use Application Insights to monitor workflow runs and transformation results.

## Clean Up

- Delete Logic App resources to avoid charges.

## Summary

You performed data transformation using Liquid templates in Logic Apps and validated the transformation with C#-based automated tests.
