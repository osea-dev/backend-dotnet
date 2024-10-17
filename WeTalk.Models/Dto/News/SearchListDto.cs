
using System;
using System.Collections.Generic;
using WeTalk.Models.Dto.Course;

namespace WeTalk.Models.Dto.News
{
    /// <summary>
    /// 搜索页(课程+新闻)
    /// </summary>
    public class SearchListDto
    {
        public SearchListDto()
        {
            Courses = new List<CourseDto>();
            Newss=new List<NewsDto>();
        }
        public List<CourseDto> Courses { get; set; }
        public List<NewsDto> Newss { get; set; }
    }
}
