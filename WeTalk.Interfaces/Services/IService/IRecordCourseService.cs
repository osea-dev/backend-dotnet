using System.Threading.Tasks;
using WeTalk.Models;
using WeTalk.Models.Dto.Course;
using WeTalk.Models.Dto.RecordCourse;
using WeTalk.Models.Dto.ShortVideo;

namespace WeTalk.Interfaces.Services
{
	public partial interface IRecordCourseService : IBaseService
	{
		Task<ApiResult<ConfirmOrderDto>> ConfirmOrder(long recordCourseid);
		Task<ApiResult<Models.Dto.RecordCourse.CourseDetailDto>> CourseDetail(long RecordCourseid);
		Task<ApiResult<Pages<Models.Dto.RecordCourse.CourseDto>>> CourseList(string key, int page = 1, int pageSize = 10);
		Task<ApiResult<OrderDto>> CreateOrder(long recordCourseid);
	}
}