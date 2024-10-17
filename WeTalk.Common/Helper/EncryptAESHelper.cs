﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace WeTalk.Common
{
	/// <summary>
	/// 高级加密标准 (AES) 算法
	/// </summary>
	public class EncryptAESHelper
	{
		#region 隐藏构造方法
		private EncryptAESHelper()
		{ }
		#endregion

		private static byte[] IV = new byte[] { 0x41, 0x72, 0x65, 0x79, 0x6f, 0x75, 0x6d, 0x79, 0x53, 110, 0x6f, 0x77, 0x6d, 0x61, 110, 0x3f };
	

		/// <summary>
		/// AES 加密
		/// </summary>
		/// <param name="cipherKey">加密密钥</param>
		/// <param name="data">待加密的文本</param>
		/// <returns>返回与此实例等效的加密文本</returns>
		public static string Encrypt(string cipherKey, string data)
		{
			byte[] byDncs;
			try
			{
				byDncs = Encoding.UTF8.GetBytes(data);
			}
			catch
			{
				return "";
			}

			RijndaelManaged cryptoProvider = new RijndaelManaged();
			cryptoProvider.Key = Encoding.UTF8.GetBytes(cipherKey);
			cryptoProvider.IV = IV;
			ICryptoTransform cTtransform = cryptoProvider.CreateEncryptor();

			byte[] byEncs = cTtransform.TransformFinalBlock(byDncs, 0, byDncs.Length);
			return Convert.ToBase64String(byEncs);
		}

		/// <summary>
		/// AES 解密
		/// </summary>
		/// <param name="cipherKey">解密密钥</param>
		/// <param name="data">待解密的文本</param>
		/// <returns>返回与此实例等效的解密文本</returns>
		public static string Decrypt(string cipherKey, string data)
		{
			byte[] byEnc;
			try
			{
				byEnc = Convert.FromBase64String(data);
			}
			catch
			{
				return "";
			}

			RijndaelManaged cryptoProvider = new RijndaelManaged();
			cryptoProvider.Key = Encoding.UTF8.GetBytes(cipherKey);
			cryptoProvider.IV = IV;
			
			ICryptoTransform cTransform = cryptoProvider.CreateDecryptor();

			byte[] byDncs = cTransform.TransformFinalBlock(byEnc, 0, byEnc.Length);
			return Encoding.UTF8.GetString(byDncs);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cipherKey">解密密钥</param>
		/// <param name="iv">向量</param>
		/// <param name="data">待解密的文本</param>
		/// <returns></returns>
		public static string Decrypt(string cipherKey, string iv, string data)
		{
			byte[] byEnc;
			try
			{
				byEnc = Convert.FromBase64String(data);
			}
			catch
			{
				return "";
			}

			RijndaelManaged cryptoProvider = new RijndaelManaged();
			cryptoProvider.Key = Convert.FromBase64String(cipherKey);
			cryptoProvider.IV = Convert.FromBase64String(iv);
			cryptoProvider.Mode = CipherMode.CBC;
			cryptoProvider.Padding = PaddingMode.PKCS7;
			ICryptoTransform cTransform = cryptoProvider.CreateDecryptor();

			byte[] byDncs = cTransform.TransformFinalBlock(byEnc, 0, byEnc.Length);
			return Encoding.UTF8.GetString(byDncs);
		}

	}
}
