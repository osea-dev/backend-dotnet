using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.News
{
	public class NewsDetailRequestDto
	{
		/// <summary>
		/// 新闻ID
		/// </summary>
		[Required]
		public long newsid { get; set; }
	}
}
