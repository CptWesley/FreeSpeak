using System;
using System.Net;
using System.Net.Sockets;
using FreeSpeak.Loggers;
using FreeSpeak.Packets;

namespace FreeSpeak
{
    public class RelayServer : IDisposable
    {
        private readonly ILogger logger;
        private readonly IPEndPoint serverEndPoint;
        private readonly UdpClient client;
        private IPEndPoint clientEndPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayServer"/> class.
        /// </summary>
        /// <param name="hostname">The hostname.</param>
        /// <param name="port">The port.</param>
        /// <param name="ownPort">The own port.</param>
        /// <param name="logger">The logger.</param>
        public RelayServer(string hostname, int port, int ownPort, ILogger logger)
        {
            client = new UdpClient(ownPort);
            clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            this.logger = logger;
            serverEndPoint = new IPEndPoint(IPAddress.Parse(hostname), port);
        }

        /// <summary>
        /// Start listening to incoming traffic.
        /// </summary>
        public void Listen()
        {
            while (true)
            {
                Single();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                client.Dispose();
            }
        }

        private void Single()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] received = client.Receive(ref endpoint);
            if (endpoint.Equals(serverEndPoint))
            {
                logger.WriteWarning($"Sending to client: {ServerPacket.Parse(received)}");
                client.Send(received, received.Length, clientEndPoint);
            }
            else
            {
                logger.WriteError($"Sending to server: {ClientPacket.Parse(endpoint, received)}");
                clientEndPoint = endpoint;
                client.Send(received, received.Length, serverEndPoint);
            }
        }
    }
}
