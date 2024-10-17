using System.Threading.Tasks;
using WeTalk.Models;
using WeTalk.Models.Dto.ShortVideo;

namespace WeTalk.Interfaces.Services
{
	public partial interface IShortVideoService : IBaseService
	{
		Task<ApiResult<VideoDetailDto>> VideoDetail(long shortVideoid, string key = "", string sortType = "");
		Task<ApiResult<Pages<VideoDto>>> VideoList(string key, string sortType = "", int page = 1, int pageSize = 10);
	}
}