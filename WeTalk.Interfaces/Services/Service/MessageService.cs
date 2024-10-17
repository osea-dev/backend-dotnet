using System.Threading.Tasks;
using WeTalk.Interfaces.Base;

namespace WeTalk.Interfaces.Services
{
    public partial class MessageService : BaseService, IMessageService
    {
		private readonly IMessageBaseService _messageBaseService;

        public MessageService(
            IMessageBaseService messageBaseService)
		{
            _messageBaseService = messageBaseService;

        }

        /// <summary>
        /// 执行邮件任务
        /// </summary>
        /// <returns></returns>
        public async Task SendEmailTask()
        {
            await _messageBaseService.SendEmailTask();
        }

    }
}