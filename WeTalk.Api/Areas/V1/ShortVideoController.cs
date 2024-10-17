using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WeTalk.Api.Dto.ShortVideo;
using WeTalk.Common.Helper;
using WeTalk.Interfaces;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto.ShortVideo;
using static WeTalk.Api.HttpHeaderFilter;

namespace WeTalk.Api
{
	/// <summary>
	/// 直播课程接口
	/// </summary>
	[Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
	public partial class ShortVideoController : Base.WebApiController
    {
        private readonly IStringLocalizer<LangResource> _localizer;//语言包
        private readonly IShortVideoService _shortVideoService;
		private readonly ILogger<ShortVideoController> _logger;
		private readonly IHttpContextAccessor _accessor;
		public ShortVideoController(IHttpContextAccessor accessor, IShortVideoService shortVideoService, IStringLocalizer<LangResource>  localizer, ILogger<ShortVideoController> logger) : base(accessor)
		{
			_shortVideoService = shortVideoService;
			_localizer = localizer;
			_accessor = accessor;
			_logger = logger;
		}

		#region "获取短视频列表"
		/// <summary>
		/// 获取短视频列表
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[Currency]
		public async Task<ApiResult<Pages<VideoDto>>> VideoList([FromBody] VideoListRequestDto data)
		{
			try
			{
				return await _shortVideoService.VideoList( data.key,data.sortType, data.page, data.pageSize);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<Pages<VideoDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "播放短视频"   		
		/// <summary>
		/// 播放短视频
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[Currency]
		public async Task<ApiResult<VideoDetailDto>> VideoDetail([FromBody] VideoDetailRequestDto data)
		{
			try
			{
				return await _shortVideoService.VideoDetail(data.shortVideoid,data.key,data.sortType);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<VideoDetailDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

	}
}
