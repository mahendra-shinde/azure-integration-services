// The product class represents a product with properties that correspond to the columns in the CSV file
public class Product 
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}