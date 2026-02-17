# Implementing queue-triggered batch processing

## Overview

In this lab, you will create an Azure Function in Java that is triggered by messages in an Azure Storage Queue. The function will process messages in batches, demonstrating how to handle batch processing scenarios.

## Prerequisites

- Java 11 or later
- [Azure Functions Core Tools](https://docs.microsoft.com/azure/azure-functions/functions-run-local)
- [Maven](https://maven.apache.org/)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)
- An Azure subscription
- [Azurite](https://docs.microsoft.com/azure/storage/common/storage-use-azurite) (for local Storage Emulator) or an Azure Storage Account

---

## Step 1: Create a new Azure Functions project

1. Open a terminal and create a new directory for your project:

    ```sh
    mkdir java-queue-batch-lab
    cd java-queue-batch-lab
    ```

2. Initialize a new Azure Functions project:

    ```sh
    mvn archetype:generate -DarchetypeGroupId=com.microsoft.azure -DarchetypeArtifactId=azure-functions-archetype
    ```

3. Follow the prompts to set groupId, artifactId, etc.

---

## Step 2: Add a queue-triggered function

1. In `src/main/java/com/yourgroup/functions/`, open or create `QueueBatchFunction.java`:

    ```java
    import com.microsoft.azure.functions.annotation.*;
    import com.microsoft.azure.functions.*;
    import java.util.List;

    public class QueueBatchFunction {
        @FunctionName("QueueBatchProcessor")
        public void run(
            @QueueTrigger(
                name = "messages",
                queueName = "batch-items",
                connection = "AzureWebJobsStorage",
                dataType = "string",
                cardinality = Cardinality.MANY // Enables batch processing
            )
            List<String> messages,
            final ExecutionContext context) {

            context.getLogger().info("Batch size: " + messages.size());
            for (String message : messages) {
                context.getLogger().info("Processing message: " + message);
                // Add your processing logic here
            }
        }
    }
    ```

---

## Step 3: Configure local.settings.json

1. Ensure your `local.settings.json` contains the storage connection string:

    ```json
    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "java"
      }
    }
    ```

---

## Step 4: Create and populate the queue

1. If using Azurite, start it:

    ```sh
    azurite
    ```

2. Use Azure Storage Explorer or Azure CLI to create a queue named `batch-items`.

3. Add several messages to the queue for batch processing.

    Example using Azure CLI:

    ```sh
    az storage message put --queue-name batch-items --account-name <your_storage_account> --content "Message 1"
    az storage message put --queue-name batch-items --account-name <your_storage_account> --content "Message 2"
    az storage message put --queue-name batch-items --account-name <your_storage_account> --content "Message 3"
    ```

---

## Step 5: Run and test the function locally

1. Build the project:

    ```sh
    mvn clean package
    ```

2. Start the function locally:

    ```sh
    mvn azure-functions:run
    ```

3. Observe the logs to see batch processing in action.

---

## Step 6: Deploy to Azure

1. Log in to Azure:

    ```sh
    az login
    ```

2. Create a resource group and function app (if needed):

    ```sh
    az group create --name java-queue-batch-rg --location westus
    az functionapp create --resource-group java-queue-batch-rg --consumption-plan-location westus --runtime java --functions-version 4 --name <YOUR_FUNCTION_APP_NAME> --storage-account <YOUR_STORAGE_ACCOUNT>
    ```

3. Deploy your function:

    ```sh
    mvn azure-functions:deploy
    ```

4. Add messages to the Azure queue and verify processing in the Azure portal or via logs.

---

## Conclusion

You have implemented a queue-triggered Azure Function in Java that processes messages in batches. This pattern is useful for high-throughput scenarios and efficient resource utilization.

---