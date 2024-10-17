using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Order
{
	public class OrderListRequestDto
	{
		/// <summary>
		/// 支付状态:0全部，1待支付，2已支付
		/// </summary>
		[Required]
		public int payStatus { get; set; }

		/// <summary>
		/// 页码
		/// </summary>
		[DefaultValue(1)]
		public int page { get; set; } = 1;

		/// <summary>
		/// 每页条数
		/// </summary>
		[DefaultValue(10)]
		public int pageSize { get; set; } = 10;

	}
}
