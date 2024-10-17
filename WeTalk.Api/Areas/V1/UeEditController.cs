using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Common.Helper;

namespace WeTalk.Api
{
    [Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
	public partial class UeEditController : Base.WebApiController
	{
		public UeEditController(IHttpContextAccessor accessor) : base(accessor)
		{
		}

		#region "编辑器"
		[HttpPost]
		[ApiExplorerSettings(IgnoreApi = true)] //让此接口不在swagger文档中出现
		public async Task<IActionResult> UploadFile(string type,IFormFile Filedata)
		{
			string filename = "",ext="", fileurl = Appsettings.app("Web:Upfile").ToString() + "/UeEditor/" + DateTime.Now.ToString("yyyyMMdd") + "/";
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
