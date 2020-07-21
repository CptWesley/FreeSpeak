using System;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace FreeSpeak.PacketProcessing
{
    /// <summary>
    /// Provides logic for encrypting and decrypting packets.
    /// </summary>
    internal static class Encryption
    {
        /// <summary>
        /// Decrypts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="nonce">The nonce.</param>
        /// <param name="header">The header.</param>
        /// <param name="data">The data.</param>
        /// <param name="mac">The mac.</param>
        /// <returns>The encrypted data.</returns>
        public static byte[] Decrypt(byte[] key, byte[] nonce, byte[] header, byte[] data, byte[] mac)
        {
            byte[] n = Cmac(key, 0, nonce);
            byte[] h = Cmac(key, 1, header);
            byte[] c = Cmac(key, 2, data);

            byte[] mac2 = Xor(Xor(n, h), c);

            return null;
        }

        private static byte[] Cmac(byte[] key, byte iv, byte[] data)
        {
            CMac cmac = new CMac(new AesEngine());
            cmac.Init(new KeyParameter(key));
            cmac.BlockUpdate(new byte[15], 0, 15);
            cmac.Update(iv);
            cmac.BlockUpdate(data, 0, data.Length);
            byte[] buffer = new byte[4096];
            int size = cmac.DoFinal(buffer, 0);
            byte[] result = new byte[size];
            Array.Copy(buffer, result, size);
            return result;
        }

        private static byte[] Xor(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length];

            for (int i = 0; i < a.Length; i++)
            {
                c[i] = (byte)(a[i] ^ b[i]);
            }

            return c;
        }
    }
}
