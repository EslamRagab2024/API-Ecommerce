namespace E_commerce.DTO
{
    public class Categoryview
    {
        public int Id { get; set; } 
        public string ?Name { get; set; }   
        public List<Productview> ?Products { get; set; }=new List<Productview>();

    }
}
