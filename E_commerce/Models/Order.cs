using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.Models
{
    public class Order
    {
        public int OrderId { get; set; } // Primary key
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Foreign key for User
        public string? UserId { get; set; }

        // Navigation property for User
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        // Navigation property for OrderDetails
        public ICollection<OrderDetail> ?OrderDetails { get; set; }
    }
}
