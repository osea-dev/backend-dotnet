using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models;
using WeTalk.Models.Dto;
using WeTalk.Models.Dto.Teacher;

namespace WeTalk.Interfaces.Services
{
	public partial interface ITeacherService : IBaseService
	{
        Task<ApiResult<TeacherReportDto>> LessonReport(int menkeLessonid, string code);
        Task<ApiResult<List<SubmitReportDto>>> SubmitLessonReport(int menkeLessonid, string code, List<SubmitLessonReportDto.StudentReport> studentReports);
        Task<ApiResult<List<TeacherCategoryDto>>> TeacherCategory();
        Task<ApiResult<TeacherDetailDto>> TeacherDetail(long teacherid);
        Task<ApiResult<TeacherTeacherHeadImgDto>> TeacherHeadImg(long teacherid);
        Task<ApiResult<Pages<TeacherDto>>> TeacherList(long teacherCategoryid = 0, int page = 1, int pageSize = 12);
    }
}