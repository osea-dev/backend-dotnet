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
using WeTalk.Models.Dto.RecordCourse;

namespace WeTalk.Interfaces.Services
{
	/// <summary>
	/// 录播课
	/// </summary>
	public partial class RecordCourseService : BaseService, IRecordCourseService
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly ILogger<RecordCourseService> _logger;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包

		private readonly IUserManage _userManage;
		private readonly ICommonBaseService _commonBaseService;

        public RecordCourseService(IHttpContextAccessor accessor, SqlSugarScope dbcontext, ILogger<RecordCourseService> logger,IWebHostEnvironment env, IStringLocalizer<LangResource> localizer, IUserManage userManage, 
			ICommonBaseService commonBaseService)
		{
			_context = dbcontext;
			_accessor = accessor;
			_logger = logger;
			_localizer = localizer;
			_userManage = userManage;
			_commonBaseService = commonBaseService;
        }

		#region "获取录播课列表"
		/// <summary>
		/// 获取录播课列表
		/// </summary>
		/// <param name="onlineCategoryid">分类ID</param>
		/// <param name="key">搜索关键词</param>
		/// <param name="page">页码</param>
		/// <param name="pageSize">每页条数</param>
		/// <returns></returns>
		public async Task<ApiResult<Pages<CourseDto>>> CourseList(string key, int page = 1, int pageSize = 10)
		{
			var result = new ApiResult<Pages<CourseDto>>();
			int total = 0;
			var list_Course = _context.Queryable<WebRecordCourse>().LeftJoin<WebRecordCourseLang>((l, r) => l.RecordCourseid == r.RecordCourseid)
				.Where((l, r) => l.Status == 1 && r.Lang == _userManage.Lang)
				.WhereIF(!string.IsNullOrEmpty(key), (l, r) => r.Keys.Contains(key) || r.Title.Contains(key) || r.Message.Contains(key))
				.OrderBy((l, r) => l.Sort).OrderBy((l, r) => l.Sendtime, OrderByType.Desc)
				.Select((l, r) => new { l.RecordCourseid, l.Img, r.Title, r.Message, r.Keys, l.LessonCount, l.StudentCount,l.Price,l.MarketPrice })
				.ToPageList(page, pageSize, ref total);
			var list_Price = _context.Queryable<WebRecordCoursePrice>()
				.Where(u => u.CurrencyCode == _userManage.CurrencyCode && list_Course.Select(s => s.RecordCourseid).Contains(u.RecordCourseid))
				.ToList();
			result.Data = new Pages<CourseDto>();
			result.Data.Total = total;
			var model_Currency = _context.Queryable<PubCurrency>().First(u => u.CurrencyCode == _userManage.CurrencyCode);
			foreach (var model in list_Course)
			{
				var model_Price = list_Price.FirstOrDefault(u => u.RecordCourseid == model.RecordCourseid);
				var price = model_Price != null ? model_Price.Price : model.Price;
                var market_price = model_Price != null ? model_Price.MarketPrice : model.MarketPrice;
                result.Data.List.Add(new CourseDto()
				{
					RecordCourseid = model.RecordCourseid,
					Img = _commonBaseService.ResourceDomain(model.Img),
					Title = model.Title,
					Message = model.Message,
					Keys = !string.IsNullOrEmpty(model.Keys) ? model.Keys.Split(',').Where(u => !string.IsNullOrEmpty(u)).ToList() : new List<string>(),
					Price = price,
                    MarketPrice = market_price,
                    Ico = model_Currency?.Ico??"$",
					LessonCount = model.LessonCount,
					StudentCount = model.StudentCount
				});
			}
			return result;
		}
		#endregion

		#region "获取录播课详情"
		/// <summary>
		/// 获取录播课详情
		/// </summary>
		/// <param name="courseid">课程ID</param>
		/// <returns></returns>
		public async Task<ApiResult<CourseDetailDto>> CourseDetail(long RecordCourseid)
		{
			var result = new ApiResult<CourseDetailDto>();
			var model = new CourseDetailDto();
			var model_token = _userManage.GetUserToken();
			long userid = (model_token != null) ? model_token.Userid : 0;
			var list_Currery = _context.Queryable<PubCurrency>().ToList();
			var model_Course = _context.Queryable<WebRecordCourse>().First(u => u.RecordCourseid == RecordCourseid && u.Status == 1);
			if (model_Course != null)
			{
				var model_CourseLang = _context.Queryable<WebRecordCourseLang>().First(u => u.RecordCourseid == model_Course.RecordCourseid && u.Lang == _userManage.Lang);
				model.RecordCourseid = model_Course.RecordCourseid;
				model.Title = model_CourseLang?.Title + "";
				model.Message = model_CourseLang?.Message + "";
				model.Banner = _commonBaseService.ResourceDomain(model_Course.Banner + "");
				model.BannerH5 = _commonBaseService.ResourceDomain(model_Course.BannerH5 + "");
				model.Img = _commonBaseService.ResourceDomain(model_Course.Img + "");
				model.Keys = (model_CourseLang?.Keys + "").Split(",").Where(u => !string.IsNullOrEmpty(u)).ToList();
				model.LessonCount = model_Course.LessonCount;
				model.StudentCount = model_Course.StudentCount;
				if (!string.IsNullOrEmpty(model_CourseLang?.IntroUp))
				{

					var o = JObject.Parse(model_CourseLang?.IntroUp);
					model.IntroUp = (o["data"] != null ? o["data"].ToString() : "");
				}
				if (!string.IsNullOrEmpty(model_CourseLang?.IntroLow))
				{

					var o = JObject.Parse(model_CourseLang?.IntroLow);
					model.IntroLow = (o["data"] != null ? o["data"].ToString() : "");
				}
				var list_Info = _context.Queryable<WebRecordCourseInfo>().LeftJoin<WebRecordCourseInfoLang>((l, r) => l.RecordCourseInfoid == r.RecordCourseInfoid && r.Lang == _userManage.Lang)
					.Where((l, r) => l.Status == 1 && l.RecordCourseid == RecordCourseid)
					.OrderBy((l, r) => l.Sort)
					.OrderBy((l, r) => l.RecordCourseInfoid, OrderByType.Desc)
					.Select((l, r) => new { l.Duration, r.Title, l.ViewCount })
					.ToList();
				foreach (var model_info in list_Info)
				{
					model.Videos.Add(new CourseDetailDto.Info
					{
						Title = model_info.Title,
						Duration= model_info.Duration,
						ViewCount= model_info.ViewCount
					});
				}
				model.CurrencyCode = _userManage.CurrencyCode;
				model.CurrencyIco = list_Currery.FirstOrDefault(u => u.CurrencyCode == _userManage.CurrencyCode)?.Ico ?? "$";
				var model_Price = _context.Queryable<WebRecordCoursePrice>().First(u => u.RecordCourseid == model_Course.RecordCourseid);
				if (model_Price != null)
				{
					model.MarketPrice = model_Price.MarketPrice;
					model.Price = model_Price.Price;
					model.Discount = (model.MarketPrice == 0) ? "1.0" : (10 * model.Price / model.MarketPrice).ToString("0.0");
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
				result.Message = _localizer["录播课程不存在"];
			}
			return result;
		}
		#endregion

		#region 加载确认录播课订单信息
		/// <summary>
		/// 加载确认直播订单信息
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<ConfirmOrderDto>> ConfirmOrder(long recordCourseid)
		{
			var result = new ApiResult<ConfirmOrderDto>();
			var model_RecordCourse = _context.Queryable<WebRecordCourse>().LeftJoin<WebRecordCourseLang>((l, r) => l.RecordCourseid == r.RecordCourseid && r.Lang == _userManage.Lang)
				.Where((l, r) => l.RecordCourseid == recordCourseid && l.Status == 1)
				.Select((l, r) => new { l.RecordCourseid, r.Title, r.Message, r.IntroUp,r.IntroLow, l.Img, l.LessonCount, l.StudentCount, l.Price, l.MarketPrice })
				.First();
			if (model_RecordCourse == null)
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
			var model_Price = _context.Queryable<WebRecordCoursePrice>().First(u => u.CurrencyCode == currency_code && u.RecordCourseid == recordCourseid);
			if (model_Price != null)
			{
				market_price = model_Price.MarketPrice;
				price = model_Price.Price;
			}
			else
			{
				//美元
				market_price = model_RecordCourse.MarketPrice;
				price = model_RecordCourse.Price;
			}
			var model_Currency = _context.Queryable<PubCurrency>().First(u => u.CurrencyCode.ToUpper() == currency_code.ToUpper());
			var payTypes = new List<PayType>();
			var result_Paytype = await _commonBaseService.PayTypes();
			payTypes = (result_Paytype.StatusCode == 0) ? result_Paytype.Data : null;

			string intro_up = "", intro_low = "";
			if (!string.IsNullOrEmpty(model_RecordCourse.IntroUp))
			{
				var o = JObject.Parse(model_RecordCourse.IntroUp);
				intro_up = (o["data"] != null ? o["data"].ToString() : "");
			}
			if (!string.IsNullOrEmpty(model_RecordCourse.IntroLow))
			{
				var o = JObject.Parse(model_RecordCourse.IntroLow);
				intro_low = (o["data"] != null ? o["data"].ToString() : "");
			}

			var model = new ConfirmOrderDto();

			var list_Info = _context.Queryable<WebRecordCourseInfo>().LeftJoin<WebRecordCourseInfoLang>((l, r) => l.RecordCourseInfoid == r.RecordCourseInfoid && r.Lang == _userManage.Lang)
				.Where((l, r) => l.Status == 1 && l.RecordCourseid == recordCourseid)
				.OrderBy((l, r) => l.Sort)
				.OrderBy((l, r) => l.RecordCourseInfoid, OrderByType.Desc)
				.Select((l, r) => new { l.Duration, r.Title, l.ViewCount })
				.ToList();
			foreach (var model_info in list_Info)
			{
				model.Videos.Add(new ConfirmOrderDto.Info
				{
					Title = model_info.Title,
					Duration = model_info.Duration,
					ViewCount = model_info.ViewCount
				});
			}
			model.RecordCourseid = recordCourseid;
			model.Img = _commonBaseService.ResourceDomain(model_RecordCourse.Img);
			model.Title = model_RecordCourse.Title;
			model.Message = model_RecordCourse.Message;
			model.IntroUp = intro_up;
			model.IntroLow = intro_low;
			model.LessonCount = model_RecordCourse.LessonCount;
			model.StudentCount = model_RecordCourse.StudentCount;
			model.Ico = model_Currency?.Ico + "";
			model.CurrencyCode = currency_code;
			model.MarketPrice = market_price;
			model.Price = price;
			model.PayTypes = payTypes;
			result.Data = model;
			return result;
		}
		#endregion

		#region 创建录播课订单
		/// <summary>
		/// 创建订单
		/// </summary>
		/// <param name="recordCourseid">录播课程Id</param>
		/// <returns></returns>
		public async Task<ApiResult<OrderDto>> CreateOrder(long recordCourseid)
		{
			var result = new ApiResult<OrderDto>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			var model_RecordCourse = _context.Queryable<WebRecordCourse>().LeftJoin<WebRecordCourseLang>((l, r) => l.RecordCourseid == r.RecordCourseid && r.Lang == _userManage.Lang)
				.Where((l, r) => l.RecordCourseid == recordCourseid && l.Status == 1)
				.Select((l, r) => new { l.RecordCourseid, l.LessonCount, l.StudentCount, l.Img, l.Price, l.MarketPrice, r.Title, r.Message })
				.First();
			if (model_RecordCourse == null)
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
			var model_Price = _context.Queryable<WebRecordCoursePrice>().First(u => u.CurrencyCode == currency_code && u.RecordCourseid == recordCourseid);
			if (model_Price != null)
			{
				market_price = model_Price.MarketPrice;
				price = model_Price.Price;
				online_course_priceid = model_Price.RecordCoursePriceid;
			}
			else
			{
				//美元
				market_price = model_RecordCourse.MarketPrice;
				price = model_RecordCourse.Price;
				online_course_priceid = 0;
			}

			string order_sn = _commonBaseService.CreateOrderSn("A", _userManage.Userid, 0);
			var model_Order = new WebOrder();
			model_Order.Type = 2;
			model_Order.RecordCourseid = recordCourseid;
			model_Order.OrderSn = order_sn;
			model_Order.Userid = _userManage.Userid;
			model_Order.Title = model_RecordCourse.Title;
			model_Order.Img = model_RecordCourse.Img;
			model_Order.Message = model_RecordCourse.Message;
			model_Order.MarketPrice = model_RecordCourse.MarketPrice;
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