using Google.Cloud.Firestore;

namespace WebApplication2.Models.FirestoreModels
{
    [FirestoreData]
    public class Event
    {
        [FirestoreProperty]
        public string Id { get; set; }
        
        [FirestoreProperty]
        public string Name { get; set; }
        
        [FirestoreProperty]
        public string Description { get; set; }
        
        [FirestoreProperty]
        public DateTime Date { get; set; }

        [FirestoreProperty]
        public string Location { get; set; }

        [FirestoreProperty]
        public double Price { get; set; }
    }
}
