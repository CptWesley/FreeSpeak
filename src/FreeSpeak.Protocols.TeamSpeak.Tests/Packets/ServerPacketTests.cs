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
    public class ServerPacketTests
    {
        private ServerPacket packet;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerPacketTests"/> class.
        /// </summary>
        public ServerPacketTests()
        {
            packet = new ServerPacket()
            {
                MessageAuthenticationCode = 34,
                PacketId = 3,
                Flags = PacketFlags.Compressed | PacketFlags.NewProtocol,
                Type = PacketType.CommandLow,
                Data = new byte[] { 2, 123 }
            };
        }

        /// <summary>
        /// Checks that the constructor sets the correct default values.
        /// </summary>
        [Fact]
        public void ConstructorTest()
        {
            AssertThat(packet.MessageAuthenticationCode).IsEqualTo(34);
            AssertThat(packet.PacketId).IsEqualTo(3);
            AssertThat(packet.Flags).IsEqualTo(PacketFlags.Compressed | PacketFlags.NewProtocol);
            AssertThat(packet.Type).IsEqualTo(PacketType.CommandLow);
            AssertThat(packet.Data).HasSize(2).ContainsExactly(2, 123);
        }

        /// <summary>
        /// Checks that we can't set too large data.
        /// </summary>
        [Fact]
        public void TooMuchDataTest()
        {
            byte[] bytes = new byte[2048];
            AssertThat(() => packet.Data = bytes).ThrowsExactlyException<ArgumentException>()
                .WithMessage("Maximum data length is 489, but tried to set 2048 bytes.");
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
            ServerPacket copy = packet.Copy(true);
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
            ServerPacket copy = packet.Copy(true);
            copy.MessageAuthenticationCode = 0;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with a different pid.
        /// </summary>
        [Fact]
        public void EqualsDifferentPidTest()
        {
            ServerPacket copy = packet.Copy(true);
            copy.PacketId = 0;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with different flags.
        /// </summary>
        [Fact]
        public void EqualsDifferentFlagsTest()
        {
            ServerPacket copy = packet.Copy(true);
            copy.Flags = PacketFlags.None;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with a different type.
        /// </summary>
        [Fact]
        public void EqualsDifferentTypeTest()
        {
            ServerPacket copy = packet.Copy(true);
            copy.Type = PacketType.Pong;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with different data.
        /// </summary>
        [Fact]
        public void EqualsDifferentDataTest()
        {
            ServerPacket copy = packet.Copy(true);
            copy.Data = new byte[42];
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that we can convert a packet to bytes and read them as bytes again.
        /// </summary>
        [Fact]
        public void ToAndFromBytesTest()
        {
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
    }
}
