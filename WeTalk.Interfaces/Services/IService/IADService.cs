using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models;
using WeTalk.Models.Dto.AD;

namespace WeTalk.Interfaces.Services
{
	public partial interface IADService : IBaseService
	{
		Task<ApiResult<List<AdDto>>> AdList(string type = "");
		Task<ApiResult<List<BannerDto>>> BannerList(string type = "");
		Task<ApiResult<AdDto>> SingleAD(string type = "");
	}
}