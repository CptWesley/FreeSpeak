using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace FreeSpeak.PacketProcessing
{
    /// <summary>
    /// Provides extension methods for encryption related things.
    /// </summary>
    public static class EncryptionExtensions
    {
        /// <summary>
        /// Gets the private key.
        /// </summary>
        /// <param name="pair">The pair.</param>
        /// <returns>The bytes of the private key.</returns>
        public static byte[] GetPrivateKey(this AsymmetricCipherKeyPair pair)
        {
            if (pair is null)
            {
                throw new ArgumentNullException(nameof(pair));
            }

            ECPrivateKeyParameters privateKey = pair.Private as ECPrivateKeyParameters;

            return privateKey.D.ToByteArrayUnsigned();
        }

        /// <summary>
        /// Gets the public key.
        /// </summary>
        /// <param name="pair">The pair.</param>
        /// <returns>The bytes of the public key.</returns>
        public static byte[] GetPublicKey(this AsymmetricCipherKeyPair pair)
        {
            if (pair is null)
            {
                throw new ArgumentNullException(nameof(pair));
            }

            ECPublicKeyParameters publicKey = pair.Public as ECPublicKeyParameters;

            return publicKey.Q.GetEncoded();
        }
    }
}
