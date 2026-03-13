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
        private readonly CacheRepository _cacheRepository;
        public EventsController(FirestoreRepository firestoreRepository, 
            BucketsRepository bucketsRepository, CacheRepository cacheRepository)
        { _firestoreRepository = firestoreRepository;
            _bucketsRepository = bucketsRepository;
            _cacheRepository = cacheRepository;
        }


        public IActionResult Index()
        {
           /* DateTime fireStoreTimeStart = DateTime.Now;
            var listOfEvents = _firestoreRepository.GetEvents();
            TimeSpan tsFirestore = DateTime.Now.Subtract(fireStoreTimeStart);


            //for the experiment we are synching the two
            foreach(var e in listOfEvents)
            {
                _cacheRepository.AppendEvent(e);
            }
           */

            DateTime cacheTimeStart = DateTime.Now;
            var listOfEventsForCache = _cacheRepository.GetEvents();
            TimeSpan tsCache = DateTime.Now.Subtract(cacheTimeStart);

            return View(listOfEventsForCache);
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

            if (guestList != null)
            {
                using (var myGuestList = guestList.OpenReadStream())
                {
                    guestListPath = await _bucketsRepository.Upload(myGuestList, config.GetValue<string>("GuestListBucket"),
                         guestList.FileName);
                }
                int lastSlashIndex = guestListPath.LastIndexOf('/');
                string filename = guestListPath.Substring(lastSlashIndex + 1);

                string guestListFormedPath = await _bucketsRepository.AssignPermission(config.GetValue<string>("GuestListBucket"), eventObj.Organiser,
                    filename);

                eventObj.GuestList = guestListFormedPath;
            }
            eventObj.Poster = posterPath;   

            eventObj.Date = eventObj.Date.ToUniversalTime(); // Convert to UTC before storing

            await _firestoreRepository.AddEventAsync(eventObj);

            _cacheRepository.AppendEvent(eventObj);

            TempData["success"] = "Event created successfully!";

            return RedirectToAction("Index");
        
        }
       
    }
}
