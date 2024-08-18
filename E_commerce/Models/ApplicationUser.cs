using Microsoft.AspNetCore.Identity;

namespace E_commerce.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string ?Address { get; set; }

        // Navigation property for Orders
        public ICollection<Order> ?Orders { get; set; }

        // Navigation property for Cart
        public Cart ?Cart { get; set; }
    }
}
