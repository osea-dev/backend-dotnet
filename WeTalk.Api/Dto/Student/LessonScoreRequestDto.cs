using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
    /// <summary>
    /// 指定课节学生对老师的评分
    /// </summary>
    public class LessonScoreRequestDto
    {
        /// <summary>
        /// 已排课节的ID
        /// </summary>
        public long userLessonid { get; set; }
    }
}
