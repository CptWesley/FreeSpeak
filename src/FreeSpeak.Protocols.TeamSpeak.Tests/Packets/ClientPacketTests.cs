using System;
using ExtensionNet.Copy;
using FreeSpeak.Protocols.TeamSpeak.Packets;
using Xunit;
using static AssertNet.Xunit.Assertions;

namespace FreeSpeak.Protocols.TeamSpeak.Tests.Packets
{
    /// <summary>
    /// Test class for the <see cref="ClientPacket"/> class.
    /// </summary>
    public class ClientPacketTests : PacketTests
    {
        /// <summary>
        /// Checks that the constructor sets the correct default values.
        /// </summary>
        [Fact]
        public void ClientConstructorTest()
        {
            ClientPacket packet = (ClientPacket)CreatePacket();
            AssertThat(packet.ClientId).IsEqualTo(435);
        }

        /// <summary>
        /// Checks that an object is not equal with a different cid.
        /// </summary>
        [Fact]
        public void EqualsDifferentCidTest()
        {
            ClientPacket packet = (ClientPacket)CreatePacket();
            ClientPacket copy = (ClientPacket)CreatePacket();
            copy.ClientId = 0;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that we can convert a packet to bytes and read them as bytes again.
        /// </summary>
        [Fact]
        public void ToAndFromBytesTest()
        {
            ClientPacket packet = (ClientPacket)CreatePacket();
            byte[] bytes = ClientPacket.ToBytes(packet);
            ClientPacket copy = ClientPacket.FromBytes(bytes);
            AssertThat(copy).IsEqualTo(packet);
        }

        /// <summary>
        /// Checks that the correct exception is thrown when we pass invalid arguments to the function.
        /// </summary>
        [Fact]
        public void ToBytesArgumentNullExceptionTest()
            => AssertThat(() => ClientPacket.ToBytes(null)).ThrowsExactlyException<ArgumentNullException>();

        /// <summary>
        /// Checks that the correct exception is thrown when we pass invalid arguments to the function.
        /// </summary>
        [Fact]
        public void FromBytesArgumentNullExceptionTest()
            => AssertThat(() => ClientPacket.FromBytes(null)).ThrowsExactlyException<ArgumentNullException>();

        /// <summary>
        /// Checks that the correct exception is thrown when we pass invalid arguments to the function.
        /// </summary>
        [Fact]
        public void FromBytesArgumentExceptionTest()
            => AssertThat(() => ClientPacket.FromBytes(new byte[7])).ThrowsExactlyException<ArgumentException>();

        /// <summary>
        /// Creates a packet for testing.
        /// </summary>
        /// <returns>Packet for testing.</returns>
        protected override Packet CreatePacket()
            => new ClientPacket()
            {
                MessageAuthenticationCode = 34,
                PacketId = 3,
                ClientId = 435,
                Flags = PacketFlags.Compressed | PacketFlags.NewProtocol,
                Type = PacketType.CommandLow,
                Data = new byte[] { 2, 123 }
            };
    }
}
