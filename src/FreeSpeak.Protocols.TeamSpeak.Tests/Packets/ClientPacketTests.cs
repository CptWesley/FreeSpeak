using FreeSpeak.Protocols.TeamSpeak.Packets;
using Xunit;

using static AssertNet.Xunit.Assertions;

namespace FreeSpeak.Protocols.TeamSpeak.Tests.Packets
{
    /// <summary>
    /// Test class for the <see cref="ClientPacket"/> class.
    /// </summary>
    public static class ClientPacketTests
    {
        /// <summary>
        /// Checks that the constructor sets the correct default values.
        /// </summary>
        [Fact]
        public static void ConstructorTest()
        {
            ClientPacket packet = new ClientPacket()
            {
                MessageAuthenticationCode = 3425,
                PacketId = 2,
                ClientId = 435,
                PacketType = 12,
                Data = new byte[] { 33, 45, 72 }
            };

            AssertThat(packet.MessageAuthenticationCode).IsEqualTo(3425);
            AssertThat(packet.PacketId).IsEqualTo(2);
            AssertThat(packet.ClientId).IsEqualTo(435);
            AssertThat(packet.PacketType).IsEqualTo(12);
            AssertThat(packet.Data).HasSize(3).ContainsExactly(33, 45, 72);
        }
    }
}
