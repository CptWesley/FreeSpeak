using System;
using FreeSpeak.Protocols.TeamSpeak.Packets;
using Xunit;
using static AssertNet.Xunit.Assertions;

namespace FreeSpeak.Protocols.TeamSpeak.Tests.Packets
{
    /// <summary>
    /// Test class for the <see cref="ClientPacket"/> class.
    /// </summary>
    public class ServerPacketTests : PacketTests
    {
        /// <summary>
        /// Checks that we can convert a packet to bytes and read them as bytes again.
        /// </summary>
        [Fact]
        public void ToAndFromBytesTest()
        {
            ServerPacket packet = (ServerPacket)CreatePacket();
            byte[] bytes = ServerPacket.ToBytes(packet);
            ServerPacket copy = ServerPacket.FromBytes(bytes);
            AssertThat(copy).IsEqualTo(packet);
        }

        /// <summary>
        /// Checks that the correct exception is thrown when we pass invalid arguments to the function.
        /// </summary>
        [Fact]
        public void ToBytesArgumentNullExceptionTest()
            => AssertThat(() => ServerPacket.ToBytes(null)).ThrowsExactlyException<ArgumentNullException>();

        /// <summary>
        /// Checks that the correct exception is thrown when we pass invalid arguments to the function.
        /// </summary>
        [Fact]
        public void FromBytesArgumentNullExceptionTest()
            => AssertThat(() => ServerPacket.FromBytes(null)).ThrowsExactlyException<ArgumentNullException>();

        /// <summary>
        /// Checks that the correct exception is thrown when we pass invalid arguments to the function.
        /// </summary>
        [Fact]
        public void FromBytesArgumentExceptionTest()
            => AssertThat(() => ServerPacket.FromBytes(new byte[7])).ThrowsExactlyException<ArgumentException>();

        /// <summary>
        /// Creates a packet for testing.
        /// </summary>
        /// <returns>Packet for testing.</returns>
        protected override Packet CreatePacket()
            => new ServerPacket()
            {
                MessageAuthenticationCode = 34,
                PacketId = 3,
                Flags = PacketFlags.Compressed | PacketFlags.NewProtocol,
                Type = PacketType.CommandLow,
                Data = new byte[] { 2, 123 }
            };
    }
}
