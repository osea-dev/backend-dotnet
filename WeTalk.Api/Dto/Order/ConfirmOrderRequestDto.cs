using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Order
{
	public class ConfirmOrderRequestDto
	{
		/// <summary>
		/// 课程的SkuID
		/// </summary>
		[Required]
		public long courseSkuid { get; set; }
		
	}
}
