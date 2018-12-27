using ExtensionNet.Reflective;
using Xunit;
using static AssertNet.Xunit.Assertions;

namespace FreeSpeak.Protocols.TeamSpeak.Tests.Data
{
    /// <summary>
    /// Test class for classes inheriting from the <see cref="TeamSpeak.Data.Data"/> class.
    /// </summary>
    public abstract class DataTests
    {
        /// <summary>
        /// Checks that equality works correctly.
        /// </summary>
        [Fact]
        public void EqualSelfTest()
        {
            TeamSpeak.Data.Data data = CreateData();
            AssertThat(data).IsEqualTo(data);
            AssertThat(data).HasSameHashCodeAs(data);
        }

        /// <summary>
        /// Checks that equality works correctly.
        /// </summary>
        [Fact]
        public void EqualCopyTest()
        {
            TeamSpeak.Data.Data data = CreateData();
            TeamSpeak.Data.Data copy = data.Copy(true);
            AssertThat(copy).IsEqualTo(data);
            AssertThat(copy).HasSameHashCodeAs(data);
        }

        /// <summary>
        /// Gets the data instance under test.
        /// </summary>
        /// <returns>The data instance under test.</returns>
        protected abstract TeamSpeak.Data.Data CreateData();
    }
}
