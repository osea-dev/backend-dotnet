using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Api.Dto.UserLogin;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Extensions;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto;
using static WeTalk.Api.HttpHeaderFilter;
using WeTalk.Common.Helper;
using Microsoft.Extensions.Logging;
using System;

namespace WeTalk.Api
{
	/// <summary>
	/// 测试类
	/// </summary>
	[Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
	public partial class TestController : Controller
	{
		private readonly SqlSugarScope _context;
		private readonly IHttpContextAccessor _accessor;
		private readonly ILogger<TestController> _logger;

		private readonly IUserService _userService;
		private readonly ISobotService _sobotService;
		private readonly IMenkeService _menkeService;
		private readonly ICourseService _courseService;
		public TestController(IHttpContextAccessor accessor, SqlSugarScope dbcontext, IUserService userService, ISobotService sobotService, IMenkeService menkeService, ICourseService courseService
			, ILogger<TestController> logger)
		{
			_context = dbcontext;
			_accessor = accessor;
			_logger = logger;
			_userService = userService;
			_sobotService = sobotService;
			_menkeService = menkeService;
			_courseService = courseService;

		}

        #region "测试API"
        /// <summary>
        /// 测试API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<ApiResult<TestResponseDto>> Test1([FromBody]TestRequestDto data)
		{
			var result = new ApiResult<TestResponseDto>();

            var result_menke_course = await _courseService.UserCourseArranging(224);
            if (result_menke_course.StatusCode != 0)
            {
                _logger.LogError("支付回调时创建拓课云课程[user_courseid=224]异常:" + result_menke_course.Message);
            }
            return result;
		}
        /// <summary>
        /// 测试2
        /// </summary>
        /// <param name="abc">测试字段</param>
		/// <remarks>设置测试接口</remarks>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<ApiResult<object>> Test2(string abc)
		{
			var result = new ApiResult<object>();
			throw new NotImplementedException();

			return result;
		}
		#endregion

	}
}
