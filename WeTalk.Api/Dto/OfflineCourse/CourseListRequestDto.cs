using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.OfflineCourse
{
	/// <summary>
	/// 线下课程列表入参
	/// </summary>
	public class CourseListRequestDto
	{
		/// <summary>
		/// 搜索关键词
		/// </summary>
		public string key { get; set; }

		/// <summary>
		/// 页码
		/// </summary>
		[DefaultValue(1)]
		public int page { get; set; } = 1;

		/// <summary>
		/// 每页条数
		/// </summary>
		[DefaultValue(10)]
		public int pageSize { get; set; } = 10;
	}
}
