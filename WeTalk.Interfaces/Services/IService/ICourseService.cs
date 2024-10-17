using System.Threading.Tasks;
using WeTalk.Models;
using WeTalk.Models.Dto.Course;

namespace WeTalk.Interfaces.Services
{
	public partial interface ICourseService : IBaseService
	{
        Task<ApiResult> TrialCourseApply(string courseName, string mobileCode, string mobile, string email);
        Task<ApiResult<Models.Dto.Course.CourseDetailDto>> CourseDetail(long Courseid);
		Task<ApiResult<Pages<CourseDto>>> CourseList(string key, int page = 1, int pagesize = 12);
        Task<ApiResult> CourseMessage(long courseGroupInfoid, string mobileCode, string mobile, string email);
        Task<ApiResult<CourseSkuPriceDto>> CourseSkuPrice(long CourseSkuid);
		Task<ApiResult<SubCourseDetailDto>> SubCourseDetail(long CourseGroupInfoid);
		Task<ApiResult<int>> UserCourseArranging(long user_courseid);
		Task<ApiResult> LockUserCourse(long user_courseid);
		Task<ApiResult> UnLockUserCourse(long user_courseid);
		Task<ApiResult> DelUserCourse(long user_courseid);
		Task<ApiResult> AdjustUserCourse(long user_courseid, int menke_courseid);
		Task<ApiResult<int>> UserCourseArranging(WebUserCourse model_UserCourse);
	}
}