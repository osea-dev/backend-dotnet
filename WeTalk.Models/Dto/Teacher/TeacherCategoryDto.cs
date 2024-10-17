
using System;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.Teacher
{
	/// <summary>
	/// 老师分类
	/// </summary>
	public class TeacherCategoryDto
	{
		/// <summary>
		/// 分类ID
		/// </summary>
		public long TeacherCategoryid { get; set; }
		/// <summary>
		/// 分类标题
		/// </summary>
		public string Title { get; set; }
	}
}
