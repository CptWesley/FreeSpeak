using System;
using System.Net;
using System.Net.Sockets;
using FreeSpeak.Packets;

namespace FreeSpeak
{
    /// <summary>
    /// Represents a TeamSpeak server.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public class TeamSpeakServer : IDisposable
    {
        private readonly UdpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSpeakServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public TeamSpeakServer(int port)
        {
            Port = port;
            client = new UdpClient(port);
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Receive a client packet.
        /// </summary>
        /// <returns>The received client packet.</returns>
        public ClientPacket Receive()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            byte[] packetBytes = client.Receive(ref ep);
            ClientPacket packet = ClientPacket.Parse(ep, packetBytes);
            return packet;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
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
    }
}
