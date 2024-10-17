using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.News
{
	public class SearchListRequestDto
    {
		/// <summary>
		/// 搜索关键词
		/// </summary>
		public string key { get; set; }
	}
}
