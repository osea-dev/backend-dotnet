using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Extensions;
using WeTalk.Interfaces.Base;
using WeTalk.Models;
using WeTalk.Models.Dto.Common;

namespace WeTalk.Interfaces.Services
{
	public partial class CommonService : BaseService, ICommonService
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly ILogger<CommonService> _logger;
		private readonly IWebHostEnvironment _env;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包
		private readonly IUserManage _userManage;
		private readonly ICommonBaseService _commonBaseService;
		/// <summary>
		/// Redis通讯
		/// </summary>
		private readonly bool _isRedis = true;

		public CommonService(SqlSugarScope dbcontext, ILogger<CommonService> logger,IWebHostEnvironment env, IStringLocalizer<LangResource> localizer, IHttpContextAccessor accessor,
			IUserManage userManage, ICommonBaseService commonBaseService
			)
		{
			_accessor = accessor;
			_context = dbcontext;
			_logger = logger;
			_localizer = localizer;
			_env = env;
			_userManage = userManage;
			_commonBaseService = commonBaseService;

			try
			{
				_isRedis = RedisServer.Cache.Ping();
			}
			catch
			{
				_isRedis = false;
			}

		}

		#region "国家列表"
		/// <summary>
		/// 国家列表
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<CountryDto>>> CountryList()
		{
			var result = new ApiResult<List<CountryDto>>();
			try
			{
				var list_Country = new List<CountryDto>();
				List<PubCountry> list = new List<PubCountry>();
				var lang = _userManage.Lang.ToLower();
				if (lang == "zh-cn")
				{
					list = _context.Queryable<PubCountry>().OrderBy(u => u.Country).ToList();
				}
				else
				{
					list = _context.Queryable<PubCountry>().OrderBy(u => u.CountryEn).ToList();
				}
				foreach (var item in list)
				{
					list_Country.Add(new CountryDto()
					{
						Countryid = item.Countryid,
						Code = item.Code,
						Number = item.Number,
						Country = (lang == "zh-cn" ? item.Country : item.CountryEn)
					});
				}
				result.Data = list_Country;
			} catch (Exception ex) {
				_logger.LogError(ex, "CountryList");
				result.StatusCode = 4012;
			}
			return result;
		}
		#endregion

		#region "语言列表"
		/// <summary>
		/// 语言列表
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<LangDto>>> LangList()
		{
			var result = new ApiResult<List<LangDto>>();
			try
			{
				var list_Lang = new List<LangDto>();
				int expire = 86400;//1天的有效期
				var model_Lang = _context.Queryable<PubCurrencyLang>().First(u => u.Lang.ToLower() == _userManage.Lang.ToLower()||_userManage.Lang.ToLower().Contains(u.Lang.ToLower()));
				if (model_Lang != null) {
					_userManage.Lang = model_Lang.Lang.Trim();
				}else
                {
                    _userManage.Lang = "zh-cn";
                }
                var key = "LangList_" + _userManage.Lang;
				if (_isRedis && RedisServer.Cache.Exists(key))
				{
					list_Lang = RedisServer.Cache.Get<List<LangDto>>(key);
					RedisServer.Cache.Expire(key, expire);
				}
				else
				{
					foreach (var item in _context.Queryable<PubLanguage>().Where(u => u.Status == 1).ToList())
					{
						list_Lang.Add(new LangDto()
						{
							Languageid = item.Languageid,
							Lang = item.Lang.Trim(),
							Title = item.Title
						});
					}
					if (_isRedis) RedisServer.Cache.Set(key, list_Lang, expire);
				}
				result.Data = list_Lang;
			}
			catch (Exception ex) {
				result.StatusCode = 4012;
				result.Message= ex.Message;
			}
			return result;
		}
		#endregion

		#region "时区列表"
		/// <summary>
		/// 时区列表
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<TimezoneDto>>> TimezoneList()
		{
			var result = new ApiResult<List<TimezoneDto>>();			
			var list = new List<TimezoneDto>();
			var list_Timezone = _context.Queryable<PubTimezone>().Where(u => u.Status == 1).ToList();
			var list_Country = _context.Queryable<PubCountry>()
				.Where(u => list_Timezone.Select(s => s.Country).Contains(u.Code))
				.Select(u => new { u.Code, u.Country, u.CountryEn })
				.ToList();
			foreach (var item in list_Country)
			{
				var timeZones = new List<TimezoneDto.TimeZone>();
				list_Timezone.Where(u => u.Country == item.Code).ToList().ForEach(model =>
				{
					timeZones.Add(new TimezoneDto.TimeZone()
					{
						Timezoneid = model.Timezoneid,
						TimezoneName = model.Title,
						UtcSec = model.UtcSec
					});
				});
				list.Add(new TimezoneDto()
				{
					CountryCode = item.Code,
					CountryName = (_userManage.Lang.ToLower() == "zh-cn" ? item.Country : item.CountryEn),
					TimeZones = timeZones
				});
			}
			result.Data = list;
			return result;
		}
		#endregion

		#region "币种列表"
		/// <summary>
		/// 币种列表
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<CurrencyDto>>> CurrencyList()
		{
			var result = new ApiResult<List<CurrencyDto>>();
			try
			{
				var list_Currency = new List<CurrencyDto>();
				int expire = 86400;//1天的有效期
                var model_Lang = _context.Queryable<PubCurrencyLang>().First(u => u.Lang.ToLower() == _userManage.Lang.ToLower() || _userManage.Lang.ToLower().Contains(u.Lang.ToLower()));
                if (model_Lang != null)
                {
                    _userManage.Lang = model_Lang.Lang.Trim();
                }
                else
                {
                    _userManage.Lang = "zh-cn";
                }
                var key = "CurrencyList_" + _userManage.Lang;
				if (_isRedis && RedisServer.Cache.Exists(key))
				{
					list_Currency = RedisServer.Cache.Get<List<CurrencyDto>>(key);
					RedisServer.Cache.Expire(key, expire);
				}
				else
				{
					list_Currency = _context.Queryable<PubCurrency>().LeftJoin<PubCurrencyLang>((l, r) => l.PubCurrencyid == r.PubCurrencyid && r.Lang == _userManage.Lang)
					.Where((l, r) => l.Status != -1).OrderBy((l, r) => l.Sort)
					.Select((l, r) => new CurrencyDto
					{
						Currencyid = l.PubCurrencyid,
						CountryCode = l.CountryCode,
						CurrencyCode = l.CurrencyCode,
						Currency = r.Currency,
						IsDefault = 0,
						Country = r.Country,
						Ico = l.Ico
					}).ToList();
					if (_isRedis) RedisServer.Cache.Set(key, list_Currency, expire);//10分钟的有效期
				}
				if (!list_Currency.Any(u => u.CountryCode == _userManage.CurrencyCode))
				{
					//设置美元
					foreach (var currency in list_Currency)
					{
						if (currency.IsDefault == 1) currency.IsDefault = 0;
						if (currency.CurrencyCode == "USD") currency.IsDefault = 1;
					}
				}
				else
				{
					foreach (var currency in list_Currency)
					{
						if (currency.IsDefault == 1) currency.IsDefault = 0;
						if (currency.CurrencyCode == _userManage.CurrencyCode) currency.IsDefault = 1;
					}
				}
				result.Data = list_Currency;
			}
			catch(Exception ex) {
				result.StatusCode = 4012;
				result.Message = ex.Message;
			}
			return result;
		}
		#endregion

		#region 上传图片至本服务器
		/// <summary>
		/// 上传图片至本服务器
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<FileDto>> UpdateFile(IFormFile upfile,string type="Common", bool islogin = false)
		{
			return await _commonBaseService.UpdateFile(upfile, type, islogin);
		}
		#endregion

		#region 上传图片至本服务器
		/// <summary>
		/// 替换资源文件前缀域名
		/// </summary>
		/// <param name="fileurl">含标签变量的资源路径</param>
		/// <returns></returns>
		public string ResourceDomain(string fileurl)
		{
			return _commonBaseService.ResourceDomain(fileurl);
		}
		/// <summary>
		/// 将资源文件的域名替换为变量
		/// </summary>
		/// <param name="fileurl">含完整域名的资源路径</param>
		/// <returns></returns>
		public string ResourceVar(string fileurl)
		{
			return _commonBaseService.ResourceVar(fileurl);
		}
		public string ResourceClear(string fileurl)
		{
			return _commonBaseService.ResourceClear(fileurl);
		}
		#endregion

		#region "获取标签列表"
		/// <summary>
		/// 获取标签列表
		/// </summary>
		public async Task<ApiResult<List<TagsDto>>> TagsList(int sty)
		{
			//0关键词，1课程标签，2课节取消原因，3订单取消原因，4短视频标签，5录播课标签，6直播课标签，7线下课标签
			var result = new ApiResult<List<TagsDto>>();
			var list_Keys = _context.Queryable<PubKeys>().LeftJoin<PubKeysLang>((l, r) => l.Keysid == r.Keysid && r.Lang == _userManage.Lang)
				.Where((l, r) => l.Status == 1 && l.Sty == sty).OrderBy(l => l.Sort).Select((l, r) => new { r.Title }).ToList();
			result.Data = new List<TagsDto>();
			foreach (var model in list_Keys)
			{
				result.Data.Add(new TagsDto()
				{
					Tag = model.Title,
				});
			}
			return result;
		}
		#endregion

	}
}