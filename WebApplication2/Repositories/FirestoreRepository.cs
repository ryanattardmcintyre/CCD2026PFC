using Google.Cloud.Firestore;
using WebApplication2.Models.FirestoreModels;

namespace WebApplication2.Repositories
{
    public class FirestoreRepository
    {
        private readonly FirestoreDb _firestoreDb;
        public FirestoreRepository(string projectId)
        {
            // Initialize Firestore client here
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        public List<Event> GetEvents()
        {   var events = new List<Event>();
            var collection = _firestoreDb.Collection("events");
            var snapshot = collection.GetSnapshotAsync().Result;
            foreach (var doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    var eventData = doc.ConvertTo<Event>();
                    eventData.Id = doc.Id; // Set the ID from the document
                    events.Add(eventData);
                }
            }
            return events;
        }


    }
}
