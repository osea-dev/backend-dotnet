using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.RecordCourse
{
    /// <summary>
    /// 创建录播课订单入参
    /// </summary>
    public class CreateOrderRequestDto
	{
        /// <summary>
        /// 录播课程ID
        /// </summary>
        [Required]
        public long recordCourseid { get; set; }

    }
}
