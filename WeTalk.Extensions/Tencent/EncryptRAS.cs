using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Security.Cryptography;
using System.Text;

namespace WeTalk.Extensions.Tencent
{
	/// <summary>
	/// 高级加密标准 (AES) 算法
	/// </summary>
	public class EncryptRAS
	{
		#region 隐藏构造方法
		private EncryptRAS()
		{ }
        #endregion
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="publickey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string publickey, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);

            return Convert.ToBase64String(cipherbytes);
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privatekey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSADecrypt(string privatekey, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privatekey);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(content), false);

            return Encoding.UTF8.GetString(cipherbytes);
        }

        /// <summary>
        /// 使用BouncyCastle进行AEAD_AES_256_GCM 解密
        /// </summary>
        /// <param name="key">key:32位字符</param>
        /// <param name="nonce">随机串12位</param>
        /// <param name="cipherData">密文(Base64字符)</param>
        /// <param name="associatedData">附加数据可能null</param>
        /// <returns></returns>
        public static string AesGcmDecrypt(string key, string nonce, string cipherData, string associatedData)
        {
            var associatedBytes = associatedData == null ? null : Encoding.UTF8.GetBytes(associatedData);

            var gcmBlockCipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(
                new KeyParameter(Encoding.UTF8.GetBytes(key)),
                128,  //128 = 16 * 8 => (tag size * 8)
                Encoding.UTF8.GetBytes(nonce),
                associatedBytes);
            gcmBlockCipher.Init(false, parameters);

            var data = Convert.FromBase64String(cipherData);
            var plaintext = new byte[gcmBlockCipher.GetOutputSize(data.Length)];

            var length = gcmBlockCipher.ProcessBytes(data, 0, data.Length, plaintext, 0);
            gcmBlockCipher.DoFinal(plaintext, length);
            return Encoding.UTF8.GetString(plaintext);
        }

        /// <summary>
        /// 使用BouncyCastle进行AEAD_AES_256_GCM 加密
        /// </summary>
        /// <param name="key">key32位字符</param>
        /// <param name="nonce">随机串12位</param>
        /// <param name="plainData">明文</param>
        /// <param name="associatedData">附加数据可能null</param>
        /// <returns></returns>
        public static string AesGcmEncrypt(string key, string nonce, string plainData, string associatedData)
        {
            var associatedBytes = associatedData == null ? null : Encoding.UTF8.GetBytes(associatedData);

            var gcmBlockCipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(
                new KeyParameter(Encoding.UTF8.GetBytes(key)),
                128, //128 = 16 * 8 => (tag size * 8)
                Encoding.UTF8.GetBytes(nonce),
                associatedBytes);
            gcmBlockCipher.Init(true, parameters);

            var data = Encoding.UTF8.GetBytes(plainData);
            var cipherData = new byte[gcmBlockCipher.GetOutputSize(data.Length)];

            var length = gcmBlockCipher.ProcessBytes(data, 0, data.Length, cipherData, 0);
            gcmBlockCipher.DoFinal(cipherData, length);
            return Convert.ToBase64String(cipherData);
        }

    }
}
