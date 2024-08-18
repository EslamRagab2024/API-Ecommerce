namespace E_commerce.Models
{
    public class Category
    {
        public int CategoryId { get; set; } // Primary key

        public string? Name { get; set; }

        // Navigation property for Products
        public ICollection<Product> ?Products { get; set; }
    }
}
