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
    public class ClientPacketTests
    {
        private ClientPacket packet;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientPacketTests"/> class.
        /// </summary>
        public ClientPacketTests()
        {
            packet = new ClientPacket()
            {
                MessageAuthenticationCode = 3425,
                PacketId = 2,
                ClientId = 435,
                Flags = PacketFlags.Compressed | PacketFlags.Fragmented,
                Type = PacketType.Voice,
                Data = new byte[] { 33, 45, 72 }
            };
        }

        /// <summary>
        /// Checks that the constructor sets the correct default values.
        /// </summary>
        [Fact]
        public void ConstructorTest()
        {
            AssertThat(packet.MessageAuthenticationCode).IsEqualTo(3425);
            AssertThat(packet.PacketId).IsEqualTo(2);
            AssertThat(packet.ClientId).IsEqualTo(435);
            AssertThat(packet.Flags).IsEqualTo(PacketFlags.Compressed | PacketFlags.Fragmented);
            AssertThat(packet.Type).IsEqualTo(PacketType.Voice);
            AssertThat(packet.Data).HasSize(3).ContainsExactly(33, 45, 72);
        }

        /// <summary>
        /// Checks that we can't set too large data.
        /// </summary>
        [Fact]
        public void TooMuchDataTest()
        {
            byte[] bytes = new byte[2048];
            AssertThat(() => packet.Data = bytes).ThrowsExactlyException<ArgumentException>()
                .WithMessage("Maximum data length is 487, but tried to set 2048 bytes.");
        }

        /// <summary>
        /// Checks that an object is equal to itself.
        /// </summary>
        [Fact]
        public void EqualsSelfTest()
        {
            AssertThat(packet)
                .IsSameAs(packet)
                .IsEqualTo(packet)
                .HasSameHashCodeAs(packet);
        }

        /// <summary>
        /// Checks that an object is equal to a deep copy.
        /// </summary>
        [Fact]
        public void EqualsCopyTest()
        {
            ClientPacket copy = packet.Copy(true);
            AssertThat(packet)
                .IsNotSameAs(copy)
                .IsEqualTo(copy)
                .HasSameHashCodeAs(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with a different mac.
        /// </summary>
        [Fact]
        public void EqualsDifferentMacTest()
        {
            ClientPacket copy = packet.Copy(true);
            copy.MessageAuthenticationCode = 0;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with a different pid.
        /// </summary>
        [Fact]
        public void EqualsDifferentPidTest()
        {
            ClientPacket copy = packet.Copy(true);
            copy.PacketId = 0;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with a different cid.
        /// </summary>
        [Fact]
        public void EqualsDifferentCidTest()
        {
            ClientPacket copy = packet.Copy(true);
            copy.ClientId = 0;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with different flags.
        /// </summary>
        [Fact]
        public void EqualsDifferentFlagsTest()
        {
            ClientPacket copy = packet.Copy(true);
            copy.Flags = PacketFlags.None;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with a different type.
        /// </summary>
        [Fact]
        public void EqualsDifferentTypeTest()
        {
            ClientPacket copy = packet.Copy(true);
            copy.Type = PacketType.Pong;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with different data.
        /// </summary>
        [Fact]
        public void EqualsDifferentDataTest()
        {
            ClientPacket copy = packet.Copy(true);
            copy.Data = new byte[42];
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that we can convert a packet to bytes and read them as bytes again.
        /// </summary>
        [Fact]
        public void ToAndFromBytesTest()
        {
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
    }
}
