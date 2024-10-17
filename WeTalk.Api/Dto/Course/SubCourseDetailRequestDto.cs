using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Course
{
    /// <summary>
    /// 子课程详情入参
    /// </summary>
    public class SubCourseDetailRequestDto
    {
        /// <summary>
        /// 子课程ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public long courseGroupInfoid { get; set; }

    }
}
