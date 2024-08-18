using E_commerce.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.DTO
{
    public class Orderview
    {
        public int Id { get; set; }
        public string ?UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailview> ?OrderItems { get; set; }
    }
}
