using System;
using ExtensionNet;

namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Second packet of the low level handshake.
    /// </summary>
    /// <seealso cref="HandshakeData" />
    public class Handshake1Data : HandshakeData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Handshake1Data"/> class.
        /// </summary>
        /// <param name="serverLeft">The left server bytes.</param>
        /// <param name="serverRight">The right server bytes.</param>
        /// <param name="reversedRandom">The reversed random bits.</param>
        public Handshake1Data(ulong serverLeft, ulong serverRight, uint reversedRandom)
            : base(1)
        {
            ServerLeft = serverLeft;
            ServerRight = serverRight;
            ReversedRandom = reversedRandom;
        }

        /// <summary>
        /// Gets the left server bytes.
        /// </summary>
        public ulong ServerLeft { get; }

        /// <summary>
        /// Gets the right server bytes.
        /// </summary>
        public ulong ServerRight { get; }

        /// <summary>
        /// Gets the reversed random bits.
        /// </summary>
        public uint ReversedRandom { get; }

        /// <inheritdoc/>
        public override byte[] ToBytes()
        {
            byte[] result = new byte[21];
            result[0] = Step;

            byte[] sl = BitConverter.IsLittleEndian ? BitConverter.GetBytes(ServerLeft.ChangeEndianness()) : BitConverter.GetBytes(ServerLeft);
            Array.Copy(sl, 0, result, 1, 8);

            byte[] sr = BitConverter.IsLittleEndian ? BitConverter.GetBytes(ServerRight.ChangeEndianness()) : BitConverter.GetBytes(ServerRight);
            Array.Copy(sr, 0, result, 9, 8);

            byte[] rnd = BitConverter.IsLittleEndian ? BitConverter.GetBytes(ReversedRandom.ChangeEndianness()) : BitConverter.GetBytes(ReversedRandom);
            Array.Copy(rnd, 0, result, 17, 4);

            return result;
        }
    }
}
