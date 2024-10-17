using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Api.Dto.OnlineCourse;
using WeTalk.Common.Helper;
using WeTalk.Interfaces;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto.OnlineCourse;
using static WeTalk.Api.HttpHeaderFilter;

namespace WeTalk.Api
{
	/// <summary>
	/// 直播课程接口
	/// </summary>
	[Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
	public partial class OnlineCourseController : Base.WebApiController
    {
        private readonly IStringLocalizer<LangResource> _localizer;//语言包
        private readonly IOnlineCourseService _onlineCourseService;
		private readonly ILogger<OnlineCourseController> _logger;
		private readonly IHttpContextAccessor _accessor;
		public OnlineCourseController(IHttpContextAccessor accessor, IOnlineCourseService onlineCourseService, IStringLocalizer<LangResource>  localizer, ILogger<OnlineCourseController> logger) : base(accessor)
		{
			_onlineCourseService = onlineCourseService;
			_localizer = localizer;
			_accessor = accessor;
			_logger = logger;
		}


		#region "获取直播课分类"
		/// <summary>
		/// 获取直播课分类
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[Currency]
        public async Task<ApiResult<List<CategoryDto>>> CategoryList()
		{
			try
			{
				return await _onlineCourseService.CategoryList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<CategoryDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion


		#region "课程列表"
		/// <summary>
		/// 课程列表
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[Currency]
		public async Task<ApiResult<Pages<CourseDto>>> CourseList([FromBody] CourseListRequestDto data)
		{
			try
			{
				return await _onlineCourseService.CourseList( data.onlineCategoryid, data.key, data.page, data.pageSize);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<Pages<CourseDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "课程详情"   		
		/// <summary>
		/// 课程详情
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[Currency]
		public async Task<ApiResult<CourseDetailDto>> CourseDetail([FromBody] CourseDetailRequestDto data)
		{
			try
			{
				return await _onlineCourseService.CourseDetail(data.onlineCourseid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<CourseDetailDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "加载确认直播课订单信息"   		
		/// <summary>
		/// 加载确认直播课订单信息
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[Currency]
		public async Task<ApiResult<ConfirmOrderDto>> ConfirmOrder([FromBody] ConfirmOrderRequestDto data)
		{
			try
			{
				return await _onlineCourseService.ConfirmOrder(data.onlineCourseid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<ConfirmOrderDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "创建直播课订单"   		
		/// <summary>
		/// 创建直播课订单
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[Currency]
		[UserToken(Power = "")]
		public async Task<ApiResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequestDto data)
		{
			try
			{
				return await _onlineCourseService.CreateOrder(data.onlineCourseid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<OrderDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion
	}
}
