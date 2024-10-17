
using System;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.OnlineCourse
{
	/// <summary>
	/// 在线直播课分类
	/// </summary>
	public class CategoryDto
	{
		/// <summary>
		/// 分类ID
		/// </summary>
		public long OnlineCategoryid { get; set; }
		/// <summary>
		/// 分类标题
		/// </summary>
		public string Title { get; set; }
	}
}
