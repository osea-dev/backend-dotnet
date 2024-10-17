using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models;
using WeTalk.Models.Dto.Common;
using WeTalk.Models.Dto.Course;
using WeTalk.Models.Dto.Student;
using WeTalk.Models.Dto.User;

namespace WeTalk.Interfaces.Services
{
    public partial interface IStudentService : IBaseService
	{
        Task<ApiResult> ApplyCancelLesson(long UserLessonid, string[] keys, string message);
        Task<ApiResult<List<string>>> CancelLessonKeys();
        Task<ApiResult<List<CourseDto>>> RecommendCourse();
		Task<ApiResult<StudentDto>> StudentInfo();
		Task<ApiResult<StudentHomePageDto>> StudentHomePage();
		Task<ApiResult> ToTeacherSorce(long UserLessonid, int score = 0);
		Task<ApiResult> UpdateStudentInfo(string Userpwd, string FirstName, string LastName, DateTime? Birthdate, int? Gender, string native, long timezoneid, string Education, string Native, string GuardianName, string GuardianMobileCode, string GuardianMobile, string GuardianSmsCode, string GuardianshipFee, string Email,string countryCode, string MobileCode, string Mobile, string SmsCode);
		Task<ApiResult<FileDto>> UploadHeadImg(IFormFile upfile);
		Task<ApiResult<List<LevelReportDto>>> LevelReportList();
		Task<ApiResult<List<LessonReportDto>>> LessonReportList();
		Task<ApiResult<LessonReportDetailDto>> LessonReport(long userLessonReportid);
		Task<ApiResult<List<TrialUserLessonDto>>> TrialUserLessonList();
		Task<ApiResult<List<UserCourseDto>>> UserCourseList();
        Task<ApiResult<UserLessonDto>> UserLessonDetail(long userLessonid);
        Task<ApiResult<Pages<UserLessonDto>>> UserLessonList(long userCourseid, int startStatus, int endStatus, int page = 1, int pageSize = 10);
        Task<ApiResult<HomeworkDto>> HomeworkDetail(long userLessonid);
        Task<ApiResult<Pages<MessageDto>>> MessageList(int page = 1, int pageSize = 10);
        Task<ApiResult<Pages<MessageDto>>> MessageDetail(long sendUserid, int page = 1, int pageSize = 10);
        Task<ApiResult> DelUserMessage(long sendUserid);
        Task<ApiResult> DelMessage(long messageid);
        Task<ApiResult> TrialCourseApply(string courseName, string mobileCode, string mobile, string email, string birthdate,int gender, int isChinese);
        Task<ApiResult> CourseApply(long userCourseid);
        Task<ApiResult<LessonScoreDto>> LessonScore(long userLessonid);
        Task<ApiResult<UserCourseDto>> UserCourse(long userCourseid);
        Task<ApiResult<List<UserLessonDto>>> MySchedule(int begintime, int endtime, int menkeState = 0);
		Task<ApiResult<List<OnlineCourseDto>>> MyOnlineCourseList();
		Task<ApiResult<List<MyRecordCourseDto>>> MyRecordCourseList();
		Task<ApiResult<List<MyOfflineCourseDto>>> MyOfflineCourseList();
		Task<ApiResult<MyRecordCourseDto>> MyRecordCourse(long orderid);
	}
}