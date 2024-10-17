using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Base;
using WeTalk.Models;
using WeTalk.Models.Dto.Course;

namespace WeTalk.Interfaces.Services
{
	public partial class CourseService : BaseService, ICourseService
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly ILogger<CourseService> _logger;
		private readonly IWebHostEnvironment _env;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包

		private readonly IUserManage _userManage;
		private readonly ICommonBaseService _commonBaseService;
        private readonly ISobotBaseService _sobotBaseService;
		private readonly IMenkeBaseService _menkeBaseService;
		private readonly IPubConfigBaseService _pubConfigBaseService;
		private readonly ICourseBaseService _courseBaseService;

        public CourseService(IHttpContextAccessor accessor, SqlSugarScope dbcontext, ILogger<CourseService> logger,IWebHostEnvironment env, IStringLocalizer<LangResource> localizer, IUserManage userManage, 
			ICommonBaseService commonBaseService,ISobotBaseService sobotBaseService, IMenkeBaseService menkeBaseService, IPubConfigBaseService pubConfigBaseService, ICourseBaseService courseBaseService)
		{
			_context = dbcontext;
			_accessor = accessor;
			_logger = logger;
			_localizer = localizer;
			_env = env;
			_userManage = userManage;
			_commonBaseService = commonBaseService;
			_sobotBaseService = sobotBaseService;
			_menkeBaseService = menkeBaseService;
			_pubConfigBaseService = pubConfigBaseService;
			_courseBaseService = courseBaseService;

        }


		#region "获取课程列表"
		/// <summary>
		/// 获取课程列表
		/// </summary>
		/// <param name="key">搜索关键词</param>
		/// <param name="page">页码</param>
		/// <param name="pageSize">每页条数</param>
		/// <returns></returns>
		public async Task<ApiResult<Pages<CourseDto>>> CourseList(string key,int page = 1,int pageSize=10)
		{
			var result = new ApiResult<Pages<CourseDto>>();
			int total = 0;
			var list_Course = _context.Queryable<WebCourse>().LeftJoin<WebCourseLang>((l,r)=>l.Courseid==r.Courseid)
				.Where((l,r)=>l.Status==1 && r.Lang==_userManage.Lang)
				.WhereIF(!string.IsNullOrEmpty(key), (l,r) => SqlFunc.Subqueryable<PubKeysLang>().Where(s => s.Title.Contains(key) && SqlFunc.MergeString(",", l.Keysids, ",").Contains(SqlFunc.MergeString(",", s.Keysid.ToString(), ","))).Any()
			|| r.Title.Contains(key) || r.Message.Contains(key))
				.OrderBy((l, r) => l.Sort).OrderBy((l, r) => l.Sendtime, OrderByType.Desc)
				.Select((l,r)=>new { l.Courseid,l.Img,r.Title,r.Message, r.Keys})
				.ToPageList(page, pageSize, ref total);
			var list_CourseSku = _context.Queryable<WebCourseSku>()
				.LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid)
				.Where((l, r) => r.Lang == _userManage.Lang && list_Course.Select(s => s.Courseid).Contains(l.Courseid) && l.Status == 1)
				.Select((l, r) => new {l.SkuTypeid, l.CourseSkuid,l.Courseid,r.Title,l.Price})
				.ToList();
			var list_Price = _context.Queryable<WebCourseSkuPrice>()
				.Where(u =>u.CurrencyCode==_userManage.CurrencyCode && list_CourseSku.Select(s => s.CourseSkuid).Contains(u.CourseSkuid))
				.GroupBy(u=>u.Courseid)
				.Select(u=>new { u.Courseid,Price= SqlFunc.AggregateMin(u.Price)}).ToList();
			result.Data = new Pages<CourseDto>();
			result.Data.Total = total;
            var model_Currency = _context.Queryable<PubCurrency>().First(u => u.CurrencyCode == _userManage.CurrencyCode);
            foreach (var model in list_Course)
			{
				var model_SkuPrice = list_Price.FirstOrDefault(u => u.Courseid == model.Courseid);
				var model_CourseSku = list_CourseSku.Where(u => u.Courseid == model.Courseid).OrderBy(u=>u.Price).FirstOrDefault();
				var price = model_SkuPrice != null ? model_SkuPrice.Price : (model_CourseSku?.Price??0);
				result.Data.List.Add(new CourseDto()
                {
                    Courseid = model.Courseid,
					Img = _commonBaseService.ResourceDomain(model.Img),
                    Title = model.Title,
                    Message = model.Message,
                    Keys = !string.IsNullOrEmpty(model.Keys) ? model.Keys.Split(',').Where(u => !string.IsNullOrEmpty(u)).ToList() : new List<string>(),
                    MinPrice = price,
                    Ico = model_Currency?.Ico,
                    SkuTypes = list_CourseSku.Where(u => u.Courseid == model.Courseid).GroupBy(u=>new { u.SkuTypeid,u.Title}).Select(u => new CourseDto.SkuType { SkuTypeid = u.Key.SkuTypeid,Type = u.Key.Title }).ToList()
                });
			}
			return result;
		}
		#endregion

		#region "获取课程详情"
		/// <summary>
		/// 获取课程列表
		/// </summary>
		/// <param name="courseid">课程ID</param>
		/// <returns></returns>
		public async Task<ApiResult<CourseDetailDto>> CourseDetail(long courseid)
		{
			var result = new ApiResult<CourseDetailDto>();
			var model = new CourseDetailDto();
			var model_token = _userManage.GetUserToken();
			long userid = (model_token != null) ? model_token.Userid : 0;
			double age = (model_token != null) ? Math.Ceiling((DateTime.Now - model_token.Birthdate).TotalDays / 365) : 0;

            var list_Currery = _context.Queryable<PubCurrency>().ToList();
            var model_Course = _context.Queryable<WebCourse>().First(u => u.Courseid == courseid && u.Status == 1);
			if(model_Course!=null)
			{
				var model_CourseLang = _context.Queryable<WebCourseLang>().First(u => u.Courseid == model_Course.Courseid && u.Lang == _userManage.Lang);
				model.Courseid = model_Course.Courseid;
				model.Title = model_CourseLang?.Title+"";
				model.Message = model_CourseLang?.Message + "";
				model.Banner = _commonBaseService.ResourceDomain(model_Course.Banner + "");
                model.BannerH5 = _commonBaseService.ResourceDomain(model_Course.BannerH5 + "");
                model.Curricula = model_CourseLang?.Curricula;
				model.Keys = (model_CourseLang?.Keys+"").Split(",").Where(u=>!string.IsNullOrEmpty(u)).ToList();
				model.CurrencyCode = _userManage.CurrencyCode;
				model.CurrencyIco = list_Currery.FirstOrDefault(u => u.CurrencyCode == _userManage.CurrencyCode)?.Ico ?? "$";
                var list_Sku = _context.Queryable<WebCourseSku>()
					.LeftJoin<WebSkuType>((l, m) => l.SkuTypeid == m.SkuTypeid)
					.LeftJoin<WebSkuTypeLang>((l, m, r) => l.SkuTypeid == r.SkuTypeid && r.Lang == _userManage.Lang)
					.Where((l, m, r) => l.Courseid == model_Course.Courseid && l.Status == 1)
					.WhereIF(userid > 0, (l, m, r) => (m.AgeMax >= age && m.AgeMin <= age) || (m.AgeMin == 0 && m.AgeMax == 0))
					.OrderBy((l, m, r) => new { l.Sort,l.SkuTypeid,l.ClassHour})
					.Select((l, m, r) => new
					{
						l.CourseSkuid,
						l.SkuTypeid,
						Type = r.Title,
						l.ClassHour,
						l.MarketPrice,
						l.Price
					})
					.ToList();
				var list_SkuPrice = _context.Queryable<WebCourseSkuPrice>().Where(u => list_Sku.Select(s => s.CourseSkuid).Contains(u.CourseSkuid) && u.CurrencyCode == _userManage.CurrencyCode).ToList();
				foreach (var sku in list_Sku)
                {
					var model_Price = list_SkuPrice.FirstOrDefault(u => u.CourseSkuid == sku.CourseSkuid);
					var market_price = model_Price != null ? model_Price.MarketPrice : sku.MarketPrice;
                    var price = model_Price != null ? model_Price.Price : sku.Price;
                    model.Skus.Add(new CourseDetailDto.CourseSku
					{
						CourseSkuid = sku.CourseSkuid,
						SkuTypeid = sku.SkuTypeid,
						Type = sku.Type,
						ClassHour = sku.ClassHour,
						Price = price,
						MarketPrice = market_price,
						Discount = (market_price==0)?"1.0":(10 * price / market_price).ToString("0.0")
					});
                }

                var list_Group = _context.Queryable<WebCourseGroup>()
					.LeftJoin<WebCourseGroupLang>((l,r)=>l.CourseGroupid==r.CourseGroupid)
					.Where((l,r) => l.Courseid == model_Course.Courseid && l.Status == 1 && r.Lang==_userManage.Lang)
					.OrderBy((l, r) => l.Sort)
					.OrderBy((l, r) => l.CourseGroupid, OrderByType.Desc)
					.Select((l,r)=>new {l.CourseGroupid,r.GroupName })
					.ToList();
				var list_GroupInfo = _context.Queryable<WebCourseGroupInfo>()
					.LeftJoin<WebCourseGroupInfoLang>((l,r)=>l.CourseGroupInfoid==r.CourseGroupInfoid)
					.Where((l, r) => list_Group.Select(s=>s.CourseGroupid).Contains(l.CourseGroupid) && l.Status == 1 && r.Lang == _userManage.Lang)
					.OrderBy((l, r) =>l.Sort)
					.OrderBy((l,r)=>l.CourseGroupInfoid,OrderByType.Desc)
					.Select((l, r) => new { l.CourseGroupid, l.CourseGroupInfoid, l.Img, r.Title,r.Message,r.Keys,l.Level })
					.ToList();
				foreach (var model_Group in list_Group) {
					var list_GroupInfos = list_GroupInfo.Where(u => u.CourseGroupid == model_Group.CourseGroupid).Select(u=>new{
                        CourseGroupInfoid = u.CourseGroupInfoid,
                        CourseGroupid = u.CourseGroupid,
                        Title = u.Title,
						Img = u.Img,
						Message = u.Message+"",
						Keys = u.Keys+"",
                        Level = u.Level+""
                    }).ToList();
					var groupInfos = new List<CourseDetailDto.CourseGroupInfo>();
					foreach (var item in list_GroupInfos) {
						groupInfos.Add(new CourseDetailDto.CourseGroupInfo()
						{
                            CourseGroupInfoid=item.CourseGroupInfoid,
							CourseGroupid=item.CourseGroupid,
							Title= item.Title,
							Message = item.Message,
							Img=_commonBaseService.ResourceDomain(item.Img),
							Keys= (item.Keys + "").Split(',').Where(u => !string.IsNullOrEmpty(u)).ToList(),
                            Level= item.Level
                        });
                    }
                    model.Groups.Add(new CourseDetailDto.CourseGroup()
					{
						CourseGroupid = model_Group.CourseGroupid,
						GroupName = model_Group.GroupName,
						GroupInfos = groupInfos
					});
				}
				result.Data = model;
			}
			else {
				result.StatusCode = 4003;
				result.Message = _localizer["课程不存在"];
			}
			return result;
		}
		#endregion

		#region "获取课程SKU价格"
		/// <summary>
		/// 获取课程SKU价格
		/// </summary>
		/// <param name="courseSkuid">课程SkuID</param>
		/// <returns></returns>
		public async Task<ApiResult<CourseSkuPriceDto>> CourseSkuPrice(long courseSkuid)
		{
			var result = new ApiResult<CourseSkuPriceDto>();
			var model = new CourseSkuPriceDto();
			var list_Currery = _context.Queryable<PubCurrency>().ToList();
			var model_CourseSkuPrice = _context.Queryable<WebCourseSkuPrice>().First(u => u.CourseSkuid == courseSkuid && u.CurrencyCode == _userManage.CurrencyCode);
			if (model_CourseSkuPrice != null)
			{
				model.CourseSkuid = courseSkuid;
				model.MarketPrice = model_CourseSkuPrice.MarketPrice;
				model.Price = model_CourseSkuPrice.Price;
				model.CurrencyCode = model_CourseSkuPrice.CurrencyCode;
				model.CurrencyIco = list_Currery.FirstOrDefault(u => u.CurrencyCode == model_CourseSkuPrice.CurrencyCode)?.Ico;

                result.Data = model;
			}
			else
			{
				var model_CouserSku = _context.Queryable<WebCourseSku>().InSingle(courseSkuid);
				if (model_CouserSku != null)
				{
					model.CourseSkuid = courseSkuid;
					model.MarketPrice = model_CouserSku.MarketPrice;
					model.Price = model_CouserSku.Price;
					model.CurrencyCode = "USD";
                    model.CurrencyIco = "$";
                    result.Data = model;
				}
				else
				{
					result.StatusCode = 4003;
					result.Message = _localizer["课程不存在"];
				}
			}
			return result;
		}
		#endregion

		#region "获取子课程详情"
		/// <summary>
		/// 获取子课程详情
		/// </summary>
		/// <param name="courseGroupInfoid">子课程ID</param>
		/// <returns></returns>
		public async Task<ApiResult<SubCourseDetailDto>> SubCourseDetail(long courseGroupInfoid)
		{
			var result = new ApiResult<SubCourseDetailDto>();
			var model = new SubCourseDetailDto();
			var model_CourseGroupInfo = _context.Queryable<WebCourseGroupInfo>().First(u => u.CourseGroupInfoid == courseGroupInfoid && u.Status == 1);
			if (model_CourseGroupInfo != null)
			{
				var model_CourseGroupInfoLang = _context.Queryable<WebCourseGroupInfoLang>().First(u => u.CourseGroupInfoid == courseGroupInfoid && u.Lang == _userManage.Lang);
				model.CourseGroupInfoid = model_CourseGroupInfo.CourseGroupInfoid;
				model.Courseid = model_CourseGroupInfo.Courseid;
				model.Img = _commonBaseService.ResourceDomain(model_CourseGroupInfo.Img);
				if (model_CourseGroupInfoLang != null)
				{
					model.Title = model_CourseGroupInfoLang.Title;
					model.Message = model_CourseGroupInfoLang.Message;
					model.Keys = (model_CourseGroupInfoLang.Keys + "").Split(",").Where(u=>!string.IsNullOrEmpty(u)).ToList();
					JObject o = null;
					try
					{
						o = JObject.Parse(model_CourseGroupInfoLang.Intro);
                        model.Intro = o["data"].ToString();
					}
					catch { }
                    try
                    {
                        o = JObject.Parse(model_CourseGroupInfoLang.Objectives);
                        model.Objectives = o["data"].ToString();
                    }
                    catch { }
                    try
                    {
                        o = JObject.Parse(model_CourseGroupInfoLang.Crowd);
                        model.Crowd = o["data"].ToString();
                    }
                    catch { }
                    try
                    {
                        o = JObject.Parse(model_CourseGroupInfoLang.Merit);
                        model.Merit = o["data"].ToString();
                    }
                    catch { }
                    try
                    {
                        o = JObject.Parse(model_CourseGroupInfoLang.Catalog);
                        model.Catalog = o["data"].ToString();
                    }
                    catch { }
				}
				result.Data = model;
			}
			else
			{
				result.StatusCode = 4003;
				result.Message = _localizer["课程不存在"];
			}
			return result;
		}
		#endregion

		#region "指定课程咨询"
		/// <summary>
		/// 指定课程咨询
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult> CourseMessage(long courseGroupInfoid, string mobileCode, string mobile, string email)
		{
			var result = new ApiResult();
			var str = new StringBuilder();
			var model_Apply = new WebCourseApply();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid>0)
			{
				model_Apply.Userid = _userManage.Userid;
			}
			var model_CourseGroupInfo = _context.Queryable<WebCourseGroupInfo>().LeftJoin<WebCourseGroupInfoLang>((l,r)=>l.CourseGroupInfoid==r.CourseGroupInfoid)
				.Where((l,r)=>l.CourseGroupInfoid== courseGroupInfoid && r.Lang==_userManage.Lang)
				.Select((l,r)=>new { l.CourseGroupInfoid,r.Title,l.Courseid})
				.First();
            mobile = mobile.Replace("-", "").Replace(" ", "");

            long courseid = model_CourseGroupInfo!=null? model_CourseGroupInfo.Courseid:0;
			model_Apply.Sty = 1;
			model_Apply.Courseid = courseid;
			model_Apply.CourseGroupInfoid = courseGroupInfoid;
			model_Apply.ContactMobileCode = mobileCode;
			model_Apply.ContactMobile = mobile;
			model_Apply.ContactEmail = email;
			model_Apply.Ip = IpHelper.GetCurrentIp(_accessor.HttpContext);
			model_Apply.Status = 0;
			model_Apply.Remark = $"[{DateTime.Now}]用户提交咨询表";
			model_Apply.Message = "";

			//创建工单提醒
			var dic_data = new Dictionary<string, object>();
			var model_Course = _context.Queryable<WebCourseLang>().First(u=>u.Courseid==courseid && u.Lang==_userManage.Lang);
			var model_User = _context.Queryable<WebUser>().First(u => u.Status != -1 && u.Mobile == mobile && u.MobileCode == mobileCode);
			dic_data.Add(_localizer["所选课程"], model_Course?.Title+" "+ model_CourseGroupInfo?.Title);
			dic_data.Add(_localizer["手机"], mobileCode + "-" + mobile);
			dic_data.Add(_localizer["邮箱"], email);
			if (model_User != null)
            {
                model_Apply.Userid = model_User.Userid;
                if (model_User.Status == 1)
				{
					dic_data.Add(_localizer["姓名"], model_User.FirstName + " " + model_User.LastName);
					dic_data.Add(_localizer["出生日期"], model_User.Birthdate.ToString("d"));
					dic_data.Add(_localizer["备注"], _localizer["已注册正式用户"]);
				}
				else
				{
					dic_data.Add(_localizer["姓名"], model_User.FirstName + " " + model_User.LastName);
					dic_data.Add(_localizer["备注"], _localizer["用户已注册第一步,可引导用户在网站完成第二步注册"]);
				}
			}
			else
			{
				dic_data.Add(_localizer["备注"], _localizer["非注册用户,可引导用户在网站注册"]);
			}
			var body = string.Join("<br>", dic_data.ToList());
			var result_ticket = await _sobotBaseService.AddUserTicket(_localizer["前台课程咨询"], body);
			if (result_ticket.StatusCode == 0)
			{
				model_Apply.IsTicket = 1;
				model_Apply.TicketTime = DateTime.Now;
				model_Apply.TicketId = result_ticket.Data;
			}
			else
			{
				_logger.LogError(_localizer["前台课程咨询"] + "," + result_ticket.Message);
			}
			_context.Insertable(model_Apply).ExecuteCommand();
			return result;
		}
		#endregion

		#region "试听课程申请"
		/// <summary>
		/// 试听课程申请
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult> TrialCourseApply(string courseName, string mobileCode, string mobile, string email)
		{
			var result = new ApiResult();
			var str = new StringBuilder();
			mobile = mobile.Replace("-", "").Replace(" ", "");

			if (_context.Queryable<WebCourseApply>().Where(u => u.Sty == 0 && u.ContactMobileCode == mobileCode && u.ContactMobile == mobile && u.Status != -1).Count() > 0)
				return new ApiResult()
				{
					StatusCode = 4000,
					Message = _localizer["每个用户只能提交一次试听,如需多次申请，请联系在线客服"]
				};
			var model_Apply = new WebCourseApply();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid > 0)
			{
				model_Apply.Userid = _userManage.Userid;
			}
			model_Apply.Sty = 0;
			model_Apply.CourseName = courseName;
			model_Apply.ContactMobileCode = mobileCode;
			model_Apply.ContactMobile = mobile;
			model_Apply.ContactEmail = email;
			model_Apply.Ip = IpHelper.GetCurrentIp(_accessor.HttpContext);
			model_Apply.Status = 0;
			model_Apply.Remark = $"[{DateTime.Now}]用户提交申请表";
			model_Apply.Message = "";

			//创建工单提醒
			var dic_data = new Dictionary<string, object>();
			var model_User = _context.Queryable<WebUser>().First(u => u.Status != -1 && u.Mobile == mobile && u.MobileCode == mobileCode);
			if (model_User != null)
            {
				model_Apply.Userid = model_User.Userid;

                dic_data.Add(_localizer["姓名"], model_User.FirstName + " " + model_User.LastName);
                dic_data.Add(_localizer["手机"], mobileCode + "-" + mobile);
                dic_data.Add(_localizer["邮箱"], email);
                dic_data.Add(_localizer["所在地时区"], model_User.Utc);
                if (model_User.Status == 1)
                {
                    var model_Native = _context.Queryable<PubCountry>().First(u => u.Code.ToLower() == model_User.Native.ToLower());
                    dic_data.Add(_localizer["母语"], _userManage.Lang == "zh-cn" ? model_Native?.Country : model_Native?.CountryEn);
                    dic_data.Add(_localizer["出生日期"], model_User.Birthdate.ToString("d"));
                    dic_data.Add(_localizer["性别"], model_User.Gender == 0 ? _localizer["女"] : _localizer["男"]);
                    dic_data.Add(_localizer["备注"], _localizer["请在[拓课云]后台的课程[通用试听课程(绝对不允许删除)]下创建试听课节"]);
				}
				else
				{
					dic_data.Add(_localizer["姓名"], model_User.FirstName + " " + model_User.LastName);
					dic_data.Add(_localizer["备注"], _localizer["用户已注册第一步,可引导用户在网站完成第二步注册,并在[拓课云]后台的课程[通用试听课程]下创建试听课节"]);
				}
			}
			else
            {
                dic_data.Add(_localizer["手机"], mobileCode + "-" + mobile);
                dic_data.Add(_localizer["邮箱"], email);
                dic_data.Add(_localizer["备注"], _localizer["非注册用户,可引导用户在网站注册,在[拓课云]后台的课程[通用试听课程]下创建试听课节"]);
			}
			var body = string.Join("<br>", dic_data.ToList());
			var result_ticket = await _sobotBaseService.AddUserTicket(_localizer["前台试听课申请"], body);
			if (result_ticket.StatusCode == 0)
			{
				model_Apply.IsTicket = 1;
				model_Apply.TicketTime = DateTime.Now;
				model_Apply.TicketId = result_ticket.Data;
			}
			else
			{
				_logger.LogError(_localizer["前台试听课申请"] + "," + result_ticket.Message);
			}
			_context.Insertable(model_Apply).ExecuteCommand();
			return result;
		}
		#endregion

		#region 已购课程自动安排课程
		/// <summary>
		/// 已购课程自动安排课程
		/// </summary>
		/// <param name="user_courseid">已购课程ID</param>
		/// <returns></returns>
		public async Task<ApiResult<int>> UserCourseArranging(long user_courseid)
		{
			return await _courseBaseService.UserCourseArranging(user_courseid);
		}
		public async Task<ApiResult<int>> UserCourseArranging(WebUserCourse model_UserCourse)
		{
			return await _courseBaseService.UserCourseArranging(model_UserCourse);
		}
		#endregion

		#region 锁定/解锁已购课程
		/// <summary>
		/// 锁定已购课程（删除未上课节）
		/// </summary>
		/// <param name="user_courseid">已购课程ID</param>
		/// <returns></returns>
		public async Task<ApiResult> LockUserCourse(long user_courseid)
		{
			return await _courseBaseService.LockUserCourse(user_courseid);
		}

		/// <summary>
		/// 解锁已购课程（不恢复课节）
		/// </summary>
		/// <param name="user_courseid"></param>
		/// <returns></returns>
		public async Task<ApiResult> UnLockUserCourse(long user_courseid)
		{
            return await _courseBaseService.UnLockUserCourse(user_courseid);

        }
		#endregion

		#region 删除已购课程
		/// <summary>
		/// 删除已购课程
		/// </summary>
		/// <param name="user_courseid">已购课程ID</param>
		/// <returns></returns>
		public async Task<ApiResult> DelUserCourse(long user_courseid)
		{
            return await _courseBaseService.DelUserCourse(user_courseid);

        }
		#endregion

		#region 调整已购课程
		/// <summary>
		/// 调整已购课程
		/// </summary>
		/// <param name="user_courseid">已购课程ID</param>
		/// <param name="menke_courseid">要调整的新课程ID</param>
		/// <returns></returns>
		public async Task<ApiResult> AdjustUserCourse(long user_courseid,int menke_courseid)
		{
            return await _courseBaseService.AdjustUserCourse(user_courseid, menke_courseid);

        }
		#endregion
	}
}