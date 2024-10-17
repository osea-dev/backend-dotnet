using Aspose.Slides;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Base;
using WeTalk.Models;
using WeTalk.Models.Dto.ShortVideo;

namespace WeTalk.Interfaces.Services
{
	/// <summary>
	/// 短视频
	/// </summary>
	public partial class ShortVideoService : BaseService, IShortVideoService
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly ILogger<ShortVideoService> _logger;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包

		private readonly IUserManage _userManage;
		private readonly ICommonBaseService _commonBaseService;

        public ShortVideoService(IHttpContextAccessor accessor, SqlSugarScope dbcontext, ILogger<ShortVideoService> logger,IWebHostEnvironment env, IStringLocalizer<LangResource> localizer, IUserManage userManage, 
			ICommonBaseService commonBaseService)
		{
			_context = dbcontext;
			_accessor = accessor;
			_logger = logger;
			_localizer = localizer;
			_userManage = userManage;
			_commonBaseService = commonBaseService;
        }

		#region "获取短视频列表"
		/// <summary>
		/// 获取短视频列表
		/// </summary>
		/// <param name="onlineCategoryid">分类ID</param>
		/// <param name="key">搜索关键词</param>
		/// <param name="page">页码</param>
		/// <param name="pageSize">每页条数</param>
		/// <returns></returns>
		public async Task<ApiResult<Pages<VideoDto>>> VideoList( string key, string sortType="", int page = 1,int pageSize=10)
		{
			var result = new ApiResult<Pages<VideoDto>>();
			int total = 0;
			var query = _context.Queryable<WebShortVideo>().LeftJoin<WebShortVideoLang>((l, r) => l.ShortVideoid == r.ShortVideoid)
				.Where((l, r) => l.Status == 1 && r.Lang == _userManage.Lang)
				.WhereIF(!string.IsNullOrEmpty(key), (l, r) => r.Keys.Contains(key) || r.Title.Contains(key) || r.Message.Contains(key));

			switch (sortType) {
				case "hot"://热门
					query = query.OrderBy((l, r) => l.Recommend,OrderByType.Desc).OrderBy((l, r) => l.Sort).OrderBy((l, r) => l.Sendtime, OrderByType.Desc);
					break;
				case "views"://播放量
					query = query.OrderBy((l, r) => l.Hits, OrderByType.Desc).OrderBy((l, r) => l.Sort).OrderBy((l, r) => l.Sendtime, OrderByType.Desc);
					break;
				default:
					query = query.OrderBy((l, r) => l.Sort).OrderBy((l, r) => l.Sendtime, OrderByType.Desc);
					break;
			}
			var list_Video=query
				.Select((l, r) => new { l.ShortVideoid, l.Img, r.Title, r.Message, r.Keys, l.Hits, l.Recommend, l.Dtime, l.Sendtime,l.Video })
				.ToPageList(page, pageSize, ref total);
			result.Data = new Pages<VideoDto>();
			result.Data.Total = total;
            var model_Currency = _context.Queryable<PubCurrency>().First(u => u.CurrencyCode == _userManage.CurrencyCode);
            foreach (var model in list_Video)
			{
				result.Data.List.Add(new VideoDto()
				{
					ShortVideoid = model.ShortVideoid,
					Img = _commonBaseService.ResourceDomain(model.Img),
					Title = model.Title,
					Message = model.Message,
					Video = _commonBaseService.ResourceDomain(model.Video),
					Keys = !string.IsNullOrEmpty(model.Keys) ? model.Keys.Split(',').Where(u => !string.IsNullOrEmpty(u)).ToList() : new List<string>()
				});
			}
			return result;
		}
		#endregion

		#region "获取短视频详情"
		/// <summary>
		/// 获取短视频详情
		/// </summary>
		/// <param name="courseid">课程ID</param>
		/// <returns></returns>
		public async Task<ApiResult<VideoDetailDto>> VideoDetail(long shortVideoid, string key = "",string sortType = "")
		{
			var result = new ApiResult<VideoDetailDto>();
			var model = new VideoDetailDto();
			var model_token = _userManage.GetUserToken();
			long userid = (model_token != null) ? model_token.Userid : 0;
			var model_Video = _context.Queryable<WebShortVideo>().First(u => u.ShortVideoid == shortVideoid && u.Status == 1);
			if (model_Video != null)
			{
				var model_CourseLang = _context.Queryable<WebShortVideoLang>().First(u => u.ShortVideoid == model_Video.ShortVideoid && u.Lang == _userManage.Lang);
				model.ShortVideoid = model_Video.ShortVideoid;
				model.Title = model_CourseLang?.Title + "";
				model.Message = model_CourseLang?.Message + "";
				model.Keys = (model_CourseLang?.Keys + "").Split(",").Where(u => !string.IsNullOrEmpty(u)).ToList();
				model.Hits = model_Video.Hits;
				model.Video = _commonBaseService.ResourceDomain(model_Video.Video);
				model.Img = model_Video.Img;

				var query = _context.Queryable<WebShortVideo>().LeftJoin<WebShortVideoLang>((l, r) => l.ShortVideoid == r.ShortVideoid)
				.Where((l, r) => l.Status == 1 && r.Lang == _userManage.Lang)
				.WhereIF(!string.IsNullOrEmpty(key), (l, r) => r.Keys.Contains(key) || r.Title.Contains(key) || r.Message.Contains(key));
				switch (sortType)
				{
					case "hot"://热门
						query = query.OrderBy((l, r) => l.Recommend, OrderByType.Desc).OrderBy((l, r) => l.Sort).OrderBy((l, r) => l.Sendtime, OrderByType.Desc);
						break;
					case "views"://播放量
						query = query.OrderBy((l, r) => l.Hits, OrderByType.Desc).OrderBy((l, r) => l.Sort).OrderBy((l, r) => l.Sendtime, OrderByType.Desc);
						break;
					default:
						query = query.OrderBy((l, r) => l.Sort).OrderBy((l, r) => l.Sendtime, OrderByType.Desc);
						break;
				}

				var list_Videoid = query.Select((l, r) => l.ShortVideoid).ToList();
				int n = list_Videoid.IndexOf(shortVideoid);
				if (n > 0)
				{
					model.BackShortVideoid = list_Videoid[n - 1];
				}
				else {
					model.BackShortVideoid = 0;
				}
				if (n < list_Videoid.Count-1)
				{
					model.NextShortVideoid = list_Videoid[n + 1];
				}
				else {
					model.NextShortVideoid = 0;
				}
				result.Data = model;
				string ip = IpHelper.GetCurrentIp(_accessor.HttpContext);
				if (model_Video.Ip!=ip || model_Video.Lasttime.AddMinutes(3) < DateTime.Now)
				{
					model_Video.Hits += 1;
					model_Video.Lasttime = DateTime.Now;
					model_Video.Ip = ip;
					_context.Updateable(model_Video).UpdateColumns(u => new { u.Hits, u.Lasttime, u.Ip }).ExecuteCommand();
				}
			}
			else
			{
				result.StatusCode = 4003;
				result.Message = _localizer["短视频不存在"];
			}
			return result;
		}
		#endregion
	}
}