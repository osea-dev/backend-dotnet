using System.ComponentModel;

namespace WeTalk.Models.Dto
{
	/// <summary>
	/// 老师列表入参
	/// </summary>
	public class TestResponseDto
	{
		/// <summary>
		/// 老师分类ID
		/// </summary>
		public long teacherCategoryid { get; set; }

		/// <summary>
		/// 页码
		/// </summary>
		[DefaultValue(1)]
		public int page { get; set; } = 1;

		/// <summary>
		/// 每页条数
		/// </summary>
		[DefaultValue(12)]
		public int pageSize { get; set; } = 12;
	}
}
