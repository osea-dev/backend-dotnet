using System.Threading.Tasks;
using WeTalk.Models;

namespace WeTalk.Interfaces.Base
{
	public partial interface ICourseBaseService : IBaseService
	{
		Task<ApiResult<int>> UserCourseArranging(long user_courseid);
		Task<ApiResult> LockUserCourse(long user_courseid);
		Task<ApiResult> UnLockUserCourse(long user_courseid);
		Task<ApiResult> DelUserCourse(long user_courseid);
		Task<ApiResult> AdjustUserCourse(long user_courseid, int menke_courseid);
		Task<ApiResult<int>> UserCourseArranging(WebUserCourse model_UserCourse);
	}
}