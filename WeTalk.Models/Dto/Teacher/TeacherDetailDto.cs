
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.Teacher
{
	/// <summary>
	/// 老师
	/// </summary>
	public class TeacherDetailDto
	{
        public TeacherDetailDto()
        {
			Advantages = new List<Advantage>();

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
		/// 照片
		/// </summary>
		public string Photo { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 来自
		/// </summary>
		public string ComeFrom { get; set; }
		/// <summary>
		/// 标签
		/// </summary>
		public List<string> Keys { get; set; }
		/// <summary>
		/// 教育/学历
		/// </summary>
		public string Edu { get; set; }
		/// <summary>
		/// 宗教信仰
		/// </summary>
		public string Religion { get; set; }
		/// <summary>
		/// 描述
		/// </summary>
		public string Desc { get; set; }
		/// <summary>
		/// 所属分类
		/// </summary>
		public string Category { get; set; }
		/// <summary>
		/// 数字标
		/// </summary>
		public List<Advantage> Advantages { get; set; }
		/// <summary>
		/// 座右铭
		/// </summary>
		public string Motto { get; set; }
		/// <summary>
		/// 哲学修养
		/// </summary>
		public string Philosophy { get; set; }

		public class Advantage
		{
			public string Value { get; set; }
			public string Sup { get; set; }
			public string Sub { get; set; }
			public string Title { get; set; }
		}
	}
}
