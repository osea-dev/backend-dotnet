using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace WeTalk.Common
{
	/// <summary>
	/// 复制对象
	/// </summary>
	public class CloneHelper
	{
		/// <summary>
		/// 使用二进制流
		/// </summary>
		public static class CloneByStream
		{
			public static T Clone<T>(T source)
			{
				if (!typeof(T).IsSerializable)
				{
					throw new ArgumentException("The type must be serializable.", "source");
				}

				if (Object.ReferenceEquals(source, null))
				{
					return default(T);
				}

				IFormatter formatter = new BinaryFormatter();
				Stream stream = new MemoryStream();
				using (stream)
				{
					formatter.Serialize(stream, source);
					stream.Seek(0, SeekOrigin.Begin);
					return (T)formatter.Deserialize(stream);
				}
			}
		}

		/// <summary>
		/// 使用反射
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static T CloneByReflect<T>(T obj)
		{
			//如果是字符串或值类型则直接返回
			if (obj is string || obj.GetType().IsValueType) return obj;
			object retval = Activator.CreateInstance(obj.GetType());
			FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
			foreach (FieldInfo field in fields)
			{
				try { field.SetValue(retval, CloneByReflect(field.GetValue(obj))); }
				catch { }
			}
			return (T)retval;
		}

		/// <summary>
		/// 使用序列化与反序列化
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T CloneByJson<T>(T source)
		{
			if (Object.ReferenceEquals(source, null))
			{
				return default(T);
			}

			var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
			return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
		}

	}

}