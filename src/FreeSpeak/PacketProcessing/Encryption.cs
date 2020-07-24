using System;
using System.IO;
using System.Linq;
using ExtensionNet;
using FreeSpeak.Packets;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
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
        public static byte[] Decrypt(byte[] key, byte[] nonce, byte[] header, byte[] data, ulong mac)
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

            byte[] macBytes = mac.GetBytes(Endianness.BigEndian);
            byte[] otherMac = new byte[macBytes.Length];
            Array.Copy(Xor(Xor(n, h), c), otherMac, otherMac.Length);

            if (!Enumerable.SequenceEqual(macBytes, otherMac))
            {
                throw new IllegalClientOperationException("Encryption MAC does not match.");
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
        /// <returns>The encrypted data.</returns>
        public static (byte[] Data, ulong Mac) Encrypt(byte[] key, byte[] nonce, byte[] header, byte[] data)
        {
            byte[] n;
            byte[] h;
            byte[] c;

            try
            {
                n = Cmac(key, 0, nonce);
                h = Cmac(key, 1, header);
            }
            catch
            {
                throw new IllegalClientOperationException("Failed to pass the CMAC pass of EAX mode while encrypting.");
            }

            byte[] result;
            try
            {
                result = Ctr(key, n, data, true);
            }
            catch
            {
                throw new IllegalClientOperationException("Failed to pass the AES counter mode pass of EAX mode while encrypting.");
            }

            try
            {
                c = Cmac(key, 2, result);
            }
            catch
            {
                throw new IllegalClientOperationException("Failed to pass the CMAC pass of EAX mode while encrypting.");
            }

            byte[] mac = new byte[8];
            Array.Copy(Xor(Xor(n, h), c), mac, mac.Length);

            return (result, mac.ToUInt64(Endianness.BigEndian));
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
            using System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
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
        public static (ECPrivateKeyParameters Private, ECPublicKeyParameters Public) GenerateKeys()
        {
            X9ECParameters curve = ECNamedCurveTable.GetByName("prime256v1");
            ECDomainParameters domainParameters = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            ECKeyGenerationParameters parameters = new ECKeyGenerationParameters(domainParameters, new SecureRandom());
            ECKeyPairGenerator generator = new ECKeyPairGenerator("ECDH");

            generator.Init(parameters);
            AsymmetricCipherKeyPair keys = generator.GenerateKeyPair();
            return ((ECPrivateKeyParameters)keys.Private, (ECPublicKeyParameters)keys.Public);
        }

        /// <summary>
        /// Computes the shared mac and iv.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="beta">The beta.</param>
        /// <param name="privateKey">The private key.</param>
        /// <param name="publicKey">The public key.</param>
        /// <returns>The shared mac and shared iv.</returns>
        public static (byte[] SharedMac, byte[] SharedIV) ComputeShared(string alpha, string beta, ECPrivateKeyParameters privateKey, ECPublicKeyParameters publicKey)
        {
            ECPoint sharedSecret = publicKey.Q.Multiply(privateKey.D).Normalize();

            byte[] sharedData = new byte[32];

            byte[] x = sharedSecret.XCoord.GetEncoded();
            if (x.Length <= 32)
            {
                Array.Copy(x, 0, sharedData, 32 - x.Length, x.Length);
            }
            else
            {
                Array.Copy(x, x.Length - 32, sharedData, 0, 32);
            }

            using System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] sharedIV = sha1.ComputeHash(sharedData);

            byte[] siv1 = new byte[10];
            Array.Copy(sharedIV, 0, siv1, 0, 10);
            byte[] siv2 = new byte[10];
            Array.Copy(sharedIV, 10, siv2, 0, 10);

            siv1 = Xor(siv1, Encoding.Base64(alpha));
            siv2 = Xor(siv2, Encoding.Base64(beta));

            Array.Copy(siv1, 0, sharedIV, 0, 10);
            Array.Copy(siv2, 0, sharedIV, 10, 10);

            byte[] sharedMac = new byte[8];
            Array.Copy(sha1.ComputeHash(sharedIV), sharedMac, 8);
            return (sharedMac, sharedIV);
        }

        /// <summary>
        /// Creates a public key from an omega string.
        /// </summary>
        /// <param name="omega">The omega string.</param>
        /// <returns>The public key represented by the omega string.</returns>
        public static ECPublicKeyParameters FromOmega(string omega)
        {
            byte[] publicKey = Encoding.Base64(omega);
            DerSequence asn = Asn1Object.FromByteArray(publicKey) as DerSequence;
            DerInteger x = asn[2] as DerInteger;
            DerInteger y = asn[3] as DerInteger;
            X9ECParameters curveParameters = ECNamedCurveTable.GetByName("prime256v1");
            ECPoint p = curveParameters.Curve.CreatePoint(x.Value, y.Value);
            ECDomainParameters domainParameters = new ECDomainParameters(curveParameters.Curve, curveParameters.G, curveParameters.N, curveParameters.H, curveParameters.GetSeed());
            return new ECPublicKeyParameters("ECDH", p, domainParameters);
        }

        /// <summary>
        /// Creates an omega string from a public key.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <returns>The omega string.</returns>
        public static string ToOmega(ECPublicKeyParameters publicKey)
        {
            byte[] xBytes = publicKey.Q.AffineXCoord.ToBigInteger().ToByteArray();
            byte[] yBytes = publicKey.Q.AffineYCoord.ToBigInteger().ToByteArray();

            Asn1Encodable bit = new DerBitString(new byte[] { 0 }, 7);
            Asn1Encodable keySize = new DerInteger(32);
            Asn1Encodable x = new DerInteger(new BigInteger(xBytes));
            Asn1Encodable y = new DerInteger(new BigInteger(yBytes));
            DerSequence seq = new DerSequence(bit, keySize, x, y);
            byte[] bytes = seq.ToAsn1Object().GetDerEncoded();
            return Encoding.Base64(bytes);
        }

        /// <summary>
        /// Gets the header bytes.
        /// </summary>
        /// <param name="packetId">The packet ID.</param>
        /// <param name="clientId">The client ID.</param>
        /// <param name="type">The packet type.</param>
        /// <param name="flags">The packet flags.</param>
        /// <returns>The bytes of the header.</returns>
        public static byte[] GetHeader(ushort packetId, ushort clientId, PacketType type, PacketFlags flags)
        {
            using MemoryStream ms = new MemoryStream();
            ms.Write(packetId, Endianness.BigEndian);
            ms.Write(clientId, Endianness.BigEndian);
            ms.Write((byte)((byte)type + (byte)flags));
            return ms.ToArray();
        }

        /// <summary>
        /// Gets the header bytes.
        /// </summary>
        /// <param name="packetId">The packet ID.</param>
        /// <param name="type">The packet type.</param>
        /// <param name="flags">The packet flags.</param>
        /// <returns>The bytes of the header.</returns>
        public static byte[] GetHeader(ushort packetId, PacketType type, PacketFlags flags)
        {
            using MemoryStream ms = new MemoryStream();
            ms.Write(packetId, Endianness.BigEndian);
            ms.Write((byte)((byte)type + (byte)flags));
            return ms.ToArray();
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
