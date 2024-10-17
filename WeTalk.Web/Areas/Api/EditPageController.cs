using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeTalk.Common;
using WeTalk.Common.Helper;
using SqlSugar;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Web.Extensions;
using WeTalk.Models;
using Microsoft.Extensions.Localization;

namespace WeTalk.Web.Areas.Api.Controllers
{
	/// <summary>
	/// Uelik 编辑器
	/// </summary>
	[Route("Api/[controller]/[action]")]
	[ApiController]
	public partial class EditPageController : Base.ApiController
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		public EditPageController(SqlSugarScope dbcontext,IHttpContextAccessor accessor)
		{
			_context = dbcontext;
			_accessor = accessor;
		}

		#region "编辑器"
		[HttpPost]
		[ApiExplorerSettings(IgnoreApi = true)] //让此接口不在swagger文档中出现
		public async Task<IActionResult> UploadFile(string type,IFormFile Filedata)
		{
			string filename = "",ext="", fileurl = Appsettings.app("Web:Upfile").ToString() + "/EditPage/" + DateTime.Now.ToString("yyyyMMdd") + "/";
			string ImgRoot = Appsettings.app("Web:ImgRoot");
			string url = "";
			if (Filedata!=null && Filedata.Length > 0) {
				FileHelper.AddFolder(ImgRoot + fileurl);
				ext = Path.GetExtension(Filedata.FileName);
				filename = Guid.NewGuid().ToString() + ext;//纯文件名
				using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
				{
					await Filedata.CopyToAsync(fs);
					url = fileurl + filename;
				}
			}
			return Content("{\"data\":\""+ url +"\"}", "applicatoin/json", System.Text.Encoding.UTF8);
		}
		#endregion
	}
}
