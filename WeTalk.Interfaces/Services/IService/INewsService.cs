using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models;
using WeTalk.Models.Dto.News;

namespace WeTalk.Interfaces.Services
{
    public partial interface INewsService : IBaseService
    {
        Task<ApiResult<List<NewsCategoryDto>>> NewsCategoryList();
        Task<ApiResult<NewsDetailDto>> NewsDetail(long newsid);
        Task<ApiResult<Pages<NewsDto>>> NewsList(long newsCategoryid, string key, int page = 1, int pageSize = 10);
        Task<ApiResult<SearchListDto>> SearchList(string key = "");
    }
}