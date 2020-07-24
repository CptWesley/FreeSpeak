using System;
using System.Net;
using ExtensionNet;
using FreeSpeak.Loggers;
using FreeSpeak.PacketProcessing;
using FreeSpeak.Packets;
using Org.BouncyCastle.Crypto.Parameters;

namespace FreeSpeak
{
    /// <summary>
    /// Represents an established connection.
    /// </summary>
    public class Connection
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="logger">The logger.</param>
        public Connection(IPEndPoint endPoint, ILogger logger)
        {
            EndPoint = endPoint;
            this.logger = logger;
            ReceiveQueue = new PacketReceiveQueue(endPoint, logger);
        }

        /// <summary>
        /// Gets the end point.
        /// </summary>
        public IPEndPoint EndPoint { get; }

        /// <summary>
        /// Gets the receive queue.
        /// </summary>
        public PacketReceiveQueue ReceiveQueue { get; }

        /// <summary>
        /// Gets the shared iv.
        /// </summary>
        public byte[] SharedIV { get; private set; }

        /// <summary>
        /// Gets the shared mac.
        /// </summary>
        public ulong SharedMac { get; private set; }

        /// <summary>
        /// Sets the shared iv.
        /// </summary>
        /// <param name="privateKey">The private key.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="beta">The beta.</param>
        /// <param name="omega">The omega.</param>
        public void SetSharedIV(ECPrivateKeyParameters privateKey, string alpha, string beta, string omega)
        {
            if (privateKey is null)
            {
                throw new ArgumentNullException(nameof(privateKey));
            }

            if (alpha is null)
            {
                throw new ArgumentNullException(nameof(alpha));
            }

            if (beta is null)
            {
                throw new ArgumentNullException(nameof(beta));
            }

            if (omega is null)
            {
                throw new ArgumentNullException(nameof(omega));
            }

            ECPublicKeyParameters publicKey = Encryption.FromOmega(omega);
            (byte[] mac, byte[] siv) = Encryption.ComputeShared(alpha, beta, privateKey, publicKey);
            SharedMac = mac.ToUInt64();
            SharedIV = siv;
        }

        /// <summary>
        /// Gets the packet identifier of the type.
        /// </summary>
        /// <param name="type">The packet type.</param>
        /// <returns>The next packet id of the type.</returns>
        public ushort GetPacketId(PacketType type)
            => ReceiveQueue.GetPacketId(type);
    }
}
