using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.ViewModel;
using WeTalk.Web.Extensions;
using WeTalk.Web.ViewModels.General;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class GeneralController : Base.BaseController
	{

		private readonly TokenManager _tokenManager;
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包
		private readonly IMenkeService _menkeService;
		public GeneralController(TokenManager tokenManager, IStringLocalizer<LangResource> localizer, SqlSugarScope context, IHttpContextAccessor accessor, IMenkeService menkeService
			)
			: base(tokenManager)
		{
			_tokenManager = tokenManager;
			_context = context;
			_accessor = accessor;
			_localizer = localizer;
			_menkeService = menkeService;
		}

		#region "货币管理" 
		#region "货币列表" 
		[Authorization(Power = "Main")]
		public IActionResult CurrencyList()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> CurrencyListData()
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			long PubCurrencyid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "PubCurrencyid", 0);
			string PubCurrencyids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "PubCurrencyids");
			int count = 0;

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					if (PubCurrencyids.Trim() == "") PubCurrencyids = "0";
					var list_countryid = PubCurrencyids.Split(',').Select(u=> long.Parse(u)).ToList();
					if (PubCurrencyid > 0) list_countryid.Add(PubCurrencyid);
					int n = _context.Updateable<PubCurrency>().SetColumns(u => u.Status == -1).Where(u => u.Status == 1 && list_countryid.Contains(u.PubCurrencyid)).ExecuteCommand();
					if (n>0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\""+_localizer["删除成功"]+"！\"}");
					}
					else {
						str.Append("{\"errorcode\":\"-1\",\"msg\":\""+ _localizer["删除失败,不存在此记录"] +"！\"}");
					}
					break;
				case "enableu"://启用
					count = _context.Updateable<PubCurrency>().SetColumns(u=>u.Status == 1).UpdateColumns(u => u.PubCurrencyid == PubCurrencyid).ExecuteCommand();
					if (count>0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\""+ _localizer["解冻成功"]+"！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\""+ _localizer["解冻失败,不存在此用户"] +"！\"}");
					}
					break;
				case "enablef"://禁用   
					count = _context.Updateable<PubCurrency>().SetColumns(u => u.Status == 0).UpdateColumns(u => u.PubCurrencyid == PubCurrencyid).ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["冻结成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["冻结失败,不存在此用户"] + "！\"}");
					}
					break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					string son_str = "";
					var exp = new Expressionable<PubCurrency>();
					exp.And(u => u.Status == 1);
					
					if (sort != "")
					{
						sort = StringHelper.ChgToUnderLine(sort);
						if (order == "desc")
						{
							sort += " desc,";
						}
						else {
							sort += ",";
						}
					}
					sort += "sort,pub_countryid desc";

					var total = 0;
					var list_country = _context.Queryable<PubCurrency>().Where(exp.ToExpression()).ToPageList(page,pagesize,ref total);
					var list_CountryLang = _context.Queryable<PubCurrencyLang>().Where(u => list_country.Select(s => s.PubCurrencyid).Contains(u.PubCurrencyid) && u.Lang == Lang).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_country.Count; i++)
					{
						var model_CountryLang = list_CountryLang.FirstOrDefault(u => u.PubCurrencyid == list_country[i].PubCurrencyid);
						str.Append("{");
						str.Append("\"PubCurrencyid\":\"" + list_country[i].PubCurrencyid + "\",");
						str.Append("\"States\":\"" + JsonHelper.JsonCharFilter(model_CountryLang?.States + "") + "\",");
						str.Append("\"Country\":\"" + JsonHelper.JsonCharFilter(model_CountryLang?.Country + "") + "\",");
						str.Append("\"Isuse\":\"" + list_country[i].Isuse + "\",");
						str.Append("\"Flag\":\"" + list_country[i].Flag + "\",");
						str.Append("\"CountryCode\":\"" + list_country[i].CountryCode + "\",");
						str.Append("\"Currency\":\"" + JsonHelper.JsonCharFilter(model_CountryLang?.Currency + "(" + list_country[i].CurrencyCode + ")") + "\",");
						str.Append("\"Dtime\":\"" + list_country[i].Dtime + "\",");
						str.Append("\"Sort\":\"" + list_country[i].Sort + "\",");
						str.Append("\"Status\":\"" + list_country[i].Status + "\",");
						str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						str.Append(son_str);
						if (i < (list_country.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}

		#endregion

		#region "添加修改货币"  
		[Authorization(Power = "Main")]
		public async Task<IActionResult> CurrencyAdd(long PubCurrencyid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			CurrencyAdd vm_CounctryAdd = new CurrencyAdd();
			var model_Country = _context.Queryable<PubCurrency>().InSingle(PubCurrencyid);
			if (model_Country != null)
			{
				vm_CounctryAdd.PubCurrencyid = PubCurrencyid;
				vm_CounctryAdd.CurrencyCode = model_Country.CurrencyCode;
				vm_CounctryAdd.Sort = model_Country.Sort;
				vm_CounctryAdd.Dtime = model_Country.Dtime;
				vm_CounctryAdd.Flag = model_Country.Flag;
				vm_CounctryAdd.Status = model_Country.Status;
				vm_CounctryAdd.CountryCode = model_Country.CountryCode;

				var model_Lang = _context.Queryable<PubCurrencyLang>().First(u => u.Lang == Lang && u.PubCurrencyid == model_Country.PubCurrencyid);
				if (model_Lang != null)
				{
					vm_CounctryAdd.Country = model_Lang.Country;
					vm_CounctryAdd.Currency = model_Lang.Currency;
				}
			}
			else
			{
				vm_CounctryAdd.PubCurrencyid = 0;
				vm_CounctryAdd.Sort = 10;
				vm_CounctryAdd.Status = 1;
			}
			vm_CounctryAdd.Lang = Lang;
			return View(vm_CounctryAdd);
		}

		//保存添改的菜单
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> CurrencyAdd(PubCurrencyVM vm_CurrencyAdd)
		{
			StringBuilder str = new StringBuilder();
			PubCurrencyLang model_Lang = null;
			var model_Country = _context.Queryable<PubCurrency>().InSingle(vm_CurrencyAdd.PubCurrencyid);
			if (model_Country != null)
			{
				model_Country.CurrencyCode = vm_CurrencyAdd.CurrencyCode;
				model_Country.Sort = vm_CurrencyAdd.Sort;
				model_Country.Flag = vm_CurrencyAdd.Flag;
				model_Country.Status = vm_CurrencyAdd.Status;
				model_Country.CountryCode = vm_CurrencyAdd.CountryCode;

				model_Lang = _context.Queryable<PubCurrencyLang>().First(u => u.Lang == vm_CurrencyAdd.Lang && u.PubCurrencyid == model_Country.PubCurrencyid);
				if (model_Lang != null)
				{
					model_Lang.Country = vm_CurrencyAdd.Country;
					model_Lang.Currency = vm_CurrencyAdd.Currency;
					_context.Updateable(model_Lang).UpdateColumns(u => new { u.Country, u.Currency }).ExecuteCommand();
				}
				else
				{
					model_Lang = new PubCurrencyLang();
					model_Lang.PubCurrencyid = model_Country.PubCurrencyid;
					model_Lang.Lang = vm_CurrencyAdd.Lang;
					model_Lang.Country = vm_CurrencyAdd.Country;
					model_Lang.Currency = vm_CurrencyAdd.Currency;
					_context.Insertable(model_Lang).ExecuteCommand();
				}
				_context.Updateable(model_Country).UpdateColumns(u => new { u.CurrencyCode, u.Sort, u.Flag, u.Status, u.CountryCode }).ExecuteCommand(); 
				return Content("<script>parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改货币信息"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); location.href='?PubCurrencyid=" + vm_CurrencyAdd.PubCurrencyid + "&lang=" + vm_CurrencyAdd.Lang + "';</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				#region "添加货币"
				//添加货币主表
				model_Country = new PubCurrency();
				model_Country.CurrencyCode = vm_CurrencyAdd.CurrencyCode;
				model_Country.Sort = vm_CurrencyAdd.Sort;
				model_Country.Flag = vm_CurrencyAdd.Flag;
				model_Country.Status = vm_CurrencyAdd.Status;
				model_Country.CountryCode = vm_CurrencyAdd.CountryCode;
				model_Country.Dtime = DateTime.Now;
				vm_CurrencyAdd.PubCurrencyid = _context.Insertable(model_Country).ExecuteReturnBigIdentity();

				model_Lang = new PubCurrencyLang();
				model_Lang.PubCurrencyid = vm_CurrencyAdd.PubCurrencyid;
				model_Lang.Lang = vm_CurrencyAdd.Lang;
				model_Lang.Country = vm_CurrencyAdd.Country;
				model_Lang.Currency = vm_CurrencyAdd.Currency;
				_context.Insertable(model_Lang).ExecuteCommand();
				# endregion
				return Content("<script>parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["添加货币信息"] + "',msg:'"+ _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'});location.href='?PubCurrencyid="+ vm_CurrencyAdd.PubCurrencyid + "&lang=" + vm_CurrencyAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}


		}
		#endregion
		#endregion

		#region "语言管理" 
		#region "语言列表" 
		[Authorization(Power = "Main")]
		public IActionResult LangList()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> LangListData()
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			long Languageid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Languageid", 0);
			string Languageids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Languageids");
			int count = 0;

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					if (Languageids.Trim() == "") Languageids = "0";
					var list_countryid = Languageids.Split(',').Select(u => long.Parse(u)).ToList();
					if (Languageid > 0) list_countryid.Add(Languageid);
					if (_context.Queryable<PubLanguage>().Any(u => u.Status == 1 && list_countryid.Contains(u.Languageid) && u.Isadmin == 1)) {
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除失败,锁定语言不允许删除"] + "！\"}");
						return Content(str.ToString());
					}
					int n = _context.Updateable<PubLanguage>().SetColumns(u => u.Status == -1).Where(u => u.Status == 1 && list_countryid.Contains(u.Languageid)).ExecuteCommand();
					
					if (n > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除失败,不存在此记录"] + "！\"}");
					}
					break;
				case "enableu"://启用
					count = _context.Updateable<PubLanguage>().SetColumns(u => u.Status == 1).UpdateColumns(u => u.Languageid == Languageid).ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["解冻成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["解冻失败,不存在此用户"] + "！\"}");
					}
					break;
				case "enablef"://禁用   
					count = _context.Updateable<PubLanguage>().SetColumns(u => u.Status == 0).UpdateColumns(u => u.Languageid == Languageid).ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["冻结成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["冻结失败,不存在此用户"] + "！\"}");
					}
					break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					string son_str = "";
					var exp = new Expressionable<PubLanguage>();
					exp.And(u => u.Status != -1);

					if (sort != "")
					{
						sort = StringHelper.ChgToUnderLine(sort);
						if (order == "desc")
						{
							sort += " desc,";
						}
						else
						{
							sort += ",";
						}
					}
					sort += "isadmin desc,pub_languageid desc";

					var total = 0;
					var list_Lang = _context.Queryable<PubLanguage>().Where(exp.ToExpression()).ToPageList(page, pagesize, ref total);
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Lang.Count; i++)
					{
						str.Append("{");
						str.Append("\"Languageid\":\"" + list_Lang[i].Languageid + "\",");
						str.Append("\"Ttile\":\"" + JsonHelper.JsonCharFilter(list_Lang[i].Title + "") + "\",");
						str.Append("\"Lang\":\"" + list_Lang[i].Lang + "\",");
						str.Append("\"Dtime\":\"" + list_Lang[i].Dtime + "\",");
						str.Append("\"Isadmin\":\"" + list_Lang[i].Isadmin + "\",");
						str.Append("\"Status\":\"" + list_Lang[i].Status + "\",");
						str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						str.Append(son_str);
						if (i < (list_Lang.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}
		#endregion

		#region "添加修改语言"  
		[Authorization(Power = "Main")]
		public async Task<IActionResult> LangAdd(long Languageid = 0, long Fid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			var vm_LangAdd = new LangAdd();
			var model_Lang = _context.Queryable<PubLanguage>().InSingle(Languageid);
			if (model_Lang != null)
			{
				vm_LangAdd.Languageid = Languageid;
				vm_LangAdd.Title = model_Lang.Title;
				vm_LangAdd.Lang = model_Lang.Lang;
				vm_LangAdd.Dtime = model_Lang.Dtime;
				vm_LangAdd.Status = model_Lang.Status;
			}
			else
			{
				vm_LangAdd.Languageid = 0;
				vm_LangAdd.Status = 1;
			}
			return View(vm_LangAdd);
		}

		//保存添改的菜单
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> LangAdd(PubLanguage vm_LangAdd)
		{
			StringBuilder str = new StringBuilder();
			var model_Lang = _context.Queryable<PubLanguage>().InSingle(vm_LangAdd.Languageid);
			if (model_Lang != null)
			{
				model_Lang.Title = vm_LangAdd.Title;
				model_Lang.Lang = vm_LangAdd.Lang;
				model_Lang.Status = vm_LangAdd.Status;

				_context.Updateable(model_Lang).UpdateColumns(u => new { u.Title, u.Lang, u.Status}).ExecuteCommand();
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改语言信息"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				#region "添加语言"
				model_Lang = new PubLanguage();
				model_Lang.Title = vm_LangAdd.Title;
				model_Lang.Lang = vm_LangAdd.Lang;
				model_Lang.Status = vm_LangAdd.Status;
				model_Lang.Dtime = DateTime.Now;
				_context.Insertable(model_Lang).ExecuteCommand();
				#endregion
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["添加语言信息"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
        #endregion

        #endregion

        #region "标签" 
        #region "标签列表"   
        [Authorization(Power = "Main")]
        public async Task<IActionResult> Keys()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> KeysData(string keys = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();

			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string Keysids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Keysids");
			int sty = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "sty", 0);
			PubKeys model_PubKeys = new PubKeys();
			List<PubKeys> list_PubKeyss;
			if (Keysids.Trim() == "") Keysids = "0";
			string[] arr = Keysids.Split(',');
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					list_PubKeyss = await _context.Queryable<PubKeys>().Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Keysid)).ToListAsync();
					if (list_PubKeyss.Count > 0)
					{
						for (int i = 0; i < list_PubKeyss.Count; i++)
						{
							list_PubKeyss[i].Status = -1;
						}
						_context.Updateable(list_PubKeyss).UpdateColumns(u => u.Status).ExecuteCommand();
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除失败,不存在此记录"] + "！\"}");
					}
					break;
				case "enableu"://启用
					list_PubKeyss = await _context.Queryable<PubKeys>().Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Keysid)).ToListAsync();
					if (list_PubKeyss.Count > 0)
					{
						for (int i = 0; i < list_PubKeyss.Count; i++)
						{
							list_PubKeyss[i].Status = 1;
						}
						_context.Updateable(list_PubKeyss).UpdateColumns(u => u.Status).ExecuteCommand();
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["启用成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["启用失败,不存在此记录"] + "！\"}");
					}
					break;
				case "enablef"://禁用
					list_PubKeyss = await _context.Queryable<PubKeys>().Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Keysid)).ToListAsync();
					if (list_PubKeyss.Count > 0)
					{
						for (int i = 0; i < list_PubKeyss.Count; i++)
						{
							list_PubKeyss[i].Status = 0;
						}
						_context.Updateable(list_PubKeyss).UpdateColumns(u => u.Status).ExecuteCommand();
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["禁用成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["禁用失败,不存在此记录"] + "！\"}");
					}
					break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					int total = 0;
					var exp = new Expressionable<PubKeys>();
					exp.And(u => u.Status != -1);
                    exp.AndIF(sty > 0, u=>u.Sty == sty);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<PubKeysLang>().Where(s => s.Keysid == u.Keysid && (s.Title + "").Contains(keys.Trim())).Any());
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<PubKeys>().Where(exp.ToExpression());
					if (sort != "")
					{
						sort = StringHelper.ChgToUnderLine(sort);
						if (order == "desc")
						{
							query.OrderBy(sort + " desc");
						}
						else
						{
							query.OrderBy(sort);
						}
					}
					query.OrderBy("sort,keysid desc");
					list_PubKeyss = query.ToPageList(page, pagesize, ref total);
					var list_KeysLang = _context.Queryable<PubKeysLang>().Where(u => list_PubKeyss.Select(s => s.Keysid).Contains(u.Keysid)).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_PubKeyss.Count; i++)
					{
						var model_Lang = list_KeysLang.FirstOrDefault(u => u.Keysid == list_PubKeyss[i].Keysid && u.Lang == Lang);
						str.Append("{");
						str.Append("\"Keysid\":\"" + list_PubKeyss[i].Keysid + "\",");
						str.Append("\"Sty\":" + list_PubKeyss[i].Sty + ",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_Lang?.Title + "") + "\",");
						str.Append("\"IcoFont\":\"" + list_PubKeyss[i].IcoFont + "\",");
						str.Append("\"Sort\":\"" + list_PubKeyss[i].Sort + "\",");
						str.Append("\"Dtime\":\"" + list_PubKeyss[i].Dtime + "\",");
						str.Append("\"Status\":" + list_PubKeyss[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_PubKeyss.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}
        #endregion

        #region "标签添加修改"      
        [Authorization(Power = "Main")]
        public async Task<IActionResult> KeysAdd(long Keysid = 0)
		{
			var vm_KeysAdd = new KeysAdd();

			var model_PubKeys = _context.Queryable<PubKeys>().First(u => u.Keysid == Keysid);
			if (model_PubKeys != null)
			{
				vm_KeysAdd.Keysid = Keysid;
				vm_KeysAdd.IcoFont = model_PubKeys.IcoFont;
				vm_KeysAdd.Sort = model_PubKeys.Sort;
				vm_KeysAdd.Status = model_PubKeys.Status;
				vm_KeysAdd.Sty = model_PubKeys.Sty;
				var model_KeysLang = _context.Queryable<PubKeysLang>().First(u => u.Lang == Lang && u.Keysid == vm_KeysAdd.Keysid);
				vm_KeysAdd.Title = model_KeysLang?.Title;
			}
			else
			{
				vm_KeysAdd.Sort = 100;
				vm_KeysAdd.Status = 1;
			}
			vm_KeysAdd.Lang = Lang;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_KeysAdd);
		}

		//保存添改的数据字典
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> KeysAdd(KeysAdd vm_KeysAdd)
		{
			var model_PubKeys = await _context.Queryable<PubKeys>().InSingleAsync(vm_KeysAdd.Keysid);

			if (model_PubKeys != null)
			{
				model_PubKeys.IcoFont = vm_KeysAdd.IcoFont;
				model_PubKeys.Sty = vm_KeysAdd.Sty;
				model_PubKeys.Status = vm_KeysAdd.Status;
				model_PubKeys.Sort = vm_KeysAdd.Sort;

				var model_KeysLang = _context.Queryable<PubKeysLang>().First(u => u.Lang == vm_KeysAdd.Lang && u.Keysid == vm_KeysAdd.Keysid);
				if (model_KeysLang != null)
				{
					model_KeysLang.Title = vm_KeysAdd.Title;
					_context.Updateable(model_KeysLang).UpdateColumns(u => u.Title).ExecuteCommand();
				}
				else
				{
					model_KeysLang = new PubKeysLang();
					model_KeysLang.Keysid = vm_KeysAdd.Keysid;
					model_KeysLang.Lang = vm_KeysAdd.Lang;
					model_KeysLang.Title = vm_KeysAdd.Title;
					_context.Insertable(model_KeysLang).ExecuteCommand();
				}
				_context.Updateable(model_PubKeys).ExecuteCommand();

				return Content("<script>parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改标签信息"] + "',msg:'" + _localizer["保存标签成功"] + "',timeout:3000,showType:'slide'}); location.href='?Sty=" + vm_KeysAdd.Sty + "&Keysid=" + vm_KeysAdd.Keysid + "&lang=" + vm_KeysAdd.Lang + "'</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_PubKeys = new PubKeys();
				model_PubKeys.IcoFont = vm_KeysAdd.IcoFont;
				model_PubKeys.Sty = vm_KeysAdd.Sty;
				model_PubKeys.Status = vm_KeysAdd.Status;
				model_PubKeys.Sort = vm_KeysAdd.Sort;
				model_PubKeys.Dtime = DateTime.Now;
				vm_KeysAdd.Keysid = _context.Insertable(model_PubKeys).ExecuteReturnBigIdentity();

				var model_KeysLang = new PubKeysLang();
				model_KeysLang.Keysid = vm_KeysAdd.Keysid;
				model_KeysLang.Lang = vm_KeysAdd.Lang;
				model_KeysLang.Title = vm_KeysAdd.Title;
				_context.Insertable(model_KeysLang).ExecuteReturnBigIdentity();
			}
			return Content("<script>parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["添加标签信息"] + "',msg:'" + _localizer["保存标签成功"] + "',timeout:3000,showType:'slide'}); location.href='?Sty=" + vm_KeysAdd.Sty + "&Keysid=" + vm_KeysAdd.Keysid + "&lang=" + vm_KeysAdd.Lang + "'</script>", "text/html", Encoding.GetEncoding("utf-8"));
		}
        #endregion
        #endregion

        #region "上课类型" 
        #region "上课类型列表"   
        [Authorization(Power = "Main")]
        public async Task<IActionResult> SkuType()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> SkuTypeData(string keys = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();

			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string SkuTypeids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "SkuTypeids");
			var model_SkuType = new WebSkuType();
			List<WebSkuType> list_SkuType;
			if (SkuTypeids.Trim() == "") SkuTypeids = "0";
			string[] arr = SkuTypeids.Split(',');
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					list_SkuType = await _context.Queryable<WebSkuType>().Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.SkuTypeid)).ToListAsync();
					if (list_SkuType.Count > 0)
					{
						for (int i = 0; i < list_SkuType.Count; i++)
						{
							list_SkuType[i].Status = -1;
						}
						_context.Updateable(list_SkuType).UpdateColumns(u => u.Status).ExecuteCommand();
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除失败,不存在此记录"] + "！\"}");
					}
					break;
				case "enableu"://启用
					list_SkuType = await _context.Queryable<WebSkuType>().Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.SkuTypeid)).ToListAsync();
					if (list_SkuType.Count > 0)
					{
						for (int i = 0; i < list_SkuType.Count; i++)
						{
							list_SkuType[i].Status = 1;
						}
						_context.Updateable(list_SkuType).UpdateColumns(u => u.Status).ExecuteCommand();
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["启用成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["启用失败,不存在此记录"] + "！\"}");
					}
					break;
				case "enablef"://禁用
					list_SkuType = await _context.Queryable<WebSkuType>().Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.SkuTypeid)).ToListAsync();
					if (list_SkuType.Count > 0)
					{
						for (int i = 0; i < list_SkuType.Count; i++)
						{
							list_SkuType[i].Status = 0;
						}
						_context.Updateable(list_SkuType).UpdateColumns(u => u.Status).ExecuteCommand();
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["禁用成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["禁用失败,不存在此记录"] + "！\"}");
					}
					break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					int total = 0;
					var exp = new Expressionable<WebSkuType>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<WebSkuTypeLang>().Where(s => s.SkuTypeid == u.SkuTypeid && (s.Title + "").Contains(keys.Trim())).Any());
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebSkuType>().Where(exp.ToExpression());
					if (sort != "")
					{
						sort = StringHelper.ChgToUnderLine(sort);
						if (order == "desc")
						{
							query.OrderBy(sort + " desc");
						}
						else
						{
							query.OrderBy(sort);
						}
					}
					query.OrderBy("sort,Sku_Typeid desc");
					list_SkuType = query.ToPageList(page, pagesize, ref total);
					var list_Lang = _context.Queryable<WebSkuTypeLang>().Where(u => list_SkuType.Select(s => s.SkuTypeid).Contains(u.SkuTypeid)).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_SkuType.Count; i++)
					{
						var model_Lang = list_Lang.FirstOrDefault(u => u.SkuTypeid == list_SkuType[i].SkuTypeid && u.Lang == Lang);
						str.Append("{");
						str.Append("\"SkuTypeid\":\"" + list_SkuType[i].SkuTypeid + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_Lang?.Title + "") + "\",");
						str.Append("\"AgeMin\":\"" + list_SkuType[i].AgeMin + "\",");
						str.Append("\"AgeMax\":\"" + list_SkuType[i].AgeMax + "\",");
						str.Append("\"IcoFont\":\"" + list_SkuType[i].IcoFont + "\",");
						str.Append("\"Sort\":\"" + list_SkuType[i].Sort + "\",");
                        str.Append("\"MenkeTemplateId\":\"" + list_SkuType[i].MenkeTemplateId + "\",");
                        str.Append("\"Dtime\":\"" + list_SkuType[i].Dtime + "\",");
						str.Append("\"Status\":" + list_SkuType[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_SkuType.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}
        #endregion

        #region "上课类型添加修改" 
        [Authorization(Power = "Main")]
        public async Task<IActionResult> SkuTypeAdd(int Sty = 0, long SkuTypeid = 0)
		{
			var vm_KeysAdd = new SkuTypeAdd();

			var result_Template = await _menkeService.GetTemplate();
			if (result_Template.StatusCode == 0) {
				var list_Template = result_Template.Data;
				vm_KeysAdd.MenkeTemplateItem = list_Template.Select(u => new SelectListItem((u.menke_name+"(" + u.menke_template_id + ")"), u.menke_template_id.ToString())).ToList();
			}

			var model_SkuType = _context.Queryable<WebSkuType>().First(u => u.SkuTypeid == SkuTypeid);
			if (model_SkuType != null)
			{
				vm_KeysAdd.SkuTypeid = SkuTypeid;
				vm_KeysAdd.IcoFont = model_SkuType.IcoFont;
				vm_KeysAdd.AgeMin = model_SkuType.AgeMin;
				vm_KeysAdd.AgeMax = model_SkuType.AgeMax;
				vm_KeysAdd.Status = model_SkuType.Status;
				vm_KeysAdd.Sort = model_SkuType.Sort;
				vm_KeysAdd.MenkeTemplateId = model_SkuType.MenkeTemplateId;
				var model_Lang = _context.Queryable<WebSkuTypeLang>().First(u => u.Lang == Lang && u.SkuTypeid == vm_KeysAdd.SkuTypeid);
				vm_KeysAdd.Title = model_Lang?.Title;
			}
			else
			{
				vm_KeysAdd.Status = 1;
			}
			vm_KeysAdd.Lang = Lang;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_KeysAdd);
		}

		//保存添改的数据字典
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> SkuTypeAdd(SkuTypeAdd vm_KeysAdd)
		{
			var model_SkuType = await _context.Queryable<WebSkuType>().InSingleAsync(vm_KeysAdd.SkuTypeid);

			int menke_type = 0;
			var result_Template = await _menkeService.GetTemplate();
			if (result_Template.StatusCode == 0)
			{
				var list_Template = result_Template.Data;
				menke_type = list_Template.FirstOrDefault(u => u.menke_template_id == vm_KeysAdd.MenkeTemplateId)?.menke_type??0;
			}

			if (model_SkuType != null)
			{
				model_SkuType.IcoFont = vm_KeysAdd.IcoFont;
				model_SkuType.AgeMin = vm_KeysAdd.AgeMin;
				model_SkuType.AgeMax = vm_KeysAdd.AgeMax;
				model_SkuType.Status = vm_KeysAdd.Status;
				model_SkuType.Sort = vm_KeysAdd.Sort;
				model_SkuType.MenkeTemplateId = vm_KeysAdd.MenkeTemplateId;
				model_SkuType.MenkeType = menke_type;

				var model_KeysLang = _context.Queryable<WebSkuTypeLang>().First(u => u.Lang == vm_KeysAdd.Lang && u.SkuTypeid == vm_KeysAdd.SkuTypeid);
				if (model_KeysLang != null)
				{
					model_KeysLang.Title = vm_KeysAdd.Title;
					_context.Updateable(model_KeysLang).UpdateColumns(u => u.Title).ExecuteCommand();
				}
				else
				{
					model_KeysLang = new WebSkuTypeLang();
					model_KeysLang.SkuTypeid = vm_KeysAdd.SkuTypeid;
					model_KeysLang.Lang = vm_KeysAdd.Lang;
					model_KeysLang.Title = vm_KeysAdd.Title;
					_context.Insertable(model_KeysLang).ExecuteCommand();
				}
				_context.Updateable(model_SkuType).ExecuteCommand();

				return Content("<script>parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改上课类型信息"] + "',msg:'" + _localizer["保存上课类型成功"] + "',timeout:3000,showType:'slide'}); location.href='?SkuTypeid=" + vm_KeysAdd.SkuTypeid + "&lang=" + vm_KeysAdd.Lang + "'</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_SkuType = new WebSkuType();
				model_SkuType.IcoFont = vm_KeysAdd.IcoFont;
				model_SkuType.AgeMin = vm_KeysAdd.AgeMin;
				model_SkuType.Status = vm_KeysAdd.Status;
				model_SkuType.AgeMax = vm_KeysAdd.AgeMax;
				model_SkuType.Sort = vm_KeysAdd.Sort;
				model_SkuType.Dtime = DateTime.Now;
                model_SkuType.MenkeTemplateId = vm_KeysAdd.MenkeTemplateId;
				model_SkuType.MenkeType = menke_type;
				vm_KeysAdd.SkuTypeid = _context.Insertable(model_SkuType).ExecuteReturnBigIdentity();

				var model_Lang = new WebSkuTypeLang();
				model_Lang.SkuTypeid = vm_KeysAdd.SkuTypeid;
				model_Lang.Lang = vm_KeysAdd.Lang;
				model_Lang.Title = vm_KeysAdd.Title;
				_context.Insertable(model_Lang).ExecuteReturnBigIdentity();
			}
			return Content("<script>parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["添加上课类型信息"] + "',msg:'" + _localizer["保存上课类型成功"] + "',timeout:3000,showType:'slide'}); location.href='?SkuTypeid=" + vm_KeysAdd.SkuTypeid + "&lang=" + vm_KeysAdd.Lang + "'</script>", "text/html", Encoding.GetEncoding("utf-8"));
		}
		#endregion
		#endregion
	}
}