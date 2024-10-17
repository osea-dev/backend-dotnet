
using System;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.Order
{
	/// <summary>
	/// 订单详情
	/// </summary>
	public class OrderDetailDto
	{
		public OrderDetailDto()
		{
			PayType = new PayTypeClass();
		}
		/// <summary>
		/// 订单ID
		/// </summary>
		public long Orderid { get; set; }
		/// <summary>
		/// 订单号
		/// </summary>
		public string OrderSn { get; set; }
		/// <summary>
		/// 订单状态(0未支付，1已支付)
		/// </summary>
		public int Status { get; set; }
		/// <summary>
		/// 课程缩略图
		/// </summary>
		public string Img { get; set; }
		/// <summary>
		/// 课程标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 课程简介
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// 上课类型
		/// </summary>
		public string Type { get; set; }
		/// <summary>
		/// 上课类型ID
		/// </summary>
		public long SkuTypeid { get; set; }
		/// <summary>
		/// 课时数
		/// </summary>
		public int ClassHour { get; set; }
		/// <summary>
		/// 币种符号
		/// </summary>
		public string Ico { get; set; }
		/// <summary>
		/// 原价
		/// </summary>
		public decimal MarketPrice { get; set; }
		/// <summary>
		/// 实售价（按IP取对应国家真实售价）
		/// </summary>
		public decimal Price { get; set; }
		/// <summary>
		/// 支付时间
		/// </summary>
		public int Paytime { get; set; }
		/// <summary>
		/// 支付方式
		/// </summary>
		public PayTypeClass PayType { get; set; }
		/// <summary>
		/// 支付方式
		/// </summary>
		public class PayTypeClass {
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
