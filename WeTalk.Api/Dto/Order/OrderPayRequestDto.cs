using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Order
{
	public class OrderPayRequestDto
    {
		/// <summary>
		/// 订单ID
		/// </summary>
		[Required]
		public long orderid { get; set; }
        /// <summary>
        /// 支付类型ID
        /// </summary>
        [Required]
        public long payTypeid { get; set; }
        /// <summary>
        /// 付款方式,默认1;1网页,2扫码
        /// </summary>
        public int type { get; set; } = 1;

    }
}
