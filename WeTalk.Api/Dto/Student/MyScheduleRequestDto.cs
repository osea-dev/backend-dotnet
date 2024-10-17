using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
    /// <summary>
    /// 我的课表
    /// </summary>
    public class MyScheduleRequestDto
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        public int begintime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public int endtime { get; set; }
        /// <summary>
        /// 课节状态（0全部1未开始2进行中3已结课4已过期）
        /// </summary>
        public int menkeState { get; set; }
    }
}
