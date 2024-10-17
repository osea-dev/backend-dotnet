using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Order
{
	public class OrderDetailRequestDto
	{
		/// <summary>
		/// 订单ID
		/// </summary>
		[Required]
		public long orderid { get; set; }
		
	}
}
