using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Api.Dto.News;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto.News;

namespace WeTalk.Api
{
	/// <summary>
	/// 新闻
	/// </summary>
	[Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
	public partial class NewsController : Base.WebApiController
	{
		private readonly INewsService _newsService;
		private readonly IHttpContextAccessor _accessor;
		private readonly ILogger<NewsController> _logger;
		public NewsController(IHttpContextAccessor accessor,INewsService newsService, ILogger<NewsController> logger) :base(accessor)
		{
			_newsService = newsService;
			_accessor = accessor;
			_logger = logger;
		}

		#region "获取新闻分类"
		/// <summary>
		/// 获取新闻分类
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<List<NewsCategoryDto>>> NewsCategoryList()
		{
			try
			{
				return await _newsService.NewsCategoryList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<NewsCategoryDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "获取新闻列表"
		/// <summary>
		/// 获取新闻列表）
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<Pages<NewsDto>>> NewsList([FromBody] NewsListRequestDto data)
		{
			try
			{
				return await _newsService.NewsList(data.newsCategoryid, data.key, data.page, data.pageSize);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<Pages<NewsDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "获取新闻详情"
		/// <summary>
		/// 获取新闻详情）
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<NewsDetailDto>> NewsDetail([FromBody] NewsDetailRequestDto data)
		{
			try
			{
				return await _newsService.NewsDetail(data.newsid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<NewsDetailDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
        #endregion

        #region "搜索栏目页"
        /// <summary>
        /// 搜索页(课程+新闻)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
		public async Task<ApiResult<SearchListDto>> SearchList([FromBody] SearchListRequestDto data)
		{
			try
			{
				return await _newsService.SearchList(data.key);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<SearchListDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

	}
}
