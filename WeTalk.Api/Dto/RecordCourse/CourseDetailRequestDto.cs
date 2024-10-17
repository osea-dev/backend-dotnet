using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.RecordCourse
{
	/// <summary>
	/// 录播课详情入参
	/// </summary>
	public class CourseDetailRequestDto
    {
		/// <summary>
		/// 录播课程ID
		/// </summary>
		[Required]
        public long recordCourseid { get; set; }

    }
}
