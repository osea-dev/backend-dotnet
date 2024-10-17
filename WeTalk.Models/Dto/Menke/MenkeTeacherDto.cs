using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Menke
{
	/// <summary>
	/// 老师
	/// </summary>
	public class MenkeTeacherDto
	{
		public string name { get; set; }
		public int sex { get; set; }
		public string birthday { get; set; }
		public string email { get; set; }
		public string locale { get; set; }
		public string code { get; set; }
		public string mobile { get; set; }
		public string pwd { get; set; }
	}
}
