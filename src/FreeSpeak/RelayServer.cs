using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FreeSpeak.Loggers;
using FreeSpeak.PacketProcessing;
using FreeSpeak.Packets;
using FreeSpeak.Packets.Data;

namespace FreeSpeak
{
    /// <summary>
    /// Provides a server which simply relays all traffic from a client to a server while logging all traffic.
    /// </summary>
    /// <seealso cref="IDisposable" />
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
            byte[] key = new byte[] { 0x63, 0x3A, 0x5C, 0x77, 0x69, 0x6E, 0x64, 0x6F, 0x77, 0x73, 0x5C, 0x73, 0x79, 0x73, 0x74, 0x65 };
            byte[] nonce = new byte[] { 0x6D, 0x5C, 0x66, 0x69, 0x72, 0x65, 0x77, 0x61, 0x6C, 0x6C, 0x33, 0x32, 0x2E, 0x63, 0x70, 0x6C };

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] received = client.Receive(ref endpoint);
            if (endpoint.Equals(serverEndPoint))
            {
                ServerPacket packet = ServerPacket.Parse(received);
                logger.WriteWarning($"Sending to client: {packet}");
                if (packet.Type == PacketType.Command && packet.PacketId == 0)
                {
                    string x = Encoding.UTF8.GetString(Encryption.Decrypt(key, nonce, packet.GetHeader(), ((RawData)packet.Data).Data.ToArray(), packet.MessageAuthenticationCode));
                    logger.WriteWarning(x);
                }

                client.Send(received, received.Length, clientEndPoint);
            }
            else
            {
                ClientPacket packet = ClientPacket.Parse(endpoint, received);
                logger.WriteError($"Sending to server: {packet}");
                if (packet.Type == PacketType.Command && packet.PacketId == 0)
                {
                    string x = Encoding.UTF8.GetString(Encryption.Decrypt(key, nonce, packet.GetHeader(), ((RawData)packet.Data).Data.ToArray(), packet.MessageAuthenticationCode));
                    logger.WriteError(x);
                }

                clientEndPoint = endpoint;
                client.Send(received, received.Length, serverEndPoint);
            }
        }
    }
}
