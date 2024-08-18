using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.Models
{
    public class Cart
    {
        public int CartId { get; set; } // Primary key

        // Foreign key for User
        public string ? UserId { get; set; }

        // Navigation property for User
        [ForeignKey("UserId")]
        public ApplicationUser ?User { get; set; }

        // Navigation property for CartItems
        public ICollection<CartItem> ?CartItems { get; set; }
    }
}
