using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models;
using WeTalk.Models.Dto.AD;

namespace WeTalk.Interfaces.Services
{
	public partial class ADService : BaseService, IADService
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly ILogger<ADService> _logger;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包

		public ADService(SqlSugarScope dbcontext, ILogger<ADService> logger,IStringLocalizer<LangResource> localizer, IHttpContextAccessor accessor
			)
		{
			_accessor = accessor;
			_context = dbcontext;
			_logger = logger;
		}

		#region "Banner广告"
		/// <summary>
		/// Banner广告
		/// </summary>
		/// <param name="type">
		/// 广告位置:<br/>
		/// 进贤进能Banner:teachers_banner<br/>
		/// </param>
		/// <returns></returns>
		public async Task<ApiResult<List<BannerDto>>> BannerList(string type = "")
		{
			var result = new ApiResult<List<BannerDto>>();
			try
			{
				var list_AD = new List<BannerDto>();
				var list = _context.Queryable<WebAd>().Where(u =>u.Type== type && u.Status == 1 && u.Begintime <= DateTime.Now && u.Endtime >= DateTime.Now).OrderBy(u=>u.Sort).OrderBy(u=>u.Sendtime,OrderByType.Desc).ToList();
				foreach (var item in list)
				{
					list_AD.Add(new BannerDto()
					{
						Adid = item.Adid,
						Img = item.Img+"",
						Title = item.Title + "",
						Url = item.Url + ""
					});
				}
				result.Data = list_AD;
			} catch (Exception ex) {
				_logger.LogError(ex, "BannerList?type="+ type);
				result.StatusCode = 4012;
			}
			return result;
		}
		#endregion

		#region "单广告"
		/// <summary>
		/// 单广告
		/// </summary>
		/// <param name="type">
		/// 广告位置:<br/>
		/// 进贤进能老师介绍:teachers_img<br/>
		/// </param>
		/// <returns></returns>
		public async Task<ApiResult<AdDto>> SingleAD(string type = "")
		{
			var result = new ApiResult<AdDto>();
			try
			{
				var data = new AdDto();
				var model_Ad = _context.Queryable<WebAd>().First(u =>u.Type == type && u.Status == 1 && u.Begintime <= DateTime.Now && u.Endtime >= DateTime.Now);
				if(model_Ad!=null)
				{
					data.Adid = model_Ad.Adid;
					data.Img = model_Ad.Img + "";
					data.Url = model_Ad.Url + "";
					data.Title = model_Ad.Title + "";
				}
				result.Data = data;
			} catch (Exception ex) {
				_logger.LogError(ex, "SingleAD?type="+ type);
				result.StatusCode = 4012;
			}
			return result;
		}
		#endregion

		#region "多广告位"
		/// <summary>
		/// 多广告位
		/// </summary>
		/// <param name="type">
		/// 广告位置:<br/>
		/// 进贤进能Banner:teachers_banner<br/>
		/// </param>
		/// <returns></returns>
		public async Task<ApiResult<List<AdDto>>> AdList(string type = "")
		{
			var result = new ApiResult<List<AdDto>>();
			try
			{
				var list_AD = new List<AdDto>();
				var list = _context.Queryable<WebAd>().Where(u => u.Type == type && u.Status == 1 && u.Begintime <= DateTime.Now && u.Endtime >= DateTime.Now).OrderBy(u => u.Sort).OrderBy(u => u.Sendtime, OrderByType.Desc).ToList();
				foreach (var item in list)
				{
					list_AD.Add(new AdDto()
					{
						Adid = item.Adid,
						Img = item.Img + "",
						Title = item.Title + "",
						Url = item.Url + ""
					});
				}
				result.Data = list_AD;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "AdList?type=" + type);
				result.StatusCode = 4012;
			}
			return result;
		}
		#endregion
	}
}