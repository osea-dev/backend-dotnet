using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using WeTalk.Models;

namespace WeTalk.Interfaces.Base
{
	public partial class PubConfigBaseService : BaseService, IPubConfigBaseService
	{
		private readonly IUserManage _userManage;
		private readonly SqlSugarScope _context;
		public PubConfigBaseService(SqlSugarScope dbcontext, IUserManage userManage)
		{
			_context = dbcontext;
			_userManage = userManage;
		}
		#region "数据字典操作"
		/// <summary>
		/// 获取字典值
		/// </summary>
		/// <param name="lang">语言,通用字典填空值</param>
		/// <param name="key"></param>
		/// <param name="str"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public string GetConfig( string key, string str = "", string action = "val")
		{
			string lang = _userManage.Lang;
			string val = str;
			var pubConfig = _context.Queryable<ViewPubConfig>()
				.Where(u => u.Status == 1 && u.Key == key.Trim().ToLower() && (u.Islang == 0 || (u.Islang == 1 && u.Lang == lang)))
				.OrderBy(u => u.PubConfigid, OrderByType.Desc)
                .First();
			if (pubConfig != null)
			{
				switch (action.ToLower())
				{
					case "description":
						if (!string.IsNullOrEmpty(pubConfig.Description)) val = pubConfig.Description;
						break;
					case "title":
						if (!string.IsNullOrEmpty(pubConfig.Title)) val = pubConfig.Title;
						break;
					case "val":
						if (!string.IsNullOrEmpty(pubConfig.Val))
						{
							val = pubConfig.Val;
						}
						else if (!string.IsNullOrEmpty(pubConfig.Longval)) val = pubConfig.Longval;
						break;
					case "longval":
						if (!string.IsNullOrEmpty(pubConfig.Longval)) val = pubConfig.Longval;
						break;
					case "all":
						val = "{\"pub_configid\":\"" + pubConfig.PubConfigid + "\",\"name\":\"" + JsonCharFilter(pubConfig.Key) + "\",\"description\":\"" + JsonCharFilter(pubConfig.Description) + "\",\"val\":\"" + JsonCharFilter(pubConfig.Val) + "\",\"dtime\":\"" + JsonCharFilter(pubConfig.Dtime.ToString()) + "\",\"title\":\"" + JsonCharFilter(pubConfig.Title) + "\"}";
						break;
				}
			}
			return val;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefix">个性化前缀，如company:27,userid:5,cardid:6</param>
		/// <param name="key"></param>
		/// <param name="str"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public string GetConfig(string prefix, string key, string str = "", string action = "val")
		{
			string lang = _userManage.Lang;
			string val = str;
			var pubConfig = _context.Queryable<ViewPubConfig>()
				.Where(u => u.Status == 1 && (u.Key == key.Trim().ToLower() || u.Key == SqlFunc.MergeString(prefix, "|", key.ToLower().Trim())) && (u.Islang == 0 || (u.Islang == 1 && u.Lang == lang)))
				.OrderBy(u => u.PubConfigid , OrderByType.Desc)
				.First();

			//优先取带userid的值，否则判断默认val是否有变化，无则再取一次，有则忽略；
			if (pubConfig.Key == (prefix + "|" + key.ToLower().Trim()))
			{
				switch (action.ToLower())
				{
					case "description":
						if (!string.IsNullOrEmpty(pubConfig.Description)) val = pubConfig.Description;
						break;
					case "title":
						if (!string.IsNullOrEmpty(pubConfig.Title)) val = pubConfig.Title;
						break;
					case "val":
						if (!string.IsNullOrEmpty(pubConfig.Val.Trim()))
						{
							val = pubConfig.Val;
						}
						else if (!string.IsNullOrEmpty(pubConfig.Longval)) val = pubConfig.Longval;
						break;
					case "longval":
						if (!string.IsNullOrEmpty(pubConfig.Longval)) val = pubConfig.Longval;
						break;
					case "all":
						val = "{\"pub_configid\":\"" + pubConfig.PubConfigid + "\",\"name\":\"" + JsonCharFilter(pubConfig.Key) + "\",\"description\":\"" + JsonCharFilter(pubConfig.Description) + "\",\"val\":\"" + JsonCharFilter(pubConfig.Val) + "\",\"dtime\":\"" + JsonCharFilter(pubConfig.Dtime.ToString()) + "\",\"title\":\"" + JsonCharFilter(pubConfig.Title) + "\"}";
						break;
				}
			}
			else if (val == str)
			{
				switch (action.ToLower())
				{
					case "description":
						if (!string.IsNullOrEmpty(pubConfig.Description)) val = pubConfig.Description;
						break;
					case "title":
						if (!string.IsNullOrEmpty(pubConfig.Title)) val = pubConfig.Title;
						break;
					case "val":
						if (!string.IsNullOrEmpty(pubConfig.Val.Trim()))
						{
							val = pubConfig.Val;
						}
						else if (!string.IsNullOrEmpty(pubConfig.Longval)) val = pubConfig.Longval;
						break;
					case "longval":
						if (!string.IsNullOrEmpty(pubConfig.Longval)) val = pubConfig.Longval;
						break;
					case "all":
						val = "{\"pub_configid\":\"" + pubConfig.PubConfigid + "\",\"name\":\"" + JsonCharFilter(pubConfig.Key) + "\",\"description\":\"" + JsonCharFilter(pubConfig.Description) + "\",\"val\":\"" + JsonCharFilter(pubConfig.Val) + "\",\"dtime\":\"" + JsonCharFilter(pubConfig.Dtime.ToString()) + "\",\"title\":\"" + JsonCharFilter(pubConfig.Title) + "\"}";
						break;
				}
			}
			return val;
		}

		/// <summary>
		/// 取多个字典项，优先取渠道字典，次取公共字典
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public Dictionary<string, string> GetConfigs(string keys, string action = "val")
		{
			string lang = _userManage.Lang;
			Dictionary<string, string> data_result = new Dictionary<string, string>();
			string[] arr = keys.ToLower().Trim().Split(',').Select(u=>u.Trim()).ToArray();
			var list_pubConfig = _context.Queryable<ViewPubConfig>().Where(u => arr.Contains(u.Key.ToLower().Trim()) && u.Status == 1 && (u.Islang == 0 || (u.Islang == 1 && u.Lang == lang)))
				.OrderBy(u => u.PubConfigid)
				.ToList();
			//公共的先取
			for (int i = 0; i < list_pubConfig.Count; i++)
			{
				if (string.IsNullOrEmpty(list_pubConfig[i].Key)) continue;
				switch (action.ToLower())
				{
					case "description":
						if (!string.IsNullOrEmpty(list_pubConfig[i].Description)) data_result.Add(list_pubConfig[i].Key, list_pubConfig[i].Description);
						break;
					case "title":
						if (!string.IsNullOrEmpty(list_pubConfig[i].Title)) data_result.Add(list_pubConfig[i].Key, list_pubConfig[i].Title);
						break;
					case "val":
						if (!string.IsNullOrEmpty(list_pubConfig[i].Val))
						{
							data_result.Add(list_pubConfig[i].Key, list_pubConfig[i].Val);
						}
						else
						{
							data_result.Add(list_pubConfig[i].Key, list_pubConfig[i].Longval);
						}
						break;
					case "longval":
						data_result.Add(list_pubConfig[i].Key, list_pubConfig[i].Longval);
						break;
					case "all":
						string val = "{\"pub_configid\":\"" + list_pubConfig[i].PubConfigid + "\",\"name\":\"" + JsonCharFilter(list_pubConfig[i].Key + "") + "\",\"description\":\"" + JsonCharFilter(list_pubConfig[i].Description) + "\",\"val\":\"" + JsonCharFilter(list_pubConfig[i].Val + "") + "\",\"dtime\":\"" + JsonCharFilter(list_pubConfig[i].Dtime + "") + "\",\"title\":\"" + JsonCharFilter(list_pubConfig[i].Title + "") + "\"}";
						data_result.Add(list_pubConfig[i].Key, val);
						break;
				}
			}
			return data_result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefix">个性化前缀，如company:27,userid:5,cardid:6</param>
		/// <param name="keys"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public Dictionary<string, string> GetConfigs(string prefix, string keys, string action = "val")
		{
			string lang = _userManage.Lang;
			Dictionary<string, string> data_result = new Dictionary<string, string>();
			string[] arr = keys.ToLower().Trim().Split(',');
			var list_userconfig = arr.Select(u => prefix + "|" + u).ToList();
			var list_pubConfig = _context.Queryable<ViewPubConfig>()
				.Where(u => (arr.Contains(u.Key.ToLower().Trim()) || list_userconfig.Contains(u.Key.ToLower().Trim())) && u.Status == 1 && (u.Islang == 0 || (u.Islang == 1 && u.Lang == lang)))
				.OrderBy(u => u.PubConfigid, OrderByType.Desc)
				.ToList();

			for (int i = 0; i < list_pubConfig.Count; i++)
			{
				if (string.IsNullOrEmpty(list_pubConfig[i].Key)) continue;
				if (list_pubConfig[i].Key.ToLower().Trim().StartsWith(prefix + "|"))
				{
					if (data_result.Keys.Contains(list_pubConfig[i].Key.Replace(prefix + "|", ""))) continue;//重复了，只取第一个
																											  // userid开头的
					switch (action.ToLower())
					{
						case "description":
							if (!string.IsNullOrEmpty(list_pubConfig[i].Description)) data_result.Add(list_pubConfig[i].Key.Replace(prefix + "|", ""), list_pubConfig[i].Description);
							break;
						case "title":
							if (!string.IsNullOrEmpty(list_pubConfig[i].Title)) data_result.Add(list_pubConfig[i].Key.Replace(prefix + "|", ""), list_pubConfig[i].Title);
							break;
						case "val":
							if (!string.IsNullOrEmpty(list_pubConfig[i].Val))
							{
								data_result.Add(list_pubConfig[i].Key.Replace(prefix + "|", ""), list_pubConfig[i].Val);
							}
							else
							{
								data_result.Add(list_pubConfig[i].Key.Replace(prefix + "|", ""), list_pubConfig[i].Longval);
							}
							break;
						case "all":
							string val = "{\"pub_configid\":\"" + list_pubConfig[i].PubConfigid + "\",\"name\":\"" + JsonCharFilter(list_pubConfig[i].Key.Replace(prefix + "|", "") + "") + "\",\"description\":\"" + JsonCharFilter(list_pubConfig[i].Description) + "\",\"val\":\"" + JsonCharFilter(list_pubConfig[i].Val + "") + "\",\"dtime\":\"" + JsonCharFilter(list_pubConfig[i].Dtime + "") + "\",\"title\":\"" + JsonCharFilter(list_pubConfig[i].Title + "") + "\"}";
							data_result.Add(list_pubConfig[i].Key.Replace(prefix + "|", ""), val);
							break;
					}
				}
				else if (!data_result.ContainsKey(list_pubConfig[i].Key))
				{
					switch (action.ToLower())
					{
						case "description":
							if (!string.IsNullOrEmpty(list_pubConfig[i].Description)) data_result.Add(list_pubConfig[i].Key, list_pubConfig[i].Description);
							break;
						case "title":
							if (!string.IsNullOrEmpty(list_pubConfig[i].Title)) data_result.Add(list_pubConfig[i].Key, list_pubConfig[i].Title);
							break;
						case "val":
							if (!string.IsNullOrEmpty(list_pubConfig[i].Val))
							{
								data_result.Add(list_pubConfig[i].Key, list_pubConfig[i].Val);
							}
							else
							{
								data_result.Add(list_pubConfig[i].Key, list_pubConfig[i].Longval);
							}
							break;
						case "all":
							string val = "{\"pub_configid\":\"" + list_pubConfig[i].PubConfigid + "\",\"name\":\"" + JsonCharFilter(list_pubConfig[i].Key + "") + "\",\"description\":\"" + JsonCharFilter(list_pubConfig[i].Description) + "\",\"val\":\"" + JsonCharFilter(list_pubConfig[i].Val + "") + "\",\"dtime\":\"" + JsonCharFilter(list_pubConfig[i].Dtime + "") + "\",\"title\":\"" + JsonCharFilter(list_pubConfig[i].Title + "") + "\"}";
							data_result.Add(list_pubConfig[i].Key, val);
							break;
					}
				}
			}
			return data_result;
		}

		public int GetConfigInt(string key, int n = 0)
		{
			string lang = _userManage.Lang;
			int val = n;
			var pubConfig = _context.Queryable<ViewPubConfig>()
				.Where(u => u.Key == key.Trim().ToLower() && (u.Islang == 0 || (u.Islang == 1 && u.Lang == lang)))
				.OrderBy(u => u.PubConfigid , OrderByType.Desc)
				.First();
			if (pubConfig != null)
			{
				if (!string.IsNullOrEmpty(pubConfig.Val)) val = int.Parse(pubConfig.Val);
			}
			return val;
		}
		public decimal GetConfigDecimal(string key, decimal n = 0M)
		{
			string lang = _userManage.Lang;
			decimal val = n;
			var pubConfig = _context.Queryable<ViewPubConfig>()
				.Where(u => u.Key == key.Trim().ToLower() && (u.Islang == 0 || (u.Islang == 1 && u.Lang == lang)))
				.OrderBy(u => u.PubConfigid, OrderByType.Desc)
				.First();
			if (pubConfig != null)
			{
				if (!string.IsNullOrEmpty(pubConfig.Val)) val = decimal.Parse(pubConfig.Val);
			}
			return val;
		}

		public bool UpdateConfig(string key, string val, string action = "val")
		{
			string lang = _userManage.Lang;
			bool isok = false;
			int n = 0;
			var model_ConfigLang = new PubConfigLang();
			var model_Config = _context.Queryable<PubConfig>().First(u => u.Key.ToLower() == key.ToLower());
			if (model_Config != null)
			{
				switch (action.ToLower())
				{
					case "description":
						model_Config.Description = val;
						n = _context.Updateable(model_Config).ExecuteCommand();
						break;
					case "title":
						model_Config.Title = val;
						n = _context.Updateable(model_Config).ExecuteCommand();
						break;
					case "longval":
						if (model_Config.Islang == 1)
						{
							model_ConfigLang = _context.Queryable<PubConfigLang>().First(u => u.PubConfigid == model_Config.PubConfigid && u.Lang == lang);
							if (model_ConfigLang != null)
							{
								model_ConfigLang.Longval = val;
								_context.Updateable(model_ConfigLang).ExecuteCommand();
							}
							else
							{
								model_ConfigLang = new PubConfigLang();
								model_ConfigLang.PubConfigid = model_Config.PubConfigid;
								model_ConfigLang.Longval = val;
								model_ConfigLang.Lang = lang;
								_context.Insertable(model_ConfigLang).ExecuteCommand();
							}
						}
						else
						{
							model_Config.Longval = val;
							n = _context.Updateable(model_Config).ExecuteCommand();
						}
						break;
					case "val":
						if (model_Config.Islang == 1)
						{
							model_ConfigLang = _context.Queryable<PubConfigLang>().First(u => u.PubConfigid == model_Config.PubConfigid && u.Lang == lang);
							if (model_ConfigLang != null)
							{
								model_ConfigLang.Val = val;
								_context.Updateable(model_ConfigLang).ExecuteCommand();
							}
							else
							{
								model_ConfigLang = new PubConfigLang();
								model_ConfigLang.PubConfigid = model_Config.PubConfigid;
								model_ConfigLang.Val = val;
								model_ConfigLang.Lang = lang;
								_context.Insertable(model_ConfigLang).ExecuteCommand();
							}
						}
						else
						{
							model_Config.Val = val;
							n = _context.Updateable(model_Config).ExecuteCommand();
						}
						break;
					default:
						if (model_Config.Islang == 1)
						{
							model_ConfigLang = _context.Queryable<PubConfigLang>().First(u => u.PubConfigid == model_Config.PubConfigid && u.Lang == lang);
							if (model_ConfigLang != null)
							{
								model_ConfigLang.Val = val;
								_context.Updateable(model_ConfigLang).ExecuteCommand();
							}
							else
							{
								model_ConfigLang = new PubConfigLang();
								model_ConfigLang.PubConfigid = model_Config.PubConfigid;
								model_ConfigLang.Val = val;
								model_ConfigLang.Lang = lang;
								_context.Insertable(model_ConfigLang).ExecuteCommand();
							}
						}
						else
						{
							model_Config.Val = val;
							n = _context.Updateable(model_Config).ExecuteCommand();
						}
						break;
				}
			}
			else {
				//不存在都默认添加通用版
				model_Config.Key = key;
				model_Config.Title = key;
				switch (action.ToLower())
				{
					case "description":
						model_Config.Description = val;
						break;
					case "title":
						model_Config.Title = val;
						break;
					case "longval":
						model_Config.Longval = val;
						break;
					case "val":
						model_Config.Val = val;
						break;
					default:
						model_Config.Val = val;
						break;
				}
				model_Config.Status = 1;
				model_Config.Islang = 0;
				var pub_configid = _context.Insertable(model_Config).ExecuteReturnBigIdentity();
				model_ConfigLang = _context.Queryable<PubConfigLang>().First(u => u.PubConfigid == pub_configid && u.Lang == lang);
				if (model_ConfigLang != null)
				{
					switch (action.ToLower())
					{
						case "longval":
							model_ConfigLang.Longval = val;
							break;
						case "val":
							model_ConfigLang.Val = val;
							break;
						default:
							model_ConfigLang.Val = val;
							break;
					}
					_context.Updateable(model_ConfigLang).ExecuteCommand();
				}
				else
				{
					model_ConfigLang = new PubConfigLang();
					model_ConfigLang.PubConfigid = pub_configid;
					switch (action.ToLower())
					{
						case "longval":
							model_ConfigLang.Longval = val;
							break;
						case "val":
							model_ConfigLang.Val = val;
							break;
						default:
							model_ConfigLang.Val = val;
							break;
					}
					model_ConfigLang.Lang = lang;
					_context.Insertable(model_ConfigLang).ExecuteCommand();
				}
			}
			if (n > 0) isok = true;
			_context.DataCache.RemoveDataCache("view_pub_config");
			return isok;
		}

		/// <summary>
		/// 批量更新字典，有更新无则新增
		/// </summary>
		/// <param name="lang">空则为非多语言字段</param>
		/// <param name="dic"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public bool UpdateConfigs(Dictionary<string, string> dic, string action = "val")
		{
			string lang = _userManage.Lang;
			bool isok = false;
			var list_Config = _context.Queryable<PubConfig>().Where(u => dic.Keys.ToList().Contains(u.Key)).ToList();
			var list_ConfigLang = _context.Queryable<PubConfigLang>().Where(u => list_Config.Select(s => s.PubConfigid).Contains(u.PubConfigid)).ToList();
			PubConfigLang model_ConfigLang = null;
			//更新
			list_Config.ForEach(pubConfig =>
			{
				string val = dic[pubConfig.Key];
				switch (action.ToLower())
				{
					case "description":
						pubConfig.Description = val;
						break;
					case "title":
						pubConfig.Title = val;
						break;
					case "val":
						if (pubConfig.Islang == 1)
						{
							model_ConfigLang = _context.Queryable<PubConfigLang>().First(u => u.PubConfigid == pubConfig.PubConfigid && u.Lang == lang);
							if (model_ConfigLang != null)
							{
								model_ConfigLang.Val = val;
								_context.Updateable(model_ConfigLang).ExecuteCommand();
							}
							else
							{
								model_ConfigLang = new PubConfigLang();
								model_ConfigLang.PubConfigid = pubConfig.PubConfigid;
								model_ConfigLang.Val = val;
								model_ConfigLang.Lang = lang;
								_context.Insertable(model_ConfigLang).ExecuteCommand();
							}
						}
						else
						{
							pubConfig.Val = val;
						}
						break;
					case "longval":
						if (pubConfig.Islang == 1)
						{
							model_ConfigLang = _context.Queryable<PubConfigLang>().First(u => u.PubConfigid == pubConfig.PubConfigid && u.Lang == lang);
							if (model_ConfigLang != null)
							{
								model_ConfigLang.Longval = val;
								_context.Updateable(model_ConfigLang).ExecuteCommand();
							}
							else
							{
								model_ConfigLang = new PubConfigLang();
								model_ConfigLang.PubConfigid = pubConfig.PubConfigid;
								model_ConfigLang.Longval = val;
								model_ConfigLang.Lang = lang;
								_context.Insertable(model_ConfigLang).ExecuteCommand();
							}
						}
						else
						{
							pubConfig.Longval = val;
						}
						break;
				}
			});
			_context.Updateable(list_Config).ExecuteCommand();

			//新增
			dic = dic.Where(u => !list_Config.Select(s => s.Key).Contains(u.Key)).ToDictionary(u => u.Key, u => u.Value);
			foreach (var item in dic) {
				var model_Config = new PubConfig();
				model_Config.Key = item.Key;
				model_Config.Title = item.Key;
				model_Config.Islang = string.IsNullOrEmpty(lang) ? 0 : 1;
				string val = item.Value;
				
				switch (action.ToLower())
				{
					case "description":
						model_Config.Description = val;
						model_Config.Status = 1;
						list_Config.Add(model_Config);
						break;
					case "title":
						model_Config.Title = val;
						model_Config.Status = 1;
						list_Config.Add(model_Config);
						break;
					case "val":
						model_Config.Val = val;
						model_Config.Status = 1;
						if (model_Config.Islang == 1)
						{
							var pub_configid = _context.Insertable(model_Config).ExecuteReturnBigIdentity();
							model_ConfigLang = _context.Queryable<PubConfigLang>().First(u => u.PubConfigid == pub_configid && u.Lang == lang);
							if (model_ConfigLang != null)
							{
								model_ConfigLang.Val = val;
								_context.Updateable(model_ConfigLang).ExecuteCommand();
							}
							else
							{
								model_ConfigLang = new PubConfigLang();
								model_ConfigLang.PubConfigid = pub_configid;
								model_ConfigLang.Val = val;
								model_ConfigLang.Lang = lang;
								_context.Insertable(model_ConfigLang).ExecuteCommand();
							}
						}
						else
						{
							list_Config.Add(model_Config);
						}
						break;
					case "longval":
						model_Config.Longval = val;
						model_Config.Status = 1;
						if (model_Config.Islang == 1)
						{
							var pub_configid = _context.Insertable(model_Config).ExecuteReturnBigIdentity();
							model_ConfigLang = _context.Queryable<PubConfigLang>().First(u => u.PubConfigid == pub_configid && u.Lang == lang);
							if (model_ConfigLang != null)
							{
								model_ConfigLang.Longval = val;
								_context.Updateable(model_ConfigLang).ExecuteCommand();
							}
							else
							{
								model_ConfigLang = new PubConfigLang();
								model_ConfigLang.PubConfigid = pub_configid;
								model_ConfigLang.Longval = val;
								model_ConfigLang.Lang = lang;
								_context.Insertable(model_ConfigLang).ExecuteCommand();
							}
						}
						else
						{
							list_Config.Add(model_Config);
						}
						break;
				}
			}
			int n = 0;
			var x = _context.Storageable(list_Config).ToStorage();
			n += x.AsInsertable.ExecuteCommand();
			n += x.AsUpdateable.ExecuteCommand();
			if (n > 0) isok = true;
			_context.DataCache.RemoveDataCache("view_pub_config");
			return isok;
		}
		#endregion
	}
}
