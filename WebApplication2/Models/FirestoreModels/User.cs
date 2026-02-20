using Google.Cloud.Firestore;

namespace WebApplication2.Models.FirestoreModels
{
    [FirestoreData]
    public class User
    {
        public User()
        {
            CreatedAt = DateTime.UtcNow;
        }

        [FirestoreProperty]
        public string Email { get; set; }
        
        [FirestoreProperty]
        public string FirstName { get; set; }
        
        [FirestoreProperty]
        public string LastName { get; set; }
        
        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }
    }
}
