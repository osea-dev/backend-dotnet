using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
    /// <summary>
    /// 删除指定发送方所有消息
    /// </summary>
    public class DelUserMessageRequestDto
    {
        /// <summary>
        /// 发送方ID
        /// </summary>
        public long sendUserid { get; set; }
    }
}
