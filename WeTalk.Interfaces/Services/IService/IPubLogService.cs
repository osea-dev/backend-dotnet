using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models;
namespace WeTalk.Interfaces.Services
{
	public interface IPubLogService 
	{
		Task<ApiResult> AddLog(string username, string content);
		Task<ApiResult> DelLog(List<long> list_logid);
	}
}
