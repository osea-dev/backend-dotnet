/*
* ==============================================================================
*
* FileName: RedisServer.cs
* Created: 2020/3/28 16:59:45
* Author: Meiam
* Description: 
*
* ==============================================================================
*/
using CSRedis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using WeTalk.Common;

namespace WeTalk.Extensions
{
	public static class MemoryServer
	{
		public static MemoryCacheHelper Cache;
		public static void Initalize()
		{
			Cache = new MemoryCacheHelper();
		}
	}
}
