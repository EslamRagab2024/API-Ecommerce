using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; } // Primary key

        // Foreign key for Order
        public int OrderId { get; set; }

        // Navigation property for Order
        [ForeignKey("OrderId")]
        public Order ?Order { get; set; }

        // Foreign key for Product
        public int ProductId { get; set; }

        // Navigation property for Product
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
