using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
    /// <summary>
    /// 删除单条消息
    /// </summary>
    public class DelMessageRequestDto
    {
        /// <summary>
        /// 单条消息ID
        /// </summary>
        public long messageid { get; set; }
    }
}
