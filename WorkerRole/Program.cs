using Jobs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace WorkerRole
{
    public class Program
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        public static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddScoped<IMessageDispatcher, MessageDispatcher>();
            services.AddSingleton<IQueueSender, QueueSender>();
            services.AddSingleton<IQueueListener, QueueListener>((provider) =>
            {
                return new QueueListener(async (message) =>
                {
                    var dispatcher = provider.GetService<IMessageDispatcher>();

                    await dispatcher.Dispatch(message);
                });
            });

            var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine($"[{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}] -> Worker role started");

            var listener = serviceProvider.GetService<IQueueListener>();

            Console.CancelKeyPress += (sender, eArgs) => {
                Console.WriteLine($"[{DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}] -> Worker role finished");

                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            _quitEvent.WaitOne();
        }
    }
}
