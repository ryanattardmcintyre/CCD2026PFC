using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Repositories;

namespace WebApplication2.Controllers
{
    public class TicketsController : Controller
    {
        private FirestoreRepository _firestoreRepository;
        private PublisherRepository _publisherRepository;
        public TicketsController(FirestoreRepository firestoreRepository, PublisherRepository publisherRepository)
        {
            _firestoreRepository = firestoreRepository;
            _publisherRepository = publisherRepository;
        }

        [Authorize] //protects a method from being accessed anonymously
        public async Task<IActionResult> Buy(string eventId, int qty = 0)
        {
            try
            {
                if (qty <= 0)
                {
                    TempData["error"] = "Quantity must be greater than zero.";
                    return RedirectToAction("Index", "Events");
                }
                //SingleOrDefault, FirstOrDefault, Where, Contains, Count....

                //userEmail read it from here
                string emailAddress = User.Claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value;

                //price we read it from the event document
                //SingleOrDefault is a LINQ method that searches a list, evaluates a condition e.g. where eventId is equal to any
                //event's Id in the list.  If there is a match it will return only that ONE record that matches the condition

                double price = _firestoreRepository.GetEvents().SingleOrDefault(x => x.Id == eventId)?.Price ?? 0;
                price *= qty;
                //discounts workings
                 var docReference = await _firestoreRepository.AddTicketAsync(emailAddress, eventId, qty, price);

                await _publisherRepository.PublishMessageWithCustomAttributesAsync(
                      new Models.FirestoreModels.Ticket()
                      {
                          BoughtOn = DateTime.UtcNow,
                          Price = price,
                          Event = eventId,
                           Quantity = qty,
                            UserEmail = emailAddress,
                            Id= docReference.Id
                      }
                    );

                TempData["success"] = "Ticket bought successfully!"; //this is a way how to pass a message back to the view 
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while buying the ticket: ";
                //log ex.Message; on the cloud
            }
            return RedirectToAction("Index", "Events"); //it re-lists the events index page after buying the ticket
        }


        [Authorize]
        public IActionResult MyTickets()
        {
            string userEmail = User.Claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value;
            if(userEmail == null)
            {
                TempData["error"] = "User email not found.";
                return RedirectToAction("Index", "Events");
            }
            var list = _firestoreRepository.GetUserTickets(userEmail);

            return View(list);
        }
    }
}
