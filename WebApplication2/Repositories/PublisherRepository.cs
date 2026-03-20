using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using WebApplication2.Models.FirestoreModels;

namespace WebApplication2.Repositories
{
    public class PublisherRepository
    {
        private string _projectId;
        private string _topicId;
        public PublisherRepository(string projectId, string topicId) { 
          _projectId = projectId;
            _topicId = topicId;
        }

        //User -> ticket -> Pub/Sub
        public async Task PublishMessageWithCustomAttributesAsync(Ticket t)
        {
            TopicName topicName = TopicName.FromProjectTopic(_projectId, _topicId);
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);
            //Ticket -> string
            string messageText =  System.Text.Json.JsonSerializer.Serialize(t); ;
            var pubsubMessage = new PubsubMessage
            {
                Data = ByteString.CopyFromUtf8(messageText),
                /*Attributes =
                {
                        { "type", t.Type },
                        { "author", "unknown" }
                }*/
            };
            string message = await publisher.PublishAsync(pubsubMessage);
            
            await publisher.ShutdownAsync(TimeSpan.FromSeconds(15));
        }
    }
}
