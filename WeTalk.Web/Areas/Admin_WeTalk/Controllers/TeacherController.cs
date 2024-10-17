using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto.Menke;
using WeTalk.Web.Extensions;
using WeTalk.Web.ViewModels.Teacher;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class TeacherController : Base.BaseController
	{
		private readonly TokenManager _tokenManager;
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly IPubConfigService _pubConfigService;
		private readonly IMenkeService _menkeService;

		private readonly ILogger<TeacherController> _logger;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包
		public TeacherController(IConfiguration config, IHttpContextAccessor accessor,SqlSugarScope dbcontext, ILogger<TeacherController> logger, IPubConfigService pubConfigService,
			TokenManager tokenManager, IStringLocalizer<LangResource> localizer, IMenkeService menkeService)
			: base(tokenManager)
		{
			_tokenManager = tokenManager;
			_pubConfigService = pubConfigService;
			_context = dbcontext;
			_accessor = accessor;
			_logger = logger;
			_localizer = localizer;
			_menkeService = menkeService;
		}

        #region "分类列表"    
        [Authorization(Power = "Main")]
        public IActionResult Category(string type = "")
		{
			ViewBag.Type = type;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CategoryData(string keys = "", string type = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string TeacherCategoryids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "TeacherCategoryids");
			long TeacherCategoryid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "TeacherCategoryid", 0);
			List<WebTeacherCategory> list_Category = new List<WebTeacherCategory>();
			if (TeacherCategoryids.Trim() == "") TeacherCategoryids = "0";
			string[] arr = TeacherCategoryids.Split(',');

			int total = 0, count = 0;
			var exp = new Expressionable<WebTeacherCategory>();
			string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
			string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
			string son_str = "";

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebTeacherCategory>()
						.SetColumns(u => u.Status == -1)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.TeacherCategoryid) || u.TeacherCategoryid == TeacherCategoryid))
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除成功"] +"！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除失败,不存在此记录"] + "！\"}");
					}
					break;
				case "enableu"://启用
					count = _context.Updateable<WebTeacherCategory>()
						.SetColumns(u => u.Status == 1)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.TeacherCategoryid) || u.TeacherCategoryid == TeacherCategoryid))
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["启用成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["启用失败,不存在此记录"] + "！\"}");
					}
					break;
				case "enablef"://禁用
					count = _context.Updateable<WebTeacherCategory>()
						.SetColumns(u => u.Status == 0)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.TeacherCategoryid) || u.TeacherCategoryid == TeacherCategoryid))
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["禁用成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["禁用失败,不存在此记录"] + "！\"}");
					}
					break;
				default:
					exp.And(u => u.Status != -1);
					if (!string.IsNullOrEmpty(keys))
					{
						exp.And(u => SqlFunc.Subqueryable<WebTeacherCategoryLang>().Where(s=>s.TeacherCategoryid==u.TeacherCategoryid && (s.Title.Contains(keys))).Any());
					}
					if (!string.IsNullOrEmpty(status))
					{
						exp.And(u => u.Status.ToString() == status.Trim());
					}
					if (TeacherCategoryid > 0)
					{
						exp.And(u => u.TeacherCategoryid == TeacherCategoryid);
					}
					if (!string.IsNullOrEmpty(begintime))
					{
						exp.And(u => u.Dtime >= DateTime.Parse(begintime));
					}
					if (!string.IsNullOrEmpty(endtime))
					{
						exp.And(u => u.Dtime <= DateTime.Parse(endtime));
					}
					var query = _context.Queryable<WebTeacherCategory>().Where(exp.ToExpression());
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
					query.OrderBy(u=>u.Sort).OrderBy(u => u.TeacherCategoryid, OrderByType.Desc);
					var list_Category_all = await query.ToListAsync();
					list_Category = query.Where(u => u.Fid == 0).ToPageList(page, pagesize, ref total);
					var list_CategoryLang = _context.Queryable<WebTeacherCategoryLang>().Where(u => list_Category_all.Select(s => s.TeacherCategoryid).Contains(u.TeacherCategoryid) && u.Lang == Lang).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Category.Count; i++)
					{
						var model_CategoryLang = list_CategoryLang.FirstOrDefault(u => u.TeacherCategoryid == list_Category[i].TeacherCategoryid);
						son_str = CategorySon(list_Category[i].TeacherCategoryid, list_Category_all, list_CategoryLang);
						str.Append("{");
						str.Append("\"TeacherCategoryid\":\"" + list_Category[i].TeacherCategoryid + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_CategoryLang?.Title+"") + "\",");
						str.Append("\"Depth\":\"" + list_Category[i].Depth + "\",");
						str.Append("\"Dtime\":\"" + list_Category[i].Dtime.ToString() + "\",");
						str.Append("\"Sort\":\"" + list_Category[i].Sort + "\",");
						str.Append("\"Isadmin\":\"" + list_Category[i].Isadmin + "\",");
						str.Append("\"Status\":\"" + list_Category[i].Status + "\",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						str.Append(son_str);
						if (i < (list_Category.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}
		private string CategorySon(long f_id, List<WebTeacherCategory> list_Category_all,List<WebTeacherCategoryLang> list_CategoryLang)
		{
			StringBuilder str = new StringBuilder();
			var list_Category = list_Category_all.Where(u => u.Status == 1 && u.Fid == f_id).OrderBy(u => u.Sort).ToList();
			for (int i = 0; i < list_Category.Count; i++)
			{
				var model_CategoryLang = list_CategoryLang.FirstOrDefault(u => u.TeacherCategoryid == list_Category[i].TeacherCategoryid);
				str.Append(",{");
				str.Append("\"TeacherCategoryid\":\"" + list_Category[i].TeacherCategoryid + "\",");
				str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_CategoryLang?.Title+"") + "\",");
				str.Append("\"Depth\":\"" + list_Category[i].Depth + "\",");
				str.Append("\"Dtime\":\"" + list_Category[i].Dtime.ToString() + "\",");
				str.Append("\"Sort\":\"" + list_Category[i].Sort + "\",");
				str.Append("\"Isadmin\":\"" + list_Category[i].Isadmin + "\",");
				str.Append("\"Status\":\"" + list_Category[i].Status + "\",");
				str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\",");
				str.Append("\"_parentId\":" + f_id + "}");

				str.Append(CategorySon(list_Category[i].TeacherCategoryid, list_Category_all, list_CategoryLang));
			}
			return str.ToString();
		}
        #endregion

        #region "分类修改" 
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CategoryAdd(long TeacherCategoryid = 0, long fid = 0)
		{
			ViewModels.Teacher.CategoryAdd vm_ListAdd = new ViewModels.Teacher.CategoryAdd();
			var model_Category = _context.Queryable<WebTeacherCategory>().InSingle(TeacherCategoryid);
			if (model_Category != null)
			{
				vm_ListAdd.TeacherCategoryid = model_Category.TeacherCategoryid;
				vm_ListAdd.Fid = model_Category.Fid;
				vm_ListAdd.Depth = model_Category.Depth;
				vm_ListAdd.Status = model_Category.Status;
				vm_ListAdd.Dtime = model_Category.Dtime;
				vm_ListAdd.Sort = model_Category.Sort;
				vm_ListAdd.Sendtime = model_Category.Sendtime;
				vm_ListAdd.Img = model_Category.Img;

				var model_CategoryLang = _context.Queryable<WebTeacherCategoryLang>().First(u => u.Lang == Lang && u.TeacherCategoryid == model_Category.TeacherCategoryid);
				if (model_CategoryLang != null)
				{
					vm_ListAdd.Title = model_CategoryLang.Title;
				}
			}
			else
			{
				vm_ListAdd.Fid = fid;
				vm_ListAdd.Sort = 100;
				vm_ListAdd.Status = 1;
				vm_ListAdd.Dtime = DateTime.Now;
			}
			vm_ListAdd.Lang = Lang;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}

		//保存添改
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CategoryAdd(ViewModels.Teacher.CategoryAdd vm_ListAdd)
		{
			string filename = "", fileurl = Appsettings.app("Web:Upfile").ToString() + "/Category/" + DateTime.Now.ToString("yyyyMMdd") + "/";
			int chk_img = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_img", 0);
			int chk_img_h5 = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_img_h5", 0);
			int chk_load_torque = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_load_torque", 0);
			var files = _accessor.HttpContext.Request.Form.Files;
			long fid = GSRequestHelper.GetFormLong(_accessor.HttpContext.Request, "input_fid", 0);
			string ImgRoot = Appsettings.app("Web:ImgRoot");
			string ext = "";//扩展名
			var model_master = _tokenManager.GetAdminInfo();
			string message = "[" + DateTime.Now + "]" + model_master?.Username + "修改分类信息";

			var model_Category_f = (fid > 0) ? _context.Queryable<WebTeacherCategory>().InSingle(fid) : null;
			if (model_Category_f != null && model_Category_f.Depth > 1)
			{
				return Content("<script>alert('" + _localizer["二级分类下不允许再创建子分类"] + "！');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			var model_Category = _context.Queryable<WebTeacherCategory>().InSingle(vm_ListAdd.TeacherCategoryid);
			if (model_Category != null)
			{
				if (("|" + model_Category.Path + "|").Contains("|" + fid + "|") && fid != 0 && fid != model_Category.Fid)
				{
					return Content("<script>alert('" + _localizer["不允许选持本分类下的直系菜单，这将会造成死循环，请后退重选"] + "！');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				model_Category.Fid = fid;
				model_Category.Status = vm_ListAdd.Status;
				model_Category.Dtime = DateTime.Now;
				model_Category.Sort = vm_ListAdd.Sort;
				model_Category.Sendtime = vm_ListAdd.Sendtime;

				if (model_Category_f != null)
				{
					model_Category.Depth = model_Category_f.Depth + 1;
					model_Category.Path = model_Category_f.Path + "|" + model_Category.TeacherCategoryid;
					model_Category.Rootid = model_Category_f.Rootid;
				}
				else
				{
					model_Category.Depth = 1;
					model_Category.Path = model_Category.TeacherCategoryid.ToString();
					model_Category.Rootid = model_Category.TeacherCategoryid;
				}
				#region "上传"
				if (chk_img == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Category.Img)) FileHelper.FileDel(ImgRoot + model_Category.Img);
					}
					catch { }
					model_Category.Img = "";
				}
				if (files.Count > 0)
				{
					var formFile = files.FirstOrDefault(u => u.Name == "input_img" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Category.Img)) FileHelper.FileDel(ImgRoot + model_Category.Img);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Category.Img = fileurl + filename;
						}
					}
				}
				#endregion

				var model_CategoryLang = _context.Queryable<WebTeacherCategoryLang>().First(u => u.Lang == vm_ListAdd.Lang && u.TeacherCategoryid == model_Category.TeacherCategoryid);
				if (model_CategoryLang != null)
				{
					model_CategoryLang.Title = vm_ListAdd.Title;
					_context.Updateable(model_CategoryLang).ExecuteCommand();
				}
				else {
					model_CategoryLang=new WebTeacherCategoryLang();
					model_CategoryLang.Lang = vm_ListAdd.Lang;
					model_CategoryLang.TeacherCategoryid = model_Category.TeacherCategoryid;
					model_CategoryLang.Title = vm_ListAdd.Title;
					_context.Insertable(model_CategoryLang).ExecuteCommand();
				}
				_context.Updateable(model_Category).ExecuteCommand();
				return Content("<script>parent.$('#table_view').treegrid('reload');alert('" + _localizer["保存分类成功"] + "'); location.href='?TeacherCategoryid=" + vm_ListAdd.TeacherCategoryid + "&lang="+ vm_ListAdd.Lang +"';</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_Category = new WebTeacherCategory();
				model_Category.Fid = fid;
				model_Category.Status = vm_ListAdd.Status;
				model_Category.Dtime = DateTime.Now;
				model_Category.Sort = vm_ListAdd.Sort;
				model_Category.Sendtime = vm_ListAdd.Sendtime;

				#region "上传"
				if (files.Count > 0)
				{
					var formFile = files.FirstOrDefault(u => u.Name == "input_img" && u.Length > 0);
					if (formFile != null && formFile.Length > 0)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Category.Img)) FileHelper.FileDel(ImgRoot + model_Category.Img);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Category.Img = fileurl + filename;
						}
					}
				}
				#endregion
				vm_ListAdd.TeacherCategoryid = _context.Insertable(model_Category).ExecuteReturnBigIdentity();
				if (model_Category_f != null)
				{
					await _context.Updateable<WebTeacherCategory>()
					.SetColumns(u => new WebTeacherCategory { Path = SqlFunc.MergeString(model_Category_f.Path, "|", vm_ListAdd.TeacherCategoryid.ToString()), Rootid = model_Category_f.Rootid, Depth = (model_Category_f.Depth + 1) })
					.Where(u => u.TeacherCategoryid == vm_ListAdd.TeacherCategoryid)
					.ExecuteCommandAsync();
				}
				else
				{
					string path = vm_ListAdd.TeacherCategoryid.ToString();
					await _context.Updateable<WebTeacherCategory>()
						.SetColumns(u => new WebTeacherCategory { Path = path, Rootid = vm_ListAdd.TeacherCategoryid, Depth = 1 })
						.Where(u => u.TeacherCategoryid == vm_ListAdd.TeacherCategoryid)
						.ExecuteCommandAsync();
				}
				var model_CategoryLang = new WebTeacherCategoryLang();
				model_CategoryLang.Lang = vm_ListAdd.Lang;
				model_CategoryLang.TeacherCategoryid = vm_ListAdd.TeacherCategoryid;
				model_CategoryLang.Title = vm_ListAdd.Title;
				_context.Insertable(model_CategoryLang).ExecuteCommand();
				return Content("<script>parent.$('#table_view').treegrid('reload');alert('" + _localizer["保存分类成功"] + "');location.href='?TeacherCategoryid=" + vm_ListAdd.TeacherCategoryid + "&lang="+ vm_ListAdd.Lang +"';</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion

		#region "树型的JSON"
		[HttpPost]
		public async Task<IActionResult> CategoryClass(long TeacherCategoryid = 0, long Fid = 0)
		{
			StringBuilder str = new StringBuilder();
			string path = "";
			long id = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "id", 0);//每次点击节点打开下一节点时
			var list_Category = await _context.Queryable<WebTeacherCategory>()
				.LeftJoin<WebTeacherCategoryLang>((l,r)=>l.TeacherCategoryid==r.TeacherCategoryid && r.Lang==Lang)
				.Where(l => l.Status == 1).OrderBy(l => l.Sort)
				.Select((l, r) => new CategoryAdd { 
					TeacherCategoryid = l.TeacherCategoryid,
					Fid = l.Fid,
					Status=l.Status,
					Path= l.Path,
					Depth= l.Depth,
					Sort= l.Sort,
					Title= r.Title })
				.ToListAsync();//((IsDevelopment == 1) ? "" : " and sty=1") 
			var model_Category = _context.Queryable<WebTeacherCategory>().InSingle(TeacherCategoryid);
			if (model_Category != null)
			{
				path = model_Category.Path;
			}
			else
			{
				var model_CategoryF = list_Category.FirstOrDefault(u => u.TeacherCategoryid == Fid);
				path = model_CategoryF != null ? model_CategoryF.Path : "";
			}

			str.Append("[");
			if (Fid == 0) str.Append("{\"id\":0,\"text\":\"" + _localizer["根菜单"] + "\"},");
			var model_Categorys = list_Category.Where(u => u.Fid == id && u.Status == 1).OrderBy(u => u.Sort).ToList();//((IsDevelopment == 1) ? "" : " and sty=1") 
			for (int i = 0; i < model_Categorys.Count; i++)
			{
				str.Append("{");
				str.Append("    \"id\":" + model_Categorys[i].TeacherCategoryid + ",");
				str.Append("    \"text\":\"" + model_Categorys[i].Title + "\",");
				//判断是否加载下级子菜单
				if (("|" + path + "|").Contains("|" + model_Categorys[i].TeacherCategoryid.ToString() + "|"))
				{
					//加载，则判断是否有子菜单
					if (list_Category.Any(u => u.Fid == model_Categorys[i].TeacherCategoryid))
					{
						str.Append("    \"iconCls\":\"icon-ok\",");
						str.Append("    \"state\":\"open\",");
						str.Append(CategoryClassSon(model_Categorys[i].TeacherCategoryid, list_Category, path));
					}
				}
				else
				{
					//不加载，则判断是否有子菜单
					if (list_Category.Any(u => u.Fid == model_Categorys[i].TeacherCategoryid))
					{
						str.Append("    \"state\":\"closed\",");
					}
				}
				str.Remove(str.Length - 1, 1);
				str.Append("    },");
			}
			str.Remove(str.Length - 1, 1);
			str.Append("]");
			return Content(str.ToString());
		}

		private string CategoryClassSon(long fid, List<CategoryAdd> list_Category, string path)
		{
			StringBuilder str = new StringBuilder();
			var model_Category = list_Category.Where(u => u.Fid == fid && u.Status == 1).OrderBy(u => u.Sort).ToList();
			str.Append("    \"children\":[");
			for (int i = 0; i < model_Category.Count; i++)
			{
				str.Append("{");
				str.Append("    \"id\":" + model_Category[i].TeacherCategoryid.ToString() + ",");
				str.Append("    \"text\":\"" + model_Category[i].Title + "\",");
				//判断是否加载下级子菜单
				if (("|" + path + "|").Contains("|" + model_Category[i].TeacherCategoryid.ToString() + "|"))
				{
					//加载，则判断是否有子菜单
					if (list_Category.Any(u => u.Fid == model_Category[i].TeacherCategoryid))
					{
						str.Append("    \"iconCls\":\"icon-ok\",");
						str.Append("    \"state\":\"open\",");
						str.Append(CategoryClassSon(int.Parse(model_Category[i].TeacherCategoryid.ToString()), list_Category, path));
					}
				}
				else
				{
					//不加载，则判断是否有子菜单
					if (list_Category.Any(u => u.Fid == model_Category[i].TeacherCategoryid))
					{
						str.Append("    \"state\":\"closed\",");
					}
				}
				str.Remove(str.Length - 1, 1);
				str.Append("},");
			}
			str.Remove(str.Length - 1, 1);
			str.Append("],");
			return str.ToString();
		}
        #endregion

        #region "老师列表" 
        [Authorization(Power = "Main")]
        public IActionResult List(long categoryid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			ViewBag.Categoryid = categoryid;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListData(string keys = "",long categoryid = 0, string sty = "" ,string status = "",string recommend="", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string Teacherids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Teacherids");
			long Teacherid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Teacherid", 0);
			var list_Teacher = new List<WebTeacher>();
			if (Teacherids.Trim() == "") Teacherids = "0";
			string[] arr = Teacherids.Split(',');
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebTeacher>()
						.SetColumns(u => u.Status == -1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Teacherid) || u.Teacherid == Teacherid)
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除失败,不存在此记录"] + "！\"}");
					}
					break;
				case "union"://关联用户
					var model_Teacher = _context.Queryable<WebTeacher>().InSingle(Teacherid);
					if (model_Teacher != null)
					{
						var result = await _menkeService.UnionMenkeUserId(model_Teacher.MobileCode, model_Teacher.Mobile, 1);
						if (result.StatusCode == 0)
                        {
                            str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["关联成功"] + "！\"}");
                        }
						else
                        {
                            str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + JsonHelper.JsonCharFilter(result.Message) + "！\"}");
                        }
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["关联失败"] + "！\"}");
					}
					break;
				case "enableu"://启用
					count = _context.Updateable<WebTeacher>()
						.SetColumns(u => u.Status == 1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Teacherid) || u.Teacherid == Teacherid)
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["启用成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["启用失败,不存在此记录"] + "！\"}");
					}
					break;
				case "enablef"://禁用
					count = _context.Updateable<WebTeacher>()
						.SetColumns(u => u.Status == 0)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Teacherid) || u.Teacherid == Teacherid)
						.ExecuteCommand();
					if (count > 0)
					{
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

					var list_Category = _context.Queryable<WebTeacherCategory>().LeftJoin<WebTeacherCategoryLang>((l,r)=>l.TeacherCategoryid==r.TeacherCategoryid && r.Lang==Lang)
						.Where((l,r) => l.Status==1)
						.Select((l,r)=>new { l.TeacherCategoryid ,r.Title})
						.ToList();
					int total = 0;
					var exp = new Expressionable<WebTeacher>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<WebTeacherLang>().Where(s=>s.Teacherid == u.Teacherid && s.Lang == Lang && (s.Name.Contains(keys) ||s.Message.Contains(keys))).Any());
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(sty), u => u.Sty.ToString() == sty.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Sendtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Sendtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebTeacher>().Where(exp.ToExpression());
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
					query.OrderBy("sort,sendtime desc,Teacherid desc");
					list_Teacher = query.ToPageList(page, pagesize, ref total);
					var list_TeacherLang = _context.Queryable<WebTeacherLang>().Where(u => list_Teacher.Select(s=>s.Teacherid).Contains(u.Teacherid) && u.Lang == Lang).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Teacher.Count; i++)
					{
                        var model_Lang = list_TeacherLang.FirstOrDefault(u => u.Teacherid == list_Teacher[i].Teacherid);
						var model_Category = list_Category.FirstOrDefault(u => u.TeacherCategoryid == list_Teacher[i].TeacherCategoryid);
						str.Append("{");
						str.Append("\"Teacherid\":\"" + list_Teacher[i].Teacherid + "\",");
						str.Append("\"Sty\":" + list_Teacher[i].Sty + ",");
						if(model_Category != null) str.Append("\"TeacherCategory\":\"" + JsonHelper.JsonCharFilter(model_Category.Title) + "\",");
						str.Append("\"Name\":\"" + JsonHelper.JsonCharFilter(model_Lang?.Name + "") + "\",");
						str.Append("\"Keys\":\"" +JsonHelper.JsonCharFilter(model_Lang?.Keys+"") + "\",");
						str.Append("\"Mobile\":\"" + list_Teacher[i].MobileCode + "-" + list_Teacher[i].Mobile + "\",");
						str.Append("\"Img\":\"" + list_Teacher[i].Img + "\",");
						str.Append("\"MenkeUserId\":\"" + list_Teacher[i].MenkeUserId + "\",");
						str.Append("\"Dtime\":\"" + list_Teacher[i].Dtime.ToString() + "\",");
						str.Append("\"Sort\":\"" + list_Teacher[i].Sort + "\",");
						str.Append("\"Sendtime\":\"" + list_Teacher[i].Sendtime + "\",");
						str.Append("\"Status\":" + list_Teacher[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_Teacher.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}
        #endregion

        #region "老师修改"
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListAdd(long Teacherid = 0)
		{
			var vm_ListAdd = new ViewModels.Teacher.ListAdd();
			if (Lang == "zh-cn")
			{
				vm_ListAdd.CountryCodeItem = _context.Queryable<PubCountry>().ToList().Select(u => new SelectListItem(u.Country, u.Code)).ToList();
			}
			else {
				vm_ListAdd.CountryCodeItem = _context.Queryable<PubCountry>().ToList().Select(u => new SelectListItem(u.CountryEn, u.Code)).ToList();
			}
			vm_ListAdd.LangItems = _context.Queryable<PubLanguage>().Where(u => u.Status == 1).OrderBy(u=>u.Dtime,OrderByType.Desc).Select(u => new { u.Title, u.Lang }).ToList().Select(u => new SelectListItem(u.Title, u.Lang)).ToList();
			vm_ListAdd.TimezoneItems = _context.Queryable<PubTimezone>().Where(u => u.Status == 1).OrderBy(u => u.Title).Select(u => new { u.Title, u.Timezoneid }).ToList().Select(u => new SelectListItem(u.Title, u.Timezoneid.ToString())).ToList();

			var model_Teacher = _context.Queryable<WebTeacher>().InSingle(Teacherid);
			if (model_Teacher != null)
			{
				vm_ListAdd.Teacherid = Teacherid;
				vm_ListAdd.Sty = model_Teacher.Sty;
				vm_ListAdd.TeacherCategoryid = model_Teacher.TeacherCategoryid;
				vm_ListAdd.Img = model_Teacher.Img;
				vm_ListAdd.HeadImg = model_Teacher.HeadImg;
				vm_ListAdd.Mobile = (model_Teacher.Mobile+"").Replace(" ", "").Replace("-", "");
				vm_ListAdd.MobileCode = model_Teacher.MobileCode;
				vm_ListAdd.Email = model_Teacher.Email;
				vm_ListAdd.TeacherLang = model_Teacher.TeacherLang;
				vm_ListAdd.Timezoneid = model_Teacher.Timezoneid;
				vm_ListAdd.Sort = model_Teacher.Sort;
				vm_ListAdd.Dtime = model_Teacher.Dtime;
				vm_ListAdd.Status = model_Teacher.Status;
				vm_ListAdd.Sendtime = model_Teacher.Sendtime;
				vm_ListAdd.Remark = model_Teacher.Remark;
				vm_ListAdd.Likes = model_Teacher.Likes;
				vm_ListAdd.MenkeUserId = model_Teacher.MenkeUserId;
				vm_ListAdd.CountryCode=model_Teacher.CountryCode;
				vm_ListAdd.Gender = model_Teacher.Gender;
				vm_ListAdd.Birthdate = vm_ListAdd.Birthdate;

				var model_TeacherLang = _context.Queryable<WebTeacherLang>().First(u => u.Lang == Lang && u.Teacherid == model_Teacher.Teacherid);
				if (model_TeacherLang != null)
				{
					vm_ListAdd.Name = model_TeacherLang.Name;
					vm_ListAdd.Message = model_TeacherLang.Message;
					vm_ListAdd.Keys = model_TeacherLang.Keys;
					vm_ListAdd.Comefrom = model_TeacherLang.Comefrom;
					vm_ListAdd.Edu = model_TeacherLang.Edu;
					vm_ListAdd.Religion = model_TeacherLang.Religion;
					if (!string.IsNullOrEmpty(model_TeacherLang.Advantage))
					{
						var o = JObject.Parse(model_TeacherLang.Advantage);
						vm_ListAdd.Tag1 = (o != null && o["tag1"] != null) ? o["tag1"].ToString() : "";
						vm_ListAdd.Tag2 = (o != null && o["tag2"] != null) ? o["tag2"].ToString() : "";
						vm_ListAdd.Tag3 = (o != null && o["tag3"] != null) ? o["tag3"].ToString() : "";
						vm_ListAdd.Tag4 = (o != null && o["tag4"] != null) ? o["tag4"].ToString() : "";
						vm_ListAdd.Tag5 = (o != null && o["tag5"] != null) ? o["tag5"].ToString() : "";
						vm_ListAdd.Tag6 = (o != null && o["tag6"] != null) ? o["tag6"].ToString() : "";
						vm_ListAdd.Tag7 = (o != null && o["tag7"] != null) ? o["tag7"].ToString() : "";
						vm_ListAdd.Tag8 = (o != null && o["tag8"] != null) ? o["tag8"].ToString() : "";
						vm_ListAdd.Tag9 = (o != null && o["tag9"] != null) ? o["tag9"].ToString() : "";
						vm_ListAdd.Tag10 = (o != null && o["tag10"] != null) ? o["tag10"].ToString() : "";
						vm_ListAdd.Tag11 = (o != null && o["tag11"] != null) ? o["tag11"].ToString() : "";
						vm_ListAdd.Tag12 = (o != null && o["tag12"] != null) ? o["tag12"].ToString() : "";
					}
					vm_ListAdd.Motto = model_TeacherLang.Motto;
					vm_ListAdd.Philosophy = model_TeacherLang.Philosophy;
				}
			}
			else
			{
				vm_ListAdd.Birthdate = DateTime.Now.AddYears(-20);
				vm_ListAdd.Status = 1;
				vm_ListAdd.Sort = 100;
				vm_ListAdd.Sendtime = DateTime.Now;
			}
			if (DateTime.Now.AddYears(-20) < vm_ListAdd.Birthdate) vm_ListAdd.Birthdate= DateTime.Now.AddYears(-20);
			vm_ListAdd.Lang = Lang;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}

		//保存添改
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListAdd(ViewModels.Teacher.ListAdd vm_ListAdd)
		{
			string filename = "", fileurl = Appsettings.app("Web:Upfile").ToString() + "/Teacher/" + DateTime.Now.ToString("yyyyMMdd") + "/";
			int chk_photo = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_photo", 0);
			int chk_img = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_img", 0);
			int chk_head_img = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_head_img", 0);
			var files = _accessor.HttpContext.Request.Form.Files;
			string ImgRoot = Appsettings.app("Web:ImgRoot");
			string ext = "";//扩展名
			string advantage = "{\"tag1\":\"" + vm_ListAdd.Tag1 + "\",\"tag2\":\"" + vm_ListAdd.Tag2 + "\",\"tag3\":\"" + vm_ListAdd.Tag3 + "\",\"tag4\":\"" + vm_ListAdd.Tag4 + "\",\"tag5\":\"" + vm_ListAdd.Tag5 + "\",\"tag6\":\"" + vm_ListAdd.Tag6 + "\",\"tag7\":\"" + vm_ListAdd.Tag7 + "\",\"tag8\":\"" + vm_ListAdd.Tag8 + "\",\"tag9\":\"" + vm_ListAdd.Tag9 + "\",\"tag10\":\"" + vm_ListAdd.Tag10 + "\",\"tag11\":\"" + vm_ListAdd.Tag11 + "\",\"tag12\":\"" + vm_ListAdd.Tag12 + "\"}";

			if (DateTime.Now.AddYears(-20) < vm_ListAdd.Birthdate)
			{
				return Content("<script>alert('" + _localizer["老师最小年龄为20岁"] + "!');history.go(-1)</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			if (string.IsNullOrEmpty(vm_ListAdd.Mobile)) vm_ListAdd.Mobile = "";
			var model_master = _tokenManager.GetAdminInfo();
			string message = "[" + DateTime.Now + "]" + model_master?.Username + "修改老师信息";
			var model_Teacher = _context.Queryable<WebTeacher>().InSingle(vm_ListAdd.Teacherid);
			if (model_Teacher != null)
			{
				var model_TeacherLang = _context.Queryable<WebTeacherLang>().First(u => u.Lang == vm_ListAdd.Lang && u.Teacherid == model_Teacher.Teacherid);

				if (!string.IsNullOrEmpty(vm_ListAdd.Mobile) && _context.Queryable<WebTeacher>().Any(u=>u.MobileCode == vm_ListAdd.MobileCode && u.Mobile == vm_ListAdd.Mobile && u.Teacherid!=vm_ListAdd.Teacherid)) {
					return Content("<script>alert('老师的手机号不允许重复!'); history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				if (model_Teacher.TeacherCategoryid != vm_ListAdd.TeacherCategoryid)
				{
					message += ",将分类由[" + model_Teacher.TeacherCategoryid + "]改为[" + vm_ListAdd.TeacherCategoryid + "]";
				}
				if (model_Teacher.CountryCode != vm_ListAdd.CountryCode)
				{
					message += ",将国籍由[" + model_Teacher.CountryCode + "]改为[" + vm_ListAdd.CountryCode + "]";
				}
				if (model_Teacher.MobileCode != vm_ListAdd.MobileCode || (model_Teacher.Mobile+"").Replace(" ","") != (vm_ListAdd.Mobile+"").Replace(" ", ""))
				{
					model_Teacher.MenkeUserId = 0;//清0,后边逻辑会新建
					message += ",将老师手机由[" + model_Teacher.MobileCode + model_Teacher.Mobile + "]改为[" + vm_ListAdd.MobileCode + vm_ListAdd.Mobile.Replace(" ", "") + "]";
				}
				if (model_Teacher.Sort != vm_ListAdd.Sort)
				{
					message += ",将排序字段由[" + model_Teacher.Sort + "]改为[" + vm_ListAdd.Sort + "]";
				}
				if (model_Teacher.Sendtime != vm_ListAdd.Sendtime)
				{
					message += ",将发布时间由[" + model_Teacher.Sendtime + "]改为[" + vm_ListAdd.Sendtime + "]";
				}
				if (model_Teacher.Status != vm_ListAdd.Status)
				{
					message += ",将状态由[" + model_Teacher.Status + "]改为[" + vm_ListAdd.Status + "]";
				}
				if (model_TeacherLang?.Name != vm_ListAdd.Name)
				{
					message += ",将老师名称由[" + model_TeacherLang?.Name + "]改为[" + vm_ListAdd.Name + "]";
				}
				if (model_TeacherLang?.Keys != vm_ListAdd.Keys)
				{
					message += ",将标签由[" + model_TeacherLang?.Keys + "]改为[" + vm_ListAdd.Keys + "]";
				}
				if (model_TeacherLang?.Message != vm_ListAdd.Message)
				{
					message += ",更改过简介";
				}
				if (model_TeacherLang?.Comefrom != vm_ListAdd.Comefrom)
				{
					message += ",修改来自哪";
				}
				if (model_TeacherLang?.Edu != vm_ListAdd.Edu)
				{
					message += ",将教育由[" + model_TeacherLang?.Edu + "]改为[" + vm_ListAdd.Edu + "]";
				}
				if (model_TeacherLang?.Religion != vm_ListAdd.Religion)
				{
					message += ",将宗教信仰由[" + model_TeacherLang?.Religion + "]改为[" + vm_ListAdd.Religion + "]";
				}
				if (model_TeacherLang?.Motto != vm_ListAdd.Motto)
				{
					message += ",修改座右铭";
				}
				if (model_TeacherLang?.Philosophy != vm_ListAdd.Philosophy)
				{
					message += ",修改哲学修养";
				}
				if (model_TeacherLang?.Advantage != advantage)
				{
					message += ",修改数字标";
				}

				if (!string.IsNullOrEmpty(vm_ListAdd.Mobile))
				{
					if (model_Teacher.MenkeUserId == 0)
					{
						var result_menke = await _menkeService.CreateTeachers(new MenkeTeacherDto
						{
							name = vm_ListAdd.Name,
							sex = vm_ListAdd.Gender,
							birthday = vm_ListAdd.Birthdate.ToString("yyyy-MM-dd"),
							//email = vm_ListAdd.Email,
							locale = (vm_ListAdd.CountryCode + "").ToUpper(),
							code = vm_ListAdd.MobileCode,
							mobile = vm_ListAdd.Mobile.Replace(" ", "")
						}); ;
						if (result_menke.StatusCode == 0)
						{
							if (model_Teacher.MenkeUserId != result_menke.Data)
							{
								message += ",同步关联拓课云老师ID=[" + result_menke.Data + "]";
							}
							model_Teacher.MenkeUserId = result_menke.Data;
						}
					}
					else
					{
						var result_menke = await _menkeService.ModifyTeacher(vm_ListAdd.MobileCode, vm_ListAdd.Mobile.Replace(" ", ""), new MenkeTeacherDto
						{
							name = vm_ListAdd.Name,
							sex = vm_ListAdd.Gender,
							birthday = vm_ListAdd.Birthdate.ToString("yyyy-MM-dd")
						});
						if (result_menke.StatusCode != 0)
						{
							if (result_menke.Message.Contains("数据不存在")) model_Teacher.MenkeUserId = 0;
						}
					}
				}

				model_Teacher.Sty = vm_ListAdd.Sty;
				model_Teacher.TeacherCategoryid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "input_fid", vm_ListAdd.TeacherCategoryid);
				model_Teacher.MobileCode = vm_ListAdd.MobileCode;
				model_Teacher.Mobile = vm_ListAdd.Mobile.Replace(" ", "").Replace("-", "");
				model_Teacher.Email = vm_ListAdd.Email;
				model_Teacher.TeacherLang = vm_ListAdd.TeacherLang;
				model_Teacher.Timezoneid = vm_ListAdd.Timezoneid;
				model_Teacher.Sort = vm_ListAdd.Sort;
				model_Teacher.Sendtime = vm_ListAdd.Sendtime;
				model_Teacher.Status = vm_ListAdd.Status;
				model_Teacher.Masterid = model_master.AdminMasterid;
				model_Teacher.CountryCode = vm_ListAdd.CountryCode;
				model_Teacher.Gender = vm_ListAdd.Gender;
				model_Teacher.Birthdate = vm_ListAdd.Birthdate;
				if (model_TeacherLang != null)
				{
					model_TeacherLang.Name = vm_ListAdd.Name;
					model_TeacherLang.Keys = vm_ListAdd.Keys;
					model_TeacherLang.Message = vm_ListAdd.Message;
					model_TeacherLang.Comefrom = vm_ListAdd.Comefrom;
					model_TeacherLang.Edu = vm_ListAdd.Edu;
					model_TeacherLang.Religion = vm_ListAdd.Religion;
					model_TeacherLang.Advantage = advantage;
					model_TeacherLang.Motto = vm_ListAdd.Motto;
					model_TeacherLang.Philosophy = vm_ListAdd.Philosophy;
					_context.Updateable(model_TeacherLang).ExecuteCommand();
				}
				else{
					model_TeacherLang = new WebTeacherLang();
					model_TeacherLang.Lang = vm_ListAdd.Lang;
					model_TeacherLang.Teacherid = vm_ListAdd.Teacherid;
					model_TeacherLang.Name = vm_ListAdd.Name;
					model_TeacherLang.Keys = vm_ListAdd.Keys;
					model_TeacherLang.Message = vm_ListAdd.Message;
					model_TeacherLang.Comefrom = vm_ListAdd.Comefrom;
					model_TeacherLang.Edu = vm_ListAdd.Edu;
					model_TeacherLang.Religion = vm_ListAdd.Religion;
					model_TeacherLang.Advantage = advantage;
					model_TeacherLang.Motto = vm_ListAdd.Motto;
					model_TeacherLang.Philosophy = vm_ListAdd.Philosophy;
					_context.Insertable(model_TeacherLang).ExecuteCommand();
				}

				model_Teacher.Remark = message + "<hr>" + vm_ListAdd.Remark;
				#region "上传"
				if (chk_img == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Teacher.Img)) FileHelper.FileDel(ImgRoot + model_Teacher.Img);
					}
					catch { }
					model_Teacher.Img = "";
				}
				if (chk_photo == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Teacher.Photo)) FileHelper.FileDel(ImgRoot + model_Teacher.Photo);
					}
					catch { }
					model_Teacher.Photo = "";
				}
				if (chk_head_img == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Teacher.HeadImg)) FileHelper.FileDel(ImgRoot + model_Teacher.HeadImg);
					}
					catch { }
					model_Teacher.HeadImg = "";
				}
				if (files.Count > 0)
				{
					//缩略图
					var formFile = files.FirstOrDefault(u => u.Name == "input_img" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Teacher.Img)) FileHelper.FileDel(ImgRoot + model_Teacher.Img);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Teacher.Img = fileurl + filename;
						}
					}

					//照片图
					formFile = files.FirstOrDefault(u => u.Name == "input_photo" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Teacher.Photo)) FileHelper.FileDel(ImgRoot + model_Teacher.Photo);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Teacher.Photo = fileurl + filename;
						}
					}
					//头像图
					formFile = files.FirstOrDefault(u => u.Name == "input_head_img" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Teacher.HeadImg)) FileHelper.FileDel(ImgRoot + model_Teacher.HeadImg);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Teacher.HeadImg = fileurl + filename;
						}
					}
				}
				#endregion

				_context.Updateable(model_Teacher).ExecuteCommand();

				//20230106,老师改号码与排课没关系，排课换老师由计划任务来完成
				//string teacher_mobile = vm_ListAdd.MobileCode + vm_ListAdd.Mobile;
				//var list_MenkeLesson = _context.Queryable<MenkeLesson>().Where(u => SqlFunc.MergeString(u.MenkeTeacherCode, u.MenkeTeacherMobile) == teacher_mobile).ToList();
				//await _menkeService.UnionLessonUserid(list_MenkeLesson.Select(u=>u.MenkeCourseId).ToList());//修改完老师，维护关联信息

				return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?Teacherid=" + vm_ListAdd.Teacherid + "&lang=" + vm_ListAdd.Lang + "';</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				if (_context.Queryable<WebTeacher>().Any(u => u.MobileCode == vm_ListAdd.MobileCode && u.Mobile == vm_ListAdd.Mobile.Replace(" ", "")))
				{
					return Content("<script>alert('老师的手机号不允许重复!'); </script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				model_Teacher = new WebTeacher();
				model_Teacher.Sty = vm_ListAdd.Sty;
				model_Teacher.TeacherCategoryid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "input_fid", vm_ListAdd.TeacherCategoryid);
				model_Teacher.MobileCode = vm_ListAdd.MobileCode;
				model_Teacher.Mobile = vm_ListAdd.Mobile.Replace(" ", "");
				model_Teacher.Email = vm_ListAdd.Email;
				model_Teacher.TeacherLang = vm_ListAdd.TeacherLang;
				model_Teacher.Timezoneid = vm_ListAdd.Timezoneid;
				model_Teacher.Sort = vm_ListAdd.Sort;
				model_Teacher.Sendtime = vm_ListAdd.Sendtime;
				model_Teacher.Status = vm_ListAdd.Status;
				model_Teacher.Masterid = model_master.AdminMasterid;
				model_Teacher.CountryCode = vm_ListAdd.CountryCode;
				model_Teacher.Gender = vm_ListAdd.Gender;
				model_Teacher.Birthdate = vm_ListAdd.Birthdate;

				model_Teacher.Remark = message + "<hr>" + vm_ListAdd.Remark;
				#region "上传"
				if (files.Count > 0)
				{
					var formFile = files.FirstOrDefault(u => u.Name == "input_photo" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Teacher.Photo = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_img" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Teacher.Img = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_head_img" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Teacher.HeadImg = fileurl + filename;
						}
					}
				}
				#endregion

				if (!string.IsNullOrEmpty(vm_ListAdd.Mobile))
				{
					var result_menke = await _menkeService.CreateTeachers(new MenkeTeacherDto
					{
						name = vm_ListAdd.Name,
						sex = vm_ListAdd.Gender,
						birthday = vm_ListAdd.Birthdate.ToString("yyyy-MM-dd"),
						//email = vm_ListAdd.Email,
						locale = (vm_ListAdd.CountryCode + "").ToUpper(),
						code = vm_ListAdd.MobileCode,
						mobile = vm_ListAdd.Mobile.Replace(" ", "")
					});
					if (result_menke.StatusCode == 0) model_Teacher.MenkeUserId = result_menke.Data;
				}
				vm_ListAdd.Teacherid = _context.Insertable(model_Teacher).ExecuteReturnBigIdentity();

				var model_TeacherLang = new WebTeacherLang();
				model_TeacherLang.Lang = vm_ListAdd.Lang;
				model_TeacherLang.Teacherid = vm_ListAdd.Teacherid;
				model_TeacherLang.Name = vm_ListAdd.Name;
				model_TeacherLang.Keys = vm_ListAdd.Keys;
				model_TeacherLang.Message = vm_ListAdd.Message;
				model_TeacherLang.Comefrom = vm_ListAdd.Comefrom;
				model_TeacherLang.Edu = vm_ListAdd.Edu;
				model_TeacherLang.Religion = vm_ListAdd.Religion;
				model_TeacherLang.Advantage = advantage;
				model_TeacherLang.Motto = vm_ListAdd.Motto;
				model_TeacherLang.Philosophy = vm_ListAdd.Philosophy;
				_context.Insertable(model_TeacherLang).ExecuteCommand();
				return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?Teacherid=" + vm_ListAdd.Teacherid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
        #endregion

        #region "老师考勤表" 
        [Authorization(Power = "Main")]
        public IActionResult Attendance()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> AttendanceData(string keys = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string MenkeAttendanceids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "MenkeAttendanceids");
			long MenkeAttendanceid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "MenkeAttendanceid", 0);
			var list_Attendance = new List<MenkeAttendance>();
			if (MenkeAttendanceids.Trim() == "") MenkeAttendanceids = "0";
			string[] arr = MenkeAttendanceids.Split(',');
			var exp = new Expressionable<MenkeAttendance>();
			string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
			string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
			List<MenkeLesson> list_MenkeLesson;
			List<MenkeCourse> list_MenkeCourse;

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "export":
					exp.And(u => u.Status != -1 && u.MenkeAttendanceUserroleid == 7);
					exp.AndIF(!string.IsNullOrEmpty(keys), u =>
						SqlFunc.MergeString(u.MenkeAttendanceMobileCode, u.MenkeAttendanceMobile).Contains(keys) ||
						SqlFunc.Subqueryable<MenkeLesson>().Where(s => s.MenkeLessonId == u.MenkeLessonId && SqlFunc.MergeString(s.MenkeTeacherName, "").Contains(keys)).Any()
					);
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					if (!string.IsNullOrEmpty(begintime))
					{
						var btime = DateHelper.ConvertDateTimeInt(DateTime.Parse(begintime));
						exp.And(u => u.MenkeStarttime >= btime);
					}
					if (!string.IsNullOrEmpty(endtime))
					{
						var etime = DateHelper.ConvertDateTimeInt(DateTime.Parse(endtime));
						exp.And(u => u.MenkeEndtime <= etime);
					}

					var query_export = _context.Queryable<MenkeAttendance>().Where(exp.ToExpression()); 
					if (sort != "")
					{
						sort = StringHelper.ChgToUnderLine(sort);
						if (order == "desc")
						{
							query_export.OrderBy(sort + " desc");
						}
						else
						{
							query_export.OrderBy(sort);
						}
					}
					query_export.OrderBy("Menke_Starttime desc,Menke_Attendanceid desc");
					list_Attendance = query_export.ToList();
                    var list_Student1 = _context.Queryable<MenkeAttendance>().Where(u => list_Attendance.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId) && !string.IsNullOrEmpty(u.MenkeAttendanceAccessRecord) && u.Status != -1 && u.MenkeAttendanceUserroleid == 8)
                        .Select(u => new { u.MenkeLessonId, u.MenkeAttendanceAccessRecord, u.MenkeTimeinfo }).ToList(); 
					list_MenkeLesson = _context.Queryable<MenkeLesson>().Where(u => list_Attendance.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
					list_MenkeCourse = _context.Queryable<MenkeCourse>().Where(u => list_MenkeLesson.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();


					string fileurl = Appsettings.app("Web:Upfile") + "/Attendance/";
					FileHelper.AddFolder(Appsettings.app("Web:ImgRoot") + fileurl);
					string filename = fileurl + "" + Guid.NewGuid() + ".csv";
					var encoding = Lang == "zh-cn" ? Encoding.GetEncoding("GB2312") : Encoding.Default;
					CsvWriterHelper csv = new CsvWriterHelper(Appsettings.app("Web:ImgRoot") + filename, encoding); 
					csv[1, 1] = "ID";
					csv[1, 2] = _localizer["老师名称"];
					csv[1, 3] = _localizer["老师手机"];
					csv[1, 4] = _localizer["课程名称"];
					csv[1, 5] = _localizer["课节名称"];
					csv[1, 6] = _localizer["排课时间"];
					csv[1, 7] = _localizer["上下课时间"];
					csv[1, 8] = _localizer["进出教室时间"];
					csv[1, 9] = _localizer["老师有效时长"];
                    csv[1, 10] = _localizer["学生有效时长"];
                    csv[1, 11] = _localizer["迟到"];
					csv[1, 12] = _localizer["早退"];
					csv[1, 13] = _localizer["缺勤"];
					for (int i = 0; i < list_Attendance.Count; i++)
					{
						//var model_UserLesson = list_UserLesson.FirstOrDefault(u => u.MenkeLessonId == list_Attendance[i].MenkeLessonId);
						var model_MenkeLesson = list_MenkeLesson.FirstOrDefault(u => u.MenkeLessonId == list_Attendance[i].MenkeLessonId);
						var model_MenkeCoursse = list_MenkeCourse.FirstOrDefault(u => u.MenkeCourseId == model_MenkeLesson?.MenkeCourseId);

						//取老师进出教室与上下课交集
						var result_time = TimeIntersection(list_Attendance[i].MenkeAttendanceAccessRecord, list_Attendance[i].MenkeTimeinfo);
						string eotime = result_time.out_eotime != null ? string.Join("\r\n", result_time.out_eotime.Select(u => u.bengintime.ToString("yyyy-MM-dd HH:mm:ss") + " | " + u.endtime.ToString("HH:mm:ss"))) : "";
						string timeinfo = result_time.out_timeinfo != null ? string.Join("\r\n", result_time.out_timeinfo.Select(u => u.bengintime.ToString("yyyy-MM-dd HH:mm:ss") + " | " + u.endtime.ToString("HH:mm:ss"))) : "";
						var teacher_thetime = result_time.timeSpan;

                        var student_thetime = TimeSpan.Zero;
                        var list_StudentTime = list_Student1.Where(u => u.MenkeLessonId == list_Attendance[i].MenkeLessonId).ToList();
                        foreach (var model in list_StudentTime)
                        {
                            var result_student_time = TimeIntersection(model.MenkeAttendanceAccessRecord, model.MenkeTimeinfo);
                            if (result_student_time.timeSpan > student_thetime) student_thetime = result_student_time.timeSpan;
                        }
                        if (teacher_thetime > student_thetime) teacher_thetime = student_thetime;

                        csv[i + 2, 1] = list_Attendance[i].MenkeAttendanceid.ToString();
						csv[i + 2, 2] = JsonHelper.JsonCharFilter(model_MenkeLesson?.MenkeTeacherName + "").Replace("\"", "");
						csv[i + 2, 3] = list_Attendance[i].MenkeAttendanceMobileCode + "-" + list_Attendance[i].MenkeAttendanceMobile;
						csv[i + 2, 4] = (model_MenkeCoursse?.MenkeName + "").Replace("\"", "");
						csv[i + 2, 5] = (model_MenkeLesson?.MenkeName + "").Replace("\"", "");
						csv[i + 2, 6] = DateHelper.ConvertIntToDateTime(list_Attendance[i].MenkeStarttime.ToString()).ToLocalTime() + "\r\n" + DateHelper.ConvertIntToDateTime(list_Attendance[i].MenkeEndtime.ToString()).ToLocalTime();
						csv[i + 2, 7] = eotime;
						csv[i + 2, 8] = timeinfo;
                        csv[i + 2, 9] = teacher_thetime.ToString();
                        csv[i + 2, 10] = student_thetime.ToString();
                        csv[i + 2, 11] = (list_Attendance[i].MenkeAttendanceLate==1? _localizer["是"]:_localizer["否"]);
						csv[i + 2, 12] = (list_Attendance[i].MenkeAttendanceLeaveEarly == 1 ? _localizer["是"] : _localizer["否"]);
						csv[i + 2, 13] = (list_Attendance[i].MenkeAttendanceAbsent == 1 ? _localizer["是"] : _localizer["否"]);
					}
					csv.Save();
					if (list_Attendance.Count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"执行成功,导出" + list_Attendance.Count + "条！\",\"filename\":\"" + filename + "\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"执行失败！\",\"filename\":\"\"}");
					}
					break;
				default:
					int total = 0;
					exp.And(u => u.Status != -1 && u.MenkeAttendanceUserroleid==7);
					exp.AndIF(!string.IsNullOrEmpty(keys), u =>
						SqlFunc.MergeString(u.MenkeAttendanceMobileCode,u.MenkeAttendanceMobile).Contains(keys) ||
						SqlFunc.Subqueryable<MenkeLesson>().Where(s=>s.MenkeLessonId==u.MenkeLessonId && SqlFunc.MergeString(s.MenkeTeacherName,"").Contains(keys)).Any()
					);
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					if (!string.IsNullOrEmpty(begintime))
					{
						var btime = DateHelper.ConvertDateTimeInt(DateTime.Parse(begintime));
						exp.And(u => u.MenkeStarttime >= btime);
					}
					if (!string.IsNullOrEmpty(endtime))
					{
						var etime = DateHelper.ConvertDateTimeInt(DateTime.Parse(endtime));
						exp.And(u => u.MenkeEndtime <= etime);
					}
					var query = _context.Queryable<MenkeAttendance>().Where(exp.ToExpression());
					if (sort != "")
					{
						sort = sort.Replace("Menketime", "MenkeStarttime");
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
					query.OrderBy("Menke_Starttime desc,Menke_Attendanceid desc");
					list_Attendance = query.ToPageList(page, pagesize, ref total);
					var list_Student = _context.Queryable<MenkeAttendance>().Where(u => list_Attendance.Select(s=>s.MenkeLessonId).Contains(u.MenkeLessonId) &&!string.IsNullOrEmpty(u.MenkeAttendanceAccessRecord) && u.Status != -1 && u.MenkeAttendanceUserroleid == 8)
						.Select(u=>new { u.MenkeLessonId,u.MenkeAttendanceAccessRecord,u.MenkeTimeinfo }).ToList();
					list_MenkeLesson = _context.Queryable<MenkeLesson>().Where(u => list_Attendance.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
					list_MenkeCourse = _context.Queryable<MenkeCourse>().Where(u => list_MenkeLesson.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();
					
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Attendance.Count; i++)
					{
						var model_MenkeLesson = list_MenkeLesson.FirstOrDefault(u => u.MenkeLessonId == list_Attendance[i].MenkeLessonId);
						var model_MenkeCoursse = list_MenkeCourse.FirstOrDefault(u => u.MenkeCourseId == model_MenkeLesson?.MenkeCourseId);

                        //取老师进出教室与上下课交集
                        var result_teacher_time = TimeIntersection(list_Attendance[i].MenkeAttendanceAccessRecord, list_Attendance[i].MenkeTimeinfo);
                        string eotime = result_teacher_time.out_eotime != null ? string.Join("<br>", result_teacher_time.out_eotime.Select(u => u.bengintime.ToString("yyyy-MM-dd HH:mm:ss") + " | " + u.endtime.ToString("HH:mm:ss"))) : "";
						string timeinfo = result_teacher_time.out_timeinfo != null ? string.Join("<br>", result_teacher_time.out_timeinfo.Select(u => u.bengintime.ToString("yyyy-MM-dd HH:mm:ss") + " | " + u.endtime.ToString("HH:mm:ss"))) : "";
						var teacher_thetime = result_teacher_time.timeSpan;
						var student_thetime = TimeSpan.Zero;

						var list_StudentTime = list_Student.Where(u => u.MenkeLessonId == list_Attendance[i].MenkeLessonId).ToList();
						foreach (var model in list_StudentTime) { 
							var result_student_time = TimeIntersection(model.MenkeAttendanceAccessRecord, model.MenkeTimeinfo);
							if(result_student_time.timeSpan> student_thetime) student_thetime = result_student_time.timeSpan;//取呆的最久的学生时长
                        }

						if (teacher_thetime > student_thetime) teacher_thetime = student_thetime;

                        str.Append("{");
						str.Append("\"MenkeAttendanceid\":" + list_Attendance[i].MenkeAttendanceid + ",");
						str.Append("\"MenkeLessonId\":" + list_Attendance[i].MenkeLessonId + ",");
						str.Append("\"Name\":\"" + JsonHelper.JsonCharFilter(model_MenkeLesson?.MenkeTeacherName + "") + "\",");
						str.Append("\"Mobile\":\"" + list_Attendance[i].MenkeAttendanceMobileCode + "-" + list_Attendance[i].MenkeAttendanceMobile + "\",");
						str.Append("\"MenkeCourseName\":\"" + model_MenkeCoursse?.MenkeName + "\",");//menke_course
						str.Append("\"MenkeLessonName\":\"" + model_MenkeLesson?.MenkeName + "\",");//web_user_lesson,menke_lesson
						str.Append("\"Menketime\":\"" + DateHelper.ConvertIntToDateTime(list_Attendance[i].MenkeStarttime.ToString()).ToLocalTime() + "<br />" + DateHelper.ConvertIntToDateTime(list_Attendance[i].MenkeEndtime.ToString()).ToLocalTime() + "\",");
						str.Append("\"EOtime\":\"" + eotime + "\",");
						str.Append("\"Timeinfo\":\"" + timeinfo + "\",");
                        str.Append("\"TeacherThetime\":\"" + teacher_thetime.ToString() + "\",");
                        str.Append("\"StudentThetime\":\"" + student_thetime.ToString() + "\",");
                        str.Append("\"MenkeAttendanceLate\":" + list_Attendance[i].MenkeAttendanceLate + ",");
						str.Append("\"MenkeAttendanceLeaveEarly\":" + list_Attendance[i].MenkeAttendanceLeaveEarly + ",");
						str.Append("\"MenkeAttendanceAbsent\":" + list_Attendance[i].MenkeAttendanceAbsent + ",");
						str.Append("\"Status\":" + list_Attendance[i].Status + "}");
						if (i < (list_Attendance.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}

        /// <summary>
        /// 取两组数据的交集，并反回时分秒
        /// </summary>
        /// <param name="list_eotime"></param>
        /// <param name="list_timeinfo"></param>
        /// <returns>timeSpan：有效时长，out_eotime：进出教室时间，out_timeinfo：上下课时间</returns>
        private (TimeSpan timeSpan, List<Thetime> out_eotime, List<Thetime> out_timeinfo) TimeIntersection(string str_eotime, string str_timeinfo) {
			List<Thetime> list_eotime = new List<Thetime>();
			List<Thetime> list_timeinfo = new List<Thetime>();
			if (!string.IsNullOrEmpty(str_eotime)) list_eotime = JArray.Parse(str_eotime).Select(u => new Thetime() { btime = (int)u["entertime"], etime = (int)u["outtime"],bengintime= DateHelper.ConvertIntToDateTime(u["entertime"].ToString()).ToLocalTime(),endtime= DateHelper.ConvertIntToDateTime(u["outtime"].ToString()).ToLocalTime() }).ToList();
			if (!string.IsNullOrEmpty(str_timeinfo)) list_timeinfo = JArray.Parse(str_timeinfo).Select(u => new Thetime() { btime = (int)u["starttime"], etime = (int)u["endtime"], bengintime = DateHelper.ConvertIntToDateTime(u["starttime"].ToString()).ToLocalTime(), endtime = DateHelper.ConvertIntToDateTime(u["endtime"].ToString()).ToLocalTime() }).ToList();
			if (string.IsNullOrEmpty(str_eotime) || string.IsNullOrEmpty(str_timeinfo)) return (TimeSpan.Zero, list_eotime, list_timeinfo);
			TimeSpan timeSpan = new TimeSpan();
			foreach (var eotime in list_eotime) {
				foreach (var timeinfo in list_timeinfo)
				{
					//拿最小的etime-最大的btime,值大于0表示相交
					int min_etime = timeinfo.etime > eotime.etime ? eotime.etime : timeinfo.etime;
					int max_btime = timeinfo.btime < eotime.btime ? eotime.btime : timeinfo.btime;
					if ((min_etime - max_btime)>0)
					{
						timeSpan += TimeSpan.FromSeconds(min_etime - max_btime);
					}
					else { 
						//无并集
					}
				}
			}
			return (timeSpan, list_eotime, list_timeinfo);
        }
        #endregion
    }

    public class Thetime
	{
		public int btime { get; set; }
		public int etime { get; set; }
		public DateTime bengintime { get; set; }
		public DateTime endtime { get; set; }
	}
}