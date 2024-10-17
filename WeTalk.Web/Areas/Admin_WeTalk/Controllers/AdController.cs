using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeTalk.Models;
using WeTalk.Web.Services;
using WeTalk.Common.Helper;
using Microsoft.Extensions.Localization;
using WeTalk.Web.Extensions;
using WeTalk.Common;
using WeTalk.Web.ViewModels.Ad;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class AdController : Base.BaseController
    {
        private SqlSugarScope _context = null;
        private IHttpContextAccessor _accessor;
        private readonly TokenManager _tokenManager;
        protected readonly IStringLocalizer<LangResource> _localizer;//语言包
        public AdController(SqlSugarScope context,IConfiguration config, IHttpContextAccessor accessor, IWebHostEnvironment env, TokenManager tokenManager, IStringLocalizer<LangResource> localizer)
            : base(tokenManager)
        {
            _accessor = accessor;
            _tokenManager = tokenManager;
            _localizer = localizer;
			_context =  context;

		}

        #region "Banner广告"
        #region "列表"    
        public IActionResult AdList()
        {
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
            return View();
        }

		[HttpGet]
		[HttpPost]
		public async Task<IActionResult> AdListData(string keys = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string type = GSRequestHelper.GetString(_accessor.HttpContext.Request, "type");
			string Adids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Adids");
			long Adid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Adid", 0);
			List<WebAd> list_WebAd = new List<WebAd>();
			if (Adids.Trim() == "") Adids = "0";
			string[] arr = Adids.Split(',');

			int total = 0;
			var exp = new Expressionable<WebAd>();
			string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
			string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebAd>()
						.SetColumns(u => u.Status == -1)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.Adid) || u.Adid == Adid))
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"删除成功！\"}");
					}
					else {
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"删除失败,不存在此记录！\"}");
					}
					break;
				case "enableu"://启用

					count = _context.Updateable<WebAd>()
						.SetColumns(u => u.Status == 1)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.Adid) || u.Adid == Adid))
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"启用成功！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"启用失败,不存在此记录！\"}");
					}
					break;
				case "enablef"://禁用
					count = _context.Updateable<WebAd>()
						.SetColumns(u => u.Status == 0)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.Adid) || u.Adid == Adid))
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"禁用成功！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"禁用失败,不存在此记录！\"}");
					}
					break;
				default:
					exp.And(u => u.Status != -1);
					var types = type.Split(',').ToArray();
					exp.AndIF(!string.IsNullOrEmpty(type),u => types.Contains(u.Type));
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.MergeString(u.Title , "").Contains(keys.Trim()));
					exp.AndIF(!string.IsNullOrEmpty(status),u => u.Status.ToString() == status.Trim());
					exp.AndIF(Adid > 0, u => u.Adid == Adid);
					exp.AndIF(!string.IsNullOrEmpty(begintime),u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));

					var query = _context.Queryable<WebAd>().Where(exp.ToExpression());
					if (sort!="")
					{
						if (order == "desc")
						{
							query.OrderBy(sort + " desc");
						}
						else
						{
							query.OrderBy(sort);
						}
					}
					query.OrderBy("Sort,Adid desc")
						.Select(u => new { u.Adid, u.Title, u.Img, u.Type, u.Begintime, u.Endtime, u.Dtime, u.Url, u.Sort, u.Status });

					list_WebAd = query.ToPageList(page,pagesize,ref total);
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_WebAd.Count; i++)
					{
						str.Append("{");
						str.Append("\"Adid\":\"" + list_WebAd[i].Adid + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(list_WebAd[i].Title+"") + "\",");
						str.Append("\"Img\":\"" + list_WebAd[i].Img+"" + "\",");
						str.Append("\"Type\":\"" + list_WebAd[i].Type + "" + "\",");
						str.Append("\"Thetime\":\"" + JsonHelper.JsonCharFilter(list_WebAd[i].Begintime.ToString() +"<br>"+ list_WebAd[i].Endtime.ToString()) + "\",");
						str.Append("\"Dtime\":\"" + list_WebAd[i].Dtime.ToString() + "\",");
						str.Append("\"Url\":\"" + list_WebAd[i].Url + "\",");
						str.Append("\"Sort\":\"" + list_WebAd[i].Sort + "\",");
						str.Append("\"Status\":\"" + list_WebAd[i].Status + "\",\"operation\":\"查看/修改\"}");
						if (i < (list_WebAd.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}

			return Content(str.ToString());
		}

		#endregion

		#region "修改"        
		public async Task<IActionResult> AdListAdd(long Adid = 0)
		{
			ViewModels.Ad.AdListAdd vm_AdListAdd = new ViewModels.Ad.AdListAdd();
			string type = GSRequestHelper.GetString(_accessor.HttpContext.Request, "type");
			
			var model_Ad = _context.Queryable<WebAd>().InSingle(Adid);
			if (model_Ad != null)
			{
				vm_AdListAdd.Type = model_Ad.Type;
				vm_AdListAdd.Title = model_Ad.Title;
				vm_AdListAdd.Img = model_Ad.Img;
				vm_AdListAdd.Url = model_Ad.Url;
				vm_AdListAdd.Begintime = model_Ad.Begintime;
				vm_AdListAdd.Endtime = model_Ad.Endtime;
				vm_AdListAdd.Sort = model_Ad.Sort;
				vm_AdListAdd.Intro = model_Ad.Intro;
				vm_AdListAdd.Dtime = model_Ad.Dtime;
				vm_AdListAdd.Status = model_Ad.Status;
				vm_AdListAdd.Adid = model_Ad.Adid;
			}
			else
			{
				vm_AdListAdd.Type = type;
				vm_AdListAdd.Status = 1;
				vm_AdListAdd.Dtime = DateTime.Now;
				vm_AdListAdd.Begintime = DateTime.Now;
				vm_AdListAdd.Endtime = DateTime.Now.AddYears(20);
				vm_AdListAdd.Sort = 100;
			}
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_AdListAdd);
		}

		//保存添改
		[HttpPost]
		public async Task<IActionResult> AdListAdd(AdListAdd vm_ListAdd)
		{
			string filename = "", fileurl = Appsettings.app("Web:Upfile") + "/AD/" + DateTime.Now.ToString("yyyyMMdd") + "/";
			string ImgRoot = Appsettings.app("Web:ImgRoot");
			string ext;
			int chk_file = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_file", 0);
			var files = _accessor.HttpContext.Request.Form.Files;
			string type = GSRequestHelper.GetString(_accessor.HttpContext.Request, "type");

			var model_Ad = _context.Queryable<WebAd>().InSingle(vm_ListAdd.Adid);
			if (model_Ad != null)
			{
				model_Ad.Type = vm_ListAdd.Type;
				model_Ad.Title = vm_ListAdd.Title;
				model_Ad.Url = vm_ListAdd.Url;
				model_Ad.Intro = vm_ListAdd.Intro;
				model_Ad.Status = vm_ListAdd.Status;
				model_Ad.Begintime = vm_ListAdd.Begintime;
				model_Ad.Endtime = vm_ListAdd.Endtime;
				model_Ad.Sort = vm_ListAdd.Sort;

				long size = files.Sum(f => f.Length);
				if (chk_file == 1)
				{
					if (!string.IsNullOrEmpty(model_Ad.Img)) FileHelper.FileDel(ImgRoot + model_Ad.Img);
					model_Ad.Img = "";
				}
				foreach (var formFile in files)
				{
					if (formFile.Length > 0)
					{
						if (!string.IsNullOrEmpty(model_Ad.Img)) FileHelper.FileDel(ImgRoot + model_Ad.Img);
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Ad.Img = fileurl + filename;
						}
					}
				}
				await _context.Updateable(model_Ad).ExecuteCommandAsync();

				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'修改广告',msg:'保存广告成功',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));

			}
			else{
				model_Ad = new WebAd();
				model_Ad.Type = vm_ListAdd.Type;
				model_Ad.Title = vm_ListAdd.Title;
				model_Ad.Url = vm_ListAdd.Url;
				model_Ad.Status = vm_ListAdd.Status;
				model_Ad.Intro = vm_ListAdd.Intro;
				model_Ad.Dtime = DateTime.Now;
				model_Ad.Begintime = vm_ListAdd.Begintime;
				model_Ad.Endtime = vm_ListAdd.Endtime;
				model_Ad.Sort = vm_ListAdd.Sort;

				foreach (var formFile in files)
				{
					if (formFile.Length > 0)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Ad.Img = fileurl + filename;
						}
					}
				}

				await _context.Insertable(model_Ad).ExecuteCommandAsync();
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'添加广告',msg:'创建广告成功',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion
		#endregion

		#region "多广告列表"
		#region "列表"    
		public IActionResult PageList()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		public async Task<IActionResult> PageListData(string keys = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			string type = GSRequestHelper.GetString(_accessor.HttpContext.Request, "type");
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string Adids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Adids");
			long appletsid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "appletsid", 0);
			long Adid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Adid", 0);
			List<WebAd> list_WebAd = new List<WebAd>();
			if (Adids.Trim() == "") Adids = "0";
			string[] arr = Adids.Split(',');
			int total = 0;
			var exp =new Expressionable<WebAd>();
			string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
			string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
			int count = 0;
			WebAd model_Ad = null;

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebAd>()
						.SetColumns(u => u.Status == -1)
						.Where(u => u.Type == type && (Array.ConvertAll(arr, long.Parse).Contains(u.Adid) || u.Adid == Adid))
						.ExecuteCommand();
					if(count>0){
						str.Append("{\"errorcode\":\"0\",\"msg\":\"删除成功！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"删除失败,不存在此记录！\"}");
					}
					break;
				case "enableu"://启用
					model_Ad = _context.Queryable<WebAd>().First(u =>u.Type== type && u.Adid == Adid);
					if (model_Ad != null)
					{
						model_Ad.Status = 1;
						_context.Updateable(model_Ad).UpdateColumns(u => u.Status).ExecuteCommand();
						str.Append("{\"errorcode\":\"0\",\"msg\":\"启用成功！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"启用失败,不存在此记录！\"}");
					}
					break;
				case "enablef"://禁用
					count = _context.Updateable(model_Ad).SetColumns(u => u.Status==0).Where(u => u.Type == type && u.Adid == Adid).ExecuteCommand();
					if (count>0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"禁用成功！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"禁用失败,不存在此记录！\"}");
					}
					break;
				default:
					exp.And(u => u.Type == type && u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.MergeString(u.Title , "").Contains(keys.Trim()) );
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(Adid > 0, u => u.Adid == Adid);
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebAd>().Where(exp.ToExpression());

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
					query.OrderBy("Sort,Adid desc")
						.Select(u=>new { u.Adid,u.Title,u.Img,u.Type,u.Begintime,u.Endtime,u.Dtime,u.Url,u.Sort,u.Status});
					list_WebAd = query.ToPageList(page,pagesize,ref total);
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_WebAd.Count; i++)
					{
						str.Append("{");
						str.Append("\"Adid\":\"" + list_WebAd[i].Adid + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(list_WebAd[i].Title + "") + "\",");
						str.Append("\"Img\":\"" + list_WebAd[i].Img + "" + "\",");
						str.Append("\"Type\":\"" + list_WebAd[i].Type + "" + "\",");
						str.Append("\"Thetime\":\"" + JsonHelper.JsonCharFilter(list_WebAd[i].Begintime.ToString() + "<br>" + list_WebAd[i].Endtime.ToString()) + "\",");
						str.Append("\"Dtime\":\"" + list_WebAd[i].Dtime.ToString() + "\",");
						str.Append("\"Url\":\"" + list_WebAd[i].Url + "\",");
						str.Append("\"Sort\":\"" + list_WebAd[i].Sort + "\",");
						str.Append("\"Status\":\"" + list_WebAd[i].Status + "\",\"operation\":\"查看/修改\"}");
						if (i < (list_WebAd.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}

			return Content(str.ToString());
		}

		#endregion

		#region "修改"        
		public async Task<IActionResult> PageListAdd(long Adid = 0)
		{
			ViewModels.Ad.PageListAdd vm_PageListAdd = new ViewModels.Ad.PageListAdd();
			string type = GSRequestHelper.GetString(_accessor.HttpContext.Request, "type");
			var model_Ad = _context.Queryable<WebAd>().InSingle(Adid);
			if (model_Ad != null)
			{
				vm_PageListAdd.Type = model_Ad.Type;
				vm_PageListAdd.Title = model_Ad.Title;
				vm_PageListAdd.Img = model_Ad.Img;
				vm_PageListAdd.Url = model_Ad.Url;
				vm_PageListAdd.Begintime = model_Ad.Begintime;
				vm_PageListAdd.Endtime = model_Ad.Endtime;
				vm_PageListAdd.Sort = model_Ad.Sort;
				vm_PageListAdd.Intro = model_Ad.Intro;
				vm_PageListAdd.Dtime = model_Ad.Dtime;
				vm_PageListAdd.Status = model_Ad.Status;
				vm_PageListAdd.Adid = model_Ad.Adid;
			}
			else
			{
				vm_PageListAdd.Type = type;
				vm_PageListAdd.Status = 1;
				vm_PageListAdd.Dtime = DateTime.Now;
				vm_PageListAdd.Begintime = DateTime.Now;
				vm_PageListAdd.Endtime = DateTime.Now.AddYears(20);
				vm_PageListAdd.Sort = 100;
			}
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_PageListAdd);
		}

		//保存添改
		[HttpPost]
		public async Task<IActionResult> PageListAdd(WebAd vm_ListAdd)
		{
			string filename = "", fileurl = Appsettings.app("Web:Upfile") + "/AD/" + DateTime.Now.ToString("yyyyMMdd") + "/";
			string ext;
			int chk_file = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_file", 0);
			var files = _accessor.HttpContext.Request.Form.Files;
			string type = GSRequestHelper.GetString(_accessor.HttpContext.Request, "type");
			string ImgRoot = Appsettings.app("Web:ImgRoot");

			var model_Ad = _context.Queryable<WebAd>().InSingle(vm_ListAdd.Adid);
			if (model_Ad != null)
			{
				model_Ad.Type = type;
				model_Ad.Title = vm_ListAdd.Title;
				model_Ad.Url = vm_ListAdd.Url;
				model_Ad.Intro = vm_ListAdd.Intro;
				model_Ad.Status = vm_ListAdd.Status;
				model_Ad.Begintime = vm_ListAdd.Begintime;
				model_Ad.Endtime = vm_ListAdd.Endtime;
				model_Ad.Sort = vm_ListAdd.Sort;

				long size = files.Sum(f => f.Length);
				if (chk_file == 1)
				{
					if (!string.IsNullOrEmpty(model_Ad.Img)) FileHelper.FileDel(ImgRoot + model_Ad.Img);
					model_Ad.Img = "";
				}
				foreach (var formFile in files)
				{
					if (formFile.Length > 0)
					{
						if (!string.IsNullOrEmpty(model_Ad.Img)) FileHelper.FileDel(ImgRoot + model_Ad.Img);
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Ad.Img = fileurl + filename;
						}
					}
				}
				await _context.Updateable(model_Ad).ExecuteCommandAsync();
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'修改广告',msg:'保存广告成功',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));

			}
			else
			{
				model_Ad = new WebAd();
				model_Ad.Type = type;
				model_Ad.Title = vm_ListAdd.Title;
				model_Ad.Url = vm_ListAdd.Url;
				model_Ad.Status = vm_ListAdd.Status;
				model_Ad.Intro = vm_ListAdd.Intro;
				model_Ad.Dtime = DateTime.Now;
				model_Ad.Begintime = vm_ListAdd.Begintime;
				model_Ad.Endtime = vm_ListAdd.Endtime;
				model_Ad.Sort = vm_ListAdd.Sort;
				foreach (var formFile in files)
				{
					if (formFile.Length > 0)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Ad.Img = fileurl + filename;
						}
					}
				}
				await _context.Insertable(model_Ad).ExecuteCommandAsync();
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'添加广告',msg:'创建广告成功',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion
		#endregion

		#region "单广告"        
		public async Task<IActionResult> SingleAD()
		{
			SingleAD vm_AdDetail = new SingleAD();
			string type = GSRequestHelper.GetString(_accessor.HttpContext.Request, "type");
			var model_Ad = _context.Queryable<WebAd>().First(u=>u.Type == type);
			if (model_Ad != null)
			{
				vm_AdDetail.Type = model_Ad.Type;
				vm_AdDetail.Title = model_Ad.Title;
				vm_AdDetail.Img = model_Ad.Img;
				vm_AdDetail.Url = model_Ad.Url;
				vm_AdDetail.Begintime = model_Ad.Begintime;
				vm_AdDetail.Endtime = model_Ad.Endtime;
				vm_AdDetail.Intro = model_Ad.Intro;
				vm_AdDetail.Dtime = model_Ad.Dtime;
				vm_AdDetail.Status = model_Ad.Status;
				vm_AdDetail.Adid = model_Ad.Adid;
			}
			else
			{
				vm_AdDetail.Type = type;
				vm_AdDetail.Status = 1;
				vm_AdDetail.Dtime = DateTime.Now;
				vm_AdDetail.Begintime = DateTime.Now;
				vm_AdDetail.Endtime = DateTime.Now.AddYears(20);
			}
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_AdDetail);
		}

		//保存添改
		[HttpPost]
		public async Task<IActionResult> SingleAD(SingleAD vm_ListAdd)
		{
			string filename = "", fileurl = Appsettings.app("Web:Upfile") + "/AD/" + DateTime.Now.ToString("yyyyMMdd") + "/";
			string ext;
			int chk_file = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_file", 0);
			var files = _accessor.HttpContext.Request.Form.Files;
			string ImgRoot = Appsettings.app("Web:ImgRoot");

			var model_Ad = _context.Queryable<WebAd>().First(u => u.Type == vm_ListAdd.Type);
			if (model_Ad != null)
			{
				model_Ad.Type = vm_ListAdd.Type;
				model_Ad.Title = vm_ListAdd.Title;
				model_Ad.Url = vm_ListAdd.Url;
				model_Ad.Intro = vm_ListAdd.Intro;
				model_Ad.Status = vm_ListAdd.Status;
				model_Ad.Begintime = vm_ListAdd.Begintime;
				model_Ad.Endtime = vm_ListAdd.Endtime;
				model_Ad.Sort = vm_ListAdd.Sort;

				long size = files.Sum(f => f.Length);
				if (chk_file == 1)
				{
					if (!string.IsNullOrEmpty(model_Ad.Img)) FileHelper.FileDel(ImgRoot + model_Ad.Img);
					model_Ad.Img = "";
				}
				foreach (var formFile in files)
				{
					if (formFile.Length > 0)
					{
						if (!string.IsNullOrEmpty(model_Ad.Img)) FileHelper.FileDel(ImgRoot + model_Ad.Img);
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Ad.Img = fileurl + filename;
						}
					}
				}
				await _context.Updateable(model_Ad).ExecuteCommandAsync();
				return Content("<script>alert('"+ _localizer["保存广告成功"] +"');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));

			}
			else
			{
				model_Ad = new WebAd();
				model_Ad.Type = vm_ListAdd.Type;
				model_Ad.Title = vm_ListAdd.Title;
				model_Ad.Url = vm_ListAdd.Url;
				model_Ad.Status = vm_ListAdd.Status;
				model_Ad.Intro = vm_ListAdd.Intro;
				model_Ad.Dtime = DateTime.Now;
				model_Ad.Begintime = vm_ListAdd.Begintime;
				model_Ad.Endtime = vm_ListAdd.Endtime;
				model_Ad.Sort = vm_ListAdd.Sort;
				foreach (var formFile in files)
				{
					if (formFile.Length > 0)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Ad.Img = fileurl + filename;
						}
					}
				}
				await _context.Insertable(model_Ad).ExecuteCommandAsync();
				return Content("<script>alert('" + _localizer["保存广告成功"] + "');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion
	}
}
