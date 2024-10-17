using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
    /// <summary>
    /// 正式课课节列表
    /// </summary>
    public class UserLessonListRequestDto
    {
        /// <summary>
        /// 已购课程的ID
        /// </summary>
        public long userCourseid { get; set; }
        /// <summary>
        /// 开始状态：0未开始，1进行中，2已结束
        /// </summary>
        [Required]
        public int startStatus { get; set; }
        /// <summary>
        /// 结束状态（当startStatus==2时有效）：0全部，1已完成，2未完成/缺席,3已取消
        /// </summary>
        public int endStatus { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        [DefaultValue(1)]
        public int page { get; set; }
        /// <summary>
        /// 每页条数
        /// </summary>
        [DefaultValue(10)]
        public int pageSize { get; set; }
    }
}
