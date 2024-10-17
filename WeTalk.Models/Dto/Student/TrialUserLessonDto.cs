using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Student
{
	/// <summary>
	/// 试听课课节
	/// </summary>
	public class TrialUserLessonDto
	{
		/// <summary>
		/// 我的课节ID
		/// </summary>
		public long UserLessonid { get; set; }
		/// <summary>
		/// 课节名称
		/// </summary>
		public string LessonName { get; set; }
		/// <summary>
		/// 上课开始时间
		/// </summary>
		public int MenkeStarttime { get; set; }
		/// <summary>
		/// 上课结束时间
		/// </summary>
		public int MenkeEndtime { get; set; }
		/// <summary>
		/// 上课类型
		/// </summary>
		public string Type { get; set; }
		/// <summary>
		/// 课节状态（1未开始2进行中3已结课4缺席）
		/// </summary>
		public int MenkeState { get; set; }
    }
}
