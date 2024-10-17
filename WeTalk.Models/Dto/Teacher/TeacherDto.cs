
using System;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.Teacher
{
	/// <summary>
	/// 老师列表项
	/// </summary>
	public class TeacherDto
	{
        public TeacherDto()
        {
			Keys = new List<string>();
		}
		/// <summary>
		/// ID
		/// </summary>
		public long Teacherid { get; set; }
		/// <summary>
		/// 缩略图
		/// </summary>
		public string Img { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 标签
		/// </summary>
		public List<string> Keys { get; set; }	}
}
