using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto;
using static WeTalk.Api.HttpHeaderFilter;
using WeTalk.Common.Helper;
using WeTalk.Api.Dto.Course;
using WeTalk.Api.Dto.Common;
using WeTalk.Models.Dto.Common;

namespace WeTalk.Api
{
    /// <summary>
    /// 公共接口
    /// </summary>
    [Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
	public partial class CommonController : Base.WebApiController
	{
		private readonly ICommonService _commonService;
		private readonly IHttpContextAccessor _accessor;
		private readonly ILogger<CommonController> _logger;
		public CommonController(IHttpContextAccessor accessor, ILogger<CommonController> logger,
			ICommonService commonService) :base(accessor)
		{
			_commonService = commonService;
			_accessor = accessor;
			_logger = logger;
		}

        #region "国家列表"
        /// <summary>
        /// 国家列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<List<CountryDto>>> CountryList()
		{
			try
			{
				return await _commonService.CountryList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<CountryDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "语言列表"
		/// <summary>
		/// 语言列表
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[HttpGet]
		public async Task<ApiResult<List<LangDto>>> LangList()
		{
			try
			{
				return await _commonService.LangList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<LangDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "时区列表"
		/// <summary>
		/// 时区列表
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<List<TimezoneDto>>> TimezoneList()
		{
			try
			{
				return await _commonService.TimezoneList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<TimezoneDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "币种列表"
		/// <summary>
		/// 币种列表
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[HttpGet]
		[Currency]
		public async Task<ApiResult<List<CurrencyDto>>> CurrencyList()
		{
			try
			{
				return await _commonService.CurrencyList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<CurrencyDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "标签列表"
		/// <summary>
		/// 标签列表
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[HttpGet]
		[Currency]
		public async Task<ApiResult<List<TagsDto>>> TagsList([FromBody] TagsRequestDto data)
		{
			try
			{
				return await _commonService.TagsList(data.sty);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<List<TagsDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion
	}
}
