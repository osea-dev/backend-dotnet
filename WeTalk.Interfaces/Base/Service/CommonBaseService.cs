using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Models;
using WeTalk.Models.Dto.Common;
using WeTalk.Models.Dto.Order;

namespace WeTalk.Interfaces.Base
{
	public partial class CommonBaseService : BaseService, ICommonBaseService
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包
		private readonly IUserManage _userManage;

		public CommonBaseService(SqlSugarScope dbcontext, IStringLocalizer<LangResource> localizer, IHttpContextAccessor accessor,
			IUserManage userManage
			)
		{
			_accessor = accessor;
			_context = dbcontext;
			_localizer = localizer;
			_userManage = userManage;

		}

		#region 上传图片至本服务器
		/// <summary>
		/// 上传图片至本服务器
		/// </summary>
		/// <param name="upfile"></param>
		/// <param name="type">资源存放文件夹</param>
		/// <param name="islogin">是否要求登录再上传</param>
		/// <returns></returns>
		public async Task<ApiResult<FileDto>> UpdateFile(IFormFile upfile,string type="Common", bool islogin = false)
		{
			var result = new ApiResult<FileDto>();
			if (upfile != null && upfile.Length > 0)
			{
				if (_userManage.Userid<=0 && islogin)
				{
					result.StatusCode = 4001;
					result.Message = _localizer["用户登录超时退出"];
					return result;
				}
				string filename = "", fileurl = Appsettings.app("Web:Upfile") + "/"+ type + "/" + DateTime.Now.ToString("yyyyMMdd") + "/";
				var model_File = new WebFile();
				string ImgRoot = Appsettings.app("Web:ImgRoot");
				FileHelper.AddFolder(ImgRoot + fileurl);
				filename = Guid.NewGuid().ToString() + Path.GetExtension(upfile.FileName);
				using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
				{
					upfile.CopyTo(fs);
					fs.Flush();
				}
				model_File.Fileurl = fileurl + filename;
				model_File.Size = (double)upfile.Length / 1024;
				model_File.Fid = _userManage.Userid;
				model_File.TableName = "web_user";
				model_File.Remark = $"[{DateTime.Now}]新建图片";
				model_File.Userid = _userManage.Userid;
				var fileid = _context.Insertable(model_File).ExecuteReturnBigIdentity();
				result.Data = new FileDto()
				{
					Fileid=fileid,
					Url = model_File.Fileurl
				};
			}
			else
			{
				result.StatusCode = 4010;
				result.Message = _localizer["上传文件异常"];
			}
			return result;
		}
		#endregion

		#region 替换资源文件前缀域名
		/// <summary>
		/// 替换资源文件前缀域名
		/// </summary>
		/// <param name="fileurl">含标签变量的资源路径</param>
		/// <returns></returns>
		public string ResourceDomain(string fileurl)
		{
			if (string.IsNullOrEmpty(fileurl)) return "";
			if (fileurl.ToLower().StartsWith("http")) return fileurl;
			var country_code = _userManage.GetCountryCode();
			var key = "";
			switch (country_code)
			{
				case "CN":
					key = "Resource:OSS_CN";
					break;
				default:
					key = "Resource:OSS";
					break;
			}
			if (fileurl.StartsWith("{"))
			{
				var obj = Appsettings.appDictionary(key);
				foreach (var item in obj)
				{
					fileurl = fileurl.Replace(item.Key, item.Value.ToString());
				}
			}
			else {
				fileurl = Appsettings.app(key) +fileurl;
			}
			return fileurl;
		}
		/// <summary>
		/// 将资源文件的域名替换为变量
		/// </summary>
		/// <param name="fileurl">含完整域名的资源路径</param>
		/// <returns></returns>
		public string ResourceVar(string fileurl)
        {
            var country_code = _userManage.GetCountryCode();
            var key = "";
            switch (country_code)
            {
                case "CN":
                    key = "Resource:OSS_CN";
                    break;
                default:
                    key = "Resource:OSS";
                    break;
            }
            var obj = Appsettings.appDictionary(key);
			foreach (var item in obj)
			{
				if (string.IsNullOrEmpty(item.Value)) continue;
				fileurl = fileurl.Replace(item.Value+"",item.Key+"");
			}
			return fileurl;
		}
		/// <summary>
		/// 清理资源文件的变量
		/// </summary>
		/// <param name="fileurl">含变量的资源路径</param>
		/// <returns></returns>
		public string ResourceClear(string fileurl)
        {
            var country_code = _userManage.GetCountryCode();
            var key = "";
            switch (country_code)
            {
                case "CN":
                    key = "Resource:OSS_CN";
                    break;
                default:
                    key = "Resource:OSS";
                    break;
            }
            var obj = Appsettings.appDictionary(key);
            foreach (var item in obj)
			{
				fileurl = fileurl.Replace(item.Key+"/","");
			}
			return fileurl;
		}
		#endregion

		#region 订单号生成器
		/// <summary>
		/// 订单号生成器
		/// 首字母：订单类型,A自营课
		/// 16位：随机数
		/// </summary>
		/// <returns></returns>
		public string CreateOrderSn(string type, long userid, long sellerId = 0)
		{
			string order_sn = "";
			order_sn += type;
			order_sn += RandomHelper.GetMd5Code(DateTime.Now.Ticks.ToString(), 16);
			return order_sn.ToUpper();
		}
		#endregion



		#region 支付方式列表
		/// <summary>
		/// 支付方式列表
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<PayType>>> PayTypes()
		{
			var result = new ApiResult<List<PayType>>();
			var list_Paytype = _context.Queryable<WebPaytype>().Where(u => u.Status == 1).Select(u => new
			{
				Paytypeid = u.Paytypeid,
				Ico = u.Ico,
				PayName = u.Title,
				isScan = u.IsScan,
				isWeb = u.IsWeb
			}).ToList();
			var payTypes = new List<PayType>();
			foreach (var model in list_Paytype)
			{
				payTypes.Add(new PayType()
				{
					Paytypeid = model.Paytypeid,
					Ico = ResourceDomain(model.Ico),
					PayName = model.PayName,
					isScan = model.isScan,
					isWeb = model.isWeb
				});
			}
			result.Data = payTypes;
			return result;
		}
		#endregion
	}
}