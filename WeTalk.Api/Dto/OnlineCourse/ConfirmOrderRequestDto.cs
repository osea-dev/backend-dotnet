using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.OnlineCourse
{
	public class ConfirmOrderRequestDto
	{
		/// <summary>
		/// 直播课程ID
		/// </summary>
		[Required]
		public long onlineCourseid { get; set; }
		
	}
}
