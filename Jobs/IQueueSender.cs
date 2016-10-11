using System.Threading.Tasks;

namespace Jobs
{
    public interface IQueueSender
    {
        Task SendAsync(QueueMessage queueMessage);
    }
}