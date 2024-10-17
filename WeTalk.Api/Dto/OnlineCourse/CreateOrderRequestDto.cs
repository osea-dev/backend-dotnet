using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.OnlineCourse
{
    /// <summary>
    /// 创建直播课订单入参
    /// </summary>
    public class CreateOrderRequestDto
	{
        /// <summary>
        /// 直播课程ID
        /// </summary>
        [Required]
        public long onlineCourseid { get; set; }

    }
}
