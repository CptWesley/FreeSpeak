namespace FreeSpeak.Servers
{
    /// <summary>
    /// Dummy logger which does nothing with the messages.
    /// </summary>
    /// <seealso cref="ILogger" />
    public class DummyLogger : ILogger
    {
        /// <inheritdoc/>
        public void WriteError(string message)
        {
        }

        /// <inheritdoc/>
        public void WriteInfo(string message)
        {
        }

        /// <inheritdoc/>
        public void WriteWarning(string message)
        {
        }
    }
}
