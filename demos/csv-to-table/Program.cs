using System;
using System.Globalization;
using Azure.Data.Tables;
using CsvHelper;

FileStream stream = File.OpenRead("products.csv");
// Create a CsvReader to read the CSV file
using var reader = new StreamReader(stream);
using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
// Read the CSV file into a list of Product objects
var products = csv.GetRecords<Product>().ToList();

TableServiceClient client = new TableServiceClient("<CONNECTION_STRING>");
TableClient tableClient = client.GetTableClient("products");
// Create the table if it doesn't exist
tableClient.CreateIfNotExists();
// Insert the products into the table
foreach (var product in products)
{
    // COnvert the product to a TableEntity and insert it into the table
    var entity = new TableEntity(product.Id, product.Name)
    {
        {"PartitionKey", product.Id},
        {"RowKey", product.Name },
        { "Price", product.Price },
        { "Quantity", product.Quantity }
    };
    tableClient.AddEntity(entity);
}
