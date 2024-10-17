using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
    /// <summary>
    /// 试听课课节列表
    /// </summary>
    public class HomeworkDetailRequestDto
    {
        /// <summary>
        /// 课节ID
        /// </summary>
        [Required]
        public long userLessonid { get; set; }
    }
}
