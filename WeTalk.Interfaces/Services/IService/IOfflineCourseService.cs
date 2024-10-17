using System.Threading.Tasks;
using WeTalk.Models.Dto;
using WeTalk.Models;
using System.Collections.Generic;
using WeTalk.Models.Dto.OfflineCourse;

namespace WeTalk.Interfaces.Services
{
	public partial interface IOfflineCourseService : IBaseService
	{
		Task<ApiResult<ConfirmOrderDto>> ConfirmOrder(long offlineCourseid);
		Task<ApiResult<CourseDetailDto>> CourseDetail(long offlineCourseid);
		Task<ApiResult<Pages<CourseDto>>> CourseList(string key, int page = 1, int pageSize = 10);
		Task<ApiResult<OrderDto>> CreateOrder(long offlineCourseid);
	}
}