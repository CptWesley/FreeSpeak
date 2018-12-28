using System.Net;

namespace FreeSpeak.Servers.TeamSpeak
{
    /// <summary>
    /// Class containing connection info with a certain client.
    /// </summary>
    public class ClientIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientIdentity"/> class.
        /// </summary>
        /// <param name="endPoint">The end point on which the client communicates.</param>
        public ClientIdentity(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
        }

        /// <summary>
        /// Gets the end point used to communicate with the client.
        /// </summary>
        public IPEndPoint EndPoint { get; }
    }
}
