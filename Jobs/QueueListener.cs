using Amqp;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jobs
{
    public class QueueListener : QueueConnection, IQueueListener, IDisposable
    {
        private ReceiverLink oldReceiver;
        private ReceiverLink receiver;

        public Action<QueueMessage> OnMessage { get; set; }

        public QueueListener(Action<QueueMessage> onMessage)
        {
            OnMessage = onMessage;
        }

        protected override void Renew(Session session)
        {
            oldReceiver = receiver;

            receiver = new ReceiverLink(session, "receiver", Environment.GetEnvironmentVariable("SERVICE_BUS_NAME"));

            receiver.Start(20, (receiver, message) =>
            {
                Console.WriteLine($"[{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}] -> received message");

                try
                {
                    var queueMessage = JsonConvert.DeserializeObject<QueueMessage>(message.GetBody<string>());

                    OnMessage(queueMessage);

                    receiver.Accept(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}] -> message threw an exception: {e.Message}");

                    receiver.Reject(message);
                }
            });

            if (oldReceiver != null)
            {
                oldReceiver.Close();
                oldReceiver = null;
            }
        }
        public new void Dispose()
        {
            if (oldReceiver != null)
            {
                oldReceiver.Close();
            }

            if (receiver != null)
            {
                receiver.Close();
            }

            base.Dispose();
        }
    }
}
