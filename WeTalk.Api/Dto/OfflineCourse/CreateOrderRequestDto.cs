using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.OfflineCourse
{
    /// <summary>
    /// 创建线下课订单入参
    /// </summary>
    public class CreateOrderRequestDto
	{
        /// <summary>
        /// 线下课程ID
        /// </summary>
        [Required]
        public long offlineCourseid { get; set; }

    }
}
