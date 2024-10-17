using System.Threading.Tasks;
using WeTalk.Models.Dto;
using WeTalk.Models;
using System.Collections.Generic;

namespace WeTalk.Interfaces.Services
{
    public partial interface IMessageService : IBaseService
    {
        Task SendEmailTask();
    }
}