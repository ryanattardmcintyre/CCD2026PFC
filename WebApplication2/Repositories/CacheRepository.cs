using StackExchange.Redis;
using WebApplication2.Models.FirestoreModels;


namespace WebApplication2.Repositories
{
    public class CacheRepository
    {
        private IDatabase _db;

        

        public CacheRepository(string password)
        {
            var muxer = ConnectionMultiplexer.Connect(
                   new ConfigurationOptions
                   {
                       EndPoints = { { "redis-15928.crce214.us-east-1-3.ec2.cloud.redislabs.com", 15928 } },
                       User = "default",
                       Password = password
                   }
               );
            _db = muxer.GetDatabase();
        }


        public List<Event> GetEvents()
        {
           //Event => "{"id":"asdklfj3lj4l", "title": "my event", "organiser": "ryanattard@gmail.com"}"
            string myEvents = "";
            if (_db.KeyExists("events"))
                myEvents = _db.StringGet("events");

            if (string.IsNullOrEmpty(myEvents))
                return new List<Event>();
            else 
                return System.Text.Json.JsonSerializer.Deserialize<List<Event>>(myEvents);
        }
        public void AppendEvent(Event e)
            {
        
                var myEventList = GetEvents();
                myEventList.Add(e);
                string serializedList = System.Text.Json.JsonSerializer.Serialize(myEventList);
                _db.StringSet("events", serializedList);
            }


    }
}
