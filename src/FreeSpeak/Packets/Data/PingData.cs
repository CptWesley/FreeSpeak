using System;

namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Represents packet data.
    /// </summary>
    /// <seealso cref="PacketData" />
    public class PingData : PacketData
    {
        /// <inheritdoc/>
        public override byte[] ToBytes()
            => Array.Empty<byte>();
    }
}
