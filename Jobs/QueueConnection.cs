using Amqp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jobs
{
    public abstract class QueueConnection : IDisposable
    {
        private Session oldSession;
        private Connection oldConnection;

        private Session session;
        private Connection connection;

        private CancellationTokenSource cancellationTokenSource;

        private readonly Address address;

        protected abstract void Renew(Session session);

        protected QueueConnection()
        {
            address = new Address(Environment.GetEnvironmentVariable("SERVICE_BUS_URL"),
                5671,
                Environment.GetEnvironmentVariable("SERVICE_BUS_SAK_POLICYNAME"),
                Environment.GetEnvironmentVariable("SERVICE_BUS_SAK_SHAREDSECRET"));

            cancellationTokenSource = new CancellationTokenSource();

            renewSession();
        }
        private void renewSession()
        {
            oldConnection = connection;
            oldSession = session;

            connection = new Connection(address);
            session = new Session(connection);

            Renew(session);

            if (oldSession != null)
            {
                oldSession.Close();
                oldSession = null;
            }

            if (oldConnection != null)
            {
                oldConnection.Close();
                oldConnection = null;
            }

            startRenewTimer();
        }

        private void startRenewTimer()
        {
            delayedRenewSession(cancellationTokenSource.Token);
        }

        private async Task delayedRenewSession(CancellationToken token)
        {
            await Task.Delay(5 * 60 * 1000, token);

            if (!token.IsCancellationRequested)
                renewSession();
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();

            if (oldSession != null)
            {
                oldSession.Close();
            }

            if (oldConnection != null)
            {
                oldConnection.Close();
            }
            
            if (session != null)
            {
                session.Close();
            }

            if (connection != null)
            {
                connection.Close();
            }

            cancellationTokenSource.Dispose();
        }
    }
}