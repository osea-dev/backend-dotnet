using Aspose.Cells.Charts;
using Aspose.Slides;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
using WeTalk.Models.Dto.Common;
using WeTalk.Models.Dto.Course;
using WeTalk.Models.Dto.Menke;
using WeTalk.Models.Dto.Student;
using WeTalk.Models.Dto.User;

namespace WeTalk.Interfaces.Services
{
	public partial class StudentService : BaseService, IStudentService
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包

		private readonly IUserManage _userManage;
		private readonly IMessageBaseService _messageBaseService;
		private readonly IPubConfigBaseService _pubConfigBaseService;
		private readonly ICommonBaseService _commonBaseService;
        private readonly ISobotBaseService _sobotBaseService;
		private readonly IMenkeBaseService _menkeBaseService;
		private readonly ILogger<StudentService> _logger;

		public StudentService(IHttpContextAccessor accessor, SqlSugarScope dbcontext, IStringLocalizer<LangResource> localizer, IMenkeBaseService menkeBaseService,
            IMessageBaseService messageBaseService, IPubConfigBaseService pubConfigBaseService, ICommonBaseService commonBaseService, ISobotBaseService sobotBaseService,
            IUserManage userManage, ILogger<StudentService> logger)
		{
			_context = dbcontext;
			_localizer = localizer;
			_accessor = accessor;
			_logger = logger;

			_messageBaseService = messageBaseService;
			_userManage = userManage;
			_pubConfigBaseService = pubConfigBaseService;
			_commonBaseService = commonBaseService;
            _sobotBaseService = sobotBaseService;
			_menkeBaseService = menkeBaseService;
        }

		#region 学生基础信息
		/// <summary>
		/// 学生基础信息
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<StudentDto>> StudentInfo()
		{
			var result = new ApiResult<StudentDto>();
			string token = _userManage.UserToken??"";
			var model_User = _context.Queryable<WebUser>().First(u => u.Status >= 0 && u.Token.ToLower() == token.ToLower());
			if (model_User != null)
			{
				var model_Timezone = _context.Queryable<PubTimezone>().InSingle(model_User.Timezoneid);
				string timezone_code = model_Timezone?.Country ?? "";
				var list_Country = _context.Queryable<PubCountry>().Where(u => u.Code.ToUpper() == model_User.Native.ToUpper() || u.Code.ToUpper() == timezone_code.ToUpper()).ToList();

				//出生地
				var model_Native = list_Country.FirstOrDefault(u => u.Code == model_User.Native);
				string native = (_userManage.Lang == "zh-cn" ? model_Native?.Country : model_Native?.CountryEn) + "";

				//居住地
				var model_Residence = list_Country.FirstOrDefault(u => u.Code == timezone_code);
				string residence = (_userManage.Lang == "zh-cn" ? model_Residence?.Country : model_Residence?.CountryEn) + "";

				var timeZone = new StudentDto.TimeZone()
				{
					Code = model_Residence?.Code+"",
                    Timezoneid = model_User.Timezoneid,
					Country = residence,
					TimezoneName = model_Timezone?.Title,
					UtcSec = -(model_Timezone?.UtcSec ?? 0) / 60,
				};
				string head_img = string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg;
				result.Data = new StudentDto()
				{
					Userid = model_User.Userid,
					Regtime = DateHelper.ConvertDateTimeInt(model_User.Regtime),
					HeadImg = _commonBaseService.ResourceDomain(head_img),
					FirstName = model_User.FirstName,
					LastName = model_User.LastName,
					Birthdate = model_User.Birthdate.ToString("yyyy-MM-dd"),
					Gender = model_User.Gender,
					Timezone = timeZone,
					Education = model_User.Education,
					Native = native,
					NativeCountryCode = model_User.Native,
					GuardianName = model_User.GuardianName,
					GuardianMobileCode = model_User.GuardianMobileCode,
					GuardianMobile = model_User.GuardianMobile,
					GuardianshipFee = model_User.GuardianshipFee,
					Email = model_User.Email,
					MobileCode = model_User.MobileCode,
					Mobile = model_User.Mobile
				};
			}
			else
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
			}
			return result;
		}
		#endregion

		#region 更新学生基础信息
		/// <summary>
		/// 更新学生基础信息
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult> UpdateStudentInfo(string userpwd, string firstName, string lastName, DateTime? birthdate, int? gender, string native, long timezoneid,
			string education, string nativeLang, string guardianName, string guardianMobileCode, string guardianMobile, string guardianSmsCode, string guardianshipFee, string email, string countryCode,string mobileCode, string mobile, string smsCode)
		{
			var result = new ApiResult();
			var model_User = _context.Queryable<WebUser>().First(u => u.Token.ToLower() == _userManage.UserToken.ToLower());
			if (model_User != null)
			{
				if (model_User.Status == 1)
				{
					if (!string.IsNullOrEmpty(userpwd)) model_User.Userpwd = MD5Helper.MD5Encrypt32(userpwd);
					if (!string.IsNullOrEmpty(firstName)) model_User.FirstName = firstName;
					if (!string.IsNullOrEmpty(lastName)) model_User.LastName = lastName;
					if (birthdate != null) model_User.Birthdate = birthdate.Value;
					if (gender != null) model_User.Gender = gender.Value;
					if (!string.IsNullOrEmpty(native)) model_User.Native = native;
					if (timezoneid > 0)
					{
						model_User.Timezoneid = timezoneid;
					}
					if (!string.IsNullOrEmpty(education)) model_User.Education = education;
					if (!string.IsNullOrEmpty(nativeLang)) model_User.Native = nativeLang.ToLower();
					if (!string.IsNullOrEmpty(guardianName)) model_User.GuardianName = guardianName;
					if (guardianSmsCode == model_User.MobileSmscode)
					{
						if (!string.IsNullOrEmpty(guardianMobileCode)) model_User.GuardianMobileCode = guardianMobileCode;
						if (!string.IsNullOrEmpty(guardianMobile)) model_User.GuardianMobile = guardianMobile;
						if (!string.IsNullOrEmpty(guardianshipFee)) model_User.GuardianshipFee = guardianshipFee;
					} else if (!string.IsNullOrEmpty(guardianMobileCode)|| !string.IsNullOrEmpty(guardianMobile)|| !string.IsNullOrEmpty(guardianshipFee)) {
						result.StatusCode = 4003;
						result.Message = _localizer["短信验证码较验失败"];
						return result;
					}
					if (!string.IsNullOrEmpty(email)) model_User.Email = email;
					if (!string.IsNullOrEmpty(mobileCode)) model_User.MobileCode = mobileCode;
					if (!string.IsNullOrEmpty(mobile)) model_User.Mobile = mobile;
					_context.Updateable(model_User).ExecuteCommand();
					//修改拓课云资料
					var result_menkeUser = await _menkeBaseService.ModifyStudent(model_User.MobileCode, model_User.Mobile, new MenkeStudentDto()
					{
						code=model_User.MobileCode,
						mobile=model_User.Mobile,
                        name = model_User.FirstName + " "+ model_User.LastName,
						nickname= model_User.FirstName + " " + model_User.LastName,
						sex= model_User.Gender,
						birthday= model_User.Birthdate.ToString("d"),
						p_name = model_User.GuardianName
                    });
					
                }
				else
				{
					result.StatusCode = 4005;
					result.Message = _localizer["用户信息未完善"];
				}
			}
			else
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
			}
			return result;
		}
		#endregion

		#region "学生主页"
		/// <summary>
		/// 学生主页
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<StudentHomePageDto>> StudentHomePage()
		{
			var result = new ApiResult<StudentHomePageDto>();
			StudentHomePageDto studentBaseDto = new StudentHomePageDto();
			var model_User = _context.Queryable<WebUser>().First(u => u.Token == _userManage.UserToken);
			if (model_User != null)
			{
				if (model_User.Status == 1)
				{

					//最新的试听课报告
					//试听状态：0未试听过，1已申请但还未试听，2已试听但报告未出，3已出报告
					//课程缺席没来，等同于未试听过
					var model_Report = _context.Queryable<WebUserLessonReport>().Where(u => SqlFunc.Subqueryable<MenkeCourse>().Where(s => s.Istrial == 1 && s.Status == 1 && u.MenkeCourseId == s.MenkeCourseId).Any() && u.Userid == model_User.Userid).OrderBy(u => u.Dtime, OrderByType.Desc).First();
					if (model_Report != null)
					{
						//已出报告
						studentBaseDto.TrialLessonReport.Status = 3;
						studentBaseDto.TrialLessonReport.UserLessonReportid = model_Report.UserLessonReportid;
						studentBaseDto.TrialLessonReport.Level = model_Report.Level;
					}
					else
					{
						//先取取所有未缺席的排课信息: 课程状态：未开始/进行中=已申请未试听，已结课=已试听但报告未出，其它状态=未试听过
						//否则取是否有提交申请（提交未处理或已同意）= 已申请未试听
						var model_TrialUserLesson = _context.Queryable<WebUserLesson>().Where(u => u.Status != -1 && u.Istrial == 1 && u.Userid == model_User.Userid).OrderBy(u => u.MenkeStarttime, OrderByType.Desc).First();
						if (model_TrialUserLesson != null)
						{
							if (model_TrialUserLesson.MenkeDeleteTime > 0 || model_TrialUserLesson.Status == 0)
							{
								//试听删除了或学生被移除,允许重新申请
								studentBaseDto.TrialLessonReport.Status = 0;
							}
							else
							{
								//（1未开始2进行中3已结课4已过期）
								switch (model_TrialUserLesson.MenkeState)
								{
									case 1://未开始
										studentBaseDto.TrialLessonReport.Status = 1;//已申请未试听
										break;
									case 2://进行中
										studentBaseDto.TrialLessonReport.Status = 1;//已申请未试听
										break;
									case 3://已结课
										studentBaseDto.TrialLessonReport.Status = 2;//已试听但报告未出
										break;
									case 4://已过期,缺课
										studentBaseDto.TrialLessonReport.Status = 1;//已申请未试听，强制不允许在线申请了
										break;
									default:
										studentBaseDto.TrialLessonReport.Status = 0;//基本不会出现
										break;
								}
							}
						}
						else
						{
							var model_Apply = _context.Queryable<WebCourseApply>().Where(u => u.Sty == 0 && u.Userid == model_User.Userid && u.Status != -1).OrderBy(u => u.Dtime, OrderByType.Desc).First();
                            if (model_Apply!=null)
							{
                                //0申请，1同意，2缺课，3拒绝
                                switch (model_Apply.Status) {
									case 0:
                                        studentBaseDto.TrialLessonReport.Status = 1;////已申请未试听,实际是未排课;不能再申请
                                        break;
                                    case 1:
                                        studentBaseDto.TrialLessonReport.Status = 1;////已申请未试听,实际是未排课;不能再申请
                                        break;
                                    case 2:
                                        studentBaseDto.TrialLessonReport.Status = 0;////缺课，如果客服把排课信息删除，是可以继续申请的
                                        break;
                                    case 3:
                                        studentBaseDto.TrialLessonReport.Status = 1;//拒绝不允许
                                        break;
									default:
                                        studentBaseDto.TrialLessonReport.Status = 1;////已申请未试听,实际是未排课,1小时内不能再申请
										break;
                                }
							}
							else
							{
								studentBaseDto.TrialLessonReport.Status = 0;
							}
						}
					}
					

					//最近一节排课信息
					var dic_config = _pubConfigBaseService.GetConfigs("classroom_hour,cancel_hour");
					var classroom_hour = dic_config.ContainsKey("classroom_hour") ? decimal.Parse(dic_config["classroom_hour"]) : 0;
					var cancel_hour = dic_config.ContainsKey("cancel_hour") ? double.Parse(dic_config["cancel_hour"]) : 0;
					var dtime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
					var model_UserLesson = _context.Queryable<WebUserLesson>().LeftJoin<MenkeCourse>((l,r)=>l.MenkeCourseId==r.MenkeCourseId)
						.Where((l,r) => l.Status == 1 && l.MenkeDeleteTime == 0 && l.MenkeState == 1 && l.MenkeStarttime >= dtime && l.MenkeStudentCode == model_User.MobileCode && l.MenkeStudentMobile == model_User.Mobile)
                        .OrderBy((l, r) => l.MenkeStarttime)
                        .Select((l,r)=>new { l.UserLessonid,l.MenkeStarttime,l.MenkeEndtime,l.MenkeTeacherName ,l.MenkeEntryurl,l.MenkeLessonName,r.MenkeName})
						.First();
					if (model_UserLesson != null)
					{
						studentBaseDto.LastUserLesson.UserLessonid = model_UserLesson.UserLessonid;
						studentBaseDto.LastUserLesson.MenkeStarttime = model_UserLesson.MenkeStarttime;
						studentBaseDto.LastUserLesson.MenkeEndtime = model_UserLesson.MenkeEndtime;
						studentBaseDto.LastUserLesson.IsCancel = (model_UserLesson.MenkeStarttime > DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddHours(cancel_hour))) ? 1 : 0; 
						studentBaseDto.LastUserLesson.MenkeTeacherName = model_UserLesson.MenkeTeacherName;
						studentBaseDto.LastUserLesson.MenkeEntryurl = (DateTime.UtcNow.AddHours((double)classroom_hour) > DateHelper.ConvertIntToDateTime(model_UserLesson.MenkeStarttime.ToString())) ? model_UserLesson.MenkeEntryurl:"";
						studentBaseDto.LastUserLesson.ClassroomMin = (int)(classroom_hour * 60);
						studentBaseDto.LastUserLesson.MenkeName = model_UserLesson.MenkeName;
						studentBaseDto.LastUserLesson.MenkeLessonName = model_UserLesson.MenkeLessonName;
                    }
					else
					{
						studentBaseDto.LastUserLesson = null;
					}

					//我的课程
					var list_UserCourse = _context.Queryable<WebUserCourse>().Where(u =>u.Istrial==0 && u.Userid == model_User.Userid && u.Status == 1).OrderBy(u => u.Dtime, OrderByType.Desc).Take(3).ToList();
					var list_SkuType = _context.Queryable<WebSkuType>().Where(u => u.Status == 1).ToList();
					var list_SkuTypeLang = _context.Queryable<WebSkuTypeLang>().Where(u => u.Lang == _userManage.Lang).ToList();
					foreach (var model in list_UserCourse)
					{
                        var model_SkuType = list_SkuType.FirstOrDefault(u => u.SkuTypeid == model.SkuTypeid);
						studentBaseDto.UserCourses.Add(new StudentHomePageDto.UserCourse()
						{
							UserCourseid = model.UserCourseid,
							CourseName = model.Title,
							ClassHour = model.ClassHour,
							Classes = model.Classes,
							Type = list_SkuTypeLang.FirstOrDefault(u => u.SkuTypeid == model.SkuTypeid)?.Title,
                            SkuTypeid = model.SkuTypeid,
							IcoFont = model_SkuType?.IcoFont,
							Status=model.Status
						});
					}

                    int student_score_days = _pubConfigBaseService.GetConfigInt("student_score_days", 7);
                    //近期已完成课节
                    var list_EndUserLesson = _context.Queryable<WebUserLesson>().LeftJoin<MenkeCourse>((l, r) => l.MenkeCourseId == r.MenkeCourseId)
						.Where((l, r) => l.Status == 1 && l.MenkeState == 3 && l.MenkeDeleteTime == 0 && l.MenkeStudentCode == model_User.MobileCode && l.MenkeStudentMobile == model_User.Mobile)
						.OrderBy((l, r) => l.MenkeStarttime, OrderByType.Desc)
						.Select((l, r) => new {l.Istrial, l.Userid,l.Courseid, l.UserLessonid, l.MenkeCourseId, l.MenkeLessonName, l.MenkeStarttime, l.MenkeEndtime, l.MenkeState, l.MenkeTeacherName, l.Score,l.ScoreTime, r.MenkeName })
						.Take(3).ToList();
					var list_EndCourse = _context.Queryable<WebCourse>().Where(u => list_EndUserLesson.Select(s => s.Courseid).Contains(u.Courseid)).Select(u => new { u.Courseid, u.Img }).ToList();
					var list_Report = _context.Queryable<WebUserLessonReport>().Where(u => u.Userid == model_User.Userid && list_EndUserLesson.Select(s => s.UserLessonid).Contains(u.UserLessonid)).ToList();
					foreach (var model in list_EndUserLesson)
					{
						int isscore = 0;
						if (model.Score > 0) isscore = 1;
						if (model.ScoreTime.AddDays(student_score_days) < DateTime.Now) isscore = 2;
						var model_report = list_Report.FirstOrDefault(u => u.Userid == model.Userid && u.UserLessonid == model.UserLessonid);
						string img = list_EndCourse.FirstOrDefault(u => u.Courseid == model.Courseid)?.Img + "";
						if (model.Istrial==1) img = "/Upfile/images/trial_lesson.png";
                        studentBaseDto.EndUserLessons.Add(new StudentHomePageDto.UserLesson()
						{
							UserLessonid = model.UserLessonid,
							MenkeName = model.MenkeName,
							Img = _commonBaseService.ResourceDomain(img),
							MenkeLessonName = model.MenkeLessonName,
							MenkeStarttime = model.MenkeStarttime,
							MenkeEndtime = model.MenkeEndtime,
							MenkeTeacherName = model.MenkeTeacherName,
							IsScore = isscore,
							IsReport = model_report!=null ? 1 : 0,
							UserLessonReportid = model_report != null ? model_report.UserLessonReportid:0,
							ReplayUrl = ""//回放URL
						}); 
					}
					result.Data = studentBaseDto;
				}
				else
				{
					result.StatusCode = 4005;
					result.Message = "用户信息未完善";
				}
			}
			else {
				result.StatusCode = 4001;
				result.Message = "用户不存在或超时已退出";
			}
			return result;
		}
        #endregion

        #region 推荐课程
        ///// <summary>
        ///// 推荐课程
        ///// </summary>
        ///// <returns></returns>
        //public async Task<ApiResult<List<CourseGroupInfoDto>>> RecommendCourse()
        //{
        //    var result = new ApiResult<List<CourseGroupInfoDto>>();
        //    var CourseGroupInfos = new List<CourseGroupInfoDto>();
        //    var model_Token = _userManage.GetUserToken();
        //    var age = Math.Ceiling((DateTime.Now - model_Token.Birthdate).TotalDays / 365);//像上取整
        //    var list_CourseGroupInfo = await _context.Queryable<WebCourseGroupInfoLang, WebCourseGroupInfo>((l, r) => l.CourseGroupInfoid == r.CourseGroupInfoid)
        //        .Where((l, r) => l.Lang == _userManage.Lang && r.Status == 1 && (r.AgeMax >= age || r.AgeMax == 0) && (r.AgeMin <= age || r.AgeMin == 0))
        //        .OrderBy((l, r) => r.Sendtime, OrderByType.Desc)
        //        .Select((l, r) => new { l.Title, l.Message, r.Img, r.CourseGroupInfoid, r.Courseid, r.CourseGroupid })
        //        .Take(3)
        //        .ToListAsync();
        //    foreach (var model in list_CourseGroupInfo)
        //    {
        //        CourseGroupInfos.Add(new CourseGroupInfoDto()
        //        {
        //            CourseGroupInfoid = model.CourseGroupInfoid,
        //            Courseid = model.Courseid,
        //            Title = model.Title,
        //            Message = model.Message,
        //            Img = _commonBaseService.ResourceDomain(model.Img + "")
        //        });
        //    }
        //    result.Data = CourseGroupInfos;
        //    return result;
        //}

        /// <summary>
        /// 推荐课程
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<List<CourseDto>>> RecommendCourse()
        {
            var result = new ApiResult<List<CourseDto>>();
            var Courses = new List<CourseDto>();
			//var model_Token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
            var list_Course = await _context.Queryable<WebCourseLang, WebCourse>((l, r) => l.Courseid == r.Courseid)
                .Where((l, r) => l.Lang == _userManage.Lang && r.Status == 1)
                .OrderBy((l, r) => r.Sendtime, OrderByType.Desc)
                .Select((l, r) => new { l.Title, l.Message, r.Img, r.Courseid})
                .Take(3)
                .ToListAsync();

            var list_Price = _context.Queryable<WebCourseSkuPrice>().LeftJoin<WebCourseSku>((l,r)=>l.CourseSkuid==r.CourseSkuid)
                .Where((l,r) =>r.Status==1 && l.CurrencyCode == _userManage.CurrencyCode && list_Course.Select(s => s.Courseid).Contains(l.Courseid))
                .GroupBy((l, r) => l.Courseid)
                .Select((l, r) => new { l.Courseid, Price = SqlFunc.AggregateMin(l.Price) }).ToList();
            foreach (var model in list_Course)
            {
                var model_SkuPrice = list_Price.FirstOrDefault(u => u.Courseid == model.Courseid);
				Courses.Add(new CourseDto()
				{
					Courseid = model.Courseid,
					Title = model.Title,
					Message = model.Message,
					Img = _commonBaseService.ResourceDomain(model.Img + ""),
					SkuTypes = null,
					MinPrice = model_SkuPrice?.Price ?? 0
				}); ;
            }
            result.Data = Courses;
            return result;
        }
        #endregion

        #region "试听课程申请"

        /// <summary>
        /// 试听课程申请
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult> TrialCourseApply(string courseName, string mobileCode, string mobile, string email,string birthdate, int gender,int isChinese)
		{
			var result = new ApiResult();
			var str = new StringBuilder();
			//var model_Token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0) {
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
			mobile = mobile.Replace("-", "").Replace(" ", "");
			if (_context.Queryable<WebCourseApply>().Where(u => u.Sty == 0 && u.ContactMobileCode == mobileCode && u.ContactMobile == mobile && u.Status != -1).Count() > 0)
				return new ApiResult()
				{
					StatusCode = 4000,
					Message = _localizer["每个用户只能提交一次试听,如需多次申请，请联系在线客服"]
				};

			var model_CourseApply = _context.Queryable<WebCourseApply>().Where(u => u.Sty == 0 && u.Userid == _userManage.Userid && u.Status != -1).OrderBy(u => u.Dtime, OrderByType.Desc).First();
			if (model_CourseApply != null)
			{
				//0申请，1同意，2缺课，3拒绝
				switch (model_CourseApply.Status)
				{
					case 0:
						//已申请未试听,实际是未排课;不能再申请
						return new ApiResult()
						{
							StatusCode = 4008,
							Message = _localizer["已提交过试听申请，不可重复提交"]
						};
					case 1:
						//已申请未试听,实际是未排课;不能再申请
						return new ApiResult()
						{
							StatusCode = 4008,
							Message = _localizer["已提交过试听申请，不可重复提交"]
						};
					case 2:
						////缺课，如果客服把排课信息删除，是可以继续申请的
						break;
					case 3:
						//拒绝不允许
						return new ApiResult()
						{
							StatusCode = 4008,
							Message = _localizer["已提交过试听申请，不可重复提交"]
						};
					default:
						////已申请未试听,实际是未排课,1小时内不能再申请
						return new ApiResult()
						{
							StatusCode = 4008,
							Message = _localizer["已提交过试听申请，不可重复提交"]
						};
				}
			}

			var model_Apply = new WebCourseApply();
			//var model_token = _userManage.GetUserToken();
            model_Apply.Userid = _userManage.Userid;
            model_Apply.Sty = 0;
			model_Apply.CourseName = courseName;
			model_Apply.ContactMobileCode = mobileCode;
			model_Apply.ContactMobile = mobile;
			model_Apply.ContactEmail = email;
			model_Apply.Ip = IpHelper.GetCurrentIp(_accessor.HttpContext);
			model_Apply.Status = 0;
			model_Apply.Remark = $"[{DateTime.Now}]用户提交申请表";
			model_Apply.Message = "";
			model_Apply.Gender = gender;
			model_Apply.Ischinese = isChinese;
			model_Apply.Birthdate = birthdate;

			//创建工单提醒
			var dic_data = new Dictionary<string, object>();
			var model_User = _context.Queryable<WebUser>().First(u => u.Status != -1 && u.Userid== _userManage.Userid);
			dic_data.Add(_localizer["试听课程"], courseName);
			dic_data.Add(_localizer["手机"], mobileCode + "-" + mobile);
			dic_data.Add(_localizer["邮箱"], email);
			dic_data.Add(_localizer["性别"], gender==0? _localizer["女"] : _localizer["男"]);
			dic_data.Add(_localizer["是否有目标学习语言基础"], isChinese == 1 ? _localizer["是"] : _localizer["否"]);
			if (model_User != null)
			{
				dic_data.Add(_localizer["所在地时区"], model_User.Utc);
				dic_data.Add(_localizer["出生日期"], birthdate);
				dic_data.Add(_localizer["姓名"], model_User.FirstName + " " + model_User.LastName);
				dic_data.Add(_localizer["备注"], _localizer["请在[拓课云]后台的课程[通用试听课程]下创建试听课节"]);
			}
			else
			{
				dic_data.Add(_localizer["备注"], _localizer["非注册用户,可引导用户在网站注册,在[拓课云]后台的课程[通用试听课程]下创建试听课节"]);
			}
			var body = string.Join("<br>", dic_data.ToList());
			var result_ticket = await _sobotBaseService.AddUserTicket(_localizer["试听课申请"], body);
			if (result_ticket.StatusCode == 0)
			{
				model_Apply.IsTicket = 1;
				model_Apply.TicketTime = DateTime.Now;
				model_Apply.TicketId = result_ticket.Data;
			}
			else
			{
				_logger.LogError(_localizer["试听课申请"] + "," + result_ticket.Message);
			}
			_context.Insertable(model_Apply).ExecuteCommand();
			return result;
		}
        #endregion

        #region "正课课节申请"
        /// <summary>
        /// 正课课节申请
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult> CourseApply(long userCourseid)
        {
            var result = new ApiResult();
            var str = new StringBuilder();
            //var model_Token = _userManage.GetUserToken();
            if (_userManage.Userid <= 0 )
            {
                result.StatusCode = 4001;
                result.Message = _localizer["用户登录超时退出"];
                return result;
            }
			var dtime = DateTime.Now.AddHours(-1);
			var model_UserCourse = _context.Queryable<WebUserCourse>().First(u=> u.Istrial == 0 && u.UserCourseid == userCourseid && u.Status==1);
			if (model_UserCourse == null) {
                result.StatusCode = 4009;
                result.Message = _localizer["我的课程不存在"];
                return result;
            }
			if (model_UserCourse.ClassHour <= model_UserCourse.Classes) {
                result.StatusCode = 4009;
                result.Message = _localizer["剩余课时不足,请在线续课或联系客服续课"];
                return result;
            }

            if (_context.Queryable<WebCourseApply>().Any(u =>u.Courseid== model_UserCourse.Courseid && u.Sty == 2 && u.Userid == _userManage.Userid && u.Status != -1 && u.Dtime> dtime))
                return new ApiResult()
                {
                    StatusCode = 4008,
                    Message = _localizer["请勿重复提交申请"]
                };
            var model_User = _context.Queryable<WebUser>().InSingle(_userManage.Userid);
            if (model_User == null)
            {
                result.StatusCode = 4001;
                result.Message = _localizer["用户登录超时退出"];
                return result;
            }
            var model_MenkeCourse = _context.Queryable<MenkeCourse>().First(u => u.MenkeCourseId == model_UserCourse.MenkeCourseId);

            var model_Apply = new WebCourseApply();
			model_Apply.Courseid = model_UserCourse.Courseid;
			model_Apply.UserCourseid = model_UserCourse.UserCourseid;
			model_Apply.SkuTypeid = model_UserCourse.SkuTypeid;
			model_Apply.ClassHour = model_UserCourse.ClassHour;
			model_Apply.UserCourseid = model_UserCourse.UserCourseid;
			model_Apply.Userid = _userManage.Userid;
			model_Apply.MenkeCourseId = model_MenkeCourse?.MenkeCourseId??0;
            model_Apply.Sty = 2;
            model_Apply.ContactMobileCode = model_User.MobileCode;
            model_Apply.ContactMobile = model_User.Mobile;
            model_Apply.ContactEmail = model_User.Email;
            model_Apply.Ip = IpHelper.GetCurrentIp(_accessor.HttpContext);
            model_Apply.Status = 0;
            model_Apply.Remark = $"[{DateTime.Now}]用户提交申请表";
            model_Apply.Message = "";
            model_Apply.Gender = model_User.Gender;
            model_Apply.Age = (int)Math.Ceiling((DateTime.Now - model_User.Birthdate).TotalDays / 365);//像上取整

			//创建工单提醒
            var dic_data = new Dictionary<string, object>();
            dic_data.Add(_localizer["课程"], model_MenkeCourse!=null? model_MenkeCourse.MenkeName: model_UserCourse.Title);
            dic_data.Add(_localizer["手机"], model_User.MobileCode + "-" + model_User.Mobile);
            dic_data.Add(_localizer["邮箱"], model_User.Email);
            dic_data.Add(_localizer["性别"], model_User.Gender == 0 ? _localizer["女"] : _localizer["男"]);
            dic_data.Add(_localizer["姓名"], model_User.FirstName + " " + model_User.LastName);
            dic_data.Add(_localizer["出生日期"], model_User.Birthdate.ToString("d"));
            dic_data.Add(_localizer["备注"], _localizer["请在[拓课云]后台的课程下创建课节"]);

            var body = string.Join("<br>", dic_data.ToList());
            var result_ticket = await _sobotBaseService.AddUserTicket(_localizer["正课排课申请"], body);
            if (result_ticket.StatusCode == 0)
            {
                model_Apply.IsTicket = 1;
                model_Apply.TicketTime = DateTime.Now;
                model_Apply.TicketId = result_ticket.Data;
			}
			else
			{
				_logger.LogError(_localizer["正课排课申请"] + "," + result_ticket.Message);
			}
			_context.Insertable(model_Apply).ExecuteCommand();
            return result;
        }
        #endregion

		#region 申请取消课节
		/// <summary>
		/// 取消课节原因标签
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<string>>> CancelLessonKeys()
		{
			var result = new ApiResult<List<string>>();
			var list_Key = _context.Queryable<PubKeys>().LeftJoin<PubKeysLang>((l,r)=>l.Keysid==r.Keysid)
				.Where((l,r) => l.Status == 1 && l.Sty == 2 && r.Lang==_userManage.Lang)
				.OrderBy(l => l.Sort).OrderBy(l => l.Keysid, OrderByType.Desc)
				.Select((l,r)=>r.Title)
				.ToList();
			result.Data = list_Key;
            return result;
		}
	
		/// <summary>
		/// 申请取消课节
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult> ApplyCancelLesson(long userLessonid,string[] keys,string message)
		{
			var result = new ApiResult();
			var model_Token = _userManage.GetUserToken();
			if (model_Token != null)
            {
                var model_Lesson = _context.Queryable<WebUserLesson>().First(u=>u.UserLessonid == userLessonid && u.MenkeDeleteTime==0 && u.Status!=-1);
				if (model_Lesson == null) {
                    result.StatusCode = 4008;
                    result.Message = _localizer["课节已取消,请勿重复提交"];
                    return result;
                }
                if (_context.Queryable<WebUserLessonCancel>().Any(u => u.UserLessonid == userLessonid && u.Dtime.AddHours(1)>DateTime.Now && u.Userid == model_Token.Userid && u.Status != -1)) {
                    result.StatusCode = 4008;
                    result.Message = _localizer["您已提交申请,请等待客服与您联系,也可以点击页面右下角的在线客服沟通取消课程"];
                    return result;
                }
				var model_Cancel = new WebUserLessonCancel();
				model_Cancel.UserLessonid = userLessonid;
				model_Cancel.Userid = model_Token.Userid;
				model_Cancel.Tags = JsonConvert.SerializeObject(keys);
				model_Cancel.Message = message;


                //创建工单提醒
                var dic_data=new Dictionary<string, object>();
                dic_data.Add(_localizer["课节名称"], model_Lesson.MenkeLessonName);
                dic_data.Add(_localizer["房间号"], model_Lesson.MenkeLiveSerial);
                dic_data.Add(_localizer["开课时间"], DateHelper.ConvertIntToDateTime(model_Lesson.MenkeStarttime.ToString()));
                dic_data.Add(_localizer["老师"], model_Lesson.MenkeTeacherName);
                dic_data.Add(_localizer["老师手机"], model_Lesson.MenkeTeacherCode + "-" + model_Lesson.MenkeTeacherMobile);
                dic_data.Add(_localizer["学生"], model_Token.FirstName + " " + model_Token.LastName);
                dic_data.Add(_localizer["学生手机"], model_Token.MobileCode + "-" + model_Token.Mobile);
				if (keys.Length > 0) message += "," + string.Join(",", keys);
				dic_data.Add(_localizer["取消原因"], message);

				dic_data.Add(_localizer["注意"], _localizer["在[拓课云]后台找到相应课节后，将学生从本次课节中移除。【特别注意】每次移除完必需重新编辑一次课节信息，编辑内容可不做任何改动直接确定保存"]);
                var body = string.Join("<br>", dic_data.ToList());
                var result_ticket = await _sobotBaseService.AddUserTicket(_localizer["申请取消课节"], body);
                if (result_ticket.StatusCode == 0)
                {
                    model_Cancel.IsTicket = 1;
                    model_Cancel.TicketTime = DateTime.Now;
                    model_Cancel.TicketId = result_ticket.Data;
				}
				else
				{
					_logger.LogError(_localizer["申请取消课节"] + "," + result_ticket.Message);
				}

				_context.Insertable(model_Cancel).ExecuteCommand();
            }
			else {
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
            }
            return result;
		}
		#endregion

		#region 学生对课节老师评价打分
		/// <summary>
		/// 学生对课节老师评价打分
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult> ToTeacherSorce(long userLessonid,int score = 0)
		{
			var result = new ApiResult();
			//var model_Token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
			int student_score_days = _pubConfigBaseService.GetConfigInt("student_score_days");
			var model_UserLesson = _context.Queryable<WebUserLesson>().First(u=>u.UserLessonid == userLessonid && u.Userid== _userManage.Userid);
			if (model_UserLesson != null)
			{
				if (model_UserLesson.Score == 0 || model_UserLesson.ScoreTime.AddDays(student_score_days) > DateTime.Now)
                {
                    if (model_UserLesson.Score == 0) model_UserLesson.ScoreFirsttime = DateTime.Now;
                    model_UserLesson.ScoreTime = DateTime.Now;
                    model_UserLesson.Score = score;

                    _context.Updateable(model_UserLesson).UpdateColumns(u => u.Score).ExecuteCommand();
				}
				else {
					result.StatusCode = 4007;
					result.Message = _localizer["超过"+ student_score_days + "天,无法修改评价"];
				}
			}
			else
			{
				result.StatusCode = 4007;
				result.Message = _localizer["您没有评价权限"];
			}
			return result;
		}
		#endregion

		#region 上传头像
		/// <summary>
		/// 上传头像
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<FileDto>> UploadHeadImg(IFormFile upfile)
		{
            var result = await _commonBaseService.UpdateFile(upfile, "Img", true);
			if (result.StatusCode == 0) {
				string img = "{local}" + result.Data.Url;//{local}：代表本地服务器的标签
				_context.Updateable<WebUser>().SetColumnsIF(!string.IsNullOrEmpty(result.Data.Url), u => u.HeadImg == img)
					.Where(u => u.Status == 1 && u.Token == _userManage.UserToken).ExecuteCommand();
				result.Data.Url = _commonBaseService.ResourceDomain(result.Data.Url);
			}
			return result;
		}
		#endregion

		#region 我的学习报告
		/// <summary>
		/// 我的定级报告(仅针对试听课)
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<LevelReportDto>>> LevelReportList()
		{
			var result = new ApiResult<List<LevelReportDto>>();
			//var model_token=_userManage.GetUserToken();
			if (_userManage.Userid <= 0) {
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
			var list_LessonReport = _context.Queryable<WebUserLessonReport, WebUserLesson>((l, r) => l.UserLessonid == r.UserLessonid)
				.Where((l, r) => l.Userid == _userManage.Userid && r.Istrial == 1)
				.OrderBy((l, r) => l.Dtime, OrderByType.Desc)
				.Select((l, r) => new 
				{
					UserLessonReportid = l.UserLessonReportid,
					UserLessonid = r.UserLessonid,
					MenkeLessonName = r.MenkeLessonName,
					Level = l.Level,
					Dtime = l.Dtime,
				})
				.ToList();

			result.Data = list_LessonReport.Select(u => new LevelReportDto() {
                UserLessonReportid =u.UserLessonReportid,
                UserLessonid = u.UserLessonid,
                MenkeLessonName = u.MenkeLessonName,
                Level = u.Level,
                Dtime = DateHelper.ConvertDateTimeInt(u.Dtime)
            }).ToList(); ;
			return result;
		}

		/// <summary>
		/// 我的课节报告(针对正价课)
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<LessonReportDto>>> LessonReportList()
		{
			var result = new ApiResult<List<LessonReportDto>>();
			var list_Report = new List<LessonReportDto>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
			var list_UserCourse = _context.Queryable<WebUserCourse>().Where(u => u.Istrial == 0 && u.Userid == _userManage.Userid && u.Status != -1).OrderBy(u => u.UserCourseid, OrderByType.Desc).ToList();
			var list_Sku = _context.Queryable<WebSkuType>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid)
				.Where((l, r) => list_UserCourse.Select(s => s.SkuTypeid).Contains(l.SkuTypeid) && r.Lang == _userManage.Lang)
				.Select((l, r) => new
				{
					l.SkuTypeid,
					l.IcoFont,
					r.Title
				})
				.ToList();
			var list_LessonReport = _context.Queryable<WebUserLessonReport>()
				.LeftJoin<WebUserLesson>((l, r) => l.UserLessonid == r.UserLessonid)
				.LeftJoin<MenkeRecord>((l, r, u) => l.MenkeLessonId == u.MenkeLessonId)
				.Where((l, r) => l.Userid == _userManage.Userid && r.Istrial == 0)
				.OrderBy((l, r, u) => l.Dtime, OrderByType.Desc)
				.Select((l, r, u) => new
				{
					l.MenkeCourseId,
					l.UserLessonReportid,
					r.MenkeStarttime,
					r.MenkeEndtime,
					r.MenkeLessonName,
					l.Dtime,
					r.Score,
					u.MenkeUrl
				})
				.ToList();
			list_UserCourse.ForEach(model_Course =>
			{
				var model_Sku = list_Sku.FirstOrDefault(u => u.SkuTypeid == model_Course.SkuTypeid);
			
				var model_Report = new LessonReportDto();
				model_Report.UserCourseid = model_Course.UserCourseid;
				model_Report.CourseName = model_Course.Title;
				model_Report.ClassHour = model_Course.ClassHour;
				model_Report.Type = model_Sku?.Title;
                model_Report.SkuTypeid = model_Course.SkuTypeid;
                model_Report.IcoFont= model_Sku?.IcoFont;
				foreach (var model in list_LessonReport.Where(u => u.MenkeCourseId == model_Course.MenkeCourseId).ToList())
				{
					model_Report.LessonReports.Add(new LessonReportDto.Report()
					{
						UserLessonReportid = model.UserLessonReportid,
						MenkeStarttime = model.MenkeStarttime,
						MenkeEndtime = model.MenkeEndtime,
						LessonName = model.MenkeLessonName,
						Dtime = DateHelper.ConvertDateTimeInt(model.Dtime),
						IsScore = model.Score > 0 ? 1 : 0,
						RecordUrl = model.MenkeUrl
					});
				}
				list_Report.Add(model_Report) ;
			});
			result.Data = list_Report;
			return result;
		}
		/// <summary>
		/// 课节报告详情
		/// </summary>
		/// <param name="userLessonReportid"></param>
		/// <returns></returns>
		public async Task<ApiResult<LessonReportDetailDto>> LessonReport(long userLessonReportid) {
			var result = new ApiResult<LessonReportDetailDto>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
			var model_UserLessonReport = _context.Queryable<WebUserLessonReport>().First(u=>u.Userid == _userManage.Userid && u.UserLessonReportid == userLessonReportid);
			if (model_UserLessonReport != null)
			{
                var model_UserLesson = _context.Queryable<WebUserLesson>().InSingle(model_UserLessonReport.UserLessonid);
                if (model_UserLesson == null)
                {
                    result.StatusCode = 4009;
                    result.Message = _localizer["课节不存在"];
                    return result;
                }
                var model_Teacher = _context.Queryable<WebTeacher>().InSingle(model_UserLesson.Teacherid);
                string head_img = (model_Teacher != null && !string.IsNullOrEmpty(model_Teacher.HeadImg)) ? model_Teacher.HeadImg : "/Upfile/images/teacher_none.png";
                
                var model_Record = _context.Queryable<MenkeRecord>().First(u => u.MenkeLessonId == model_UserLessonReport.MenkeLessonId);
				result.Data = new LessonReportDetailDto()
				{
					UserLessonReportid = model_UserLessonReport.Userid,
					MenkeStarttime = model_UserLesson.MenkeStarttime,
					MenkeEndtime = model_UserLesson.MenkeEndtime,
					LessonName = model_UserLesson.MenkeLessonName,
					Teacherid = model_UserLessonReport.Teacherid,
					Message = model_UserLessonReport.Message,
					Homework = model_UserLessonReport.Homework,
					Attention = model_UserLessonReport.Attention,
					Enthusiasm = model_UserLessonReport.Enthusiasm,
					Hear = model_UserLessonReport.Hear,
					Say = model_UserLessonReport.Say,
					Read = model_UserLessonReport.Read,
					Write = model_UserLessonReport.Write,
					Thinking = model_UserLessonReport.Thinking,
					EmotionalQuotient = model_UserLessonReport.EmotionalQuotient,
					LoveQuotient = model_UserLessonReport.LoveQuotient,
					InverseQuotient = model_UserLessonReport.InverseQuotient,
					Performance = model_UserLessonReport.Performance,
					RecordUrl = model_Record?.MenkeUrl+"",
					TeacherName = model_UserLesson.MenkeTeacherName,
                    TeacherHeadImg = _commonBaseService.ResourceDomain(head_img),
					IsTrial=model_UserLesson.Istrial,
					Level= model_UserLessonReport.Level,
                    Dtime=DateHelper.ConvertDateTimeInt(model_UserLessonReport.Dtime)
                };
			}
			else
			{
				result.StatusCode = 4007;
				result.Message = _localizer["报告未生成"];
			}
			return result;
		}
		#endregion

		#region 我的课程（众语课程）
		/// <summary>
		/// 正式课列表（众语课程）
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<UserCourseDto>>> UserCourseList()
		{
			var result = new ApiResult<List<UserCourseDto>>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid<=0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
			var list = new List<UserCourseDto>();
			var list_UserCourse = _context.Queryable<WebUserCourse>()
				.Where(u=> u.Istrial == 0 && u.Status>=1 && u.Userid== _userManage.Userid && u.Type == 0)
				.OrderBy(u=>u.UserCourseid, OrderByType.Desc)
				.ToList();
			var list_SkuType = _context.Queryable<WebSkuTypeLang>().Where(u=>u.Lang==_userManage.Lang).ToList();
			var list_Course = _context.Queryable<WebCourse>().Where(u => list_UserCourse.Select(s => s.Courseid).Contains(u.Courseid)).Select(u => new { u.Courseid, u.Img }).ToList();
			foreach (var model_UserCourse in list_UserCourse)
			{
				var model_SkuType = list_SkuType.FirstOrDefault(u => u.SkuTypeid == model_UserCourse.SkuTypeid);
				var model_Course = list_Course.FirstOrDefault(u => u.Courseid == model_UserCourse.Courseid);

                list.Add(new UserCourseDto()
				{
					UserCourseid = model_UserCourse.UserCourseid,
					Courseid = model_UserCourse.Courseid,
					Status = model_UserCourse.Status,
                    Message = model_UserCourse.Message,
					Img = _commonBaseService.ResourceDomain(model_Course?.Img?? model_UserCourse.Img + ""),
					Title = model_UserCourse.Title,
					Type = model_SkuType?.Title,
                    SkuTypeid = model_UserCourse.SkuTypeid,
					ClassHour = model_UserCourse.ClassHour,
					Classes = model_UserCourse.Classes
				}) ;
			}
			result.Data = list;
			return result;
        }
        /// <summary>
        /// 我的课详情
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<UserCourseDto>> UserCourse(long userCourseid)
        {
            var result = new ApiResult<UserCourseDto>();
            //var model_token = _userManage.GetUserToken();
            if (_userManage.Userid <= 0)
            {
                result.StatusCode = 4001;
                result.Message = _localizer["用户登录超时退出"];
                return result;
            }
			var model_UserCourse = _context.Queryable<WebUserCourse>().First(u => u.Istrial == 0 && u.UserCourseid == userCourseid && u.Status != -1);
			if (model_UserCourse != null)
			{
				var model_Course = _context.Queryable<WebCourse>().InSingle(model_UserCourse.Courseid);
                var model_SkuType = _context.Queryable<WebSkuTypeLang>().First(u => u.Lang == _userManage.Lang && u.SkuTypeid == model_UserCourse.SkuTypeid);
                var model = new UserCourseDto();
				model.Courseid = model_UserCourse.Courseid;
                model.UserCourseid = userCourseid;
                model.Title= model_UserCourse.Title;
				model.Message = model_UserCourse.Message;
                model.Img = _commonBaseService.ResourceDomain(model_UserCourse.Img + "");
				model.Img = _commonBaseService.ResourceDomain(model_Course?.Img ?? model_UserCourse.Img + "");
				model.Type = model_SkuType?.Title;
				model.SkuTypeid = model_UserCourse.SkuTypeid;
				model.ClassHour = model_UserCourse.ClassHour;
				model.Classes = model_UserCourse.Classes;
				var model_CourseLang = _context.Queryable<WebCourseLang>().First(u=>u.Lang==_userManage.Lang && u.Courseid == model_UserCourse.Courseid);
				if (model_CourseLang != null && !string.IsNullOrEmpty(model_CourseLang.Keys))
				{
					var keys = model_CourseLang.Keys.Split(",").Where(u=>!string.IsNullOrEmpty(u)).ToList();
                    model.Keys = keys;
				}
				result.Data = model;
            }
			else {
                result.StatusCode = 4009;
                result.Message = _localizer["课程不存在"];
                return result;
            }
            return result;
        }

        /// <summary>
        /// 正式课课节列表
        /// </summary>
        /// <param name="startStatus">开始状态：0未开始，1已开始，2已结束</param>
        /// <param name="endStatus">结束状态（当startStatus==2时有效）：0全部，1已完成，2未完成/缺席,3已取消</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ApiResult<Pages<UserLessonDto>>> UserLessonList(long userCourseid, int startStatus, int endStatus, int page=1,int pageSize=10)
		{
			var result = new ApiResult<Pages<UserLessonDto>>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
			var list = new List<UserLessonDto>();
			var exp = new Expressionable<WebUserLesson>();
			exp.And(u => u.Status == 1 && u.MenkeDeleteTime == 0 && u.Userid == _userManage.Userid && u.Istrial == 0 && u.UserCourseid>0 && u.UserCourseid== userCourseid);
			var thetime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
			switch (startStatus) {
				case 0://0未开始
                    //exp.And(u => u.MenkeStarttime > thetime && u.MenkeState==1);
                    exp.And(u => u.MenkeState == 1 && u.MenkeEndtime > thetime);
                    break;
				case 1://1已开始
                    //exp.And(u => (u.MenkeStarttime <= thetime && u.MenkeEndtime>=thetime) || u.MenkeState==2);
                    exp.And(u => u.MenkeState == 2);
                    break;
				case 2://2已结束
                    //exp.And(u => u.MenkeEndtime < thetime ||u.MenkeState==3 ||u.MenkeState==4);
                    exp.And(u =>u.MenkeState == 3 || u.MenkeState == 4 ||(u.MenkeState == 1 && u.MenkeEndtime<thetime));
                    break;
			}
			if (startStatus == 2)
			{
                //课次状态（1未开始2进行中3已结课4已过期）
				switch (endStatus)
				{
					case 0://0全部
						break;
					case 1://1已完成
						exp.And(u => u.MenkeState==3);
						break;
					case 2://2未完成/缺席
						exp.And(u => u.MenkeState ==4 || (u.MenkeState == 1 && u.MenkeEndtime < thetime));
						break;
					case 3://3已取消
						exp.And(u => u.MenkeDeleteTime>0);
						break;
				}
			}
			int total = 0;
            var list_UserLesson = _context.Queryable<WebUserLesson>()
				.Where(exp.ToExpression())
				.OrderBy(u => u.MenkeStarttime)
				.ToPageList(page,pageSize,ref total);
			var list_SkuType = _context.Queryable<WebSkuTypeLang>().Where(u => u.Lang == _userManage.Lang).ToList();
			var list_Homework = _context.Queryable<MenkeHomework>().Where(u => list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
			var list_Report = _context.Queryable<WebUserLessonReport>().Where(u =>u.Userid== _userManage.Userid && list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
            var list_RecordUrl = _context.Queryable<MenkeRecord>().Where(u => list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();

            var dic_config = _pubConfigBaseService.GetConfigs("cancel_hour,classroom_hour");
			double cancel_hour = dic_config.ContainsKey("cancel_hour") ? double.Parse(dic_config["cancel_hour"]) : 0;
			double classroom_hour = dic_config.ContainsKey("classroom_hour") ? double.Parse(dic_config["classroom_hour"]) : 0;
			foreach (var model_UserLesson in list_UserLesson)
			{
				var model_SkuType = list_SkuType.FirstOrDefault(u => u.SkuTypeid == model_UserLesson.SkuTypeid);
				int isCancel = (model_UserLesson.MenkeStarttime > DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddHours(cancel_hour))) ? 1 : 0;
				string menkeEntryurl = (DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddHours(classroom_hour)) >= model_UserLesson.MenkeStarttime && DateHelper.ConvertDateTimeInt(DateTime.UtcNow) <=model_UserLesson.MenkeEndtime) ?model_UserLesson.MenkeEntryurl :"";
				if (startStatus == 0 || startStatus == 1)
                {
					list.Add(new UserLessonDto()
					{
						UserLessonid = model_UserLesson.UserLessonid,
						LessonName = model_UserLesson.MenkeLessonName,
						MenkeStarttime = model_UserLesson.MenkeStarttime,
						MenkeEndtime = model_UserLesson.MenkeEndtime,
						Type = model_SkuType?.Title,
						IsCancel = isCancel,//控制申请取消
						MenkeEntryurl = menkeEntryurl,//进入教室
                        ClassroomMin = (int)(classroom_hour * 60),
						Istrial = 0,
						MenkeState = (model_UserLesson.MenkeState==1 && model_UserLesson.MenkeEndtime < thetime) ? 4 : model_UserLesson.MenkeState
					});
                }
				else
                {
                    int isscore = 0;
                    if (model_UserLesson.Score > 0) isscore = 1;
					int student_score_days = _pubConfigBaseService.GetConfigInt("student_score_days", 7);
                    if (model_UserLesson.ScoreTime.AddDays(student_score_days) < DateTime.Now) isscore = 2;
					var model_report = list_Report.FirstOrDefault(u => u.MenkeLessonId == model_UserLesson.MenkeLessonId);

					list.Add(new UserLessonDto()
					{
						UserLessonid = model_UserLesson.UserLessonid,
						LessonName = model_UserLesson.MenkeLessonName,
						MenkeStarttime = model_UserLesson.MenkeStarttime,
						MenkeEndtime = model_UserLesson.MenkeEndtime,
						Type = model_SkuType?.Title,
						IsScore = isscore,
						IsHomeWork = list_Homework.Any(u => u.MenkeLessonId == model_UserLesson.MenkeLessonId) ? 1 : 0,
						IsReport = model_report!=null ? 1 : 0,
						UserLessonReportid = model_report != null ? model_report.UserLessonReportid:0,
						RecordUrl = list_RecordUrl.FirstOrDefault(u=>u.MenkeLessonId==model_UserLesson.MenkeLessonId)?.MenkeUrl,
						ClassroomMin = (int)(classroom_hour * 60),
                        Istrial = 0,
						MenkeState = (model_UserLesson.MenkeState == 1 && model_UserLesson.MenkeEndtime < thetime) ? 4 : model_UserLesson.MenkeState
					}) ;
                }
			}
			result.Data = new Pages<UserLessonDto>();
			result.Data.Total = total;
			result.Data.List = list;
			return result;
		}

		/// <summary>
		/// 试听课(课节列表)
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<TrialUserLessonDto>>> TrialUserLessonList()
		{
			var result = new ApiResult<List<TrialUserLessonDto>>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
			var thetime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
			var list = new List<TrialUserLessonDto>();
			var list_UserLesson = _context.Queryable<WebUserLesson>()
				.Where(u => u.Status == 1 && u.Userid == _userManage.Userid && u.MenkeDeleteTime == 0 && u.Istrial==1)
				.OrderBy(u => u.MenkeStarttime)
				.ToList();
			var list_SkuType = _context.Queryable<WebSkuTypeLang>().Where(u => u.Lang == _userManage.Lang).ToList();
			foreach (var model_UserLesson in list_UserLesson)
			{
				var model_SkuType = list_SkuType.FirstOrDefault(u => u.SkuTypeid == model_UserLesson.SkuTypeid);

				list.Add(new TrialUserLessonDto()
				{
					UserLessonid = model_UserLesson.UserLessonid,
					LessonName = model_UserLesson.MenkeLessonName,
					MenkeStarttime = model_UserLesson.MenkeStarttime,
					MenkeEndtime = model_UserLesson.MenkeEndtime,
					Type = model_SkuType?.Title,
					MenkeState = (model_UserLesson.MenkeState == 1 && model_UserLesson.MenkeEndtime < thetime) ? 4 : model_UserLesson.MenkeState
				});
			}
			result.Data = new List<TrialUserLessonDto>();
			result.Data = list;
            return result;
		}

		/// <summary>
		/// 课节详情
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<UserLessonDto>> UserLessonDetail(long userLessonid)
		{
			var result = new ApiResult<UserLessonDto>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
			var thetime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
			var model = new UserLessonDto();
			var model_UserLesson = _context.Queryable<WebUserLesson>().InSingle(userLessonid);
			var list_SkuType = _context.Queryable<WebSkuTypeLang>().Where(u => u.Lang == _userManage.Lang).ToList();
			if(model_UserLesson!=null)
			{
				var model_Course=_context.Queryable<WebCourseLang>().First(u=>u.Courseid == model_UserLesson.Courseid && u.Lang==_userManage.Lang);
				var model_SkuType = _context.Queryable<WebSkuTypeLang>().First(u => u.Lang == _userManage.Lang && u.SkuTypeid == model_UserLesson.SkuTypeid);
				var model_Teacher = _context.Queryable<WebTeacherLang>().LeftJoin<WebTeacher>((l,r)=>l.Teacherid==r.Teacherid)
					.Where((l,r) => l.Lang == _userManage.Lang && l.Teacherid == model_UserLesson.Teacherid)
					.Select((l,r)=>new { r.HeadImg, l.Teacherid,l.Message,l.Keys,l.Name }).First();
                var dic_config = _pubConfigBaseService.GetConfigs("cancel_hour,classroom_hour");
                double cancel_hour = dic_config.ContainsKey("cancel_hour") ? double.Parse(dic_config["cancel_hour"]) : 0;
                double classroom_hour = dic_config.ContainsKey("classroom_hour") ? double.Parse(dic_config["classroom_hour"]) : 0;
                int isCancel = (model_UserLesson.MenkeStarttime > DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddHours(cancel_hour))) ? 1 : 0;
                string menkeEntryurl = (DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddHours(classroom_hour)) >= model_UserLesson.MenkeStarttime && DateHelper.ConvertDateTimeInt(DateTime.UtcNow) <= model_UserLesson.MenkeEndtime) ? model_UserLesson.MenkeEntryurl : "";

                string head_img = (model_Teacher != null && !string.IsNullOrEmpty(model_Teacher.HeadImg)) ? model_Teacher.HeadImg : "/Upfile/images/teacher_none.png";
                var teacher_keys = new List<string>();
				if (model_Teacher != null) {
					if (!string.IsNullOrEmpty(model_Teacher.Keys)) {
						teacher_keys = model_Teacher.Keys.Split(",").Where(u => !string.IsNullOrEmpty(u)).ToList();
					}
                }

				var model_report = _context.Queryable<WebUserLessonReport>().First(u => u.MenkeLessonId == model_UserLesson.MenkeLessonId);
				model = new UserLessonDto()
				{
					UserLessonid = model_UserLesson.UserLessonid,
					CourseName = model_Course?.Title+"",
					LessonName = model_UserLesson.MenkeLessonName,
					MenkeStarttime = model_UserLesson.MenkeStarttime,
					MenkeEndtime = model_UserLesson.MenkeEndtime,
					Type = model_SkuType?.Title+"",
					MenkeState = (model_UserLesson.MenkeState == 1 && model_UserLesson.MenkeEndtime < thetime) ? 4 : model_UserLesson.MenkeState,
					Teacherid =model_UserLesson.Teacherid,
                    TeacherHeadImg = _commonBaseService.ResourceDomain(head_img),
                    TeacherName =model_UserLesson.MenkeTeacherName,
					TeacherMessage= model_Teacher?.Message+"",
                    TeacherKeys = teacher_keys,
                    IsCancel = isCancel,
                    ClassroomMin = (int)(classroom_hour * 60),
                    MenkeEntryurl = menkeEntryurl,
                    IsScore = model_UserLesson.Score > 0 ? 1 : 0,
                    IsHomeWork = _context.Queryable<MenkeHomework>().Any(u => u.MenkeLessonId == model_UserLesson.MenkeLessonId) ? 1 : 0,
                    IsReport = model_report !=null? 1 : 0,
					UserLessonReportid= model_report != null?model_report.UserLessonReportid:0,
					RecordUrl = _context.Queryable<MenkeRecord>().First(u => u.MenkeLessonId == model_UserLesson.MenkeLessonId)?.MenkeUrl,
                    Istrial = model_UserLesson.Istrial
                };
			}
			result.Data = model;
			return result;
		}
		#endregion

		#region "我的课程(直播课)"
		/// <summary>
		/// 我的课程(直播课)
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<OnlineCourseDto>>> MyOnlineCourseList()
		{
			var result = new ApiResult<List<OnlineCourseDto>>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
			var list = new List<OnlineCourseDto>();
			var list_UserCourse = _context.Queryable<WebUserCourse>()
				.Where(u => u.Istrial == 0 && u.Status >= 1 && u.Userid == _userManage.Userid && u.Type == 1)
				.OrderBy(u => u.UserCourseid, OrderByType.Desc)
				.ToList();
			var list_Course = _context.Queryable<WebOnlineCourse>()
				.Where(u => list_UserCourse.Select(s => s.OnlineCourseid).Contains(u.OnlineCourseid))
				.Select(u => new { u.OnlineCourseid,u.LessonCount,u.LessonStart,u.StudentCount, u.Img }).ToList();
			foreach (var model_UserCourse in list_UserCourse)
			{
				var model_Course = list_Course.FirstOrDefault(u => u.OnlineCourseid == model_UserCourse.OnlineCourseid);
				list.Add(new OnlineCourseDto()
				{
					UserCourseid = model_UserCourse.UserCourseid,
					OnlineCourseid = model_UserCourse.OnlineCourseid,
					Status = model_UserCourse.Status,
					Message = model_UserCourse.Message,
					Img = _commonBaseService.ResourceDomain(model_Course?.Img ?? model_UserCourse.Img + ""),
					Title = model_UserCourse.Title,
					LessonStart = model_Course?.LessonStart,
					LessonCount = model_Course?.LessonCount??0,
					StudentCount = model_Course?.StudentCount??0
				});
			}
			result.Data = list;
			return result;
		}
		#endregion

		#region "我的课程(录播课)"
		/// <summary>
		/// 我的课程(录播课)
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<MyRecordCourseDto>>> MyRecordCourseList()
		{
			var result = new ApiResult<List<MyRecordCourseDto>>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
			var list = new List<MyRecordCourseDto>();
			var list_RecordCourse = _context.Queryable<WebOrder>().LeftJoin<WebRecordCourse>((l,r)=>l.RecordCourseid==r.RecordCourseid)
				.Where((l,r) =>l.Status == 1 && l.Userid == _userManage.Userid && l.RecordCourseid > 0 && l.Type == 2)
				.OrderBy((l,r) =>l.Orderid , OrderByType.Desc)
				.Select((l,r)=>new {l.Orderid, r.RecordCourseid,r.Img,r.LessonCount,r.StudentCount,r.Teacherid})
				.ToList();
			var list_Teacher = _context.Queryable<WebTeacher>()
				.LeftJoin<WebTeacherLang>((l,r)=>l.Teacherid==r.Teacherid && r.Lang == _userManage.Lang)
				.Where((l,r) => list_RecordCourse.Select(s=>s.Teacherid).Contains(l.Teacherid))
				.Select((l,r)=>new { l.Teacherid,r.Name})
				.ToList();
			var list_RecordCourseLang = _context.Queryable<WebRecordCourseLang>()
				.Where(u=>list_RecordCourse.Select(s=>s.RecordCourseid).Contains(u.RecordCourseid) && u.Lang==_userManage.Lang)
				.ToList();
			var list_RecordInfo = _context.Queryable<WebRecordCourseInfo>().LeftJoin<WebRecordCourseInfoLang>((l, r) => l.RecordCourseInfoid == r.RecordCourseInfoid && r.Lang == _userManage.Lang)
				.Where((l, r) => list_RecordCourse.Select(s => s.RecordCourseid).Contains(l.RecordCourseid))
				.OrderBy((l,r) => l.Sort)
				.OrderBy((l,r)=>l.RecordCourseInfoid,OrderByType.Desc)
				.Select((l, r) => new MyRecordCourseDto.Info
				{
					RecordCourseid = l.RecordCourseid,
					Title = r.Title,
					Video = l.Video,
					Duration = l.Duration,
					ViewCount = l.ViewCount
				})
				.ToList();
			foreach (var model_Course in list_RecordCourse)
			{
				var model_Lang = list_RecordCourseLang.FirstOrDefault(u => u.RecordCourseid == model_Course.RecordCourseid);
				var model_Teacher = list_Teacher.FirstOrDefault(u => u.Teacherid == model_Course.Teacherid);
				var videos = list_RecordInfo.Where(u => u.RecordCourseid == model_Course.RecordCourseid).Select(u => new MyRecordCourseDto.Info
				{
					RecordCourseid = u.RecordCourseid,
					Title = u.Title,
					Video = _commonBaseService.ResourceDomain(u.Video),
					Duration = u.Duration,
					ViewCount = u.ViewCount
				}).ToList();
				list.Add(new MyRecordCourseDto()
				{
					Orderid = model_Course.Orderid,
					RecordCourseid = model_Course.RecordCourseid,
					Img = _commonBaseService.ResourceDomain(model_Course.Img + ""),
					Message = model_Lang?.Message + "",
					Title = model_Lang?.Title + "",
					LessonCount = model_Course.LessonCount,
					StudentCount = model_Course.StudentCount,
					Teacherid = model_Course.Teacherid,
					TeacherName = model_Teacher?.Name + "",
					Keys = (model_Lang?.Keys+"").Split(',').ToList(),
					Videos = videos
				}) ;
			}
			result.Data = list;
			return result;
		}

		/// <summary>
		/// 我的录播课详情
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<MyRecordCourseDto>> MyRecordCourse(long orderid)
		{
			var result = new ApiResult<MyRecordCourseDto>();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
			var model_Order = _context.Queryable<WebOrder>().First(u=>u.Orderid == orderid && u.Type==2);
			if (model_Order == null || model_Order.RecordCourseid == 0) {
				result.StatusCode = 4003;
				result.Message = _localizer["此录播课不存在"];
				return result;
			}

			var model_RecordCourse = _context.Queryable<WebRecordCourse>().LeftJoin<WebRecordCourseLang>((l,r)=>l.RecordCourseid==r.RecordCourseid && r.Lang == _userManage.Lang)
				.Where((l,r) => l.RecordCourseid == model_Order.RecordCourseid && l.Status != -1)
				.Select((l,r)=>new { 
				l.RecordCourseid,r.Title,r.Message,l.Img,l.Teacherid,l.LessonCount,l.StudentCount,r.Keys
				}).First();
			if (model_RecordCourse != null)
			{
				var model_Teacher = _context.Queryable<WebTeacherLang>().First(u => u.Teacherid == model_RecordCourse.Teacherid && u.Lang==_userManage.Lang);
				var videos = _context.Queryable<WebRecordCourseInfo>().LeftJoin<WebRecordCourseInfoLang>((l, r) => l.RecordCourseInfoid == r.RecordCourseInfoid && r.Lang == _userManage.Lang)
					.Where((l, r) => l.RecordCourseid == model_RecordCourse.RecordCourseid)
					.Select((l, r) => new 
					{
						RecordCourseid = l.RecordCourseid,
						Title = r.Title,
						Video = l.Video,
						Duration = l.Duration,
						ViewCount = l.ViewCount
					}).ToList().Select(u=>new MyRecordCourseDto.Info
                    {
                        RecordCourseid = u.RecordCourseid,
                        Title = u.Title,
                        Video = _commonBaseService.ResourceDomain(u.Video),
                        Duration = u.Duration,
                        ViewCount = u.ViewCount
                    }).ToList();
				var model = new MyRecordCourseDto();
				model.Orderid = orderid;
				model.RecordCourseid = model_RecordCourse.RecordCourseid;
				model.Img = _commonBaseService.ResourceDomain(model_RecordCourse.Img + "");
				model.Message = model_RecordCourse.Message + "";
				model.Title = model_RecordCourse.Title + "";
				model.LessonCount = model_RecordCourse.LessonCount;
				model.StudentCount = model_RecordCourse.StudentCount;
				model.Teacherid = model_RecordCourse.Teacherid;
				model.TeacherName = model_Teacher?.Name + "";
				model.Videos = videos;
				model.Keys = (model_RecordCourse.Keys + "").Split(',').ToList();
				result.Data = model;
			}
			else
			{
				result.StatusCode = 4009;
				result.Message = _localizer["课程不存在"];
				return result;
			}
			return result;
		}
		#endregion

		#region "我的课程(线下课)"
		/// <summary>
		/// 我的课程(线下课)
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<MyOfflineCourseDto>>> MyOfflineCourseList()
		{
			var result = new ApiResult<List<MyOfflineCourseDto>>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时退出"];
				return result;
			}
			var list = new List<MyOfflineCourseDto>();
			var list_OfflineCourse = _context.Queryable<WebOrder>().LeftJoin<WebOfflineCourse>((l, r) => l.OfflineCourseid == r.OfflineCourseid)
				.Where((l, r) => l.Status == 1 && l.Userid == _userManage.Userid && l.OfflineCourseid > 0 && l.Type == 3)
				.OrderBy((l, r) => l.Orderid, OrderByType.Desc)
				.Select((l, r) => new { r.OfflineCourseid, r.Img, r.LessonCount, r.StudentCount, r.LessonStart, r.Teacherid })
				.ToList();
			var list_Teacher = _context.Queryable<WebTeacher>()
				.LeftJoin<WebTeacherLang>((l, r) => l.Teacherid == r.Teacherid && r.Lang == _userManage.Lang)
				.Where((l, r) => list_OfflineCourse.Select(s => s.Teacherid).Contains(l.Teacherid))
				.Select((l, r) => new { l.Teacherid, r.Name })
				.ToList();
			var list_RecordCourseLang = _context.Queryable<WebOfflineCourseLang>()
				.Where(u => list_OfflineCourse.Select(s => s.OfflineCourseid).Contains(u.OfflineCourseid) && u.Lang == _userManage.Lang)
				.ToList();
			foreach (var model_Course in list_OfflineCourse)
			{
				var model_Lang = list_RecordCourseLang.FirstOrDefault(u => u.OfflineCourseid == model_Course.OfflineCourseid);
				var model_Teacher = list_Teacher.FirstOrDefault(u => u.Teacherid == model_Course.Teacherid);
				list.Add(new MyOfflineCourseDto()
				{
					OfflineCourseid = model_Course.OfflineCourseid,
					Img = _commonBaseService.ResourceDomain(model_Course.Img + ""),
					Message = model_Lang?.Message + "",
					Title = model_Lang?.Title + "",
					LessonCount = model_Course.LessonCount,
					StudentCount = model_Course.StudentCount,
					LessonStart = model_Course.LessonStart,
					Teacherid = model_Course.Teacherid,
					TeacherName = model_Teacher?.Name + ""
				});
			}
			result.Data = list;
			return result;
		}
		#endregion

		#region 家庭作业详情
		/// <summary>
		/// 家庭作业详情
		/// </summary>
		/// <param name="userLessonid">课节ID</param>
		/// <returns></returns>
		public async Task<ApiResult<HomeworkDto>> HomeworkDetail(long userLessonid)
		{
			var result = new ApiResult<HomeworkDto>();
            var model_token = _userManage.GetUserToken();
            if (model_token == null)
            {
                result.StatusCode = 4001;
                result.Message = _localizer["用户登录超时退出"];
                return result;
            }
			var model_Homework = _context.Queryable<MenkeHomework>().LeftJoin<WebUserLesson>((l, r) => l.MenkeLessonId == r.MenkeLessonId)
				.Where((l, r) => r.UserLessonid == userLessonid && r.Userid == model_token.Userid && l.MenkeDeleteTime==0)
				.Select((l,r)=>new { l,r.MenkeLessonName,r.MenkeStarttime,r.MenkeEndtime,r.Teacherid,r.Courseid})
				.First();
			if (model_Homework != null)
            {
                var model_Record = _context.Queryable<MenkeRecord>().First(u => u.MenkeLessonId == model_Homework.l.MenkeLessonId);
                var list_HomeworkSubmit = _context.Queryable<MenkeHomeworkSubmit>().Where(u =>u.MenkeSubmitDelTime==0 && u.MenkeHomeworkId == model_Homework.l.MenkeHomeworkId && u.MenkeStudentCode==model_token.MobileCode && u.MenkeStudentMobile ==model_token.Mobile).ToList();
				var list_HomeworkRemark = _context.Queryable<MenkeHomeworkRemark>().Where(u => u.MenkeRemarkDelTime == 0 && u.MenkeHomeworkId == model_Homework.l.MenkeHomeworkId && list_HomeworkSubmit.Select(s => s.MenkeSubmitId).Contains(u.MenkeSubmitId)).ToList();
				var model_Course = _context.Queryable<WebCourseLang>().First(u=>u.Courseid == model_Homework.Courseid && u.Lang== _userManage.Lang);
                var model = new HomeworkDto();
				model.Homeworkid = model_Homework.l.MenkeHomeworkId;
				model.CourseName = model_Course?.Title+"";
				model.LessonName = model_Homework.MenkeLessonName;
				model.MenkeStarttime = model_Homework.MenkeStarttime;
				model.MenkeEndtime = model_Homework.MenkeEndtime;
				model.Title = model_Homework.l.MenkeTitle;
				model.Content = model_Homework.l.MenkeContent;
				model.SubmitWay = model_Homework.l.MenkeSubmitWay;
				model.EndDate = model_Homework.l.MenkeEndDate;
				if (!string.IsNullOrEmpty(model_Homework.l.MenkeResources))
				{
					var jarray = JArray.Parse(model_Homework.l.MenkeResources);
					foreach (var item in jarray)
					{
						model.Resources.Add(new HomeworkDto.Attachment()
						{
							AttachmentId = (item["id"] != null ? (int)item["id"] : 0),
							AttachmentUrl = (item["url"] != null ? item["url"].ToString() : ""),
							AttachmentPreviewUrl = (item["preview_url"] != null ? item["preview_url"].ToString() : ""),
                            Size = (item["size"] != null ? (int)item["size"] : 0),
                            AttachmentType = (item["type"] != null ? item["type"].ToString() : "")
                        });
					}
                }
				model.RecordUrl= model_Record?.MenkeUrl?? string.Empty;
				model.IsSubmit = list_HomeworkSubmit.Count > 0 ? 1 : 0;
				var model_Teacher = _context.Queryable<WebTeacherLang>().LeftJoin<WebTeacher>((l, r) => l.Teacherid == r.Teacherid)
					.Where((l, r) => l.Lang == _userManage.Lang && l.Teacherid == model_Homework.Teacherid)
					.Select((l, r) => new { l.Teacherid, l.Name, r.HeadImg, l.Keys }).First();
				foreach (var model_HomeworkSubmit in list_HomeworkSubmit)
				{
					var model_Sub = new HomeworkDto.SubmitHomework();
					model_Sub.SubmitContent = model_HomeworkSubmit.MenkeSubmitContent;
					model_Sub.SubmitTime = model_HomeworkSubmit.MenkeSubmitTime;
                    if (!string.IsNullOrEmpty(model_HomeworkSubmit.MenkeSubmitFiles))
                    {
                        var jarray = JArray.Parse(model_HomeworkSubmit.MenkeSubmitFiles);
                        foreach (var item in jarray)
                        {
							model_Sub.SubmitFiles.Add(new HomeworkDto.Attachment()
                            {
                                AttachmentId = (item["id"] != null ? (int)item["id"] : 0),
                                AttachmentUrl = (item["url"] != null ? item["url"].ToString() : ""),
                                AttachmentPreviewUrl = (item["preview_url"] != null ? item["preview_url"].ToString() : ""),
                                Size = (item["size"] != null ? (int)item["size"] : 0),
                                AttachmentType = (item["type"] != null ? item["type"].ToString() : "")
                            });
                        }
                    }

					model_Sub.Teacherid = model_Homework.Teacherid;
					if (model_Teacher != null)
					{
						model_Sub.TeacherName = model_Teacher.Name;
						string head_img = (model_Teacher != null && !string.IsNullOrEmpty(model_Teacher.HeadImg)) ? model_Teacher.HeadImg : "/Upfile/images/teacher_none.png";
						model_Sub.TeacherHeadImg = _commonBaseService.ResourceDomain(head_img);
						if (!string.IsNullOrEmpty(model_Teacher.Keys))
						{
							model_Sub.TeacherKeys = model_Teacher.Keys.Split(",").Where(u => !string.IsNullOrEmpty(u)).ToList();
						}
					}
					var model_HomeworkRemark = list_HomeworkRemark.FirstOrDefault(u => u.MenkeSubmitId == model_HomeworkSubmit.MenkeSubmitId);
					if (model_HomeworkRemark != null)
					{
						model_Sub.IsRemark = 1; 
						model_Sub.RemarkTime = model_HomeworkRemark.MenkeRemarkTime;
                        model_Sub.RemarkContent = model_HomeworkRemark.MenkeRemarkContent;
						if (!string.IsNullOrEmpty(model_HomeworkRemark.MenkeRemarkFiles))
						{
							var jarray = JArray.Parse(model_HomeworkRemark.MenkeRemarkFiles);
							foreach (var item in jarray)
							{
								model_Sub.RemarkFiles.Add(new HomeworkDto.Attachment()
								{
									AttachmentId = (item["id"] != null ? (int)item["id"] : 0),
									AttachmentUrl = (item["url"] != null ? item["url"].ToString() : ""),
									AttachmentPreviewUrl = (item["preview_url"] != null ? item["preview_url"].ToString() : ""),
									Size = (item["size"] != null ? (int)item["size"] : 0),
									AttachmentType = (item["type"] != null ? item["type"].ToString() : "")
								});
							}
						}
						model_Sub.RemarkIsPass = model_HomeworkRemark.MenkeIsPass;
						model_Sub.RemarkRank = model_HomeworkRemark.MenkeRank;
					}
					else {
						model_Sub.IsRemark = 0;
					}

					model.SubmitHomeworks.Add(model_Sub);

                }
				
                result.Data = model;
            }
			else
            {
                result.StatusCode = 4009;
                result.Message = _localizer["家庭作业不存在"];
            }
            return result;
		}
        #endregion

        #region 我的课表
        /// <summary>
        /// 我的课表列表
        /// </summary>
        /// <param name="searchDay">搜索日期</param>
        /// <param name="menkeState">课节状态（0全部1未开始2进行中3已结课4已过期）</param>
        /// <returns></returns>
        public async Task<ApiResult<List<UserLessonDto>>> MySchedule(int begintime,int endtime, int menkeState=0)
        {
            var result = new ApiResult<List<UserLessonDto>>();
            //var model_token = _userManage.GetUserToken();
            if (_userManage.Userid <= 0)
            {
                result.StatusCode = 4001;
                result.Message = _localizer["用户登录超时退出"];
                return result;
            }
			var thetime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);

			var list = new List<UserLessonDto>();
            var exp = new Expressionable<WebUserLesson>();
            exp.And(u => u.Status == 1 && u.Userid == _userManage.Userid && u.MenkeDeleteTime==0 && u.MenkeStarttime >=begintime && u.MenkeStarttime<= endtime);
			if (menkeState == 4)
			{
				exp.And(u => u.MenkeState == menkeState ||(u.MenkeState==1 && u.MenkeEndtime< thetime));
			}
			else
			{
				exp.AndIF(menkeState > 0, u => u.MenkeState == menkeState);
			}
            var list_UserLesson = _context.Queryable<WebUserLesson>()
                .Where(exp.ToExpression())
                .OrderBy(u => u.MenkeStarttime, OrderByType.Desc)
                .ToList();
            var list_SkuType = _context.Queryable<WebSkuTypeLang>().Where(u => u.Lang == _userManage.Lang).ToList();
            var list_Homework = _context.Queryable<MenkeHomework>().Where(u => list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
            var list_Report = _context.Queryable<WebUserLessonReport>().Where(u => u.Userid == _userManage.Userid && list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
            var list_RecordUrl = _context.Queryable<MenkeRecord>().Where(u => list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();

            var dic_config = _pubConfigBaseService.GetConfigs("cancel_hour,classroom_hour");
            double cancel_hour = dic_config.ContainsKey("cancel_hour") ? double.Parse(dic_config["cancel_hour"]) : 0;
            double classroom_hour = dic_config.ContainsKey("classroom_hour") ? double.Parse(dic_config["classroom_hour"]) : 0;
            foreach (var model_UserLesson in list_UserLesson)
            {
                var model_SkuType = list_SkuType.FirstOrDefault(u => u.SkuTypeid == model_UserLesson.SkuTypeid);
                int isCancel = (model_UserLesson.MenkeStarttime > DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddHours(cancel_hour))) ? 1 : 0;
                string menkeEntryurl = (DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddHours(classroom_hour)) >= model_UserLesson.MenkeStarttime && DateHelper.ConvertDateTimeInt(DateTime.UtcNow) <= model_UserLesson.MenkeEndtime) ? model_UserLesson.MenkeEntryurl : "";

				list.Add(new UserLessonDto()
				{
					UserLessonid = model_UserLesson.UserLessonid,
					LessonName = model_UserLesson.MenkeLessonName,
					MenkeStarttime = model_UserLesson.MenkeStarttime,
					MenkeEndtime = model_UserLesson.MenkeEndtime,
					Type = model_SkuType?.Title,
					MenkeState = (model_UserLesson.MenkeState == 1 && model_UserLesson.MenkeEndtime < thetime) ? 4 : model_UserLesson.MenkeState,
					MenkeEntryurl = menkeEntryurl,
                    ClassroomMin = (int)(classroom_hour * 60),
                    Istrial = model_UserLesson.Istrial
                });
            }
            result.Data = list;
            return result;
        }
        #endregion

        #region 消息列表
        /// <summary>
        /// 我的消息列表
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<Pages<MessageDto>>> MessageList(int page = 1, int pageSize = 10)
        {
            return await _messageBaseService.MessageList(page, pageSize);
        }

		/// <summary>
		/// 我的消息详情
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<Pages<MessageDto>>> MessageDetail(long sendUserid, int page = 1, int pageSize = 10) {
			return await _messageBaseService.MessageDetail(sendUserid, page, pageSize);
        }
        /// <summary>
        /// 删除指定发送方所有消息
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult> DelUserMessage(long sendUserid)
        {
           return await _messageBaseService.DelUserMessage(sendUserid);
        }
        /// <summary>
        /// 删除单条消息
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult> DelMessage(long messageid)
        {
            return await _messageBaseService.DelMessage(messageid);
        }
        #endregion

        #region 读取指定课节的学生对老师评分
        /// <summary>
        /// 读取指定课节的学生对老师评分
        /// </summary>
        /// <param name="userLessonid">排课课节ID</param>
        /// <returns></returns>
        public async Task<ApiResult<LessonScoreDto>> LessonScore(long userLessonid)
        {
            var result = new ApiResult<LessonScoreDto>();
            //var model_token = _userManage.GetUserToken();
            if (_userManage.Userid<=0)
            {
                result.StatusCode = 4001;
                result.Message = _localizer["用户登录超时退出"];
                return result;
            }           
			var model_UserLesson = _context.Queryable<WebUserLesson>().First(u => u.UserLessonid == userLessonid && u.Status == 1);
			if (model_UserLesson != null)
			{
				if (model_UserLesson.Userid == _userManage.Userid)
                {
					int student_score_days = _pubConfigBaseService.GetConfigInt("student_score_days", 7);
                    int isscore = 0;
                    if (model_UserLesson.Score > 0) isscore = 1;
                    if (model_UserLesson.ScoreTime.AddDays(student_score_days) < DateTime.Now) isscore = 2;
                    result.Data = new LessonScoreDto();
                    result.Data.Score = model_UserLesson.Score;
					result.Data.IsScore = isscore;

                    var model_Teacher = _context.Queryable<WebTeacher>().InSingle(model_UserLesson.Teacherid);
                    string head_img = (model_Teacher!=null && !string.IsNullOrEmpty(model_Teacher.HeadImg)) ? model_Teacher.HeadImg : "/Upfile/images/teacher_none.png";
                    result.Data.TeacherHeadImg = _commonBaseService.ResourceDomain(head_img);
					result.Data.TeacherName = model_UserLesson.MenkeTeacherName;
					result.Data.StudentScoreDays = student_score_days;
                }
				else
                {
                    result.StatusCode = 4007;
                    result.Message = _localizer["用户无此权限"];
                    return result;
                }
			}
			else {
                result.StatusCode = 4009;
                result.Message = _localizer["已排课节不存在"];
                return result;
            }
            return result;
        }
        #endregion

    }
}