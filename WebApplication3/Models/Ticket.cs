
namespace WebApplication3.Models
{
    public class Ticket
    {
        public string Id { get; set; }
        
        public string Event { get; set; }
        
        public int Quantity { get; set; }
        
        public double Price { get; set; }
        
        public DateTime BoughtOn { get; set; }

        public string UserEmail { get; set; }
    }
}
