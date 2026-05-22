using Google.Cloud.Logging.Type;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WebApplication2.Models;
using WebApplication2.Repositories;

namespace WebApplication2.Controllers
{

    public class Appointment
    {
        public string date { get; set; }
        public string time { get; set; }
        public string client { get; set; }
        public string status { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public async Task<IActionResult> Test([FromServices] FirestoreRepository firestore)
        {
            string appointments = "C:\\Users\\attar\\Downloads\\appointments.json";

            StreamReader sr = new StreamReader(appointments);

            string appointmentsRead = sr.ReadToEnd();
            sr.Close();

           List<Appointment> appointmentsObject =
                JsonConvert.DeserializeObject<List<Appointment>>(appointmentsRead);


            foreach(var a in appointmentsObject)
            {
                await firestore.AddAppointmentAsync(a);
            }

            return Content("done");

        }


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            LogsRepository.WriteLogEntry("ccd63a2026", LogSeverity.Info, "This is an info log entry", null);
            try
            {
                LogsRepository.WriteLogEntry("ccd63a2026", LogSeverity.Critical, "Running a critical process", null);
                throw new Exception("Exception was raised on purpose");
            }
            catch (Exception ex)
            {
                //try connecting with the database to log the error, it can pose some issues:
                //issue 1: database connection failure
                //issue 2: database storage is expensive
                //issue 3: database connection is also expensive on the performance
                //issue 4: many times database might raise exceptions (for example risking to pass an unwanted data type)
                //issue 5: database might run out-of-space

                LogsRepository.WriteLogEntry("ccd63a2026", LogSeverity.Error, "An error occurred in the Index action.", ex);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
