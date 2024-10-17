/*
* ==============================================================================
*
* FileName: RedisCache.cs
* Created: 2020/7/7 16:05:34
* Author: Meiam
* Description: 
*
* ==============================================================================
*/
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using WeTalk.Common.Helper;

namespace WeTalk.Extensions
{
	/// <summary>
	/// 实现SqlSugar的ICacheService接口，为了让SqlSugar直接支持简单缓存操作。
	/// </summary>
	public class SugarRedisCache : ICacheService
	{
		public void Add<V>(string key, V value)
		{
			RedisServer.Data.Set(key, value);
		}

		public void Add<V>(string key, V value, int cacheDurationInSeconds)
		{
			RedisServer.Data.Set(key, value, cacheDurationInSeconds);
		}

		public bool ContainsKey<V>(string key)
		{
			return RedisServer.Data.Exists(key);
		}

		public V Get<V>(string key)
		{
			return RedisServer.Data.Get<V>(key);
		}

		public IEnumerable<string> GetAllKey<V>()
		{
			return RedisServer.Data.Keys("Data:SqlSugarDataCache.*");
		}



		public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
		{
			if (ContainsKey<V>(cacheKey))
			{
				return Get<V>(cacheKey);
			}
			else
			{
				var result = create();
				Add(cacheKey, result, cacheDurationInSeconds);
				return result;
			}
		}

		public void Remove<V>(string key)
		{
			string prefix = StringHelper.StringSplit(key,0, ":");
			if (prefix.Length > 0) prefix += ":";
			RedisServer.Data.Del(key.Remove(0, prefix.Length));
		}
	}
}
