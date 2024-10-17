using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.OfflineCourse
{
    /// <summary>
    /// 课程详情入参
    /// </summary>
    public class CourseDetailRequestDto
    {
        /// <summary>
        /// 线下课程ID
        /// </summary>
        [Required]
        public long offlineCourseid { get; set; }

    }
}
