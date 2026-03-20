using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace WebApplication3.Controllers
{
    public class PdfController : Controller
    {
        public IActionResult Index()
        {
            //Pull the ticket off the queue
            //Convert ticket to pdf <- heavy process
            //Sending it as an email <- heavy process

            string projectId = "ccd63a2026";
            string subscriptionId = "ccd63a2026-sub";

            int ticketsProcessed = PullMessagesSync(projectId, subscriptionId);

            return Content($"{ticketsProcessed} tickets were processed");
        }
        private int PullMessagesSync(string projectId, string subscriptionId, bool acknowledge = true)
        {
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
            SubscriberServiceApiClient subscriberClient = SubscriberServiceApiClient.Create();
            int messageCount = 0;
            try
            {
                List<Ticket> readTickets = new List<Ticket>();
                // Pull messages from server,
                // allowing an immediate response if there are no messages.
                PullResponse response = subscriberClient.Pull(subscriptionName, maxMessages: 10);
                // Print out each received message.
                foreach (ReceivedMessage msg in response.ReceivedMessages)
                {
                    string text = msg.Message.Data.ToStringUtf8();

                    //string => Ticket Object

                    //Solution A = we create the ticket class in this project
                    //Solution B = JSON Parse the object   myObject["Id"] 

                    var myTicket = System.Text.Json.JsonSerializer.Deserialize<Ticket>(text);
                    readTickets.Add(myTicket);
                    Interlocked.Increment(ref messageCount);
                }
                // If acknowledgement required, send to server.
                if (acknowledge && messageCount > 0)
                {
                    subscriberClient.Acknowledge(subscriptionName, response.ReceivedMessages.Select(msg => msg.AckId));
                }

                //conversion comes last

                foreach (var ticket in readTickets)
                { GeneratePdf(ticket.Id
                    , $"Ticket Id:{ticket.Id} \n" +
                    $"Event Id: {ticket.Event}\nEmail: {ticket.UserEmail}\nPrice: {ticket.Price}\n" +
                    $"DateBought: {ticket.BoughtOn.ToString("dd/MM/yyyy HH:mm")}");
                        }


                //you send it as an email

            }
            catch (Exception ex)
            {


            }
            return messageCount;
        }

        private void GeneratePdf(string ticketId, string content)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Content()
                        .Text(content)
                        .FontSize(12);
                });
            })
            .GeneratePdf($"{ticketId}.pdf");
        }


    }
}
