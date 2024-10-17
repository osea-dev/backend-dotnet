using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Base;
using WeTalk.Models;
using WeTalk.Models.Dto.Course;
using WeTalk.Models.Dto.News;

namespace WeTalk.Interfaces.Services
{
	public partial class NewsService : BaseService, INewsService
	{
		private readonly SqlSugarScope _context;
		private readonly ILogger<CourseService> _logger;
		private readonly IWebHostEnvironment _env;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包

		private readonly IUserManage _userManage;
		private readonly ICommonBaseService _commonBaseService;
        private readonly ISobotBaseService _sobotBaseService;

        public NewsService(SqlSugarScope dbcontext, ILogger<CourseService> logger,IWebHostEnvironment env, IStringLocalizer<LangResource> localizer, IUserManage userManage, 
			ICommonBaseService commonBaseService,ISobotBaseService sobotBaseService)
		{
			_context = dbcontext;
			_logger = logger;
			_localizer = localizer;
			_env = env;
			_userManage = userManage;
			_commonBaseService = commonBaseService;
			_sobotBaseService = sobotBaseService;

        }

		#region "获取新闻分类"
		/// <summary>
		/// 获取新闻分类
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<NewsCategoryDto>>> NewsCategoryList()
		{
			var result = new ApiResult<List<NewsCategoryDto>>();
			var list_NewsCategory = _context.Queryable<WebNewsCategory>().LeftJoin<WebNewsCategoryLang>((l, r) => l.NewsCategoryid == r.NewsCategoryid && r.Lang == _userManage.Lang)
				.Where((l, r) => l.Status == 1 && l.Langs.Contains(_userManage.Lang))
				.OrderBy((l,r)=>l.Sort).OrderBy((l, r) => l.Sendtime,OrderByType.Desc)
				.Select((l, r) => new { NewsCategoryid=l.NewsCategoryid, Title= r.Title })
				.ToList();
			var list_News = _context.Queryable<WebNews>().Where(u => list_NewsCategory.Select(s => s.NewsCategoryid).Contains(u.NewsCategoryid) && u.Status == 1).GroupBy(u => u.NewsCategoryid)
				.Select(u => new { u.NewsCategoryid, Count = SqlFunc.AggregateCount(u.NewsCategoryid) })
				.ToList();
			result.Data = new List<NewsCategoryDto>();
			list_NewsCategory.ForEach(model =>
			{
				if (!string.IsNullOrEmpty(model.Title))
				{
					result.Data.Add(new NewsCategoryDto()
					{
						NewsCategoryid = model.NewsCategoryid,
						Title = model.Title,
						Total = list_News.Where(u => u.NewsCategoryid == model.NewsCategoryid).Count()
					});
				}
			});
			return result;
		}
		#endregion

		#region "获取新闻列表"
		/// <summary>
		/// 获取新闻列表
		/// </summary>
		/// <param name="newsCategoryid">新闻分类ID，0为全部</param>
		/// <param name="key">搜索关键词</param>
		/// <param name="page">页码</param>
		/// <param name="pageSize">每页条数</param>
		/// <returns></returns>
		public async Task<ApiResult<Pages<NewsDto>>> NewsList(long newsCategoryid,string key,int page = 1,int pageSize=10)
		{
			var result = new ApiResult<Pages<NewsDto>>();
			result.Data = new Pages<NewsDto>();
			int total = 0;
			var list_News = _context.Queryable<WebNews>().LeftJoin<WebNewsLang>((l,r)=>l.Newsid==r.Newsid && r.Lang==_userManage.Lang)
				.Where((l, r) => l.Status== 1 && l.Langs.Contains(_userManage.Lang))
                .WhereIF(newsCategoryid>0, (l, r) => l.NewsCategoryid==newsCategoryid)
				.WhereIF(!string.IsNullOrEmpty(key),(l,r)=>r.Title.Contains(key) ||r.Message.Contains(key))
				.OrderBy((l, r) => l.Sort).OrderBy((l, r) => l.Sendtime, OrderByType.Desc)
				.Select((l,r)=>new {l.Url, l.Newsid,l.Img,r.Title,l.Video,r.Message,l.Sendtime})
				.ToPageList(page ,pageSize,ref total);
			result.Data.Total = total;
            foreach (var model in list_News){
				result.Data.List.Add(new NewsDto()
                {
                    Newsid = model.Newsid,
                    Img = _commonBaseService.ResourceDomain(model.Img+""),
					Video = _commonBaseService.ResourceDomain(model.Video + ""),
					Url = model.Url + "",
					Title = model.Title+"",
					Message = model.Message+"",
					Sendtime = DateHelper.ConvertDateTimeInt(model.Sendtime)
                });
			}
			return result;
		}
		#endregion

		#region "获取新闻详情"
		/// <summary>
		/// 获取新闻详情
		/// </summary>
		/// <param name="newsCategoryid">新闻分类ID，0为全部</param>
		/// <param name="key">搜索关键词</param>
		/// <param name="page">页码</param>
		/// <param name="pageSize">每页条数</param>
		/// <returns></returns>
		public async Task<ApiResult<NewsDetailDto>> NewsDetail(long newsid)
		{
			var result = new ApiResult<NewsDetailDto>();
			var model_News = _context.Queryable<WebNews>().LeftJoin<WebNewsLang>((l,r)=>l.Newsid==r.Newsid && r.Lang==_userManage.Lang)
				.Where((l, r) => l.Status==1 && l.Newsid==newsid)
				.Select((l,r)=>new { l.Newsid,l.Source,l.Video,r.Title,r.Intro,l.Sendtime,r.Keys,l.Sort})
				.First();
			var model_NewsDetail = new NewsDetailDto();
			if (model_News != null)
			{
				model_NewsDetail.Newsid = model_News.Newsid;
				model_NewsDetail.Title = model_News.Title + "";
				model_NewsDetail.Video = _commonBaseService.ResourceDomain(model_News.Video + "");
				model_NewsDetail.Source= model_News.Source+"";
				model_NewsDetail.Sendtime = DateHelper.ConvertDateTimeInt(model_News.Sendtime);
				if (!string.IsNullOrEmpty(model_News.Intro))
				{
					var o = JObject.Parse(model_News.Intro);
					model_NewsDetail.Intro = (o!=null && o["data"]!=null)? o["data"].ToString():"";
				}
				if (!string.IsNullOrEmpty(model_News.Keys))
				{
					var list_keys = model_News.Keys.Split(',').ToList();
					model_NewsDetail.Keys = list_keys;
					model_NewsDetail.Relations = new List<NewsDetailDto.Relation>();
					var exp = new Expressionable<WebNewsLang>();
					exp.And(u => u.Lang == _userManage.Lang);
					foreach (var keys in list_keys) {
						exp.Or(u => u.Keys.Contains(keys));
					}
					var list_Newsid = _context.Queryable<WebNewsLang>().Where(exp.ToExpression()).Select(u => u.Newsid).ToList();
					var list_Relation = _context.Queryable<WebNews>().LeftJoin<WebNewsLang>((l, r) => l.Newsid == r.Newsid && r.Lang == _userManage.Lang)
						.Where((l, r) => l.Status == 1 && list_Newsid.Contains(l.Newsid) && l.Newsid != newsid)
						.OrderBy((l, r) => l.Sort).OrderBy((l, r) => l.Sendtime, OrderByType.Desc)
						.Select((l, r) => new { l.Url,l.Newsid,l.Video, l.Img, r.Title, r.Message, l.Sendtime })
						.Take(5)
						.ToList();
					foreach (var model in list_Relation) {
						model_NewsDetail.Relations.Add(new NewsDetailDto.Relation()
						{
							Newsid = model.Newsid,
							Title = model.Title,
							Url = model.Url,
							Video = _commonBaseService.ResourceDomain(model.Video+""),
							Img = _commonBaseService.ResourceDomain(model.Img),
							Message = model.Message,
							Sendtime = DateHelper.ConvertDateTimeInt(model.Sendtime)
						}) ;
					}
				}
				var model_Next = _context.Queryable<WebNews>().LeftJoin<WebNewsLang>((l, r) => l.Newsid == r.Newsid && r.Lang == _userManage.Lang)
					.Where((l, r) => l.Status == 1 && l.Sort >= model_News.Sort && l.Sendtime <= model_News.Sendtime && l.Newsid != model_News.Newsid)
					.OrderBy((l, r) => l.Sort)
					.OrderBy((l, r) => l.Sendtime, OrderByType.Desc)
					.Select((l, r) => new { l.Newsid, r.Title, l.Url })
					.First();
				if (model_Next != null)
				{
					model_NewsDetail.NextNews = new NewsDetailDto.NewsAround();
					model_NewsDetail.NextNews.Newsid = model_Next.Newsid;
					model_NewsDetail.NextNews.Title = model_Next.Title + "";
					model_NewsDetail.NextNews.Url = model_Next.Url + "";
				}
				var model_Back = _context.Queryable<WebNews>().LeftJoin<WebNewsLang>((l, r) => l.Newsid == r.Newsid && r.Lang == _userManage.Lang)
					.Where((l, r) => l.Status == 1 && l.Sort <= model_News.Sort && l.Sendtime >= model_News.Sendtime && l.Newsid != model_News.Newsid)
					.OrderBy((l, r) => l.Sort)
					.OrderBy((l, r) => l.Sendtime, OrderByType.Desc)
					.Select((l, r) => new { l.Newsid, r.Title, l.Url })
					.First();
				if (model_Back != null)
				{
					model_NewsDetail.BackNews = new NewsDetailDto.NewsAround();
					model_NewsDetail.BackNews.Newsid = model_Back.Newsid;
					model_NewsDetail.BackNews.Title = model_Back.Title + "";
					model_NewsDetail.BackNews.Url = model_Back.Url + "";
				}
				result.Data = model_NewsDetail;
			}
			else {
				result.StatusCode = 4009;
				result.Message = _localizer["新闻不存在"];
			}
			return result;
		}
        #endregion

        #region "搜索页(课程+新闻)"
        /// <summary>
        /// 搜索页(课程+新闻)
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<SearchListDto>> SearchList(string key="")
        {
            var result = new ApiResult<SearchListDto>();
			result.Data = new SearchListDto();

			var list_Course = _context.Queryable<WebCourse>()
				.LeftJoin<WebCourseLang>((l, r) => l.Courseid == r.Courseid && r.Lang == _userManage.Lang)
				.Where((l, r)=>l.Status==1)
				.Where((l, r) => r.Title.Contains(key) || r.Message.Contains(key) || r.Keys.Contains(key))
				.OrderBy((l, r) => l.Sort)
				.OrderBy((l, r) => l.Sendtime, OrderByType.Desc)
				.Select((l,r)=>new { l.Courseid,l.Img,r.Title,r.Keys,r.Message})
				.Take(3)
				.ToList();
            var list_CourseSku = _context.Queryable<WebCourseSku>()
                .LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid)
                .Where((l, r) => r.Lang == _userManage.Lang && list_Course.Select(s => s.Courseid).Contains(l.Courseid) && l.Status == 1)
                .Select((l, r) => new { l.SkuTypeid, l.CourseSkuid, l.Courseid, r.Title,l.Price })
                .ToList();
            var list_Price = _context.Queryable<WebCourseSkuPrice>()
				.Where(u => u.CurrencyCode == _userManage.CurrencyCode && list_CourseSku.Select(s => s.CourseSkuid).Contains(u.CourseSkuid))
                .GroupBy(u => u.Courseid)
                .Select(u => new { u.Courseid, Price = SqlFunc.AggregateMin(u.Price) }).ToList();
			var model_Currency = _context.Queryable<PubCurrency>().First(u => u.CurrencyCode == _userManage.CurrencyCode);
            foreach (var model in list_Course)
            {
                var model_SkuPrice = list_Price.FirstOrDefault(u => u.Courseid == model.Courseid);
                var model_CourseSku = list_CourseSku.Where(u => u.Courseid == model.Courseid).OrderBy(u => u.Price).FirstOrDefault();
                var price = model_SkuPrice != null ? model_SkuPrice.Price : (model_CourseSku?.Price ?? 0);
                result.Data.Courses.Add(new CourseDto()
				{
					Courseid = model.Courseid,
					Img = _commonBaseService.ResourceDomain(model.Img),
					Title = model.Title,
					Message = model.Message,
					Keys = !string.IsNullOrEmpty(model.Keys) ? model.Keys.Split(',').Where(u => !string.IsNullOrEmpty(u)).ToList() : new List<string>(),
					MinPrice = price,
					Ico = model_Currency?.Ico,
					SkuTypes = list_CourseSku.Where(u => u.Courseid == model.Courseid).GroupBy(u => new { u.SkuTypeid, u.Title }).Select(u => new CourseDto.SkuType { SkuTypeid = u.Key.SkuTypeid, Type = u.Key.Title }).ToList()
				});
            }

            var list_News = _context.Queryable<WebNews>().LeftJoin<WebNewsLang>((l, r) => l.Newsid == r.Newsid && r.Lang == _userManage.Lang)
				.Where((l, r) => l.Status == 1)
                .WhereIF(!string.IsNullOrEmpty(key), (l, r) => r.Title.Contains(key) || r.Message.Contains(key))
                .OrderBy((l, r) => l.Sort).OrderBy((l, r) => l.Sendtime, OrderByType.Desc)
                .Select((l, r) => new { l.Newsid,l.Url, l.Img, r.Title, r.Message, l.Sendtime,l.Video })
				.Take(3)
				.ToList();
			foreach (var model in list_News) {
				result.Data.Newss.Add(new NewsDto()
                {
                    Newsid = model.Newsid,
                    Img = _commonBaseService.ResourceDomain(model.Img + ""),
                    Video = _commonBaseService.ResourceDomain(model.Video + ""),
                    Url = model.Url + "",
                    Title = model.Title + "",
                    Message = model.Message + "",
                    Sendtime = DateHelper.ConvertDateTimeInt(model.Sendtime)
                });
            }

            return result;
        }
        #endregion

    }
}