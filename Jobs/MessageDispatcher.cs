using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobs
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly IQueueSender client;

        public MessageDispatcher(IQueueSender client) {
            this.client = client;
        }

        public async Task Dispatch(QueueMessage message)
        {
            if (message.Topic == "web")
            {
                var body = message.GetBody<string>();

                Console.WriteLine($"[{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}] -> received message from web: {body}");

                var complexObject = await new ComplexFactory().CreateSomethingComplicated(body);

                await client.SendAsync(new QueueMessage("worker", complexObject));
            }
            else if (message.Topic == "worker")
            {
                var body = message.GetBody<ComplexObject>();

                Console.WriteLine($"[{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}] -> received message from worker: {body.Name} with {body.Bells} bells and {body.Whistles} whistles.");
            }
            else
            {
                throw new Exception($"Unexpected message {message.Topic}");
            }
        }
    }
}
