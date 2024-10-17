using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Api.Dto.Order;
using WeTalk.Api.Dto.Student;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto.Common;
using WeTalk.Models.Dto.Course;
using WeTalk.Models.Dto.Student;
using WeTalk.Models.Dto.User;

namespace WeTalk.Api
{
    /// <summary>
    /// 学生会员中心
    /// </summary>
    [Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
	public partial class StudentController : Base.WebApiController
	{
		private readonly IStudentService _studentService;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogger<StudentController> _logger;
        public StudentController(IHttpContextAccessor accessor, IStudentService studentService, ILogger<StudentController> logger) :base(accessor)
		{
			_studentService = studentService;
            _accessor = accessor;
            _logger = logger;
        }

		#region "学生主页"
		/// <summary>
		/// 学生主页
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<StudentHomePageDto>> StudentHomePage()
		{
            try
            {
                return await _studentService.StudentHomePage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<StudentHomePageDto>() { StatusCode = 4012, Message = ex.Message };
            }
        }
		#endregion

        #region "推荐课程"
        /// <summary>
        /// 推荐课程（前三条）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<List<CourseDto>>> RecommendCourse()
		{
            try
            {
                return await _studentService.RecommendCourse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<List<CourseDto>>() { StatusCode = 4012, Message = ex.Message };
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
        [UserToken(Power = "")]
        public async Task<ApiResult> TrialCourseApply([FromBody] TrialCourseApplyRequestDto data)
		{
            try
            {
                return await _studentService.TrialCourseApply(data.courseName, data.mobileCode, data.mobile, data.email,data.birthdate, data.gender, data.isChinese);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult() { StatusCode = 4012, Message = ex.Message };
            }
        }
        #endregion

        #region "正课申请"
        /// <summary>
        /// 正课申请
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult> CourseApply([FromBody] CourseApplyRequestDto data)
        {
            try
            {
                return await _studentService.CourseApply(data.userCourseid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult() { StatusCode = 4012, Message = ex.Message };
            }
        }
        #endregion

        #region "读取指定课节的学生对老师评分"
        /// <summary>
        /// 读取指定课节的学生对老师评分
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<LessonScoreDto>> LessonScore([FromBody] LessonScoreRequestDto data)
        {
            try
            {
                return await _studentService.LessonScore(data.userLessonid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<LessonScoreDto>() { StatusCode = 4012, Message = ex.Message };
            }
        }
        #endregion

        #region "申请取消课节"
        /// <summary>
        /// 取消课节申请原因标签
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<List<string>>> CancelLessonKeys()
        {
            try
            {
                return await _studentService.CancelLessonKeys();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<List<string>>() { StatusCode = 4012, Message = ex.Message };
            }
        }
        /// <summary>
        /// 申请取消课节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult> ApplyCancelLesson([FromBody] ApplyCanalLessonRequestDto data)
        {
            try
            {
                return await _studentService.ApplyCancelLesson(data.userLessonid, data.keys, data.message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult() { StatusCode = 4012, Message = ex.Message };
            }
        }
		#endregion

		#region "学生对课节老师评价打分"
		/// <summary>
		/// 学生对课节老师评价打分
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult> ToTeacherSorce([FromBody] ToTeacherSorceRequestDto data)
		{
            try
            {
                return await _studentService.ToTeacherSorce(data.userLessonid, data.score);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult() { StatusCode = 4012, Message = ex.Message };
            }
        }
		#endregion

		#region "学生基础信息"
		/// <summary>
		/// 学生基础信息
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<StudentDto>> StudentInfo()
		{
            try
            {
                return await _studentService.StudentInfo();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<StudentDto>() { StatusCode = 4012, Message = ex.Message };
            }
        }
		#endregion

		#region "修改学生基础信息"
		/// <summary>
		/// 修改学生基础信息(若不修改则不填)
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult> UpdateStudentInfo([FromBody] StudentRequestDto data)
		{
			return await _studentService.UpdateStudentInfo(data.userpwd,data.firstName, data.lastName, data.birthdate, data.gender, data.native, data.timezoneid, data.education, data.nativeLang,
			data.guardianName, data.guardianMobileCode, data.guardianMobile,data.guardianSmsCode, data.guardianshipFee, data.email,data.countryCode, data.mobileCode, data.mobile,data.smsCode);
		}
		#endregion

		#region "上传头像"
		/// <summary>
		/// 上传头像
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<FileDto>> UploadHeadImg([FromForm] IFormFile upfile)
		{
			return await _studentService.UploadHeadImg(upfile);
		}
		#endregion

		#region "我的学习报告"
		/// <summary>
		/// 我的定级报告(仅针对试听课)
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<List<LevelReportDto>>> LevelReportList()
		{
            try
            {
                return await _studentService.LevelReportList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<List<LevelReportDto>>() { StatusCode = 4012, Message = ex.Message };
            }
        }

		/// <summary>
		/// 我的课节报告(针对正价课)
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<List<LessonReportDto>>> LessonReportList()
		{
            try
            {
                return await _studentService.LessonReportList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<List<LessonReportDto>>() { StatusCode = 4012, Message = ex.Message };
            }
        }

		/// <summary>
		/// 我的课节报告详情
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<LessonReportDetailDto>> LessonReport([FromBody] LessonReportDtoRequestDto data)
        {
            try
            {
                return await _studentService.LessonReport(data.userLessonReportid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<LessonReportDetailDto>() { StatusCode = 4012, Message = ex.Message };
            }
        }
		#endregion

		#region 我的课程(众语课)
		/// <summary>
		/// 我的课程列表(众语课)
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<List<UserCourseDto>>> UserCourseList()
		{
            try
            {
                return await _studentService.UserCourseList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<List<UserCourseDto>>() { StatusCode = 4012, Message = ex.Message };
            }
        }
		/// <summary>
		/// 我的课程(众语课)-正式课程
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<UserCourseDto>> UserCourse([FromBody] UserCourseDtoRequestDto data )
		{
            try
            {
                return await _studentService.UserCourse(data.userCourseid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<UserCourseDto>() { StatusCode = 4012, Message = ex.Message };
            }
        }
        /// <summary>
        /// 正式课课节列表
        /// </summary>
        /// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<Pages<UserLessonDto>>> UserLessonList([FromBody] UserLessonListRequestDto data)
		{
            try
            {
                return await _studentService.UserLessonList(data.userCourseid, data.startStatus, data.endStatus, data.page, data.pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<Pages<UserLessonDto>>() { StatusCode = 4012, Message = ex.Message };
            }
        }
		/// <summary>
		/// 我的课程(众语课)-试听课(课节列表)
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<List<TrialUserLessonDto>>> TrialUserLessonList()
		{
            try
            {
                return await _studentService.TrialUserLessonList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<List<TrialUserLessonDto>>() { StatusCode = 4012, Message = ex.Message };
            }
        }
		/// <summary>
		/// 课节详情
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<UserLessonDto>> UserLessonDetail([FromBody] UserLessonDetailRequestDto data)
		{
            try
            {
                return await _studentService.UserLessonDetail(data.userLessonid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<UserLessonDto>() { StatusCode = 4012, Message = ex.Message };
            }
        }
		#endregion

		#region 我的课程(直播课)
		/// <summary>
		/// 我的课程列表(直播课)
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[UserToken(Power = "")]
		public async Task<ApiResult<List<OnlineCourseDto>>> MyOnlineCourseList()
		{
			try
			{
				return await _studentService.MyOnlineCourseList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<OnlineCourseDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region 我的课程(录播课)
		/// <summary>
		/// 我的课程列表(录播课)
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[UserToken(Power = "")]
		public async Task<ApiResult<List<MyRecordCourseDto>>> MyRecordCourseList()
		{
			try
			{
				return await _studentService.MyRecordCourseList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<MyRecordCourseDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		/// <summary>
		/// 我的课程详情(录播课)
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[UserToken(Power = "")]
		public async Task<ApiResult<MyRecordCourseDto>> MyRecordCourse([FromBody] OrderDetailRequestDto data)
		{
			try
			{
				return await _studentService.MyRecordCourse(data.orderid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<MyRecordCourseDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region 我的课程(线下课)
		/// <summary>
		/// 我的课程列表(线下课)
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[UserToken(Power = "")]
		public async Task<ApiResult<List<MyOfflineCourseDto>>> MyOfflineCourseList()
		{
			try
			{
				return await _studentService.MyOfflineCourseList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<MyOfflineCourseDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "家庭作业详情"
		/// <summary>
		/// 家庭作业详情
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<HomeworkDto>> HomeworkDetail([FromBody] HomeworkDetailRequestDto data)
        {
            try
            {
                return await _studentService.HomeworkDetail(data.userLessonid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<HomeworkDto>() { StatusCode = 4012, Message = ex.Message };
            }
        }
		#endregion

		#region 我的课表
		/// <summary>
		/// 我的课表列表
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<List<UserLessonDto>>> MySchedule([FromBody] MyScheduleRequestDto data)
        {
            try
            {
                return await _studentService.MySchedule(data.begintime, data.endtime, data.menkeState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<List<UserLessonDto>>() { StatusCode = 4012, Message = ex.Message };
            }
        }
		#endregion

		#region 消息列表
		/// <summary>
		/// 我的消息列表
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<Pages<MessageDto>>> MessageList([FromBody] MessageListRequestDto data)
        {
            try
            {
                return await _studentService.MessageList(data.page, data.pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<Pages<MessageDto>>() { StatusCode = 4012, Message = ex.Message };
            }
        }

		/// <summary>
		/// 我的消息详情
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<Pages<MessageDto>>> MessageDetail([FromBody] MessageDetailRequestDto data)
        {
            try
            {
                return await _studentService.MessageDetail(data.sendUserid, data.page, data.pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<Pages<MessageDto>>() { StatusCode = 4012, Message = ex.Message };
            }
        }
		/// <summary>
		/// 删除指定发送方所有消息
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult> DelUserMessage([FromBody] DelUserMessageRequestDto data)
        {
            try
            {
                return await _studentService.DelUserMessage(data.sendUserid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult() { StatusCode = 4012, Message = ex.Message };
            }
        }
		/// <summary>
		/// 删除指定发送方所有消息
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult> DelMessage([FromBody] DelMessageRequestDto data)
        {
            try
            {
                return await _studentService.DelUserMessage(data.messageid);
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
