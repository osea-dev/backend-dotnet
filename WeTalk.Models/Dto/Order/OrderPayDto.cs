
using System;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.Order
{
	/// <summary>
	///  支付信息返回
	/// </summary>
	public class OrderPayDto
	{
        public OrderPayDto()
        {
			PayType = new PayTypeClass();
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
		public PayTypeClass PayType { get; set; }
		/// <summary>
		/// 支付方式
		/// </summary>
		public class PayTypeClass
		{
			/// <summary>
			/// 支付ID
			/// </summary>
			public long Paytypeid { get; set; }
			/// <summary>
			/// 支付图标
			/// </summary>
			public string Ico { get; set; }
			/// <summary>
			/// 支付名称
			/// </summary>
			public string PayName { get; set; }
			/// <summary>
			/// 是否支持扫码支付
			/// </summary>
			public int isScan { get; set; }
			/// <summary>
			/// 是否支持网页支付
			/// </summary>
			public int isWeb { get; set; }
		}
	}
}
