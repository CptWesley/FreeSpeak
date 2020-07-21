using FreeSpeak.Loggers;
using FreeSpeak.Packets;
using FreeSpeak.Packets.Data;

namespace FreeSpeak.PacketProcessing
{
    /// <summary>
    /// The processor for command packets.
    /// </summary>
    /// <seealso cref="ReorderPacketQueueProcessor" />
    internal class CommandPacketQueueProcessor : ReorderPacketQueueProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandPacketQueueProcessor"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public CommandPacketQueueProcessor(ILogger logger)
            : base(logger)
        {
        }

        /// <inheritdoc/>
        public override void Process(ClientPacket packet)
        {
            Logger.WriteInfo($"Received command: '{((CommandData)packet.Data).Command}'.");
        }
    }
}
