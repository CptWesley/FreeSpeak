using System.IO;
using FreeSpeak.Loggers;
using FreeSpeak.Packets;
using FreeSpeak.Packets.Data;

namespace FreeSpeak.PacketProcessing
{
    /// <summary>
    /// Provides logic for processing packets.
    /// </summary>
    internal abstract class PacketQueueProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketQueueProcessor"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public PacketQueueProcessor(ILogger logger)
            => Logger = logger;

        /// <summary>
        /// Gets or sets the packet identifier.
        /// </summary>
        public int PacketId { get; protected set; } = 1;

        /// <summary>
        /// Gets or sets the packet generation.
        /// </summary>
        public int PacketGeneration { get; protected set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Processes the specified packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        public abstract void Process(ClientPacket packet);

        /// <summary>
        /// Pushes the specified packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        public abstract void Push(ClientPacket packet);

        /// <summary>
        /// Processes the specified packets.
        /// </summary>
        /// <param name="packets">The packets.</param>
        public void Process(ClientPacket[] packets)
        {
            using MemoryStream ms = new MemoryStream();
            for (int i = 0; i < packets.Length - 1; i++)
            {
                ms.Write(packets[i].Data.ToBytes());
            }

            ClientPacket last = packets[^1];
            ms.Write(last.Data.ToBytes());
            ms.Position = 0;
            PacketData data = PacketData.Parse(last.Type, PacketFlags.Unencrypted, ms);
            Process(new ClientPacket(last.Sender, last.MessageAuthenticationCode, last.PacketId, last.ClientId, last.Type, PacketFlags.Unencrypted, data));
        }
    }
}
