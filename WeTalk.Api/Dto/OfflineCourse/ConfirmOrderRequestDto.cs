using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.OfflineCourse
{
	public class ConfirmOrderRequestDto
	{
		/// <summary>
		/// 线下课程ID
		/// </summary>
		[Required]
		public long offlineCourseid { get; set; }
		
	}
}
