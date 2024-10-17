using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Student
{
    /// <summary>
    /// 私信消息
    /// </summary>
    public class MessageDto
    {
        /// <summary>
        /// 私信消息ID
        /// </summary>
        public long Messageid { get; set; }
        /// <summary>
        /// 发送者ID
        /// </summary>
        public long SendUserid { get; set; }
        /// <summary>
        /// 发送者姓名
        /// </summary>
        public string SendName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImg { get; set; }
		/// <summary>
		/// 最后消息内容
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// 最后发送时间
		/// </summary>
		public int Sendtime { get; set; }
        /// <summary>
        /// 是否已读0未读，1已读
        /// </summary>
        public int IsRead { get; set; }

    }
}
