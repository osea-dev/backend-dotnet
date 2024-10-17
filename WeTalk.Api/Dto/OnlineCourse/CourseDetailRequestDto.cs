using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.OnlineCourse
{
    /// <summary>
    /// 课程详情入参
    /// </summary>
    public class CourseDetailRequestDto
    {
        /// <summary>
        /// 直播课程ID
        /// </summary>
        [Required]
        public long onlineCourseid { get; set; }

    }
}
