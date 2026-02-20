using Google.Cloud.Firestore;

namespace WebApplication2.Models.FirestoreModels
{
    [FirestoreData]
    public class Ticket
    {
        [FirestoreProperty]
        public string Id { get; set; }
        
        [FirestoreProperty]
        public string Event { get; set; }
        
        [FirestoreProperty]
        public int Quantity { get; set; }
        
        [FirestoreProperty]
        public decimal Price { get; set; }
        
        [FirestoreProperty]
        public DateTime BoughtOn { get; set; }
    }
}
