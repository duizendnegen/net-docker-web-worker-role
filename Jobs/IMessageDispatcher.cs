using System.Threading.Tasks;

namespace Jobs
{
    public interface IMessageDispatcher
    {
        Task Dispatch(QueueMessage message);
    }
}