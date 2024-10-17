using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace WeTalk.Models.Dto.Student
{
	/// <summary>
	/// 家庭作业
	/// </summary>
	public class HomeworkDto
    {
		public HomeworkDto()
		{
			Resources = new List<Attachment>();
			SubmitHomeworks = new List<SubmitHomework>();
		}
        /// <summary>
        /// 家庭作业ID
        /// </summary>
        public long Homeworkid { get; set; }
		/// <summary>
		/// 课程名称
		/// </summary>
		public string CourseName { get; set; }
		/// <summary>
		/// 课节名称
		/// </summary>
		public string LessonName { get; set; }
		/// <summary>
		/// 上课开始时间
		/// </summary>
		public int MenkeStarttime { get; set; }
		/// <summary>
		/// 上课结束时间
		/// </summary>
		public int MenkeEndtime { get; set; }
		/// <summary>
		/// 作业标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 作业要求
		/// </summary>
		public string Content { get; set; }
		/// <summary>
		/// 附件
		/// </summary>
		public List<Attachment> Resources { get; set; }
		/// <summary>
		/// 提交方式0不限制反馈方式1图片2视频3录音
		/// </summary>
		public int SubmitWay { get; set; }
		/// <summary>
		/// 作业截止时间
		/// </summary>
		public int EndDate { get; set; }
		/// <summary>
		/// 回放URL
		/// </summary>
		public string RecordUrl { get; set; }
        /// <summary>
        /// 学生是否提交的作业
        /// </summary>
        public int IsSubmit { get; set; }
       
		/// <summary>
		/// 学生提交的作业和老师点评
		/// </summary>
		public List<SubmitHomework> SubmitHomeworks { get; set; }

		/// <summary>
		/// 作业和点评对象
		/// </summary>
		public class SubmitHomework {
			public SubmitHomework()
			{
				SubmitFiles = new List<Attachment>();
				RemarkFiles = new List<Attachment>();
                TeacherKeys=new List<string>();
            }
			/// <summary>
			/// 学生提交的作业内容
			/// </summary>
			public string SubmitContent { get; set; }
           /// <summary>
		   /// 学生提交作业时间
		   /// </summary>
			public int SubmitTime { get; set; }
            /// <summary>
            /// 学生提交的作业附件
            /// </summary>
            public List<Attachment> SubmitFiles { get; set; }
			/// <summary>
			/// 老师ID
			/// </summary>
			public long Teacherid { get; set; }
			/// <summary>
			/// 老师姓名
			/// </summary>
			public string TeacherName { get; set; }
			/// <summary>
			/// 老师姓名
			/// </summary>
			public string TeacherHeadImg { get; set; }
			/// <summary>
			/// 老师姓名
			/// </summary>
			public List<string> TeacherKeys { get; set; }
			/// <summary>
			/// 老师是否对学生作业点评
			/// </summary>
			public int IsRemark { get; set; }
			/// <summary>
			/// 老师对学生作业点评内容
			/// </summary>
			public string RemarkContent { get; set; }
            /// <summary>
            /// 老师对学生作业点评时间
            /// </summary>
            public int RemarkTime { get; set; }
            /// <summary>
            /// 老师对学生作业点评附件
            /// </summary>
            public List<Attachment> RemarkFiles { get; set; }
			/// <summary>
			/// 作业是否合格
			/// </summary>
			public int RemarkIsPass { get; set; }
			/// <summary>
			/// 老师对学生作业点评评分数1-5
			/// </summary>
			public int RemarkRank { get; set; }
		}
        public class Attachment { 
			/// <summary>
			/// 附件ID
			/// </summary>
			public int AttachmentId { get; set; }
            /// <summary>
            /// 文件类型
            /// </summary>
            public string AttachmentType { get; set; }
            /// <summary>
            /// 源文件地址
            /// </summary>
            public string AttachmentUrl { get; set; }
            /// <summary>
            /// 预览地址
            /// </summary>
            public string AttachmentPreviewUrl { get; set; }
            /// <summary>
            /// 文件大小（字节）
            /// </summary>
            public int Size { get; set; }
			/// <summary>
			/// 多媒体播放时长
			/// </summary>
			public int Duration { get; set; }

		}
    }
}
