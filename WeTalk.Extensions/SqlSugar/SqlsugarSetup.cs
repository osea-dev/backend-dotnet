﻿using log4net;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using WeTalk.Common;
using WeTalk.Common.Helper;

namespace WeTalk.Extensions
{
	/// <summary>
	/// SqlSugar 启动服务
	/// </summary>
	public static class SqlsugarSetup
	{
		public static void AddSqlsugarSetup(this IServiceCollection services, ILog log)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));

			// 默认添加主数据库连接
			MainDb.CurrentDbConnId = Appsettings.app(new string[] { "MainDB" });

			// 把多个连接对象注入服务，这里必须采用Scope，因为有事务操作
			services.AddSingleton<SqlSugarScope>(o =>
			{
				// 连接字符串
				var listConfig = new List<ConnectionConfig>();
				// 从库
				var listConfig_Slave = new List<SlaveConnectionConfig>();
				BaseDBConfig.MutiConnectionString.slaveDbs.ForEach(s =>
				{
					listConfig_Slave.Add(new SlaveConnectionConfig()
					{
						HitRate = s.HitRate,
						ConnectionString = s.Connection
					});
				});

				BaseDBConfig.MutiConnectionString.allDbs.ForEach(m =>
				{
					listConfig.Add(new ConnectionConfig()
					{
						ConfigId = m.ConnId.ObjToString().ToLower(),
						ConnectionString = m.Connection,
						DbType = (DbType)m.DbType,
						IsAutoCloseConnection = true,
						AopEvents = new AopEvents
						{
							OnLogExecuted = (sql, p) =>
							{
								if (Appsettings.app("Logs:SqlLog").ObjToInt() == 1)
								{
									//全局启用SQL日志，控制台一定输出；文件类型则通过配置控制
									//ConsoleHelper.WriteColorLine(string.Join("\r\n", new string[] { "--------", "【SQL语句】：" + SqlFormatHelper.FormatParam(sql, p) }), ConsoleColor.DarkCyan);
									log.Info($"【SQL语句】：" + SqlFormatHelper.FormatParam(sql, p));
								}
								else
								{
									ConsoleHelper.WriteColorLine(string.Join("\r\n", new string[] { "--------", "【SQL语句】：" + SqlFormatHelper.FormatParam(sql, p) }), ConsoleColor.DarkCyan);
								}
							},
							OnError = (exp) =>//SQL报错
							{
								if (Appsettings.app("Logs:SqlLog").ObjToInt() == 1)
								{
									log.Info($"【错误Sql】：" + SqlFormatHelper.FormatParam(exp.Sql, exp.Parametres));
								}
								else
								{
									ConsoleHelper.WriteColorLine(string.Join("\r\n", new string[] { "--------", "【错误Sql】：" + SqlFormatHelper.FormatParam(exp.Sql, exp.Parametres) }), ConsoleColor.DarkCyan);
								}
							},
						},
						MoreSettings = new ConnMoreSettings()
						{
							//IsWithNoLockQuery = true,
							DisableNvarchar = true,
							IsAutoRemoveDataCache = true
						},
						// 从库
						SlaveConnectionConfigs = listConfig_Slave,
						// 自定义特性
						ConfigureExternalServices = new ConfigureExternalServices()
						{
							DataInfoCacheService = new SugarRedisCache(), //配置我们创建的缓存类
							EntityService = (property, column) =>
							{
								if (column.IsPrimarykey && property.PropertyType == typeof(int))
								{
									column.IsIdentity = true;
								}
							}
						},
						InitKeyType = InitKeyType.Attribute
					}
				   );
				});
				var db = new SqlSugarScope(listConfig);
				//db.Aop.OnLogExecuted = (sql, p) =>
				//{
				//	if (Appsettings.app("Logs:SqlLog").ObjToInt() == 1)
				//	{
				//		//全局启用SQL日志，控制台一定输出；文件类型则通过配置控制
				//		//ConsoleHelper.WriteColorLine(string.Join("\r\n", new string[] { "--------", "【SQL语句】：" + SqlFormatHelper.FormatParam(sql, p) }), ConsoleColor.DarkCyan);
				//		log.Info($"【SQL语句】(耗时:{db.Ado.SqlExecutionTime.ToString()})：" + SqlFormatHelper.FormatParam(sql, p));
				//	}
				//	else
				//	{
				//		ConsoleHelper.WriteColorLine(string.Join("\r\n", new string[] { "--------", $"【SQL语句】(耗时:{db.Ado.SqlExecutionTime.TotalMilliseconds})：" + SqlFormatHelper.FormatParam(sql, p) }), ConsoleColor.DarkCyan);
				//	}
				//};
				return db;
			});
		}

	}
}