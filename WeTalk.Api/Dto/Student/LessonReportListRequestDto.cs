using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
	/// <summary>
	/// 
	/// </summary>
	public class LessonReportListRequestDto
	{
		/// <summary>
		/// 是否试听/定级报告
		/// </summary>
		public int istrial { get; set; } = 0;
		/// <summary>
		/// 页码默认1
		/// </summary>
		public int page { get; set; } = 1;
		/// <summary>
		/// 分页条数,默认10
		/// </summary>
		public int pageSize { get; set; } = 10;
	}
}
