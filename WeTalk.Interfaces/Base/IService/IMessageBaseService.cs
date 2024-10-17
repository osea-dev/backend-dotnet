using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models;
using WeTalk.Models.Dto.Student;

namespace WeTalk.Interfaces.Base
{
    public interface IMessageBaseService : IBaseService
	{
		Task<ApiResult> AddEmailTask(string code, string email, Dictionary<string, string> dic_data, string lang = "");
		Task<ApiResult> SendGlobeSms(string mobile, string message, string senderid = "");
		Task<ApiResult> SendSms(string mobile, string templateParam = "{\"code\":\"测试\"}", string templateCode = "SMS_257758081");
        Task<ApiResult<Pages<MessageDto>>> MessageList(int page = 1, int pageSize = 10);
        Task<ApiResult<Pages<MessageDto>>> MessageDetail(long sendUserid, int page = 1, int pageSize = 10);
        Task<ApiResult> DelUserMessage(long sendUserid);
        Task<ApiResult> DelMessage(long messageid);
        Task SendEmailTask();
    }
}