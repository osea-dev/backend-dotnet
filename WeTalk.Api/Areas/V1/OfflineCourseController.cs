using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Api.Dto.OfflineCourse;
using WeTalk.Common.Helper;
using WeTalk.Interfaces;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto.OfflineCourse;
using static WeTalk.Api.HttpHeaderFilter;

namespace WeTalk.Api
{
	/// <summary>
	/// 线下课程接口
	/// </summary>
	[Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
	public partial class OfflineCourseController : Base.WebApiController
    {
        private readonly IStringLocalizer<LangResource> _localizer;//语言包
        private readonly IOfflineCourseService _offlineCourseService;
		private readonly ILogger<OfflineCourseController> _logger;
		private readonly IHttpContextAccessor _accessor;
		public OfflineCourseController(IHttpContextAccessor accessor, IOfflineCourseService offlineCourseService, IStringLocalizer<LangResource>  localizer, ILogger<OfflineCourseController> logger) : base(accessor)
		{
			_offlineCourseService = offlineCourseService;
			_localizer = localizer;
			_accessor = accessor;
			_logger = logger;
		}
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
				return await _offlineCourseService.CourseList(data.key, data.page, data.pageSize);
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
				return await _offlineCourseService.CourseDetail(data.offlineCourseid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<CourseDetailDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "加载确认线下课订单信息"   		
		/// <summary>
		/// 加载确认线下课订单信息
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[Currency]
		public async Task<ApiResult<ConfirmOrderDto>> ConfirmOrder([FromBody] ConfirmOrderRequestDto data)
		{
			try
			{
				return await _offlineCourseService.ConfirmOrder(data.offlineCourseid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<ConfirmOrderDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "创建线下课订单"   		
		/// <summary>
		/// 创建线下课订单
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
				return await _offlineCourseService.CreateOrder(data.offlineCourseid);
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
