using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WeTalk.Common;
using WeTalk.Extensions;

namespace WeTalk.Interfaces
{
	public class BaseService: IBaseService
	{
        #region "Json特符字符过滤"
        /// <summary>
        /// Json特符字符过滤，参见http://www.json.org/
        /// </summary>
        /// <param name="sourceStr">要过滤的源字符串</param>
        /// <returns>返回过滤的字符串</returns>
        public string JsonCharFilter(string sourceStr)
		{
			if (string.IsNullOrEmpty(sourceStr)) return "";
			sourceStr = sourceStr.Replace("\\", "\\\\");
			sourceStr = sourceStr.Replace("\b", "\\b");
			sourceStr = sourceStr.Replace("\t", "\\t");
			sourceStr = sourceStr.Replace("\n", "\\n");
			sourceStr = sourceStr.Replace("\n", "\\n");
			sourceStr = sourceStr.Replace("\f", "\\f");
			sourceStr = sourceStr.Replace("\r", "\\r");
			return sourceStr.Replace("\"", "\\\"");
		}
		#endregion

		#region "内部跳转链接"
		/// <summary>
		/// 名片链接(分享)
		/// </summary>
		/// <param name="cardid"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetCardUrl(long userid, long cardid, string name)
		{
			return $"<a href=\"/pages/model/model?isList=true&cardid={cardid}&userId={userid}&type=g3\">{name}</a>";
		}


		/// <summary>
		/// 获取动态URL
		/// </summary>
		/// <param name="userid">操作人</param>
		/// <param name="cardid">动态作者</param>
		/// <param name="newsid">动态ID</param>
		/// <returns></returns>
		public string GetNewsUrl(long cardid, long newsid)
		{
			return $"/pages/child-page/dynamic-detail/dynamic-detail?cardid={cardid}&newsid={newsid}";
		}

		/// <summary>
		/// 获取动态URL
		/// </summary>
		/// <param name="userid">操作人</param>
		/// <param name="cardid">动态作者</param>
		/// <param name="newsid">动态ID</param>
		/// <param name="reviewid">动态评论ID</param>
		/// <returns></returns>
		public string GetNewsReviewUrl(long cardid, long newsid, long reviewid)
		{
			return $"/pagesA/dynamic/dynamic-reply/dynamic-reply?cardid={cardid}&newsid={newsid}&reviewid={reviewid}";
		}
		#endregion

		#region "Redis缓存与DB实体对象互转 "
		/// <summary>
		/// 缓存转实体对象
		/// </summary>
		/// <returns></returns>
		public T GetRedisObj<T>(string key)
		{
			object obj = null;
			if (RedisServer.Cache.Exists(key))
			{
				var val = RedisServer.Cache.Get(key);
				if (string.IsNullOrEmpty(val)) return default(T);
				return JsonConvert.DeserializeObject<T>(val);
			}
			return (T)obj;
		}
		public string GetRedisString(string key)
		{
			if (!string.IsNullOrEmpty(key) && RedisServer.Cache.Exists(key))
			{
				var val = RedisServer.Cache.Get(key);
				if (val != null)return val.ToString();
			}
			return "";
		}

		/// <summary>
		/// 实体对象转缓存
		/// </summary>
		/// <param name="key"></param>
		/// <param name="obj">对象</param>
		/// <param name="times">有效时长，默认1天</param>
		/// <returns></returns>
		public bool SetRedisObj(string key, object obj, int times = 86400)
		{
			bool isok = RedisServer.Cache.Set(key, JsonConvert.SerializeObject(obj));
			if (times > 0) RedisServer.Cache.Expire(key, times);
			return isok;
		}
		public bool SetRedisObj(string key, string val, int times = 86400)
		{
			if (string.IsNullOrEmpty(val)) return false;
			bool isok = RedisServer.Cache.Set(key, val);
			if (times > 0) RedisServer.Cache.Expire(key, times);
			return isok;
		}

		/// <summary>
		/// 删除指定KEY
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool DelRedisObj(string key)
		{
			var isok = RedisServer.Cache.Del(key);
			return isok > 0;
		}
		#endregion

		#region "哈希与C#对象互转"
		/// <summary>
		/// 对象转哈希
		/// </summary>
		/// <returns></returns>
		public object[] ToHashEntries(object obj)
		{
			PropertyInfo[] properties = obj.GetType().GetProperties();
			List<string> attrs = new List<string>();
			foreach (var attr in properties)
			{
				attrs.Add(attr.Name);
				attrs.Add(attr.GetValue(obj) + "");
			}
			return attrs.ToArray();
		}

		/// <summary>
		/// 哈希转对象
		/// </summary>
		/// <returns></returns>
		public T ConvertFromRedis<T>(Dictionary<string, string> dicEntries)
		{
			PropertyInfo[] properties = typeof(T).GetProperties();
			var obj = Activator.CreateInstance(typeof(T));
			foreach (var property in properties)
			{
				if (dicEntries.Any(u => u.Key == property.Name))
				{
					var entry = dicEntries.FirstOrDefault(g => g.Key.ToString().Equals(property.Name));
					if (entry.Equals(new Dictionary<string, string>())) continue;
					property.SetValue(obj, Convert.ChangeType(entry.Value, property.PropertyType));
				}
			}
			return (T)obj;
		}
        #endregion

    }
}
