using System;
using System.Net;
using FreeSpeak.Loggers;
using FreeSpeak.Packets;

namespace FreeSpeak.PacketProcessing
{
    /// <summary>
    /// Provides a queue for incoming packets.
    /// </summary>
    public class PacketReceiveQueue
    {
        private readonly PacketQueueProcessor commandProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketReceiveQueue"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="logger">The logger used for logging.</param>
        public PacketReceiveQueue(IPEndPoint sender, ILogger logger)
        {
            Sender = sender;
            commandProcessor = new CommandPacketQueueProcessor(logger);
        }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        public IPEndPoint Sender { get; }

        /// <summary>
        /// Processes the specified packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        public void Process(ClientPacket packet)
        {
            if (packet is null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            switch (packet.Type)
            {
                case PacketType.Command: commandProcessor.Push(packet);
                    break;
            }
        }

        /// <summary>
        /// Gets the packet identifier of the type.
        /// </summary>
        /// <param name="type">The packet type.</param>
        /// <returns>The next packet id of the type.</returns>
        public ushort GetPacketId(PacketType type)
            => type switch
            {
                PacketType.Command => commandProcessor.PacketId,
                _ => throw new ArgumentException($"Unknown type: {type}", nameof(type)),
            };
    }
}
