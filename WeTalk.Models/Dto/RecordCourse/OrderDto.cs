
using System;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.RecordCourse
{
	/// <summary>
	///  创建订单返回
	/// </summary>
	public class OrderDto
	{
		/// <summary>
		/// 订单ID
		/// </summary>
		public long Orderid { get; set; }
		/// <summary>
		/// 订单号
		/// </summary>
		public string OrderSn { get; set; }
	}
}
