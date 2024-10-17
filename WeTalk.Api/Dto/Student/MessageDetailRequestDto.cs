using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
    /// <summary>
    /// 指定发送方的消息列表
    /// </summary>
    public class MessageDetailRequestDto
    {
        /// <summary>
        /// 发送方ID
        /// </summary>
        public long sendUserid { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        [DefaultValue(1)]
        public int page { get; set; }
        /// <summary>
        /// 每页条数
        /// </summary>
        [DefaultValue(10)]
        public int pageSize { get; set; }
    }
}
