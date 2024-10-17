using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models;
using WeTalk.Models.Dto.Common;
using WeTalk.Models.Dto.Order;

namespace WeTalk.Interfaces.Base
{
	public partial interface ICommonBaseService : IBaseService
	{
		string CreateOrderSn(string type, long userid, long sellerId = 0);
		Task<ApiResult<List<PayType>>> PayTypes();
		string ResourceClear(string fileurl);
        string ResourceDomain(string fileurl);
		string ResourceVar(string fileurl);
		Task<ApiResult<FileDto>> UpdateFile(IFormFile upfile, string type = "Common", bool islogin = false);
	}
}