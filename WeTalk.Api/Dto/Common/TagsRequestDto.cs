using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Common
{
	/// <summary>
	/// 试听课申请入参
	/// </summary>
	public class TagsRequestDto
	{
		/// <summary>
		/// 标签类型:0关键词，1课程标签，2课节取消原因，3订单取消原因，4短视频标签，5录播课标签，6直播课标签，7线下课标签
		/// </summary>
		[Required]
		public int sty { get; set; }
	}
}
