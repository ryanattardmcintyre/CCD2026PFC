using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
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

        //---------------------------- Events ----------------------------
        public List<Event> GetEvents()
        {
            var events = new List<Event>();
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

        public async void AddEventAsync(Event eventObj)
        {
            var collection = _firestoreDb.Collection("events");
            var docRef = collection.Document();
            await docRef.SetAsync(eventObj);
        }

        //----------------------------- Users ----------------------------

        public async void AddUserAsync(User user)
        {
            var collection = _firestoreDb.Collection("users");
            var docRef = collection.Document(user.Email);
            await docRef.SetAsync(user);
        }


        //----------------------------- Tickets ----------------------------

        public async Task<DocumentReference> AddTicketAsync(string userEmail, string eventId, int qty, double price)
        {
            var collection = _firestoreDb.Collection("events").Document(eventId).Collection("tickets");
            var ticket = new Ticket
            {
                Event = eventId,
                Quantity = qty,
                BoughtOn = DateTime.UtcNow,
                Price = price,
                UserEmail = userEmail
            };
           return await collection.AddAsync(ticket);
        }

        public List<Ticket> GetUserTickets(string userEmail)
        {
            var tickets = new List<Ticket>();
           
            var listOfEvents = GetEvents();
            foreach(var e in listOfEvents)
            {
                var ticketsForEvent = _firestoreDb.Collection("events").Document(e.Id).Collection("tickets")
                                        .WhereEqualTo("UserEmail", userEmail);

                var snapshot = ticketsForEvent.GetSnapshotAsync().Result;
                foreach (var doc in snapshot.Documents)
                {
                    if (doc.Exists)
                    {
                        var ticketData = doc.ConvertTo<Ticket>();
                        ticketData.Id = doc.Id; // Set the ID from the document
                        ticketData.Event = e.Name;
                        tickets.Add(ticketData);
                    }
                }
            }
            return tickets;
        }
    }
}
