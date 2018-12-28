using FreeSpeak.Protocols.TeamSpeak.Compression;
using Xunit;
using static AssertNet.Xunit.Assertions;

namespace FreeSpeak.Protocols.TeamSpeak.Tests.Compression
{
    /// <summary>
    /// Test class for the <see cref="Compressor"/> class.
    /// </summary>
    public class CompressorTests
    {
        private Compressor compressor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressorTests"/> class.
        /// </summary>
        public CompressorTests()
        {
            compressor = new Compressor();
        }
    }
}
