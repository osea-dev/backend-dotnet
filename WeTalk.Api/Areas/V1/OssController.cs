using Castle.Core.Logging;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WeTalk.Api.Dto.UserLogin;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto;

namespace WeTalk.Api
{
    /// <summary>
    /// 存储
    /// </summary>
    [Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
	public partial class OssController : Base.WebApiController
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly IOssService _ossService;
		private readonly ILogger<OssController> _logger;
		public OssController(IHttpContextAccessor accessor,IOssService ossService, ILogger<OssController> logger) :base(accessor)
		{
            _ossService = ossService;
			_accessor = accessor;
			_logger = logger;
		}

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> CallBack()
		{
			string str = "";
			//只允许8003端口调用
			if (_accessor.HttpContext.Connection.LocalPort != 8003)
			{
				_logger.LogError("非法OSS回调请求(" + _accessor.HttpContext.Request.Host.Port + "),拒绝响应");
				str = "非法请求";
			}
			else
			{
				str = _ossService.DoPost();
			}
			//var rq = _accessor.HttpContext.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpRequestFeature>();
			
			//_logger.LogError("Port:" +_accessor.HttpContext.Connection.LocalPort);
			//_logger.LogError("完整地址:" + Microsoft.AspNetCore.Http.Extensions.UriHelper.GetEncodedUrl(_accessor.HttpContext.Request));
			return Content(str, "applicatoin/json", System.Text.Encoding.UTF8);
        }
    }
}
