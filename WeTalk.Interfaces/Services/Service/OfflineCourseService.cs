using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Base;
using WeTalk.Models;
using WeTalk.Models.Dto.Common;
using WeTalk.Models.Dto.OfflineCourse;

namespace WeTalk.Interfaces.Services
{
	/// <summary>
	/// 在线线下课
	/// </summary>
	public partial class OfflineCourseService : BaseService, IOfflineCourseService
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly ILogger<OfflineCourseService> _logger;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包

		private readonly IUserManage _userManage;
		private readonly ICommonBaseService _commonBaseService;

        public OfflineCourseService(IHttpContextAccessor accessor, SqlSugarScope dbcontext, ILogger<OfflineCourseService> logger,IWebHostEnvironment env, IStringLocalizer<LangResource> localizer, IUserManage userManage, 
			ICommonBaseService commonBaseService)
		{
			_context = dbcontext;
			_accessor = accessor;
			_logger = logger;
			_localizer = localizer;
			_userManage = userManage;
			_commonBaseService = commonBaseService;

        }

		#region "获取线下课列表"
		/// <summary>
		/// 获取线下课列表
		/// </summary>
		/// <param name="onlineCategoryid">分类ID</param>
		/// <param name="key">搜索关键词</param>
		/// <param name="page">页码</param>
		/// <param name="pageSize">每页条数</param>
		/// <returns></returns>
		public async Task<ApiResult<Pages<CourseDto>>> CourseList(string key,int page = 1,int pageSize=10)
		{
			var result = new ApiResult<Pages<CourseDto>>();
			int total = 0;
			var list_Course = _context.Queryable<WebOfflineCourse>().LeftJoin<WebOfflineCourseLang>((l,r)=>l.OfflineCourseid == r.OfflineCourseid)
				.Where((l,r)=>l.Status==1 && r.Lang==_userManage.Lang)
				.WhereIF(!string.IsNullOrEmpty(key), (l,r) => r.Keys.Contains(key) || r.Title.Contains(key) || r.Message.Contains(key))
				.OrderBy((l, r) => l.Sort).OrderBy((l, r) => l.Sendtime, OrderByType.Desc)
				.Select((l,r)=>new { l.OfflineCourseid, l.Img,r.Title,r.Message, r.Keys,l.LessonCount,l.LessonStart,l.StudentCount,l.MarketPrice,l.Price })
				.ToPageList(page, pageSize, ref total);
			var list_Price = _context.Queryable<WebOfflineCoursePrice>()
				.Where(u =>u.CurrencyCode==_userManage.CurrencyCode && list_Course.Select(s => s.OfflineCourseid).Contains(u.OfflineCourseid))
				.ToList();
			result.Data = new Pages<CourseDto>();
			result.Data.Total = total;
            var model_Currency = _context.Queryable<PubCurrency>().First(u => u.CurrencyCode == _userManage.CurrencyCode);
            foreach (var model in list_Course)
			{
				var model_Price = list_Price.FirstOrDefault(u => u.OfflineCourseid == model.OfflineCourseid);
                var price = model_Price != null ? model_Price.Price : model.Price;
                var market_price = model_Price != null ? model_Price.MarketPrice : model.MarketPrice;
                result.Data.List.Add(new CourseDto()
				{
					OfflineCourseid = model.OfflineCourseid,
					Img = _commonBaseService.ResourceDomain(model.Img),
					Title = model.Title,
					Message = model.Message,
					Keys = !string.IsNullOrEmpty(model.Keys) ? model.Keys.Split(',').Where(u => !string.IsNullOrEmpty(u)).ToList() : new List<string>(),
					Price = price,
					MarketPrice=market_price,
					Ico = model_Currency?.Ico??"$",
					LessonCount = model.LessonCount,
					LessonStart = model.LessonStart,
					StudentCount=model.StudentCount
				});
			}
			return result;
		}
		#endregion

		#region "获取线下课详情"
		/// <summary>
		/// 获取线下课详情
		/// </summary>
		/// <param name="courseid">课程ID</param>
		/// <returns></returns>
		public async Task<ApiResult<CourseDetailDto>> CourseDetail(long offlineCourseid)
		{
			var result = new ApiResult<CourseDetailDto>();
			var model = new CourseDetailDto();
			var model_token = _userManage.GetUserToken();
			long userid = (model_token != null) ? model_token.Userid : 0;
			var list_Currery = _context.Queryable<PubCurrency>().ToList();
			var model_Course = _context.Queryable<WebOfflineCourse>().First(u => u.OfflineCourseid == offlineCourseid && u.Status == 1);
			if (model_Course != null)
			{
				var model_CourseLang = _context.Queryable<WebOfflineCourseLang>().First(u => u.OfflineCourseid == model_Course.OfflineCourseid && u.Lang == _userManage.Lang);
				model.OfflineCourseid = model_Course.OfflineCourseid;
				model.Title = model_CourseLang?.Title + "";
				model.Message = model_CourseLang?.Message + "";
				model.Address = model_CourseLang?.Address + "";
				model.Banner = _commonBaseService.ResourceDomain(model_Course.Banner + "");
				model.BannerH5 = _commonBaseService.ResourceDomain(model_Course.BannerH5 + "");
                model.Img = _commonBaseService.ResourceDomain(model_Course.Img + "");
                model.Keys = (model_CourseLang?.Keys + "").Split(",").Where(u => !string.IsNullOrEmpty(u)).ToList();
				model.LessonCount = model_Course.LessonCount;
				model.LessonStart= model_Course.LessonStart;
				model.StudentCount = model_Course.StudentCount;
				if (!string.IsNullOrEmpty(model_CourseLang?.Intro))
				{
					var o = JObject.Parse(model_CourseLang?.Intro);
					model.Intro = (o["data"] != null ? o["data"].ToString() : "");
				}
				model.CurrencyCode = _userManage.CurrencyCode;
				model.CurrencyIco = list_Currery.FirstOrDefault(u => u.CurrencyCode == _userManage.CurrencyCode)?.Ico ?? "$";
				var model_Price = _context.Queryable<WebOfflineCoursePrice>().First(u => u.OfflineCourseid == model_Course.OfflineCourseid && u.CurrencyCode==_userManage.CurrencyCode);
				if (model_Price != null)
				{
					model.MarketPrice = model_Price.MarketPrice;
					model.Price = model_Price.Price;
					model.Discount = (model.MarketPrice == 0) ? "1.0" : (10 * model.Price / model.MarketPrice).ToString("0.0");
				}
				else
                {
                    model.MarketPrice = model_Course.MarketPrice;
                    model.Price = model_Course.Price;
                    model.Discount = (model_Course.MarketPrice == 0) ? "1.0" : (10 * model_Course.Price / model_Course.MarketPrice).ToString("0.0");
                }
				result.Data = model;

				string ip = IpHelper.GetCurrentIp(_accessor.HttpContext);
				if (model_Course.Ip != ip || model_Course.Lasttime.AddMinutes(3) < DateTime.Now)
				{
					model_Course.Hits += 1;
					model_Course.Lasttime = DateTime.Now;
					model_Course.Ip = ip;
					_context.Updateable(model_Course).UpdateColumns(u => new { u.Hits, u.Lasttime, u.Ip }).ExecuteCommand();
				}
			}
			else
			{
				result.StatusCode = 4003;
				result.Message = _localizer["线下课程不存在"];
			}
			return result;
		}
		#endregion

		#region 加载确认线下课订单信息
		/// <summary>
		/// 加载确认直播订单信息
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<ConfirmOrderDto>> ConfirmOrder(long offlineCourseid)
		{
			var result = new ApiResult<ConfirmOrderDto>();
			var model_OfflineCourse = _context.Queryable<WebOfflineCourse>().LeftJoin<WebOfflineCourseLang>((l, r) => l.OfflineCourseid == r.OfflineCourseid && r.Lang == _userManage.Lang)
				.Where((l, r) => l.OfflineCourseid == offlineCourseid && l.Status == 1)
				.Select((l, r) => new { l.OfflineCourseid,  r.Title,r.Message,r.Intro,l.Img, l.LessonCount,l.LessonStart,l.StudentCount, l.Price, l.MarketPrice })
				.First();
			if (model_OfflineCourse == null)
			{
				result.StatusCode = 4009;
				result.Message = _localizer["所选课程不存在"];
				return result;
			}
			var currency_code = "";//展示币种
			var country_code = _userManage.GetCountryCode();//取IP定位的国家代码
			currency_code = _userManage.GetCurrencyCode(country_code);
			if (string.IsNullOrEmpty(currency_code))
			{
				result.StatusCode = 4002;
				result.Message = _localizer["客户端币种取值异常"];
				return result;
			}
			decimal market_price = 0.00M, price = 0.00M;
			var model_Price = _context.Queryable<WebOfflineCoursePrice>().First(u => u.CurrencyCode == currency_code && u.OfflineCourseid == offlineCourseid);
			if (model_Price != null)
			{
				market_price = model_Price.MarketPrice;
				price = model_Price.Price;
			}
			else
			{
				//美元
				market_price = model_OfflineCourse.MarketPrice;
				price = model_OfflineCourse.Price;
			}
			var model_Currency = _context.Queryable<PubCurrency>().First(u => u.CurrencyCode.ToUpper() == currency_code.ToUpper());
			var payTypes = new List<PayType>();
			var result_Paytype = await _commonBaseService.PayTypes();
			payTypes = (result_Paytype.StatusCode == 0) ? result_Paytype.Data : null;

			var intro = "";
			if (!string.IsNullOrEmpty(model_OfflineCourse.Intro)) {
				var o = JObject.Parse(model_OfflineCourse.Intro);
				intro = (o["data"] != null ? o["data"].ToString() : "");
			}
			result.Data = new ConfirmOrderDto()
			{
				OfflineCourseid = offlineCourseid,
				Img = _commonBaseService.ResourceDomain(model_OfflineCourse.Img),
				Title = model_OfflineCourse.Title,
				Message = model_OfflineCourse.Message,
				Intro= intro,
				LessonCount = model_OfflineCourse.LessonCount,
				LessonStart = model_OfflineCourse.LessonStart,
				StudentCount = model_OfflineCourse.StudentCount,
				Ico = model_Currency?.Ico + "",
				CurrencyCode = currency_code,
				MarketPrice = market_price,
				Price = price,
				PayTypes = payTypes
			};
			return result;
		}
		#endregion

		#region 创建线下课订单
		/// <summary>
		/// 创建订单
		/// </summary>
		/// <param name="offlineCourseid">线下课程Id</param>
		/// <returns></returns>
		public async Task<ApiResult<OrderDto>> CreateOrder(long offlineCourseid)
		{
			var result = new ApiResult<OrderDto>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			var model_OfflineCourse = _context.Queryable<WebOfflineCourse>().LeftJoin<WebOfflineCourseLang>((l, r) => l.OfflineCourseid == r.OfflineCourseid && r.Lang == _userManage.Lang)
				.Where((l, r) => l.OfflineCourseid == offlineCourseid && l.Status == 1)
				.Select((l, r) => new { l.OfflineCourseid, l.LessonCount,l.LessonStart,l.Img, l.Price, l.MarketPrice, r.Title,r.Message })
				.First();
			if (model_OfflineCourse == null)
			{
				result.StatusCode = 4009;
				result.Message = _localizer["所选课程不存在"];
				return result;
			}
			var currency_code = "";//展示币种
			var country_code = _userManage.GetCountryCode();//取IP定位的国家代码
			currency_code = _userManage.GetCurrencyCode(country_code);
			if (string.IsNullOrEmpty(currency_code))
			{
				result.StatusCode = 4002;
				result.Message = _localizer["客户端币种取值异常"];
				return result;
			}
			long online_course_priceid = 0;
			decimal market_price = 0.00M, price = 0.00M;
			var model_Price = _context.Queryable<WebOfflineCoursePrice>().First(u => u.CurrencyCode == currency_code && u.OfflineCourseid == offlineCourseid);
			if (model_Price != null)
			{
				market_price = model_Price.MarketPrice;
				price = model_Price.Price;
				online_course_priceid = model_Price.OfflineCoursePriceid;
			}
			else
			{
				//美元
				market_price = model_OfflineCourse.MarketPrice;
				price = model_OfflineCourse.Price;
				online_course_priceid = 0;
			}

			string order_sn = _commonBaseService.CreateOrderSn("A", _userManage.Userid, 0);
			var model_Order = new WebOrder();
			model_Order.Type = 3;
			model_Order.OfflineCourseid = offlineCourseid;
			model_Order.OrderSn = order_sn;
			model_Order.Userid = _userManage.Userid;
			model_Order.Title = model_OfflineCourse.Title;
			model_Order.Img = model_OfflineCourse.Img;
			model_Order.Message = model_OfflineCourse.Message;
			model_Order.MarketPrice = model_OfflineCourse.MarketPrice;
			model_Order.Amount = price;
			model_Order.Ip = IpHelper.GetCurrentIp(_accessor.HttpContext);
			model_Order.CurrencyCode = currency_code;
			model_Order.CountryCode = country_code;
			model_Order.ClassHour = 1;
			var orderid = _context.Insertable(model_Order).ExecuteReturnBigIdentity();
			result.Data = new OrderDto()
			{
				Orderid = orderid,
				OrderSn = order_sn
			};
			return result;
		}
		#endregion
	}
}