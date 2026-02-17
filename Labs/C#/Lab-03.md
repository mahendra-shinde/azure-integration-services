# Lab: Detecting Blob Uploads with Azure Logic App

## Objective

In this lab, you will create an Azure Storage Account and a Logic App that detects new blob uploads and sends a message to a storage queue.

## Step 1: Create an Azure Storage Account

1. Sign in to the [Azure Portal](https://portal.azure.com).
2. In the search bar, type **Storage accounts** and select it.
3. Click **Create**.
4. In the **Resource group** dropdown, click **Create new** and enter a name (e.g., `day1`).
5. Enter a globally unique, alphanumeric **Storage account name**.
6. Select a **Region** (e.g., Central India or East US).
7. For **Performance**, select **Standard**.
8. For **Redundancy**, choose **Locally-redundant storage (LRS)**.
9. Click **Review + Create**, then **Create**.
10. After deployment, note your storage account name and resource group name.
11. In the storage account, navigate to **Settings > Access keys**. Click **Show** and copy the first access key.
12. Go to **Data storage > Containers**, click **Add Container**, enter `images` as the name, and click **Create**.

## Step 2: Create a Logic App

1. In the Azure Portal, search for **Logic Apps** and click **Create**.
2. For **Type**, select **Multi-tenant (Consumption)**.
3. Choose the resource group created earlier (e.g., `day1`).
4. Enter a **Logic App name** (e.g., `detect-blob`).
5. Select the same **Region** as your resource group.
6. Click **Review + Create**, then **Create**.
7. Once deployment is complete, open your Logic App resource.
8. Under **Development Tools**, select **Logic App Designer**.
9. Click **Add a trigger**.
10. Search for and select **When a blob is added or modified (properties only) (V2)**.
11. For the connection:
	- Accept the default connection name.
	- Set **Authentication type** to **Access Key**.
	- Enter your **Storage account name** and **Access key**.
	- Click **Create**.

## Step 3: Add an Action to Send a Queue Message

1. In the Logic App Designer, click the **+ New step** button.
2. Search for **Storage queue** and select **Put a message on a queue (V2)**.
3. Use the same storage account name and access key as before, then click **Create**.
4. Select the storage account and specify the queue name (e.g., `storageupdates`).
5. In the **Message** field, enter:  
   `Detected new blobs: `  
   Then use the dynamic content picker (lightning symbol) to insert the list of file names.

## Step 4: Create a Storage Queue

1. In a new browser tab, navigate to your storage account.
2. Go to **Data storage > Queues**.
3. Click **+ Queue**, enter `storageupdates` as the name, and click **Create**.

## Summary

You have successfully created a Logic App that monitors blob uploads in a storage container and sends a message to a storage queue when a new blob is detected.
