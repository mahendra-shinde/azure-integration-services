# Implementing queue-based communication patterns

## Lab Overview

In this hands-on lab, you will learn how to implement queue-based communication patterns using Java and Azure Service Bus. You will create a simple producer and consumer application that sends and receives messages via an Azure Service Bus queue.

## Prerequisites

- Java JDK 11 or later
- Maven
- Azure Subscription
- Azure Service Bus namespace and queue
- Visual Studio Code or any Java IDE

## Step 1: Set Up Azure Service Bus

1. Log in to the [Azure Portal](https://portal.azure.com).
2. Create a new Service Bus namespace.
3. Within the namespace, create a queue (e.g., `demoqueue`).
4. Obtain the connection string for the namespace:
   - Go to "Shared access policies" > "RootManageSharedAccessKey".
   - Copy the "Primary Connection String".

## Step 2: Create a Maven Project

```bash
mvn archetype:generate -DgroupId=com.example.servicebus -DartifactId=servicebus-demo -DarchetypeArtifactId=maven-archetype-quickstart -DinteractiveMode=false
cd servicebus-demo
```

## Step 3: Add Azure Service Bus SDK Dependency

Edit `pom.xml` and add:

```xml
<!-- ...existing code... -->
<dependency>
    <groupId>com.azure</groupId>
    <artifactId>azure-messaging-servicebus</artifactId>
    <version>7.14.0</version>
</dependency>
<!-- ...existing code... -->
```

## Step 4: Implement the Queue Producer

Create a new Java class `QueueProducer.java` in `src/main/java/com/example/servicebus/`:

```java
import com.azure.messaging.servicebus.*;

public class QueueProducer {
    private static final String CONNECTION_STRING = "<YOUR_SERVICE_BUS_CONNECTION_STRING>";
    private static final String QUEUE_NAME = "demoqueue";

    public static void main(String[] args) {
        ServiceBusSenderClient senderClient = new ServiceBusClientBuilder()
            .connectionString(CONNECTION_STRING)
            .sender()
            .queueName(QUEUE_NAME)
            .buildClient();

        for (int i = 1; i <= 5; i++) {
            senderClient.sendMessage(new ServiceBusMessage("Message " + i));
            System.out.println("Sent: Message " + i);
        }

        senderClient.close();
    }
}
```

## Step 5: Implement the Queue Consumer

Create a new Java class `QueueConsumer.java` in `src/main/java/com/example/servicebus/`:

```java
import com.azure.messaging.servicebus.*;

public class QueueConsumer {
    private static final String CONNECTION_STRING = "<YOUR_SERVICE_BUS_CONNECTION_STRING>";
    private static final String QUEUE_NAME = "demoqueue";

    public static void main(String[] args) {
        ServiceBusReceiverClient receiverClient = new ServiceBusClientBuilder()
            .connectionString(CONNECTION_STRING)
            .receiver()
            .queueName(QUEUE_NAME)
            .buildClient();

        receiverClient.receiveMessages(5).forEach(message -> {
            System.out.println("Received: " + message.getBody().toString());
            receiverClient.complete(message);
        });

        receiverClient.close();
    }
}
```

## Step 6: Run the Applications

1. Replace `<YOUR_SERVICE_BUS_CONNECTION_STRING>` with your actual connection string in both classes.
2. Compile the project:

   ```bash
   mvn clean package
   ```

3. Run the producer:

   ```bash
   java -cp target/servicebus-demo-1.0-SNAPSHOT.jar com.example.servicebus.QueueProducer
   ```

4. Run the consumer:

   ```bash
   java -cp target/servicebus-demo-1.0-SNAPSHOT.jar com.example.servicebus.QueueConsumer
   ```

## Step 7: Clean Up Resources

- Delete the Service Bus namespace from the Azure portal to avoid charges.

## Summary

You have implemented a simple queue-based communication pattern using Java and Azure Service Bus. This pattern is useful for decoupling application components and enabling reliable message delivery.
