
namespace WeTalk.Models.Dto.Common
{
	/// <summary>
	/// 支付类型
	/// </summary>
	public class PayType
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
