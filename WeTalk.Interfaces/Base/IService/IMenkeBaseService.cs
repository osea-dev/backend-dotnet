using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models;
using WeTalk.Models.Dto.Menke;

namespace WeTalk.Interfaces.Base
{
    public partial interface IMenkeBaseService : IBaseService
	{
		Task<ApiResult> AttendanceSync(string search_day);
		Task<ApiResult> CourseSync(string starttime);
		Task<ApiResult<int>> CreateStudents(MenkeStudentDto data);
		Task<ApiResult<int>> CreateTeachers(MenkeTeacherDto data);
        Task<ApiResult> HomeworkRemarkSync(string starttime);
        Task<ApiResult> HomeworkSubmitSync(string starttime);
        Task<ApiResult> HomeworkSync(string starttime);
        Task<ApiResult<List<long>>> LessonSync(string starttime, int menke_userid = 0, int course_id = 0);
        Task<ApiResult> RecordSync(string starttime);
        Task UnionLessonUserid(List<int> menkeLessonIds);
		Task<ApiResult<int>> GetMenkeUserId(string code, string mobile, int sty = 0);
        Task UnionUserLessonUrl(List<int> menkeLessonIds = null);
        Task<ApiResult> ModifyStudent(string mobile_code, string mobile, MenkeStudentDto data);
        Task<ApiResult> ModifyTeacher(string mobile_code, string mobile, MenkeTeacherDto data);
        Task<ApiResult> DeleteLesson(int menke_lessonid);
        Task<ApiResult> ModifyLessonStudent(int menke_lessonid, List<string> mobiles);
        Task<ApiResult> ModifyLessonStudent(MenkeLesson model_MenkeLesson, List<string> mobiles);
        Task<ApiResult<List<MenkeTemplateDto>>> GetTemplate();
        Task<ApiResult<int>> CreateCourse(string name, List<string> students, int room_template_id);
        Task<ApiResult> ModifyCourse(int menke_course_id, string name, List<string> students, int room_template_id);
		Task<ApiResult> DelCourse(int menke_course_id);
		Task<ApiResult> LessonSplit(List<MenkeLesson> list_MenkeLesson = null, int reset = 0, int count = 100);
    }
}