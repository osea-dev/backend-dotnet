using System;
using System.Security.Cryptography;
using System.Text;

namespace WeTalk.Common.Helper
{
	/// <summary>
	/// 随机函数
	/// </summary>
	public class RandomHelper
	{

		/// <summary>
		/// 生成设置范围内的Double的随机数
		/// eg:_random.NextDouble(1.5,2.5)
		/// </summary>
		/// <param name="random">Random</param>
		/// <param name="miniDouble">生成随机数的最大值</param>
		/// <param name="maxiDouble">生成随机数的最小值</param>
		/// <returns>当Random等于NULL的时候返回0;</returns>
		public static double NextDouble(Random random, double miniDouble, double maxiDouble)
		{
			if (random != null)
			{
				return random.NextDouble() * (maxiDouble - miniDouble) + miniDouble;
			}
			else
			{
				return 0.0d;
			}
		}

		#region 数字随机数
		/// <summary>
		/// 数字随机数
		/// </summary>
		/// <param name="n">生成长度</param>
		/// <returns></returns>
		public static string RandNum(int n)
		{
			char[] arrChar = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
			StringBuilder num = new StringBuilder();

			Random rnd = new Random(DateTime.Now.Millisecond);

			for (int i = 0; i < n; i++)
			{
				num.Append(arrChar[rnd.Next(0, 9)].ToString());

			}

			return num.ToString();
		}
		/// <summary>
		/// 0~1的数字随机数
		/// </summary>
		/// <returns></returns>
		public static double RandNum()
		{
			var seed = Guid.NewGuid().GetHashCode();
			Random r = new Random(seed);
			int i = r.Next(0, 100000);
			return (double)i / 100000;
		}

		/// <summary>
		/// 生成指定范围的数字随机数
		/// </summary>
		/// <returns></returns>
		public static int RandNum(int start, int end)
		{
			var seed = Guid.NewGuid().GetHashCode();
			Random r = new Random(seed);
			int i = r.Next(start, end);
			return i;
		}
		#endregion

		#region 数字和字母随机数
		/// <summary>
		/// 数字和字母随机数
		/// </summary>
		/// <param name="n">生成长度</param>
		/// <returns></returns>
		public static string RandCode(int n)
		{
			char[] arrChar = new char[]{
										 'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
										 '0','1','2','3','4','5','6','7','8','9',
										 'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'
									 };

			StringBuilder num = new StringBuilder();

			Random rnd = new Random(DateTime.Now.Millisecond);
			for (int i = 0; i < n; i++)
			{
				num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());

			}

			return num.ToString();
		}
		#endregion

		#region 数字和字母随机数
		/// <summary>
		/// 数字和字母随机数
		/// </summary>
		/// <param name="n">生成长度</param>
		/// <returns></returns>
		public static string RandCode(int n, int seed = 0)
		{
			char[] arrChar = new char[]{
										 'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
										 '0','1','2','3','4','5','6','7','8','9',
										 'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'
									 };

			StringBuilder num = new StringBuilder();

			Random rnd = new Random(DateTime.Now.Millisecond + seed);
			for (int i = 0; i < n; i++)
			{
				num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());

			}

			return num.ToString();
		}
		#endregion

		#region 字母随机数
		/// <summary>
		/// 字母随机数
		/// </summary>
		/// <param name="n">生成长度</param>
		/// <returns></returns>
		public static string RandLetter(int n)
		{
			char[] arrChar = new char[]{
										  'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
										  '_',
										 'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'
									 };

			StringBuilder num = new StringBuilder();

			Random rnd = new Random(DateTime.Now.Millisecond);
			for (int i = 0; i < n; i++)
			{
				num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());

			}

			return num.ToString();
		}
		#endregion

		#region 日期随机函数
		/// <summary>
		/// 日期随机函数
		/// </summary>
		/// <returns></returns>
		public static string DateRndName()
		{
			string fname;
			var seed = Guid.NewGuid().GetHashCode();
			Random Rnd1 = new Random(seed);
			fname = DateTime.Now.Year.ToString();
			fname = fname + DateTime.Now.Month;
			fname = fname + DateTime.Now.Day;
			fname = fname + DateTime.Now.Hour;
			fname = fname + DateTime.Now.Minute;
			fname = fname + DateTime.Now.Second;
			fname = fname + DateTime.Now.Millisecond;
			fname = fname + Rnd1.Next(1, 7).ToString();
			return fname;
		}
		#endregion

		#region 生成GUID
		/// <summary>
		/// 生成GUID
		/// </summary>
		/// <returns></returns>
		public static string GetGuid()
		{
			System.Guid g = System.Guid.NewGuid();
			return g.ToString();
		}
		#endregion

		#region "创建MD5随机数"
		/// <summary>
		/// 创建随机数
		/// </summary>
		/// <param name="str">随机种子</param>
		/// <param name="n">16,32,64位</param>
		/// <returns></returns>
		public static string GetMd5Code(string str = "uelike", int n = 32)
		{
			System.Guid g = System.Guid.NewGuid();
			string s = MD5Helper.MD5Encrypt32(g.ToString() + str);
			switch (n)
			{
				case 16:
					s = s.Substring(8, 16);
					break;
				case 32:
					//s = s;
					break;
				case 64:
					s = s + MD5Helper.MD5Encrypt32(s + DateTime.Now.Millisecond.ToString() + str);
					break;
				default:
					if (s.Length >= n) s = s.Substring(0, n);
					break;
			}
			return s;
		}
		#endregion

		#region "获取SAH随机数"
		/// <summary>
		/// SAH加密
		/// </summary>
		/// <param name="data">加密数</param>
		/// <returns></returns>
		public static string GetSHA256(string Source_String = "uelike")
		{
			System.Guid g = System.Guid.NewGuid();
			Source_String = MD5Helper.MD5Encrypt32(g.ToString() + Source_String);
			byte[] bytes = Encoding.UTF8.GetBytes(Source_String);
			byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				builder.Append(hash[i].ToString("X2"));
			}
			return builder.ToString();
		}
		public static string GetSHA1(string Source_String = "uelike")
		{
			System.Guid g = System.Guid.NewGuid();
			Source_String = MD5Helper.MD5Encrypt32(g.ToString() + Source_String);
			//SHA1加密方法
			var sha1 = new SHA1CryptoServiceProvider();
			byte[] str01 = Encoding.Default.GetBytes(Source_String);
			byte[] str02 = sha1.ComputeHash(str01);
			var result = BitConverter.ToString(str02).Replace("-", "");
			return result;
		}
		#endregion
	}
}