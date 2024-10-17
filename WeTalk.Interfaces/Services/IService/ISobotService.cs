using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models;

namespace WeTalk.Interfaces.Services
{
	public partial interface ISobotService : IBaseService
	{
        Task<ApiResult<string>> AddUserTicket(string ticketTitle, string ticketContent, List<object> extendFields = null, string userid = "", string parentid = "", string userEmails = "", string userTels = "", string fileStr = "");
        Task<ApiResult<string>> GetDataDict();
		void UpdateSobotTicketType();
		Task<ApiResult> UpdateToken();
	}
}