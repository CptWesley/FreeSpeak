using System;
using FreeSpeak.Protocols.TeamSpeak.Packets;
using Xunit;
using static AssertNet.Xunit.Assertions;

namespace FreeSpeak.Protocols.TeamSpeak.Tests.Packets
{
    /// <summary>
    /// Abstract test class for classes inheriting from the <see cref="Packet"/> class.
    /// </summary>
    public abstract class PacketTests
    {
        /// <summary>
        /// Checks that the constructor sets the correct default values.
        /// </summary>
        [Fact]
        public void ConstructorTest()
        {
            Packet packet = CreatePacket();
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
            Packet packet = CreatePacket();
            byte[] bytes = new byte[2048];
            AssertThat(() => packet.Data = bytes).ThrowsExactlyException<ArgumentException>();
        }

        /// <summary>
        /// Checks that an object is equal to itself.
        /// </summary>
        [Fact]
        public void EqualsSelfTest()
        {
            Packet packet = CreatePacket();
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
            Packet packet = CreatePacket();
            Packet copy = CreatePacket();
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
            Packet packet = CreatePacket();
            Packet copy = CreatePacket();
            copy.MessageAuthenticationCode = 0;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with a different pid.
        /// </summary>
        [Fact]
        public void EqualsDifferentPidTest()
        {
            Packet packet = CreatePacket();
            Packet copy = CreatePacket();
            copy.PacketId = 0;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with different flags.
        /// </summary>
        [Fact]
        public void EqualsDifferentFlagsTest()
        {
            Packet packet = CreatePacket();
            Packet copy = CreatePacket();
            copy.Flags = PacketFlags.None;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with a different type.
        /// </summary>
        [Fact]
        public void EqualsDifferentTypeTest()
        {
            Packet packet = CreatePacket();
            Packet copy = CreatePacket();
            copy.Type = PacketType.Pong;
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Checks that an object is not equal with different data.
        /// </summary>
        [Fact]
        public void EqualsDifferentDataTest()
        {
            Packet packet = CreatePacket();
            Packet copy = CreatePacket();
            copy.Data = new byte[42];
            AssertThat(packet).IsNotEqualTo(copy);
        }

        /// <summary>
        /// Creates a new packet for testing purposes.
        /// </summary>
        /// <returns>A packet for testing.</returns>
        protected abstract Packet CreatePacket();
    }
}
