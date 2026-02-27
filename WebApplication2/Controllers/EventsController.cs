using Microsoft.AspNetCore.Mvc;
using WebApplication2.Repositories;

namespace WebApplication2.Controllers
{

    //Dependency Injection is a design pattern which helps in centralizing the
    //instantiation of objects. The best central place where to instantiate objects
    //is the Program.cs which is like the entry point to the application.

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
