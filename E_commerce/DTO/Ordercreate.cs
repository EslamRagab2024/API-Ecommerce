namespace E_commerce.DTO
{
    public class Ordercreate
    {
        public List<OrderDetailview> ?OrderItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
