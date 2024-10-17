using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Order
{
	public class CreateOrderRequestDto
    {
		/// <summary>
		/// 课程Sku ID
		/// </summary>
		[Required]
		public long courseSkuid { get; set; }
		
	}
}
