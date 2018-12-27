using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ExtensionNet.Reflective;

namespace FreeSpeak.Protocols.TeamSpeak.Packets
{
    /// <summary>
    /// Abstract class for generic packets.
    /// </summary>
    public abstract class Packet
    {
        private byte[] _data = Array.Empty<byte>();

        /// <summary>
        /// Gets the maximum data size.
        /// </summary>
        public abstract int MaxDataSize { get; }

        /// <summary>
        /// Gets or sets the EAX Message Authentication Code.
        /// </summary>
        public ulong MessageAuthenticationCode { get; set; }

        /// <summary>
        /// Gets or sets the packet id.
        /// </summary>
        public ushort PacketId { get; set; }

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

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
            => this.InternallyEquals(obj, true);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
            => this.GetInternalHashCode(true);
    }
}
