using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
    /// <summary>
    /// 正式课课节
    /// </summary>
    public class UserCourseDtoRequestDto
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public long userCourseid { get; set; }
    }
}
