# Creating HTTP-triggered functions with dependency injection

## Overview

In this lab, you will create an Azure Function in Java that is triggered by HTTP requests. You will also learn how to use dependency injection to manage services within your function.

## Prerequisites

- Java 11 or later
- [Azure Functions Core Tools](https://docs.microsoft.com/azure/azure-functions/functions-run-local)
- [Maven](https://maven.apache.org/)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)
- An Azure subscription

---

## Step 1: Create a new Azure Functions project

1. Open a terminal and create a new directory for your project:

    ```sh
    mkdir java-func-di-lab
    cd java-func-di-lab
    ```

2. Initialize a new Azure Functions project:

    ```sh
    mvn archetype:generate -DarchetypeGroupId=com.microsoft.azure -DarchetypeArtifactId=azure-functions-archetype
    ```

3. Follow the prompts to set groupId, artifactId, etc.

## Step 2: Add a service class

1. In `src/main/java/com/yourgroup/functions/`, create a new class called `GreetingService.java`:

    ```java
    public class GreetingService {
        public String greet(String name) {
            return "Hello, " + name + "!";
        }
    }
    ```

## Step 3: Register the service for dependency injection

1. In the same package, create a new class called `FunctionConfig.java`:

    ```java
    import com.microsoft.azure.functions.annotation.*;
    import com.microsoft.azure.functions.*;
    import com.microsoft.azure.functions.worker.binding.*;
    import com.microsoft.azure.functions.spi.inject.FunctionInstanceInjector;
    import com.microsoft.azure.functions.spi.inject.FunctionInstanceInjectorFactory;

    public class FunctionConfig implements FunctionInstanceInjectorFactory {
        @Override
        public FunctionInstanceInjector createInstance() {
            return new FunctionInstanceInjector() {
                @Override
                public <T> T getInstance(Class<T> clazz) {
                    if (clazz == GreetingService.class) {
                        return clazz.cast(new GreetingService());
                    }
                    throw new IllegalArgumentException("Unknown class: " + clazz);
                }
            };
        }
    }
    ```

2. Register the injector in `META-INF/services/com.microsoft.azure.functions.spi.inject.FunctionInstanceInjectorFactory`:

    ```
    com.yourgroup.functions.FunctionConfig
    ```

---

## Step 4: Update the Function to use dependency injection

1. Open `Function.java` (or the main function class) and update it to use the injected service:

    ```java
    public class Function {
        private final GreetingService greetingService;

        public Function(GreetingService greetingService) {
            this.greetingService = greetingService;
        }

        @FunctionName("HttpExample")
        public HttpResponseMessage run(
            @HttpTrigger(name = "req", methods = {HttpMethod.GET, HttpMethod.POST}, authLevel = AuthorizationLevel.ANONYMOUS)
            HttpRequestMessage<Optional<String>> request,
            final ExecutionContext context) {

            String name = request.getQueryParameters().get("name");
            if (name == null) {
                name = request.getBody().orElse("world");
            }
            String responseMessage = greetingService.greet(name);
            return request.createResponseBuilder(HttpStatus.OK).body(responseMessage).build();
        }
    }
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

3. Test the function:

    - Open a browser or use curl:

        ```
        curl http://localhost:7071/api/HttpExample?name=Azure
        ```

    - You should see: `Hello, Azure!`

---

## Step 6: Deploy to Azure

1. Log in to Azure:

    ```sh
    az login
    ```

2. Create a resource group and function app (if needed):

    ```sh
    az group create --name java-func-lab-rg --location westus
    az functionapp create --resource-group java-func-lab-rg --consumption-plan-location westus --runtime java --functions-version 4 --name <YOUR_FUNCTION_APP_NAME> --storage-account <YOUR_STORAGE_ACCOUNT>
    ```

3. Deploy your function:

    ```sh
    mvn azure-functions:deploy
    ```
