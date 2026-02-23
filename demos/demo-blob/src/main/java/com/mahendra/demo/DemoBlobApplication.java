package com.mahendra.demo;

import java.io.InputStream;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import com.azure.storage.blob.*;



@SpringBootApplication
public class DemoBlobApplication implements CommandLineRunner {

	@Value("${azure.storage.connection-string}")
	private String connectionString;

	public static void main(String[] args) {
		SpringApplication.run(DemoBlobApplication.class, args);
	}

	@Override
	public void run(String... args) throws Exception {
		// Create a BlobClient to interact with the blob storage
		BlobClient blobClient = new BlobClientBuilder()
				.connectionString(connectionString)
				.containerName("files")
				.blobName("product.csv")
				.buildClient();

		InputStream in = getClass().getResourceAsStream("/products.csv");
		
		blobClient.upload(in);
		
		System.out.println("File uploaded successfully to Azure Blob Storage!");
		
	}
}
