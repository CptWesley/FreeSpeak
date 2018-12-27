using FreeSpeak.Protocols.TeamSpeak.Data.Handshakes.Initiation;
using Xunit;
using static AssertNet.Xunit.Assertions;

namespace FreeSpeak.Protocols.TeamSpeak.Tests.Data.Initiation
{
    /// <summary>
    /// Test class for the <see cref="Init0"/> class.
    /// </summary>
    /// <seealso cref="DataTests" />
    public class Init0Tests : DataTests
    {
        /// <summary>
        /// Checks that we can convert the object to a byte array and back correctly.
        /// </summary>
        [Fact]
        public void ToAndFromBytes()
        {
            Init0 data = (Init0)CreateData();
            byte[] bytes = data.ToBytes();
            AssertThat(Init0.FromBytes(bytes)).IsEqualTo(data);
        }

        /// <inheritdoc/>
        protected override TeamSpeak.Data.Data CreateData()
        {
            return new Init0()
            {
                Version = 1,
                Step = 0,
                Time = 32435563,
                Random = 45654363,
                Reserved = 0
            };
        }
    }
}
