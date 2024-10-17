using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Course
{
    /// <summary>
    /// 课程SKU价格入参
    /// </summary>
    public class CourseSkuPriceRequestDto
	{
        /// <summary>
        /// 课程SkuID
        /// </summary>
        [Required]
        public long courseSkuid { get; set; }

    }
}
