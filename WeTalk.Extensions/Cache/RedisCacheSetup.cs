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
using Microsoft.Extensions.Hosting;
using WeTalk.Common;

namespace WeTalk.Extensions
{
	public static class RedisServer
	{
		public static CSRedisClient Cache, Data, Token;
		public static void Initalize()
		{
			Cache = new CSRedisClient(Appsettings.app("RedisServer:Cache"));
			Data = new CSRedisClient(Appsettings.app("RedisServer:Data"));
			Token = new CSRedisClient(Appsettings.app("RedisServer:Token"));
		}
	}
}
