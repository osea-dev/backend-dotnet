using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Api.Dto;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Api.Dto.Teacher;
using WeTalk.Models.Dto;
using WeTalk.Common.Helper;
using Microsoft.Extensions.Logging;
using System;
using WeTalk.Models.Dto.Teacher;

namespace WeTalk.Api
{
    /// <summary>
    /// 前台老师接口
    /// </summary>
    [Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
	public partial class TeacherController : Base.WebApiController
	{
		private readonly ITeacherService _teacherService;
		private readonly IHttpContextAccessor _accessor;
		private readonly ILogger<TeacherController> _logger;
		public TeacherController(IHttpContextAccessor accessor, SqlSugarScope dbcontext, ITeacherService teacherService, ILogger<TeacherController> logger) : base(accessor)
		{
            _teacherService = teacherService;
			_accessor = accessor;
			_logger = logger;
		}

		#region "老师分类"
		/// <summary>
		/// 老师分类
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<List<TeacherCategoryDto>>> TeacherCategory()
		{
			try
			{
				return await _teacherService.TeacherCategory();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<TeacherCategoryDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "老师列表"   		
		/// <summary>
		/// 老师列表
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<Pages<TeacherDto>>> TeacherList([FromBody] TeacherListRequestDto data)
		{
			try
			{
				return await _teacherService.TeacherList(data.teacherCategoryid, data.page, data.pageSize);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<Pages<TeacherDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "老师详情"   		
		/// <summary>
		/// 老师详情
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<TeacherDetailDto>> TeacherDetail([FromBody]TeacherDetailRequestDto data)
		{
			try
			{
				return await _teacherService.TeacherDetail(data.teacherid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<TeacherDetailDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "老师读取课堂报告"   		
		/// <summary>
		/// 老师读取课堂报告
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
        public async Task<ApiResult<TeacherReportDto>> LessonReport([FromBody] LessonReportRequestDto data)
		{
			try
			{
				return await _teacherService.LessonReport(data.MenkeLessonId, data.Code);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<TeacherReportDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "老师提交课节报告"   		
		/// <summary>
		/// 老师提交课节报告
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<List<SubmitReportDto>>> SubmitLessonReport([FromBody] SubmitLessonReportRequestDto data)
		{
			try
			{
				return await _teacherService.SubmitLessonReport(data.MenkeLessonId, data.Code, data.StudentReports);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<SubmitReportDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
        #endregion

        #region "老师头像"   		
        /// <summary>
        /// 老师头像
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<TeacherTeacherHeadImgDto>> TeacherHeadImg([FromBody] TeacherHeadImgRequestDto data)
        {
			try
			{
				return await _teacherService.TeacherHeadImg(data.teacherid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<TeacherTeacherHeadImgDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion
	}
}
