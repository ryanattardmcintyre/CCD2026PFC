using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models.FirestoreModels;
using WebApplication2.Repositories;

namespace WebApplication2.Controllers
{

    //Dependency Injection is a design pattern which helps in centralizing the
    //instantiation of objects. The best central place where to instantiate objects
    //is the Program.cs which is like the entry point to the application.

    public class EventsController : Controller
    {
        private readonly FirestoreRepository _firestoreRepository;
        private readonly BucketsRepository _bucketsRepository;
        public EventsController(FirestoreRepository firestoreRepository, 
            BucketsRepository bucketsRepository)
        { _firestoreRepository = firestoreRepository;
            _bucketsRepository = bucketsRepository;
        }


        public IActionResult Index()
        {
            var listOfEvents = _firestoreRepository.GetEvents();
            return View(listOfEvents);
        }

        //this will load the page with empty fields
        [HttpGet]
        public IActionResult Create()
        { return View(); }

        [HttpPost]
        public async Task<IActionResult> Create(Event eventObj, IFormFile poster, IFormFile guestList, 
            [FromServices] IConfiguration config)
        {
            string posterPath;
            string guestListPath;
            using (var myPoster = poster.OpenReadStream())
            {
                posterPath = await _bucketsRepository.Upload(myPoster, config.GetValue<string>("PosterBucket"),
                    poster.FileName);
            }
            using (var myGuestList = guestList.OpenReadStream())
            {
               guestListPath = await _bucketsRepository.Upload(myGuestList, config.GetValue<string>("GuestListBucket"),
                    guestList.FileName);
            }


            int lastSlashIndex = guestListPath.LastIndexOf('/');
            string filename =  guestListPath.Substring(lastSlashIndex + 1); 

            string guestListFormedPath = await _bucketsRepository.AssignPermission(config.GetValue<string>("GuestListBucket"), eventObj.Organiser, 
                filename);

            eventObj.GuestList = guestListFormedPath;
            eventObj.Poster = posterPath;   

            eventObj.Date = eventObj.Date.ToUniversalTime(); // Convert to UTC before storing

            _firestoreRepository.AddEventAsync(eventObj);


            TempData["success"] = "Event created successfully!";

            return RedirectToAction("Index");
        
        }
       
    }
}
