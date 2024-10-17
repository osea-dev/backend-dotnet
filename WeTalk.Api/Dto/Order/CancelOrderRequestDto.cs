using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Order
{
	public class CancelOrderRequestDto
	{
		/// <summary>
		/// 订单ID
		/// </summary>
		[Required]
		public long orderid { get; set; }
		/// <summary>
		/// 多个原因标签
		/// </summary>
		public List<string> keys { get; set; }
		/// <summary>
		/// 其它原因
		/// </summary>
		public string message { get; set; }
		
	}
}
