using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobs
{
    public class QueueMessage
    {
        public QueueMessage() { }

        public QueueMessage(string topic, object body)
            : this()
        {
            Topic = topic;
            Body = JsonConvert.SerializeObject(body);
        }
        
        public string Topic { get; set; }
        public string Body { get; set; }

        public T GetBody<T>()
        {
            return JsonConvert.DeserializeObject<T>(Body);
        }

        public void SetBody<T>(T body)
        {
            Body = JsonConvert.SerializeObject(body);
        }
    }
}
