using Amqp;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jobs
{
    public class QueueSender : QueueConnection, IQueueSender, IDisposable
    {
        private SenderLink oldSender;
        private SenderLink sender;
        
        public async Task SendAsync(QueueMessage queueMessage)
        {
            await sender.SendAsync(new Message(JsonConvert.SerializeObject(queueMessage)));
        }

        protected override void Renew(Session session)
        {
            oldSender = sender;
            
            sender = new SenderLink(session, "sender", Environment.GetEnvironmentVariable("SERVICE_BUS_NAME"));

            if (oldSender != null)
            {
                oldSender.Close();
                oldSender = null;
            }
        }

        public new void Dispose()
        {
            if (oldSender != null)
            {
                oldSender.Close();
            }

            if (sender != null)
            {
                sender.Close();
            }

            base.Dispose();
        }

    }
}