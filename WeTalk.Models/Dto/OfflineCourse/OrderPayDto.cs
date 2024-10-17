using WeTalk.Models.Dto.Common;

namespace WeTalk.Models.Dto.OfflineCourse
{
	/// <summary>
	///  支付信息返回
	/// </summary>
	public class OrderPayDto
	{
        public OrderPayDto()
        {
			PayType = new PayType();
		}
		/// <summary>
		/// 支付方式代码
		/// </summary>
		public string PayWay { get; set; }
		/// <summary>
		/// 订单ID
		/// </summary>
		public long Orderid { get; set; }
		/// <summary>
		/// 订单号
		/// </summary>
		public string OrderSn { get; set; }
		/// <summary>
		/// 二维码网址
		/// </summary>
		public string Qrcode { get; set; }
		/// <summary>
		/// 二维码网址有效时长(分)
		/// </summary>
		public int Expire { get; set; }
		/// <summary>
		/// 第三方支付跳转网址
		/// </summary>
		public string ApiUrl { get; set; }
		/// <summary>
		/// 订单应付金额
		/// </summary>
		public decimal Amount { get; set; }
		/// <summary>
		/// 币种符号
		/// </summary>
		public string Ico { get; set; }
		/// <summary>
		/// 币种
		/// </summary>
		public string CurrencyCode { get; set; }
		/// <summary>
		/// 支付方式
		/// </summary>
		public PayType PayType { get; set; }
		
	}
}
