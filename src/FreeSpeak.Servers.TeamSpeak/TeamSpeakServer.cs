using System.Collections.Generic;
using System.Net;

namespace FreeSpeak.Servers.TeamSpeak
{
    /// <summary>
    /// Represents a TeamSpeak server.
    /// </summary>
    /// <seealso cref="Server" />
    public class TeamSpeakServer : Server
    {
        private Dictionary<IPEndPoint, ClientIdentity> identities;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSpeakServer"/> class.
        /// </summary>
        /// <param name="port">The port on which the server is listening.</param>
        public TeamSpeakServer(int port)
            : base(port)
        {
            identities = new Dictionary<IPEndPoint, ClientIdentity>();
        }

        /// <inheritdoc/>
        protected override void MessageReceivedCallback(IPEndPoint ep, byte[] message)
        {
            if (identities.ContainsKey(ep))
            {
                Logger.WriteInfo($"Knows {ep}");
            }
            else
            {
                Logger.WriteInfo($"Doesn't know {ep}");
                identities.Add(ep, new ClientIdentity(ep));
                Send(ep, new byte[24]);
            }
        }
    }
}
