using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Menke
{
	/// <summary>
	/// 学生
	/// </summary>
	public class MenkeStudentDto
	{
		public string name { get; set; }
		public string nickname { get; set; }
		public int sex { get; set; }
		public string birthday { get; set; }
        public string locale { get; set; }
        public string code { get; set; }
		public string mobile { get; set; }
		public string p_name { get; set; }
	}
}
