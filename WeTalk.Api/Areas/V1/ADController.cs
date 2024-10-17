using Aspose.Slides;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Api.Dto.AD;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto.AD;

namespace WeTalk.Api
{
	/// <summary>
	/// 公共接口
	/// </summary>
	[Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
	public partial class ADController : Base.WebApiController
	{
		private readonly IADService _aDService;
		private readonly IHttpContextAccessor _accessor;
		private readonly ILogger<ADController> _logger;
		public ADController(IHttpContextAccessor accessor, ILogger<ADController> logger, IADService aDService,
			ICommonService commonService) :base(accessor)
		{
			_aDService = aDService;
			_accessor = accessor;
			_logger = logger;
		}

		#region "Banner广告"
		/// <summary>
		/// Banner广告
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        public async Task<ApiResult<List<BannerDto>>> BannerList([FromBody] BannerListRequestDto data)
		{
			try
			{
				return await _aDService.BannerList(data.type);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<BannerDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}

		#endregion

		#region "单广告位"
		/// <summary>
		/// 单广告位
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        public async Task<ApiResult<AdDto>> SingleAD([FromBody] SingleADRequestDto data)
		{
			try
			{
				return await _aDService.SingleAD(data.type);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<AdDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "多广告位"
		/// <summary>
		/// 多广告位
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<List<AdDto>>> AdList([FromBody] AdListRequestDto data)
		{
			try
			{
				return await _aDService.AdList(data.type);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<AdDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion
	}
}
