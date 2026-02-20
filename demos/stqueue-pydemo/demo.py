from azure.storage.queue import QueueServiceClient


conn_string = "DefaultEndpointsProtocol=https;AccountName=appgroupbf0e;AccountKey=hBWC8sFnEN5pPAYF1gMj2WcICcYdGLejP+d2w3LyeW1793bH0sW/5gTEqjoOiEK8h2Qt8iEPubYC+ASt6gmzbg==;EndpointSuffix=core.windows.net"
queue_name = "orders"

# Create the QueueServiceClient object which will be used to create a client for the Queue
queue_service_client = QueueServiceClient.from_connection_string(conn_string)

# Create a QueueClient object which will be used to interact with the Queue
queue_client = queue_service_client.get_queue_client(queue_name)

# Read one message from the Queue
messages = queue_client.receive_messages()
print ("Reading messages from the Queue...")

for message in messages:
    print("Message content: " + message.content)
    # Delete the message from the Queue
    queue_client.delete_message(message)