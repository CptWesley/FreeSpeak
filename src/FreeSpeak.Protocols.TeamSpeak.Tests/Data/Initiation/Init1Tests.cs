using FreeSpeak.Protocols.TeamSpeak.Data.Handshakes.Initiation;
using Xunit;
using static AssertNet.Xunit.Assertions;

namespace FreeSpeak.Protocols.TeamSpeak.Tests.Data.Initiation
{
    /// <summary>
    /// Test class for the <see cref="Init1"/> class.
    /// </summary>
    /// <seealso cref="DataTests" />
    public class Init1Tests : DataTests
    {
        /// <summary>
        /// Checks that we can convert the object to a byte array and back correctly.
        /// </summary>
        [Fact]
        public void ToAndFromBytes()
        {
            Init1 data = (Init1)CreateData();
            byte[] bytes = data.ToBytes();
            AssertThat(Init1.FromBytes(bytes)).IsEqualTo(data);
        }

        /// <inheritdoc/>
        protected override TeamSpeak.Data.Data CreateData()
        {
            return new Init1()
            {
                Step = 1,
                ServerData = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 },
                Random = 45654363
            };
        }
    }
}
