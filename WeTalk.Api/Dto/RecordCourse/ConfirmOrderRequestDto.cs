using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.RecordCourse
{
	public class ConfirmOrderRequestDto
	{
		/// <summary>
		/// 直播课程ID
		/// </summary>
		[Required]
		public long recordCourseid { get; set; }
		
	}
}
