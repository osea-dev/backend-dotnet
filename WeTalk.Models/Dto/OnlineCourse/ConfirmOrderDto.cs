
using System.Collections.Generic;
using WeTalk.Models.Dto.Common;

namespace WeTalk.Models.Dto.OnlineCourse
{
	/// <summary>
	/// 确认订单
	/// </summary>
	public class ConfirmOrderDto
	{
		public ConfirmOrderDto()
		{
			PayTypes = new List<PayType>();
		}
		/// <summary>
		/// 课程SkuID
		/// </summary>
		public long OnlineCourseid { get; set; }
		/// <summary>
		/// 课程缩略图
		/// </summary>
		public string Img { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 简介
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// 详细内容
		/// </summary>
		public string Intro { get; set; }
		/// <summary>
		/// 课节数
		/// </summary>
		public int LessonCount { get; set; }
		/// <summary>
		/// 直播上课时间
		/// </summary>
		public string LessonStart { get; set; }
		/// <summary>
		/// 学习人数
		/// </summary>
		public int StudentCount { get; set; }
		/// <summary>
		/// 结算币种
		/// </summary>
		public string CurrencyCode { get; set; }
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
		/// 支付方式
		/// </summary>
		public List<PayType> PayTypes { get; set; }

	}
}
