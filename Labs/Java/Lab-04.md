# Data transformation using Liquid templates

## Lab Overview

In this lab, you'll use Azure Logic Apps to perform data transformation with Liquid templates. You'll also automate testing of your transformation logic using Java.

## Prerequisites

- Azure subscription
- Logic Apps resource (Standard or Consumption)
- Visual Studio Code with Azure Logic Apps extension
- Java JDK (for SDK/test code)
- Sample JSON data

## Step 1: Create a Logic App Workflow

1. In the Azure Portal, create a new Logic App (Standard).
2. Open the Logic App Designer.

## Step 2: Add a Trigger

- Use the **HTTP Request** trigger.
- Define a sample JSON schema for incoming data:
    ```json
    {
      "type": "object",
      "properties": {
        "orderId": { "type": "string" },
        "customer": { "type": "string" },
        "items": {
          "type": "array",
          "items": {
            "type": "object",
            "properties": {
              "sku": { "type": "string" },
              "quantity": { "type": "integer" }
            }
          }
        }
      }
    }
    ```

## Step 3: Add a Data Transformation Action

- Add a **Transform JSON to JSON** action.
- Select **Liquid** as the transformation language.
- Create a new Liquid template, for example:
    ```liquid
    {
      "OrderId": "{{content.orderId}}",
      "CustomerName": "{{content.customer}}",
      "LineItems": [
        {% for item in content.items %}
        {
          "SKU": "{{item.sku}}",
          "Qty": {{item.quantity}}
        }{% unless forloop.last %},{% endunless %}
        {% endfor %}
      ]
    }
    ```

## Step 4: Test the Transformation

- Use the Logic App run history to view the transformed output.

## Step 5: Automated Testing with Java

Automate the transformation test using Java.

**Example:**
```java
// filepath: d:\git\azure-app-integration\Lab-04.md
// Java pseudocode for testing Liquid transformation in Logic App
LogicAppTestRunner testRunner = new LogicAppTestRunner();
Map<String, Object> testInput = new HashMap<>();
testInput.put("orderId", "ORD1001");
testInput.put("customer", "Alice");
List<Map<String, Object>> items = new ArrayList<>();
Map<String, Object> item1 = new HashMap<>();
item1.put("sku", "A123");
item1.put("quantity", 2);
items.add(item1);
testInput.put("items", items);

// Run the workflow and validate transformation output
WorkflowResult result = testRunner.runWorkflow("DataTransformWorkflow", testInput);
Map<String, Object> output = result.getOutput();
assert output.get("OrderId").equals("ORD1001");
assert output.get("CustomerName").equals("Alice");
```

## Step 6: Monitor and Debug

- Use Application Insights to monitor workflow runs and transformation results.

## Clean Up

- Delete Logic App resources to avoid charges.

## Summary

You performed data transformation using Liquid templates in Logic Apps and validated the transformation with Java-based automated tests.
