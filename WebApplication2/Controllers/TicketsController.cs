using Microsoft.AspNetCore.Mvc;
using WebApplication2.Repositories;

namespace WebApplication2.Controllers
{
    public class TicketsController : Controller
    {
        private FirestoreRepository _firestoreRepository;
        public TicketsController(FirestoreRepository firestoreRepository)
        {
            _firestoreRepository = firestoreRepository;
        }

        public async Task<IActionResult> Buy(string eventId, int qty)
        {
            //SingleOrDefault, FirstOrDefault, Where, Contains, Count....

            //userEmail read it from here
            string emailAddress = User.Claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value;

            //price we read it from the event document
            //SingleOrDefault is a LINQ method that searches a list, evaluates a condition e.g. where eventId is equal to any
            //event's Id in the list.  If there is a match it will return only that ONE record that matches the condition

            double price = _firestoreRepository.GetEvents().SingleOrDefault(x=>x.Id == eventId)?.Price ?? 0;

            await _firestoreRepository.AddTicketAsync(emailAddress, eventId, qty, price);

            TempData["success"] = "Ticket bought successfully!"; //this is a way how to pass a message back to the view 
            return RedirectToAction("Index", "Events"); //it re-lists the events index page after buying the ticket
        }
    }
}
