using System;
using System.Diagnostics.CodeAnalysis;

namespace FreeSpeak.Protocols.TeamSpeak.Packets
{
    /// <summary>
    /// Represents a packet send from the client to the server.
    /// </summary>
    public class ClientPacket
    {
        /// <summary>
        /// The maximum data size.
        /// </summary>
        public const int MaxDataSize = 487;

        private byte[] _data = Array.Empty<byte>();

        /// <summary>
        /// Gets or sets the EAX Message Authentication Code.
        /// </summary>
        public ulong MessageAuthenticationCode { get; set; }

        /// <summary>
        /// Gets or sets the packet id.
        /// </summary>
        public ushort PacketId { get; set; }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public ushort ClientId { get; set; }

        /// <summary>
        /// Gets or sets the packet flags.
        /// </summary>
        public PacketFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets the packet type.
        /// </summary>
        public PacketType Type { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "We don't mind changing the entire array.")]
        public byte[] Data
        {
            get => _data;
            set
            {
                if (value.Length > MaxDataSize)
                {
                    throw new ArgumentException($"Maximum data length is {MaxDataSize}, but tried to set {value.Length} bytes.");
                }

                _data = value;
            }
        }
    }
}
