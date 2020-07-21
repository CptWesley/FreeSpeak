using System.Collections.Generic;
using FreeSpeak.Loggers;
using FreeSpeak.Packets;

namespace FreeSpeak.PacketProcessing
{
    /// <summary>
    /// Provides a packet queue processor that reorders packets.
    /// </summary>
    /// <seealso cref="PacketQueueProcessor" />
    internal abstract class ReorderPacketQueueProcessor : PacketQueueProcessor
    {
        private readonly Dictionary<int, ClientPacket> queue = new Dictionary<int, ClientPacket>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderPacketQueueProcessor"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected ReorderPacketQueueProcessor(ILogger logger)
            : base(logger)
        {
        }

        /// <inheritdoc/>
        public override void Push(ClientPacket packet)
        {
            if (packet.PacketId >= PacketId)
            {
                queue[packet.PacketId] = packet;
                TryProcess();
            }
        }

        private void TryProcess()
        {
            if (queue.TryGetValue(PacketId, out ClientPacket packet))
            {
                if (packet.Flags.HasFlag(PacketFlags.Fragmented))
                {
                    int id = PacketId + 1;
                    List<ClientPacket> packets = new List<ClientPacket>();
                    packets.Add(packet);

                    while (true)
                    {
                        if (!queue.TryGetValue(id++, out packet))
                        {
                            return;
                        }

                        packets.Add(packet);

                        if (packet.Flags.HasFlag(PacketFlags.Fragmented))
                        {
                            break;
                        }
                    }

                    foreach (ClientPacket p in packets)
                    {
                        queue.Remove(p.PacketId);
                    }

                    Process(packets.ToArray());
                    PacketId = id;
                }
                else
                {
                    queue.Remove(packet.PacketId);
                    Process(new ClientPacket[] { packet });
                    PacketId++;
                }

                TryProcess();
            }
        }
    }
}
