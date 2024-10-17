using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
    /// <summary>
    /// 课节报告详情
    /// </summary>
    public class LessonReportDtoRequestDto
    {
        /// <summary>
        /// 课节报告ID
        /// </summary>
        public long userLessonReportid { get; set; }
    }
}
