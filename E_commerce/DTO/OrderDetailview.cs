using E_commerce.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.DTO
{
    public class OrderDetailview
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
