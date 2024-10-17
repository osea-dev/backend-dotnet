using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WeTalk.Api.Dto.Course;
using WeTalk.Common.Helper;
using WeTalk.Interfaces;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto.Course;
using static WeTalk.Api.HttpHeaderFilter;

namespace WeTalk.Api
{
	/// <summary>
	/// 前台课程接口
	/// </summary>
	[Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
	public partial class CourseController : Base.WebApiController
    {
        private readonly IStringLocalizer<LangResource> _localizer;//语言包
        private readonly ICourseService _courseService;
		private readonly ILogger<CourseController> _logger;
		private readonly IHttpContextAccessor _accessor;
		public CourseController(IHttpContextAccessor accessor, ICourseService courseService, IStringLocalizer<LangResource>  localizer, ILogger<CourseController> logger) : base(accessor)
		{
			_courseService = courseService;
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
        public async Task<ApiResult<Pages<CourseDto>>> CourseList([FromBody]CourseListRequestDto data)
		{
			try
			{
				return await _courseService.CourseList(data.key, data.page, data.pageSize);
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
        public async Task<ApiResult<CourseDetailDto>> CourseDetail([FromBody]CourseDetailRequestDto data)
		{
			try
			{
				return await _courseService.CourseDetail(data.courseid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<CourseDetailDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "获取子课程详情"   		
		/// <summary>
		/// 获取子课程详情
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
        [Currency]
        public async Task<ApiResult<SubCourseDetailDto>> SubCourseDetail([FromBody] SubCourseDetailRequestDto data)
		{
			try
			{
				return await _courseService.SubCourseDetail(data.courseGroupInfoid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<SubCourseDetailDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "获取课程SKU价格"   		
		/// <summary>
		/// 获取课程SKU价格
		/// </summary>
		/// <param name="CourseSkuid">aaaaaaaaaa</param>
		/// <returns></returns>
		[HttpPost]
        [Currency]
        public async Task<ApiResult<CourseSkuPriceDto>> CourseSkuPrice([FromBody] CourseSkuPriceRequestDto data)
		{
			try
			{
				return await _courseService.CourseSkuPrice(data.courseSkuid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<CourseSkuPriceDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "课程咨询"

		/// <summary>
		/// 课程咨询
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult> CourseMessage([FromBody] CourseMessageRequestDto data)
		{
			try
			{
				return await _courseService.CourseMessage(data.courseGroupInfoid, data.mobileCode, data.mobile, data.email);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "试听课程申请"
		/// <summary>
		/// 试听课程申请
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult> TrialCourseApply([FromBody] CourseApplyRequestDto data)
		{
			try
			{
				return await _courseService.TrialCourseApply(data.courseName, data.mobileCode, data.mobile, data.email);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion
	}
}
