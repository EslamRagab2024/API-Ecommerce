using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; } // Primary key

        // Foreign key for Cart
        public int CartId { get; set; }

        // Navigation property for Cart
        [ForeignKey("CartId")]
        public Cart ?Cart { get; set; }

        // Foreign key for Product
        public int ProductId { get; set; }

        // Navigation property for Product
        [ForeignKey("ProductId")]
        public Product ?Product { get; set; }

        public int Quantity { get; set; }
    }
}
