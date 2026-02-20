using Microsoft.AspNetCore.Mvc;
using WebApplication2.Repositories;

namespace WebApplication2.Controllers
{
    public class EventsController : Controller
    {
        private readonly FirestoreRepository _firestoreRepository;
        public EventsController(FirestoreRepository firestoreRepository)
        { _firestoreRepository = firestoreRepository; }


        public IActionResult Index()
        { 
            var listOfEvents = _firestoreRepository.GetEvents();
            return View(listOfEvents);
        }
    }
}
