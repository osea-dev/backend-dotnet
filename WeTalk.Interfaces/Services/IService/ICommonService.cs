using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models.Dto;
using WeTalk.Models;
using System;
using Microsoft.AspNetCore.Http;
using WeTalk.Models.Dto.Common;

namespace WeTalk.Interfaces.Services
{
    public partial interface ICommonService : IBaseService
	{
		Task<ApiResult<List<CountryDto>>> CountryList();
		Task<ApiResult<List<CurrencyDto>>> CurrencyList();
		Task<ApiResult<List<LangDto>>> LangList();
        string ResourceClear(string fileurl);
        string ResourceDomain(string fileurl);
		string ResourceVar(string fileurl);
		Task<ApiResult<List<TagsDto>>> TagsList(int sty);
		Task<ApiResult<List<TimezoneDto>>> TimezoneList();
		Task<ApiResult<FileDto>> UpdateFile(IFormFile upfile, string type = "Common", bool islogin = false);
	}
}