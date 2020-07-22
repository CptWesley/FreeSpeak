using System;
using System.Linq;
using System.Security.Cryptography;
using ExtensionNet;
using FreeSpeak.Packets;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

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
            byte[] n;
            byte[] h;
            byte[] c;

            try
            {
                n = Cmac(key, 0, nonce);
                h = Cmac(key, 1, header);
                c = Cmac(key, 2, data);
            }
            catch
            {
                throw new IllegalClientOperationException("Failed to pass the CMAC pass of EAX mode while decrypting.");
            }

            byte[] otherMac = new byte[mac.Length];
            Array.Copy(Xor(Xor(n, h), c), otherMac, otherMac.Length);

            if (!Enumerable.SequenceEqual(mac, otherMac))
            {
                //throw new IllegalClientOperationException("Encryption MAC does not match.");
            }

            try
            {
                return Ctr(key, n, data, false);
            }
            catch
            {
                throw new IllegalClientOperationException("Failed to pass the AES counter mode pass of EAX mode while decrypting.");
            }
        }

        /// <summary>
        /// Encrypts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="nonce">The nonce.</param>
        /// <param name="header">The header.</param>
        /// <param name="data">The data.</param>
        /// <param name="mac">The mac.</param>
        /// <returns>The encrypted data.</returns>
        public static byte[] Encrypt(byte[] key, byte[] nonce, byte[] header, byte[] data, out byte[] mac)
        {
            byte[] n;
            byte[] h;
            byte[] c;

            try
            {
                n = Cmac(key, 0, nonce);
                h = Cmac(key, 1, header);
                c = Cmac(key, 2, data);
            }
            catch
            {
                throw new IllegalClientOperationException("Failed to pass the CMAC pass of EAX mode while encrypting.");
            }

            mac = new byte[8];
            Array.Copy(Xor(Xor(n, h), c), mac, mac.Length);

            try
            {
                return Ctr(key, n, data, true);
            }
            catch
            {
                throw new IllegalClientOperationException("Failed to pass the AES counter mode pass of EAX mode while encrypting.");
            }
        }

        /// <summary>
        /// Generates the key.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="packetId">The packet identifier.</param>
        /// <param name="packetGeneration">The packet generation.</param>
        /// <param name="direction">The packet direction.</param>
        /// <param name="siv">The shared IV.</param>
        /// <returns>The key and nonce for encrypting.</returns>
        public static (byte[] Key, byte[] Nonce) GenerateKey(PacketType type, ushort packetId, uint packetGeneration, bool direction, byte[] siv)
        {
            byte[] temp = new byte[6 + siv.Length];
            temp[0] = direction ? (byte)0x30 : (byte)0x31;
            temp[1] = (byte)type;
            Array.Copy(packetGeneration.GetBytes(), 0, temp, 2, 4);
            Array.Copy(siv, 0, temp, 6, siv.Length);
            using SHA256 sha256 = SHA256.Create();
            byte[] keyNonce = sha256.ComputeHash(temp);

            byte[] key = new byte[16];
            byte[] nonce = new byte[16];
            Array.Copy(keyNonce, 0, key, 0, 16);
            Array.Copy(keyNonce, 16, nonce, 0, 16);

            key[0] = (byte)(key[0] ^ (byte)((packetId & 0xFF00) >> 8));
            key[1] = (byte)(key[1] ^ (byte)((packetId & 0x00FF) >> 0));

            return (key, nonce);
        }

        /// <summary>
        /// Generates a key pair.
        /// </summary>
        /// <returns>The generated key pair.</returns>
        public static AsymmetricCipherKeyPair GenerateKeys()
        {
            X9ECParameters curve = ECNamedCurveTable.GetByName("prime256v1");
            ECDomainParameters domainParameters = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            ECKeyGenerationParameters parameters = new ECKeyGenerationParameters(domainParameters, new SecureRandom());
            ECKeyPairGenerator generator = new ECKeyPairGenerator("ECDH");

            generator.Init(parameters);
            return generator.GenerateKeyPair();
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

        private static byte[] Ctr(byte[] key, byte[] n, byte[] data, bool encrypt)
        {
            BufferedBlockCipher ctr = new BufferedBlockCipher(new SicBlockCipher(new AesEngine()));
            ctr.Init(encrypt, new ParametersWithIV(new KeyParameter(key), n));
            byte[] buffer = new byte[4096];
            int size = ctr.ProcessBytes(data, 0, data.Length, buffer, 0);
            size += ctr.DoFinal(buffer, size);
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
