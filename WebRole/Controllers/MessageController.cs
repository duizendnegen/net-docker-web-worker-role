using Amqp;
using Jobs;
using Microsoft.AspNetCore.Mvc;

namespace WebRole.Controllers
{
    [Route("messages")]
    public class MessageController : Controller
    {
        private readonly IQueueSender queueClient;

        public MessageController(IQueueSender queueClient)
        {
            this.queueClient = queueClient;
        }

        [HttpGet]
        public void Schedule(string message)
        {
            var queueMessage = new QueueMessage("web", message);
            
            queueClient.SendAsync(queueMessage);
        }
    }
}
