using System;
using System.Net;
using System.Net.Sockets;
using FreeSpeak.Loggers;
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
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSpeakServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="logger">The logger used by the server.</param>
        public TeamSpeakServer(int port, ILogger logger)
        {
            Port = port;
            this.logger = logger;
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

            logger.WriteInfo($"{packet.Sender.Address}:{packet.Sender.Port} -> {packet.ClientId} {packet.PacketId} {packet.Type} {packet.Flags}");

            return packet;
        }

        /// <summary>
        /// Sends the given packet to the specified receiver.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        /// <param name="packet">The packet.</param>
        public void Send(IPEndPoint receiver, ServerPacket packet)
        {
            if (receiver is null)
            {
                throw new ArgumentNullException(nameof(receiver));
            }

            if (packet is null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            logger.WriteInfo($"{receiver.Address}:{receiver.Port} <- {packet.PacketId} {packet.Type} {packet.Flags}");

            byte[] data = packet.ToBytes();
            client.Send(data, data.Length, receiver);
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
