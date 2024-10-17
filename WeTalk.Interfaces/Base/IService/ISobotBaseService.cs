using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models;

namespace WeTalk.Interfaces.Base
{
	public partial interface ISobotBaseService : IBaseService
	{
        Task<ApiResult<string>> AddUserTicket(string ticketTitle, string ticketContent, List<object> extendFields = null, string userid = "", string parentid = "", string userEmails = "", string userTels = "", string fileStr = ""); 
	}
}