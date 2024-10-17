using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Menke
{
	/// <summary>
	/// 模板
	/// </summary>
	public class MenkeTemplateDto
    {
		public int menke_template_id { get; set; }
        public string menke_name { get; set; }
		public int menke_type { get; set; }
	}
}
