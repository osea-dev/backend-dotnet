
using System;
using System.Collections.Generic;

namespace WeTalk.Api.Dto.Teacher
{
	/// <summary>
	/// 老师取取课节报告
	/// </summary>
	public class LessonReportRequestDto
	{
		/// <summary>
		/// 拓课云课节ID
		/// </summary>
		public int MenkeLessonId { get; set; }
		/// <summary>
		/// 操作报告临时授权码
		/// </summary>
		public string Code { get; set; }
	}
}
