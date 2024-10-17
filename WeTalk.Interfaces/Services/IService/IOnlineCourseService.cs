using System.Threading.Tasks;
using WeTalk.Models.Dto;
using WeTalk.Models;
using System.Collections.Generic;
using WeTalk.Models.Dto.OnlineCourse;

namespace WeTalk.Interfaces.Services
{
	public partial interface IOnlineCourseService : IBaseService
	{
		Task<ApiResult<List<CategoryDto>>> CategoryList();
		Task<ApiResult<ConfirmOrderDto>> ConfirmOrder(long onlineCourseid);
		Task<ApiResult<CourseDetailDto>> CourseDetail(long onlineCourseid);
		Task<ApiResult<Pages<CourseDto>>> CourseList(long onlineCategoryid, string key, int page = 1, int pageSize = 10);
		Task<ApiResult<OrderDto>> CreateOrder(long onlineCourseid);
	}
}