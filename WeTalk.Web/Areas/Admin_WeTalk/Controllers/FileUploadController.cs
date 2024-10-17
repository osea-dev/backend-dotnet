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
using Aliyun.OSS;
using System.Net;
using WeTalk.Interfaces.Services;
using Stripe;
using System.Security.Policy;
using NetTaste;
using Newtonsoft.Json;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
    [Area("Admin_WeTalk")]
    public partial class FileUploadController : Base.BaseController
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly TokenManager _tokenManager;
        private readonly ILogger<FileUploadController> _logger;
        protected readonly IStringLocalizer<LangResource> _localizer;//语言包
		private readonly IPubConfigService _pubConfigService;
        private readonly IOssService _ossService;

        public FileUploadController( IHttpContextAccessor accessor, TokenManager tokenManager, ILogger<FileUploadController> logger, IStringLocalizer<LangResource> localizer,
			IPubConfigService pubConfigService,IOssService ossService)
            : base(tokenManager)
        {
            _accessor = accessor;
            _tokenManager = tokenManager;
            _logger = logger;
            _localizer = localizer;
			_pubConfigService = pubConfigService;
            _ossService = ossService;
        }

        [Authorization(Power = "Main")]
        public IActionResult Index(string obj,string oss,string uploadDir,int fileCount=1)
        {
            ViewBag.Obj = obj;
            ViewBag.Oss = oss;
            ViewBag.UploadDir = uploadDir;
            ViewBag.FileCount = fileCount;
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
            return View();
        }

        [Authorization(Power = "Main")]
        public async Task<IActionResult> DoGet(string uploadDir) {
            string token = _ossService.GetPolicyToken(uploadDir);
            return Content(token, "applicatoin/json", System.Text.Encoding.UTF8);
        }

        [Authorization(Power = "Main")]
        public async Task<IActionResult> FileDel(string img) {
            var isok = _ossService.DelFile(img);
            string str = "";
            if (isok)
            {
                str = "{\"status\":0}";
            }
            else {
                str = "{\"status\":1,\"msg\":\"" + _localizer["删除服务器文件失败"] + "\"}";
            }
            return Ok(JsonConvert.DeserializeObject(str));
        }


    }
}
