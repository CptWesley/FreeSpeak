using System;
using System.Net;
using System.Net.Sockets;

namespace FreeSpeak.Servers
{
    /// <summary>
    /// Abstract class for FreeSpeak servers.
    /// </summary>
    public abstract class Server : IDisposable
    {
        private UdpClient udp;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        /// <param name="port">The port on which the server is listening.</param>
        public Server(int port)
        {
            Port = port;
            Logger = new DummyLogger();
        }

        /// <summary>
        /// Gets or sets the logger used for logging messages.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets the port on which the server is listening.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            Logger.WriteInfo($"Listening to port {Port}...");
            Run();
        }

        /// <summary>
        /// Performs a callback every time a message is received.
        /// </summary>
        /// <param name="ep">The endpoint from which we received a message.</param>
        /// <param name="message">The message we received.</param>
        protected abstract void MessageReceivedCallback(IPEndPoint ep, byte[] message);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="managed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool managed)
        {
            udp.Dispose();
        }

        /// <summary>
        /// Sends a message to an endpoint over UDP.
        /// </summary>
        /// <param name="ep">The endpoint to send to.</param>
        /// <param name="message">The message to send.</param>
        protected void Send(IPEndPoint ep, byte[] message)
        {
            udp.Send(message, message.Length, ep);
        }

        private void Run()
        {
            using (udp = new UdpClient(new IPEndPoint(IPAddress.Any, Port)))
            {
                while (true)
                {
                    IPEndPoint ep = null;
                    byte[] received = udp.Receive(ref ep);
                    MessageReceivedCallback(ep, received);
                }
            }
        }
    }
}
