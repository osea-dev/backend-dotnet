using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Threading.Tasks;
using WeTalk.Models;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using WeTalk.Common.Helper;
using System;
using WeTalk.Models.Dto;
using WeTalk.Interfaces.Base;
using Microsoft.Extensions.Localization;
using WeTalk.Models.Dto.Teacher;

namespace WeTalk.Interfaces.Services
{
	public partial class TeacherService : BaseService, ITeacherService
	{
		private readonly SqlSugarScope _context;
		private readonly ILogger<TeacherService> _logger;
		private readonly IWebHostEnvironment _env;
		private readonly IUserManage _userManage;
		private readonly ICommonBaseService _commonBaseService;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包

		public TeacherService(SqlSugarScope dbcontext, ILogger<TeacherService> logger,IWebHostEnvironment env, IUserManage userManage, ICommonBaseService commonBaseService,
			IStringLocalizer<LangResource> localizer)
		{
			_context = dbcontext;
			_logger = logger;
			_localizer = localizer;

			_env = env;
			_userManage = userManage;
			_commonBaseService = commonBaseService;
		}

		#region "获取老师分类"
		/// <summary>
		/// 获取老师分类
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<TeacherCategoryDto>>> TeacherCategory()
		{
			var result = new ApiResult<List<TeacherCategoryDto>>();
			result.Data = new List<TeacherCategoryDto>();
			var list_Category = await _context.Queryable<WebTeacherCategory>().LeftJoin<WebTeacherCategoryLang>((l,r)=>l.TeacherCategoryid==r.TeacherCategoryid && r.Lang== _userManage.Lang)
				.Where((l,r) => l.Status == 1).OrderBy(l => l.Sort).Select((l,r)=>new { l.TeacherCategoryid,r.Title}).ToListAsync();
			
			foreach (var model in list_Category)
			{
				result.Data.Add(new TeacherCategoryDto()
				{
					TeacherCategoryid = model.TeacherCategoryid,
					Title= model.Title
				}) ;
			}
			return result;
		}
		#endregion

		#region "获取老师列表"

		public async Task<ApiResult<Pages<TeacherDto>>> TeacherList(long teacherCategoryid = 0,int page = 1,int pageSize=12)
		{
			var result = new ApiResult<Pages<TeacherDto>>();
			result.Data = new Pages<TeacherDto>();
			int total = 0;
			var list_Teacher = _context.Queryable<WebTeacher>().LeftJoin<WebTeacherLang>((l,r)=>l.Teacherid==r.Teacherid && r.Lang==_userManage.Lang)
				.Where((l,r) => l.Status == 1 && l.Sty==0)
				.WhereIF(teacherCategoryid > 0, (l, r) => l.TeacherCategoryid == teacherCategoryid)
				.OrderBy((l, r) => l.Sort).OrderBy((l, r) => l.Sendtime,OrderByType.Desc)
				.Select((l, r)=>new { l.Teacherid,l.Img,r.Name,r.Keys})
				.ToPageList(page, pageSize, ref total);
			var str = new StringBuilder();
			result.Data.Total = total;
			foreach (var model in list_Teacher){
				result.Data.List.Add(new TeacherDto()
				{
					Teacherid = model.Teacherid,
					Name=model.Name+"",
					Img = _commonBaseService.ResourceDomain(model.Img),
					Keys = (model.Keys + "").Split(",").Where(u => !string.IsNullOrEmpty(u)).ToList()
				});
			}
			return result;
		}
		#endregion

		#region "获取老师详情"
		public async Task<ApiResult<TeacherDetailDto>> TeacherDetail(long teacherid)
		{
			var result = new ApiResult<TeacherDetailDto>();
			result.Data = new TeacherDetailDto();

			var str = new StringBuilder();
			var model_Teacher = _context.Queryable<WebTeacher>().LeftJoin<WebTeacherLang>((l,r)=>l.Teacherid==r.Teacherid && r.Lang==_userManage.Lang)
				.Where((l,r)=>l.Teacherid == teacherid && l.Status==1)
				.Select((l,r)=>new {l.Teacherid,l.Img,l.Photo,r.Name,r.Keys,r.Comefrom,r.Edu,r.Religion,r.Message,l.TeacherCategoryid,r.Advantage,r.Motto,r.Philosophy })
				.First();
			if (model_Teacher != null)
			{
				var model_TeacherCateory = _context.Queryable<WebTeacherCategory>()
					.LeftJoin<WebTeacherCategoryLang>((l, r) => l.TeacherCategoryid == r.TeacherCategoryid && r.Lang == _userManage.Lang)
					.Where((l,r)=>l.TeacherCategoryid == model_Teacher.TeacherCategoryid)
					.Select((l,r)=>new {l.TeacherCategoryid,r.Title })
					.First();
				result.Data.Teacherid = model_Teacher.Teacherid;
				result.Data.Img = _commonBaseService.ResourceDomain(model_Teacher.Img);
				result.Data.Photo= _commonBaseService.ResourceDomain(model_Teacher.Photo);
				result.Data.Name= model_Teacher.Name+"";
				result.Data.Keys = (model_Teacher.Keys + "").Split(",").Where(u => !string.IsNullOrEmpty(u)).ToList();
				result.Data.ComeFrom = model_Teacher.Comefrom;
				result.Data.Edu= model_Teacher.Edu;
				result.Data.Religion= model_Teacher.Religion;
				result.Data.Desc = model_Teacher.Message;
				result.Data.Category = model_TeacherCateory?.Title + "";
				if (!string.IsNullOrEmpty(model_Teacher.Advantage))
				{
					var o = JObject.Parse(model_Teacher.Advantage + "");
					if (o["tag1"] != null && o["tag2"] != null && o["tag3"] != null && o["tag4"] != null)
					{
						result.Data.Advantages.Add(new TeacherDetailDto.Advantage()
						{
							Value = o["tag1"].ToString(),
							Sup = o["tag2"].ToString(),
							Sub = o["tag3"].ToString(),
							Title = o["tag4"].ToString()
						}) ;
					}
					if (o["tag5"] != null && o["tag6"] != null && o["tag7"] != null && o["tag8"] != null)
					{
						result.Data.Advantages.Add(new TeacherDetailDto.Advantage()
						{
							Value = o["tag5"].ToString(),
							Sup = o["tag6"].ToString(),
							Sub = o["tag7"].ToString(),
							Title = o["tag8"].ToString()
						});
					}
					if (o["tag9"] != null && o["tag10"] != null && o["tag11"] != null && o["tag12"] != null)
					{
						result.Data.Advantages.Add(new TeacherDetailDto.Advantage()
						{
							Value = o["tag9"].ToString(),
							Sup = o["tag10"].ToString(),
							Sub = o["tag11"].ToString(),
							Title = o["tag12"].ToString()
						});
					}
				}
				result.Data.Motto = model_Teacher.Motto;
				result.Data.Philosophy= model_Teacher.Philosophy;
			}
			else {
				result.StatusCode = 4003;
				result.Message = "参数异常，课程不存在";
			}
			return result;
		}
		#endregion

		#region "老师提交课堂报告"
		/// <summary>
		/// 老师读取课堂报告
		/// </summary>
		/// <param name="menkeLessonid">拓课云课节ID</param>
		/// <param name="code">临时授权码</param>
		/// <returns></returns>
		public async Task<ApiResult<TeacherReportDto>> LessonReport(int menkeLessonid, string code)
		{
			var result = new ApiResult<TeacherReportDto>();
			if (menkeLessonid <= 0 || string.IsNullOrEmpty(code)) {

				result.StatusCode = 4003;
				result.Message = _localizer["您的链接不完整"];
				return result;
			}
			var model_MenkeLesson = _context.Queryable<MenkeLesson>().First(u => u.MenkeLessonId == menkeLessonid && u.TeacherCode == code);
			if (model_MenkeLesson != null)
            {
                result.Data = new TeacherReportDto();
                var model_MenkeCourse = _context.Queryable<MenkeCourse>().First(u => u.MenkeCourseId == model_MenkeLesson.MenkeCourseId);
				result.Data.MenkeLessonId = menkeLessonid;
				result.Data.MenkeLessonName = model_MenkeLesson.MenkeName;
				result.Data.MenkeCourseName = model_MenkeCourse?.MenkeName + "";
				result.Data.MenkeStarttime = model_MenkeLesson.MenkeStarttime;
				result.Data.MenkeEndtime = model_MenkeLesson.MenkeEndtime;
				result.Data.IsTrial = model_MenkeLesson.Istrial;
                var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u => u.MenkeLessonId == menkeLessonid && u.MenkeState == 3 && u.Status == 1).ToList();
				var list_User = _context.Queryable<WebUser>().Where(u => list_UserLesson.Select(s => s.Userid).Contains(u.Userid)).Select(u=>new { u.Userid,u.FirstName,u.LastName,u.Mobile,u.MobileCode,u.HeadImg}).ToList();
				var list_Report = _context.Queryable<WebUserLessonReport>().Where(u => list_UserLesson.Select(s => s.UserLessonid).Contains(u.UserLessonid)).ToList();
				foreach (var model in list_UserLesson) {
					var model_User = list_User.FirstOrDefault(u => u.Userid == model.Userid);
					if (model_User == null) continue;
					var model_Report = list_Report.FirstOrDefault(u => u.UserLessonid == model.UserLessonid);
                    string head_img = string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg;

                    result.Data.StudentReports.Add(new TeacherReportDto.StudentReport()
					{
						UserLessonReportid = model_Report?.UserLessonReportid ?? 0,
						UserLessonid = model.UserLessonid,
						FirstName = model_User.FirstName,
						LastName = model_User.LastName,
						HeadImg = _commonBaseService.ResourceDomain(head_img),
						Homework = model_Report?.Homework ?? 0,
						Attention = model_Report?.Attention ?? 0,
						Enthusiasm = model_Report?.Enthusiasm ?? 0,
						Hear = model_Report?.Hear ?? 0,
						Say = model_Report?.Say ?? 0,
						Read = model_Report?.Read ?? 0,
						Write = model_Report?.Write ?? 0,
						Thinking = model_Report?.Thinking ?? 0,
						EmotionalQuotient = model_Report?.EmotionalQuotient ?? 0,
						LoveQuotient = model_Report?.LoveQuotient ?? 0,
						InverseQuotient = model_Report?.InverseQuotient ?? 0,
						Performance = model_Report?.Performance ?? 0,
						Message = model_Report?.Message + "",
						Level = model.Istrial == 1 ? model_Report?.Level : null						
					});
				}
			}
			else {
				result.StatusCode = 4007;
				result.Message = _localizer["您无权操作此课节报告"];
			}
			return result;
		}

		/// <summary>
		/// 老师提交课节报告
		/// </summary>
		/// <param name="menkeLessonid">拓课云课节ID</param>
		/// <param name="code">临时授权码</param>
		/// <param name="studentReports">报告内容</param>
		/// <returns></returns>
		public async Task<ApiResult<List<SubmitReportDto>>> SubmitLessonReport(int menkeLessonid, string code,List<SubmitLessonReportDto.StudentReport> studentReports)
		{
			var result = new ApiResult<List<SubmitReportDto>>();
			var model_MenkeLesson = _context.Queryable<MenkeLesson>().First(u => u.MenkeLessonId == menkeLessonid && u.TeacherCode == code);
			if (model_MenkeLesson == null) {
				result.StatusCode = 4007;
				result.Message = _localizer["您无权操作此课节报告"];
				return result;
			}
			var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u => u.MenkeLessonId == menkeLessonid && studentReports.Select(s => s.UserLessonid).Contains(u.UserLessonid)).ToList();

			//修改
			var list_UserLessonReport = _context.Queryable<WebUserLessonReport>().Where(u =>u.MenkeLessonId==menkeLessonid && studentReports.Select(s => s.UserLessonid).Contains(u.UserLessonid)).ToList();
			foreach (var model in list_UserLessonReport) {
				var model_UserLesson = list_UserLesson.FirstOrDefault(u => u.UserLessonid == model.UserLessonid);
				if (model_UserLesson == null) continue;
				var model_Report = studentReports.FirstOrDefault(u => u.UserLessonid == model.UserLessonid);
				if (model_Report == null) continue;
                model.MenkeUserid = model_UserLesson.MenkeUserId;
				if(model_Report.Message!=null) model.Message = model_Report.Message;
				if (model_Report.Homework != null) model.Homework = model_Report.Homework.Value;
                if (model_Report.Attention != null) model.Attention = model_Report.Attention.Value;
				if (model_Report.Enthusiasm != null) model.Enthusiasm = model_Report.Enthusiasm.Value;
                if (model_Report.Hear != null) model.Hear = model_Report.Hear.Value;
                if (model_Report.Say != null) model.Say = model_Report.Say.Value;
                if (model_Report.Read != null) model.Read = model_Report.Read.Value;
                if (model_Report.Write != null) model.Write = model_Report.Write.Value;
                if (model_Report.Thinking != null) model.Thinking = model_Report.Thinking.Value;
                if (model_Report.EmotionalQuotient != null) model.EmotionalQuotient = model_Report.EmotionalQuotient.Value;
                if (model_Report.LoveQuotient != null) model.LoveQuotient = model_Report.LoveQuotient.Value;
                if (model_Report.InverseQuotient != null) model.InverseQuotient = model_Report.InverseQuotient.Value;
                if (model_Report.Performance != null) model.Performance = model_Report.Performance.Value;
                model.MenkeCourseId = model_UserLesson.MenkeCourseId;
                model.MenkeLessonId = model_UserLesson.MenkeLessonId;
                if (model_Report.Level != null) model.Level = model_Report.Level.Value;
			}

			//新增
			var list_ReportNew = studentReports.Where(u => !list_UserLessonReport.Select(s=>s.UserLessonid).Contains(u.UserLessonid)).ToList();
			foreach(var model in list_ReportNew)
			{
				var model_UserLesson = list_UserLesson.FirstOrDefault(u => u.UserLessonid == model.UserLessonid);
				if (model_UserLesson == null) continue;
				list_UserLessonReport.Add(new WebUserLessonReport()
				{
					UserLessonid = model.UserLessonid,
					Userid = model_UserLesson.Userid,
					MenkeUserid = model_UserLesson.MenkeUserId,
					Teacherid = model_UserLesson.Teacherid,
					Message = model.Message+"",
					Homework = (model.Homework!=null)? model.Homework.Value:0,
					Attention = (model.Attention!=null)? model.Attention.Value:0,
					Enthusiasm = (model.Enthusiasm != null) ? model.Enthusiasm.Value : 0,
					Hear = (model.Hear != null) ? model.Hear.Value : 0,
					Say = (model.Say != null) ? model.Say.Value : 0,
					Read = (model.Read != null) ? model.Read.Value : 0,
					Write = (model.Write != null) ? model.Write.Value : 0,
					Thinking = (model.Thinking != null) ? model.Thinking.Value : 0,
					EmotionalQuotient = (model.EmotionalQuotient != null) ? model.EmotionalQuotient.Value : 0,
					LoveQuotient = (model.LoveQuotient != null) ? model.LoveQuotient.Value : 0,
					InverseQuotient = (model.InverseQuotient != null) ? model.InverseQuotient.Value : 0,
					Performance = (model.Performance != null) ? model.Performance.Value : 0,
					MenkeCourseId = model_UserLesson.MenkeCourseId,
					MenkeLessonId = model_UserLesson.MenkeLessonId,
					Level = (model.Level != null) ? model.Level.Value : 0
				});
			}
			var x =_context.Storageable(list_UserLessonReport).ToStorage();
			x.AsInsertable.ExecuteCommand();
			x.AsUpdateable.UpdateColumns(u => new
			{
				u.MenkeUserid,
				u.Message,
				u.Homework,
				u.Attention,
				u.Enthusiasm,
				u.Hear,
				u.Say,
				u.Read,
				u.Write,
				u.Thinking,
				u.EmotionalQuotient,
				u.LoveQuotient,
				u.InverseQuotient,
				u.Performance,
				u.MenkeCourseId,
				u.MenkeLessonId,
				u.Level
			}).ExecuteCommand();

			var list = _context.Queryable<WebUserLessonReport>().Where(u => u.MenkeLessonId == menkeLessonid && studentReports.Select(s => s.UserLessonid).Contains(u.UserLessonid))
				.Select(u => new SubmitReportDto
				{
					UserLessonReportid = u.UserLessonReportid,
					UserLessonid = u.UserLessonid
				}).ToList();
            //result.Data= new List<SubmitReportDto>();
			result.Data = list;
            return result;
		}
        #endregion

        #region "获取老师头像"
        public async Task<ApiResult<TeacherTeacherHeadImgDto>> TeacherHeadImg(long teacherid)
        {
            var result = new ApiResult<TeacherTeacherHeadImgDto>();

            var str = new StringBuilder();
			var model_Teacher = _context.Queryable<WebTeacher>().InSingle(teacherid);
            if (model_Teacher != null)
            {
                result.Data = new TeacherTeacherHeadImgDto();
                string head_img = !string.IsNullOrEmpty(model_Teacher.HeadImg) ? model_Teacher.HeadImg : "";
				result.Data.HeadImg = _commonBaseService.ResourceDomain(head_img);
            }
            else
            {
                result.StatusCode = 4003;
                result.Message = "参数异常，课程不存在";
            }
            return result;
        }
        #endregion
    }
}