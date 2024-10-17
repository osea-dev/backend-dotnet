using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Course
{
    /// <summary>
    /// 课程详情入参
    /// </summary>
    public class CourseDetailRequestDto
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        [Required]
        public long courseid { get; set; }

    }
}
