using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ExtensionNet;
using FreeSpeak.Loggers;
using FreeSpeak.PacketProcessing;
using FreeSpeak.Packets;
using FreeSpeak.Packets.Data;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

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
        private readonly Dictionary<IPEndPoint, Connection> connections = new Dictionary<IPEndPoint, Connection>();
        private readonly ECPrivateKeyParameters privateKey;
        private readonly ECPublicKeyParameters publicKey;

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
            (privateKey, publicKey) = Encryption.GenerateKeys();
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port { get; }

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

        /// <summary>
        /// Creates and sends a packet over the designated connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="type">The type.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="data">The data.</param>
        public void Send(Connection connection, PacketType type, PacketFlags flags, PacketData data)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            ushort pid = connection.GetPacketId(type);
            ulong mac = connection.SharedMac;
            if (!flags.HasFlag(PacketFlags.Unencrypted))
            {
                using MemoryStream ms = new MemoryStream();
                ms.Write(pid);
                ms.Write((byte)type);
                byte[] meta = ms.ToArray();
                (byte[] key, byte[] nonce) = Encryption.GenerateKey(type, pid, 0, true, connection.SharedIV);
                (byte[] tempData, ulong tempMac) = Encryption.Encrypt(key, nonce, meta, data.ToBytes());
                data = new RawData(tempData);
                mac = tempMac;
            }

            ServerPacket packet = new ServerPacket(mac, pid, type, flags, data);
            Send(connection.EndPoint, packet);
        }

        /// <summary>
        /// Starts listening to incoming traffic.
        /// </summary>
        public void Listen()
        {
            while (true)
            {
                try
                {
                    HandleSingle();
                }
                catch (IllegalClientOperationException e)
                {
                    logger.WriteError(e.Message);
                }
            }
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

        /// <summary>
        /// Receive a client packet.
        /// </summary>
        /// <returns>The received client packet.</returns>
        private ClientPacket Receive()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            byte[] packetBytes = client.Receive(ref ep);
            ClientPacket packet = ClientPacket.Parse(ep, packetBytes);

            logger.WriteInfo($"{packet.Sender.Address}:{packet.Sender.Port} -> {packet.ClientId} {packet.PacketId} {packet.Type} {packet.Flags}");

            return packet;
        }

        private void HandleSingle()
        {
            ClientPacket packet = Receive();
            Connection connection;
            if (!connections.TryGetValue(packet.Sender, out connection))
            {
                connection = new Connection(packet.Sender, logger);
                connections.Add(packet.Sender, connection);
            }

            if (packet.Type == PacketType.Init1)
            {
                ulong mac = 0x545333494E495431;
                if (((ClientHandshakeData)packet.Data).Step == 0)
                {
                    Handshake0Data data = packet.Data as Handshake0Data;
                    ServerPacket sp = new ServerPacket(mac, 101, PacketType.Init1, PacketFlags.Unencrypted, new Handshake1Data(0, 0, data.Random.ChangeEndianness()));
                    Send(packet.Sender, sp);
                }
                else if (((ClientHandshakeData)packet.Data).Step == 2)
                {
                    Handshake2Data data = packet.Data as Handshake2Data;
                    ServerPacket sp = new ServerPacket(mac, 101, PacketType.Init1, PacketFlags.Unencrypted, new Handshake3Data(4, 7, 10, new byte[100]));
                    Send(packet.Sender, sp);
                }
                else if (((ClientHandshakeData)packet.Data).Step == 4)
                {
                    // Do some verification?
                }
            }
            else if (packet.Type == PacketType.Command && packet.PacketId == 0)
            {
                // Handles clientinitiv command.
                byte[] key = new byte[] { 0x63, 0x3A, 0x5C, 0x77, 0x69, 0x6E, 0x64, 0x6F, 0x77, 0x73, 0x5C, 0x73, 0x79, 0x73, 0x74, 0x65 };
                byte[] nonce = new byte[] { 0x6D, 0x5C, 0x66, 0x69, 0x72, 0x65, 0x77, 0x61, 0x6C, 0x6C, 0x33, 0x32, 0x2E, 0x63, 0x70, 0x6C };

                // Send ack.
                (byte[] ackData, ulong ackMac) = Encryption.Encrypt(key, nonce, new byte[] { 0, 0, (byte)PacketType.Ack }, new byte[] { 0 });
                ServerPacket ackPacket = new ServerPacket(ackMac, 0, PacketType.Ack, PacketFlags.None, new RawData(ackData));
                Send(connection.EndPoint, ackPacket);

                // Send response.
                byte[] meta = packet.GetHeader();
                byte[] data = packet.Data.ToBytes();

                string command = Encoding.Utf8(Encryption.Decrypt(key, nonce, meta, data, packet.MessageAuthenticationCode));
                CommandData cmd = CommandData.Parse(command);

                string alpha = cmd.Attributes["alpha"];
                string omega = cmd.Attributes["omega"];

                byte[] betaBytes = new byte[10];
                new SecureRandom().NextBytes(betaBytes);
                string beta = Encoding.Base64(betaBytes);
                string serverOmega = Encryption.ToOmega(publicKey);

                connection.SetSharedIV(privateKey, alpha, beta, omega);

                PacketData responseCommand = new CommandData("initivexpand", new Dictionary<string, string>()
                {
                    { "alpha", alpha },
                    { "beta", beta },
                    { "omega", serverOmega },
                });

                byte[] header = Encryption.GetHeader(0, PacketType.Command, PacketFlags.None);
                (byte[] responseData, ulong responseMac) = Encryption.Encrypt(key, nonce, header, responseCommand.ToBytes());
                ServerPacket responsePacket = new ServerPacket(responseMac, 0, PacketType.Command, PacketFlags.None, new RawData(responseData));
                Send(connection.EndPoint, responsePacket);
            }
            else
            {
                connection.ReceiveQueue.Process(packet);
            }
        }
    }
}
