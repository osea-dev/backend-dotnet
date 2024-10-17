using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Base;
using WeTalk.Models;
using WeTalk.Models.Dto.Common;
using WeTalk.Models.Dto.Order;

namespace WeTalk.Interfaces.Services
{
	public partial class OrderService : BaseService, IOrderService
	{
		private readonly SqlSugarScope _context;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包

		private readonly IUserManage _userManage;
        private readonly ISobotBaseService _sobotBaseService;
		private readonly IPubConfigBaseService _pubConfigBaseService;
		private readonly ICommonBaseService _commonBaseService;
		private readonly IMessageBaseService _messageBaseService;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogger<OrderService> _logger;
		private readonly ICourseBaseService _courseBaseService;

        public OrderService(SqlSugarScope dbcontext, IStringLocalizer<LangResource> localizer, IPubConfigBaseService pubConfigBaseService, IHttpContextAccessor accessor,
            IMessageBaseService messageBaseService, ICommonBaseService commonBaseService,ISobotBaseService sobotBaseService, IUserManage userManage, ILogger<OrderService> logger,
            ICourseBaseService courseBaseService)
		{
			_accessor = accessor;
            _context = dbcontext;
			_localizer = localizer;

			_userManage = userManage;
			_sobotBaseService = sobotBaseService;
			_pubConfigBaseService = pubConfigBaseService;
			_messageBaseService = messageBaseService;
            _commonBaseService = commonBaseService;
			_logger = logger;
			_courseBaseService = courseBaseService;
        }

		#region 支付方式列表
		/// <summary>
		/// 支付方式列表
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<PayType>>> PayTypes()
		{
			return await _commonBaseService.PayTypes();
		}
		#endregion

		#region 加载确认订单商品信息
		/// <summary>
		/// 加载确认订单商品信息
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<ConfirmOrderDto>> ConfirmOrder(long courseSkuid)
		{
			var result = new ApiResult<ConfirmOrderDto>();
			var model_Sku = _context.Queryable<WebCourseSku>().LeftJoin<WebSkuTypeLang>((l,r)=>l.SkuTypeid==r.SkuTypeid && r.Lang==_userManage.Lang)
				.Where((l,r)=>l.CourseSkuid == courseSkuid && l.Status==1)
				.Select((l,r)=>new { l.SkuTypeid,l.CourseSkuid,l.Courseid,l.IcoFont,r.Title,l.ClassHour,l.Price,l.MarketPrice })
				.First();
			if (model_Sku == null) {
				result.StatusCode = 4009;
				result.Message = _localizer["所选课程不存在"];
				return result;
			}
			var model_Course = _context.Queryable<WebCourse>().LeftJoin<WebCourseLang>((l,r)=>l.Courseid==r.Courseid && r.Lang==_userManage.Lang)
				.Where((l,r)=>l.Courseid == model_Sku.Courseid && l.Status == 1)
				.Select((l,r)=>new {l.Courseid,r.Title,r.Message,l.Img })
				.First();
			if (model_Course == null)
			{
				result.StatusCode = 4009;
				result.Message = _localizer["所选课程不存在"];
				return result;
			}
			var currency_code = "";//展示币种
			var country_code = _userManage.GetCountryCode();//取IP定位的国家代码
			currency_code = _userManage.GetCurrencyCode(country_code);
			if (string.IsNullOrEmpty(currency_code))
			{
				result.StatusCode = 4002;
				result.Message = _localizer["客户端币种取值异常"];
				return result;
			}
			decimal market_price = 0.00M, price = 0.00M;
			var model_Price = _context.Queryable<WebCourseSkuPrice>().First(u => u.CurrencyCode == currency_code && u.CourseSkuid == courseSkuid);
			if (model_Price != null)
			{
				market_price = model_Price.MarketPrice;
				price = model_Price.Price;
			}
			else
			{
				//美元
				market_price = model_Sku.MarketPrice;
				price = model_Sku.Price;
			}
			var model_Currency = _context.Queryable<PubCurrency>().First(u => u.CurrencyCode.ToUpper() == currency_code.ToUpper());
			var payTypes = new List<PayType>();
			var result_Paytype = await _commonBaseService.PayTypes();
			payTypes=(result_Paytype.StatusCode == 0)?result_Paytype.Data:null;
			result.Data = new ConfirmOrderDto()
			{
				CourseSkuid = courseSkuid,
				Img = _commonBaseService.ResourceDomain(model_Course.Img),
				Title = model_Course.Title,
				Message = model_Course.Message,
				Type = model_Sku.Title,
				SkuTypeid = model_Sku.SkuTypeid,
				Ico = model_Currency?.Ico + "",
				ClassHour = model_Sku.ClassHour,
				CurrencyCode = currency_code,
				MarketPrice = market_price,
				Price = price,
				PayTypes = payTypes
			};
			return result;
		}
		#endregion

		#region 创建订单
		/// <summary>
		/// 创建订单
		/// </summary>
		/// <param name="courseSkuid">课程Sku Id</param>
		/// <returns></returns>
		public async Task<ApiResult<OrderDto>> CreateOrder(long courseSkuid)
		{
			var result = new ApiResult<OrderDto>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			var model_Sku = _context.Queryable<WebCourseSku>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid && r.Lang == _userManage.Lang)
				.Where((l, r) => l.CourseSkuid == courseSkuid && l.Status == 1)
				.Select((l, r) => new { l.SkuTypeid, l.CourseSkuid, l.Courseid,l.MenkeCourseId, l.IcoFont, r.Title, l.ClassHour, l.Price, l.MarketPrice })
				.First();
			if (model_Sku == null)
			{
				result.StatusCode = 4009;
				result.Message = _localizer["所选课程不存在"];
				return result;
			}
			var model_Course = _context.Queryable<WebCourse>().LeftJoin<WebCourseLang>((l, r) => l.Courseid == r.Courseid && r.Lang == _userManage.Lang)
				.Where((l, r) => l.Courseid == model_Sku.Courseid && l.Status == 1)
				.Select((l, r) => new { l.Courseid, r.Title, r.Message, l.Img })
				.First();
			if (model_Course == null)
			{
				result.StatusCode = 4009;
				result.Message = _localizer["所选课程不存在"];
				return result;
			}
			var currency_code = "";//展示币种
			var country_code = _userManage.GetCountryCode();//取IP定位的国家代码
			currency_code = _userManage.GetCurrencyCode(country_code);
			if (string.IsNullOrEmpty(currency_code))
			{
				result.StatusCode = 4002;
				result.Message = _localizer["客户端币种取值异常"];
				return result;
			}
			long course_sku_priceid = 0;
			decimal market_price = 0.00M, price = 0.00M;
			var model_Price = _context.Queryable<WebCourseSkuPrice>().First(u => u.CurrencyCode == currency_code && u.CourseSkuid == courseSkuid);
			if (model_Price != null)
			{
				market_price = model_Price.MarketPrice;
				price = model_Price.Price;
				course_sku_priceid = model_Price.CourseSkuPriceid;
			}
			else
			{
				//美元
				market_price = model_Sku.MarketPrice;
				price = model_Sku.Price;
				course_sku_priceid = 0;
			}

			string order_sn = _commonBaseService.CreateOrderSn("A", _userManage.Userid,0);
			var model_Order = new WebOrder();
			model_Order.CourseSkuid = courseSkuid;
			model_Order.OrderSn = order_sn;
			model_Order.Courseid = model_Sku.Courseid;
			model_Order.CourseSkuid = model_Sku.CourseSkuid;
			model_Order.CourseSkuPriceid = course_sku_priceid;
			model_Order.Userid = _userManage.Userid;
			model_Order.Title = model_Course.Title;
			model_Order.Img = model_Course.Img;
			model_Order.Message = model_Course.Message;
			model_Order.SkuTypeid= model_Sku.SkuTypeid;
			model_Order.ClassHour= model_Sku.ClassHour;
			model_Order.MenkeCourseId = model_Sku.MenkeCourseId;
			model_Order.MarketPrice= model_Sku.MarketPrice;
			model_Order.Amount = price;
			model_Order.Ip = IpHelper.GetCurrentIp(_accessor.HttpContext);
			model_Order.CurrencyCode = currency_code;
			model_Order.CountryCode = country_code;
			var orderid = _context.Insertable(model_Order).ExecuteReturnBigIdentity();
			result.Data = new OrderDto()
			{
				Orderid = orderid,
				OrderSn = order_sn
			};
			return result;
		}
		#endregion

		#region 确认支付
		/// <summary>
		/// 确认支付
		/// </summary>
		/// <param name="orderid">订单ID</param>
		/// <param name="payTypeid">支付类型ID</param>
		/// <param name="type">默认1;1网页,2手机网页,3微信浏览器</param>
		/// <returns></returns>
		public async Task<ApiResult<OrderPayDto>> OrderPay(long orderid, long payTypeid, int type = 1)
		{
			var result = new ApiResult<OrderPayDto>();
			result.Data = new OrderPayDto();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			var model_Paytype = _context.Queryable<WebPaytype>().InSingle(payTypeid);
			if (model_Paytype == null) {
				result.StatusCode = 4009;
				result.Message = _localizer["支付类型不存在"];
				return result;
			}
			var model_Order = _context.Queryable<WebOrder>().InSingle(orderid);
			if (model_Order == null) {
				result.StatusCode = 4009;
				result.Message = _localizer["订单不存在"];
				return result;
			}
			string lang = _userManage.Lang;
			if (model_Paytype.Code.ToLower() == "unionpay") lang = "en";
			var model_SkuType = _context.Queryable<WebSkuTypeLang>().First(u => u.SkuTypeid == model_Order.SkuTypeid && u.Lang == lang);
			string SkuType = (model_SkuType != null) ? model_SkuType.Title : "SkuTypeid="+ model_Order.SkuTypeid;

            var dic_config = _pubConfigBaseService.GetConfigs("bananapay_access_key,bananapay_sign_key,bananapay_notify_key,stripe_apikey");
			
			JObject o = null;
			string url = "", json = "";
            var arr_data = new List<string>();
            var str_code = "";
            var dic_data = new Dictionary<string, object>();

			result.Data.Amount = model_Order.Amount;
			result.Data.CurrencyCode = model_Order.CurrencyCode;
			result.Data.Ico = _context.Queryable<PubCurrency>().First(u => u.CurrencyCode.ToUpper() == model_Order.CurrencyCode.ToUpper())?.Ico + "";

			if (model_Paytype != null)
			{
				result.Data.PayType.Ico = _commonBaseService.ResourceDomain(model_Paytype.Ico);
				result.Data.PayType.PayName = _localizer[model_Paytype.Title];
				result.Data.PayType.isScan = model_Paytype.IsScan;
				result.Data.PayType.isWeb = model_Paytype.IsWeb;
			}
			switch (model_Paytype.PayGateway.ToLower())
			{
				case "bananapay":
					#region Bananapay
                    string bananapay_access_key = dic_config.ContainsKey("bananapay_access_key") ? dic_config["bananapay_access_key"] : "";
                    string bananapay_sign_key = dic_config.ContainsKey("bananapay_sign_key") ? dic_config["bananapay_sign_key"] : "";
                    string bananapay_notify_key = dic_config.ContainsKey("bananapay_notify_key") ? dic_config["bananapay_notify_key"] : "";
                    if (string.IsNullOrEmpty(bananapay_access_key) || string.IsNullOrEmpty(bananapay_sign_key) || string.IsNullOrEmpty(bananapay_notify_key))
                    {
                        result.StatusCode = 4009;
                        result.Message = _localizer["支付参数不完整"];
                        return result;
                    }
                    string title = "";
					if (model_Paytype.Code.ToLower() == "unionpay")
					{
						////银联不能有中文，特殊处理
						//var model_CourseLang = _context.Queryable<WebCourseLang>().First(u => u.Courseid == model_Order.Courseid && u.Lang == lang);
						//if (model_CourseLang != null)
						//{
						//	title = model_CourseLang.Title + $"[{SkuType}.{model_Order.ClassHour} class hours]";
						//}
						//else
						//{
						//	title = "CourseId=" + model_Order.Courseid + $"[{SkuType}.{model_Order.ClassHour} class hours]";
						//}
						title = model_Order.OrderSn;
					}
					else {
						title = model_Order.Title + $"[{SkuType}.{model_Order.ClassHour}{_localizer["课时"]}]";
					}
					int expire = 1;
					string order_sn_ext = RandomHelper.RandCode(8);
					model_Order.OrderSnExt = order_sn_ext;
					switch (model_Paytype.Code.ToLower()) {
						case "weixin"://扫码scanpay
							switch (type)
							{
								case 1://PC
									url = Appsettings.app("APIS:Cashier.Payment.ScanPay");
									dic_data.Add("access_key", bananapay_access_key);
									dic_data.Add("pay_way", model_Paytype.Code.ToLower());//支付方式：“weixin”、“alipay”、“unionpay”、”gcashpay”
									dic_data.Add("out_trade_no", model_Order.OrderSn + "" + order_sn_ext);//out_trade_no 可以一起传也可以不传，它必须唯一，可以用户系统相对应的关系字段
									dic_data.Add("tname", title);//商户自定义商品名，银联支付的商品名不能包含特殊字符或中文
									dic_data.Add("tprice", model_Order.Amount.ToString("0.00"));//价格，1.02；
									dic_data.Add("fee_type", model_Order.CurrencyCode);//币种代码
									dic_data.Add("expire", expire);//单位分钟，微信为二维码有效时间，支付宝为用户扫码后允许支付的时间
									dic_data.Add("platform", "Wetalk");//平台
									dic_data.Add("notify_url", Appsettings.app("Pay:NotifyUrl"));//回调通知 URL, 不允许带传参数，
									dic_data.Add("attach_data", "{\"order_sn\":\"" + model_Order.OrderSn + "\",\"pay_code\":\"bananapay\"}");//自定义附加内容，长度 500 以内的 JSON 字符串
									dic_data.Add("extend_params", "bananapay");//自定义参数
									dic_data.Add("ts", DateHelper.ConvertDateTimeInt(DateTime.Now));//时间戳
									arr_data = dic_data.Keys.ToList();
									str_code = "";
									arr_data.Sort(string.CompareOrdinal);
									foreach (var key in arr_data)
									{
										if (arr_data.IndexOf(key) > 0) str_code += "&";
										str_code += $"{key}={dic_data[key]}";
									}
									str_code += "&key=" + bananapay_sign_key;
									dic_data.Add("sign", MD5Helper.MD5Encrypt32(str_code));//md5签名

									json = GetRemoteHelper.HttpWebRequestUrl(url, JsonConvert.SerializeObject(dic_data), "utf-8", "post", null, "application/json");
									try
									{
										o = JObject.Parse(json);
										if (o != null && o["errno"] != null && o["errno"].ToString() == "0")
										{
											result.Data.Expire = expire;
											result.Data.Qrcode = o["results"]["qrcode"].ToString();//{out_trade_no,api_url,pay_way}
											result.Data.PayWay = o["results"]["pay_way"].ToString();
											result.Data.OrderSn = model_Order.OrderSn;
											result.Data.Orderid = model_Order.Orderid;
										}
										else
										{
											result.Data = null;
											result.StatusCode = 4002;
											result.Message = (o["message"] != null) ? o["message"].ToString() : "";
										}
									}
									catch
									{
										result.Data = null;
										result.StatusCode = 4002;
										result.Message = json;
									}
									break;
								case 2://手机
									url = Appsettings.app("APIS:Cashier.Payment.ScanPay");
									dic_data.Add("access_key", bananapay_access_key);
									dic_data.Add("pay_way", model_Paytype.Code.ToLower());//支付方式：“weixin”、“alipay”、“unionpay”、”gcashpay”
									dic_data.Add("out_trade_no", model_Order.OrderSn + "" + order_sn_ext);//out_trade_no 可以一起传也可以不传，它必须唯一，可以用户系统相对应的关系字段
									dic_data.Add("tname", title);//商户自定义商品名，银联支付的商品名不能包含特殊字符或中文
									dic_data.Add("tprice", model_Order.Amount.ToString("0.00"));//价格，1.02；
									dic_data.Add("fee_type", model_Order.CurrencyCode);//币种代码
									dic_data.Add("expire", expire);//单位分钟，微信为二维码有效时间，支付宝为用户扫码后允许支付的时间
									dic_data.Add("product_code", "WAP");//WEB,WAP,APP,MINI_APP
									dic_data.Add("platform", "Wetalk");//平台
									dic_data.Add("notify_url", Appsettings.app("Pay:NotifyUrl"));//回调通知 URL, 不允许带传参数，
									dic_data.Add("attach_data", "{\"order_sn\":\"" + model_Order.OrderSn + "\",\"pay_code\":\"bananapay\"}");//自定义附加内容，长度 500 以内的 JSON 字符串
									dic_data.Add("extend_params", "bananapay");//自定义参数
									dic_data.Add("ts", DateHelper.ConvertDateTimeInt(DateTime.Now));//时间戳
									arr_data = dic_data.Keys.ToList();
									str_code = "";
									arr_data.Sort(string.CompareOrdinal);
									foreach (var key in arr_data)
									{
										if (arr_data.IndexOf(key) > 0) str_code += "&";
										str_code += $"{key}={dic_data[key]}";
									}
									str_code += "&key=" + bananapay_sign_key;
									dic_data.Add("sign", MD5Helper.MD5Encrypt32(str_code));//md5签名

									json = GetRemoteHelper.HttpWebRequestUrl(url, JsonConvert.SerializeObject(dic_data), "utf-8", "post", null, "application/json");
									try
									{
										o = JObject.Parse(json);
										if (o != null && o["errno"] != null && o["errno"].ToString() == "0")
										{
											result.Data.Expire = expire;
											result.Data.Qrcode = o["results"]["qrcode"].ToString();//{out_trade_no,api_url,pay_way}
											result.Data.PayWay = o["results"]["pay_way"].ToString();
											result.Data.OrderSn = model_Order.OrderSn;
											result.Data.Orderid = model_Order.Orderid;
										}
										else
										{
											result.Data = null;
											result.StatusCode = 4002;
											result.Message = (o["message"] != null) ? o["message"].ToString() : "";
										}
									}
									catch
									{
										result.Data = null;
										result.StatusCode = 4002;
										result.Message = json;
									}
									break;
							}
                            break;
                        case "alipay"://支付宝用alionlinepay
                            url = Appsettings.app("APIS:Cashier.Payment.AliOnlinePay");
                            dic_data.Add("access_key", bananapay_access_key);
                            dic_data.Add("pay_way", model_Paytype.Code.ToLower());//支付方式：“weixin”、“alipay”、“unionpay”、”gcashpay”
                            dic_data.Add("tname", title);//商户自定义商品名，银联支付的商品名不能包含特殊字符或中文
                            dic_data.Add("tprice", model_Order.Amount.ToString("0.00"));//价格，1.02；
                            dic_data.Add("fee_type", model_Order.CurrencyCode);//币种代码
                            dic_data.Add("out_trade_no", model_Order.OrderSn + "" + order_sn_ext);//out_trade_no 可以一起传也可以不传，它必须唯一，可以用户系统相对应的关系字段
                            dic_data.Add("product_code", "NEW_OVERSEAS_SELLER");//web 支付填’NEW_OVERSEAS_SELLER’，wap 支付，小程序支付填 ’NEW_WAP_OVERSEAS_SELLER
							dic_data.Add("trade_information", "{\"business_type\",\"goods_info\":\""+ title + "^1|\",\"total_quantity\":1}");//交易信息,参考https://global.alipay.com/doc/global/mobile_securitypay_pay_cn#diRMf
							dic_data.Add("notify_url", Appsettings.app("Pay:NotifyUrl"));//回调通知 URL, 不允许带传参数，
                            dic_data.Add("return_url", Appsettings.app("Pay:SuccessUrl")); //付款后跳转 ur
                            dic_data.Add("platform", "Wetalk");//平台
							dic_data.Add("attach_data", "{\"order_sn\":\"" + model_Order.OrderSn + "\",\"pay_code\":\"bananapay\"}");//自定义附加内容，长度 500 以内的 JSON 字符串
                            dic_data.Add("ts", DateHelper.ConvertDateTimeInt(DateTime.Now));//时间戳
                            arr_data = dic_data.Keys.ToList();
                            str_code = "";
                            arr_data.Sort(string.CompareOrdinal);
                            foreach (var key in arr_data)
                            {
                                if (arr_data.IndexOf(key) > 0) str_code += "&";
                                str_code += $"{key}={dic_data[key]}";
                            }
                            str_code += "&key=" + bananapay_sign_key;
                            dic_data.Add("sign", MD5Helper.MD5Encrypt32(str_code));//md5签名

                            json = GetRemoteHelper.HttpWebRequestUrl(url, JsonConvert.SerializeObject(dic_data), "utf-8", "post", null, "application/json");
                            try
                            {
                                o = JObject.Parse(json);
                                if (o != null && o["errno"] != null && o["errno"].ToString() == "0")
                                {
                                    result.Data.ApiUrl = (o["results"]["pay_url"] != null) ? o["results"]["pay_url"].ToString() : "";
                                    result.Data.PayWay = (o["results"]["pay_way"] != null) ? o["results"]["pay_way"].ToString() : "";
                                    result.Data.OrderSn = model_Order.OrderSn;
                                    result.Data.Orderid = model_Order.Orderid;
                                }
                                else
                                {
                                    result.Data = null;
                                    result.StatusCode = 4002;
                                    result.Message = (o["message"] != null) ? o["message"].ToString() : "";
                                }
                            }
                            catch
                            {
                                result.Data = null;
                                result.StatusCode = 4002;
                                result.Message = json;
                            }
                            break;
                        default://银联 gcash grab用jsapi
                            url = Appsettings.app("APIS:Cashier.Payment.JsapiPay");
                            dic_data.Add("access_key", bananapay_access_key);
                            dic_data.Add("pay_way", model_Paytype.Code.ToLower());//支付方式：“weixin”、“alipay”、“unionpay”、”gcashpay”
                            dic_data.Add("out_trade_no", model_Order.OrderSn + "" + order_sn_ext);//out_trade_no 可以一起传也可以不传，它必须唯一，可以用户系统相对应的关系字段
                            dic_data.Add("tname", title);//商户自定义商品名，银联支付的商品名不能包含特殊字符或中文
                            dic_data.Add("tprice", model_Order.Amount.ToString("0.00"));//价格，1.02；
                            dic_data.Add("fee_type", model_Order.CurrencyCode);//币种代码
                            dic_data.Add("expire", expire.ToString());//单位分钟，默认10分钟
                            dic_data.Add("platform", "Wetalk");//平台
                            dic_data.Add("notify_url", Appsettings.app("Pay:NotifyUrl"));//回调通知 URL, 不允许带传参数
                            dic_data.Add("jump_url", Appsettings.app("Pay:SuccessUrl"));//支付成功后跳转链接
                            dic_data.Add("cancel_jump_url", Appsettings.app("Pay:CancelUrl"));//gcash 支付失败后跳转链
                            dic_data.Add("attach_data", "{\"order_sn\":\"" + model_Order.OrderSn + "\",\"pay_code\":\"bananapay\"}");//自定义附加内容，长度 500 以内的 JSON 字符串
                            dic_data.Add("extend_params", "bananapay");//自定义参数
                            dic_data.Add("ts", DateHelper.ConvertDateTimeInt(DateTime.Now));//时间戳
                            arr_data = dic_data.Keys.ToList();
                            str_code = "";
                            arr_data.Sort(string.CompareOrdinal);
                            foreach (var key in arr_data)
                            {
                                if (arr_data.IndexOf(key) > 0) str_code += "&";
                                str_code += $"{key}={dic_data[key]}";
                            }
                            str_code += "&key=" + bananapay_sign_key;
                            dic_data.Add("sign", MD5Helper.MD5Encrypt32(str_code));//md5签名

                            json = GetRemoteHelper.HttpWebRequestUrl(url, JsonConvert.SerializeObject(dic_data), "utf-8", "post", null, "application/json");
                            try
                            {
                                o = JObject.Parse(json);
                                if (o != null && o["errno"] != null && o["errno"].ToString() == "0" && o["results"] != null)
                                {
                                    result.Data.ApiUrl = (o["results"]["api_url"] != null) ? o["results"]["api_url"].ToString() : "";
                                    result.Data.PayWay = (o["results"]["pay_way"] != null) ? o["results"]["pay_way"].ToString() : "";
                                    result.Data.OrderSn = model_Order.OrderSn;
                                    result.Data.Orderid = model_Order.Orderid;
                                }
                                else
                                {
                                    result.StatusCode = 4002;
                                    result.Message = (o["message"] != null) ? o["message"].ToString() : "";
                                    result.Data = null;
                                }
                            }
                            catch
                            {
                                result.Data = null;
                                result.StatusCode = 4002;
                                result.Message = json;
                            }
                            break;
                    }

					model_Order.PayStatus = 1;
                    model_Order.Paytime = DateTime.Now;
                    model_Order.Paytypeid = payTypeid;
                    _context.Updateable(model_Order).ExecuteCommand();
                    #endregion
                    break;
				case "stripe":
					#region Stripe
					string stripe_apikey = dic_config.ContainsKey("stripe_apikey") ? dic_config["stripe_apikey"] : "";
                    if (string.IsNullOrEmpty(stripe_apikey))
                    {
                        result.StatusCode = 4009;
                        result.Message = _localizer["支付参数不完整"];
                        return result;
                    }
					StripeConfiguration.ApiKey = stripe_apikey;
					var options = new SessionCreateOptions
                    {
						PaymentMethodTypes = new List<string>
						{
							"card"    //支持的付款方式
						},
						//BillingAddressCollection = "required",//是否要收集帐单地址信息
                        LineItems = new List<SessionLineItemOptions>
						{
							new SessionLineItemOptions
							{
								// Provide the exact Price ID (for example, pr_1234) of the product you want to sell
								//Price = "{{PRICE_ID}}",
								PriceData = new SessionLineItemPriceDataOptions
								{
									UnitAmount = (long)(model_Order.Amount * 100),
									Currency = model_Order.CurrencyCode.ToLower(),
									ProductData = new SessionLineItemPriceDataProductDataOptions
									{
										Name = model_Order.Title + $"[{SkuType}.{model_Order.ClassHour}{_localizer["课时"]}]",
										Description = model_Order.OrderSn
									}
								},
								Quantity = 1
							},
						},
						Mode = "payment",
						SuccessUrl = string.Format(Appsettings.app("Pay:SuccessUrl"),model_Order.Orderid),//成功跳转
						CancelUrl = string.Format(Appsettings.app("Pay:CancelUrl"), model_Order.Orderid),//失败跳转
					};
                    options.PaymentIntentData = new SessionPaymentIntentDataOptions();
                    options.PaymentIntentData.Metadata = new Dictionary<string, string>
					{
                          { "order_sn", model_Order.OrderSn }, { "orderid", model_Order.Orderid.ToString() },  //传递的自定义参数，回调通知的时候会原样返回
					};
                    var service = new SessionService();
					Session session = service.Create(options);

					result.Data.ApiUrl = session.Url;
                    result.Data.PayWay = "stripe";
					result.Data.OrderSn = model_Order.OrderSn;
					result.Data.Orderid = model_Order.Orderid;
					model_Order.PayStatus = 1;
					model_Order.Paytime = DateTime.Now;
					model_Order.Paytypeid = payTypeid;
                    _context.Updateable(model_Order).ExecuteCommand();
                    #endregion
                    break;
                default:
					
					break;
			}
			return result;
		}
        #endregion

        #region 支付回调
        /// <summary>
        /// 确认支付
        /// </summary>
        /// <param name="orderid">订单ID</param>
        /// <param name="payTypeid">支付类型ID</param>
        /// <param name="type">默认1;1网页,2扫码</param>
        /// <returns></returns>
        public async Task<string> NotifyUrl(string action)
        {
			_logger.LogInformation("action=" + action);
			WebOrder model_Order = null;
            var dic_data = new Dictionary<string, object>();
			long user_courseid = 0;
            switch (action.ToLower()) {
				case "stripe"://
                    string endpointSecret = _pubConfigBaseService.GetConfig("stripe_success_key");
					try
					{
                        var stripeEvent = EventUtility.ConstructEvent(_userManage.RequestBody.Replace("\r",""), _accessor.HttpContext.Request.Headers["Stripe-Signature"].ToString(), endpointSecret);
						// Handle the event
						if (stripeEvent.Type == Events.ChargeSucceeded)
						{
                            var charge = stripeEvent.Data.Object as Charge;
							var dic_meta = charge.Metadata;
							if (dic_meta.ContainsKey("order_sn") && !string.IsNullOrEmpty(dic_meta["order_sn"]))
							{
								model_Order = _context.Queryable<WebOrder>().First(u => u.OrderSn == dic_meta["order_sn"]);
								if (model_Order != null)
								{
									model_Order.PayStatus = 2;
									model_Order.Remark = $"[{DateTime.Now}]回调检查支付成功<hr>" + model_Order.Remark;
									model_Order.Status = 1;
									model_Order.Payment = (decimal)(charge.Amount/100);
									if (!string.IsNullOrEmpty(charge.PaymentIntentId))
									{
										model_Order.TransactionId = charge.PaymentIntentId;
									}else if (!string.IsNullOrEmpty(charge.PaymentMethod))
									{
										model_Order.TransactionId = charge.PaymentMethod;
									}
									_context.Updateable(model_Order).ExecuteCommand();

									//2023-3-6,新增兼容直播课，仅众语课与直播课需要创建“我的课程”
									if (!_context.Queryable<WebUserCourse>().Any(u=>u.OrderSn==model_Order.OrderSn && u.Status==1) && (model_Order.Type == 0 || model_Order.Type == 1))
									{
										var model_UserCourse = new WebUserCourse();
										model_UserCourse.OrderSn = model_Order.OrderSn;
										model_UserCourse.Orderid = model_Order.Orderid;
										model_UserCourse.Userid = model_Order.Userid;
										model_UserCourse.Courseid = model_Order.Courseid;
										model_UserCourse.MenkeCourseId = 0;// model_Order.MenkeCourseId;不能再用课程关联ID，需要重新分配
										model_UserCourse.Title = model_Order.Title;
										model_UserCourse.Img = model_Order.Img;
										model_UserCourse.Message = model_Order.Message;
										model_UserCourse.ClassHour = model_Order.ClassHour;
										model_UserCourse.Status = 1;
										model_UserCourse.SkuTypeid = model_Order.SkuTypeid;

										model_UserCourse.Type = model_Order.Type;//2023-3-6 新增兼容直播课支付订单回调
										model_UserCourse.OnlineCourseid= model_Order.OnlineCourseid;
										if (model_Order.Type == 0)	//2023-3-6 仅众语直营课触发
										{
											//工单场景：完成购买
											dic_data.Clear();
											var model_User = _context.Queryable<WebUser>().InSingle(model_Order.Userid);
											if (model_User != null)
											{
												var model_skutype = _context.Queryable<WebSkuTypeLang>().First(u => u.SkuTypeid == model_Order.SkuTypeid && u.Lang == model_User.Lang);
												dic_data.Add(_localizer["购买人"], model_User.FirstName + " " + model_User.LastName);
												dic_data.Add(_localizer["购买人手机"], model_User.MobileCode + "-" + model_User.Mobile);
												dic_data.Add(_localizer["购买人邮箱"], model_User.Email);
												dic_data.Add(_localizer["所在地时区"], model_User.Utc);
												dic_data.Add(_localizer["所选课程"], model_Order.Title + "");
												dic_data.Add(_localizer["上课方式"], model_skutype?.Title + "");
												dic_data.Add(_localizer["购买课时"], model_Order.ClassHour);
												dic_data.Add(_localizer["购买时间"], model_Order.Paytime + "");
												dic_data.Add(_localizer["售价"], model_Order.Payment);
												dic_data.Add(_localizer["币种"], model_Order.CurrencyCode);
												dic_data.Add(_localizer["付款方式"], "Stripe");

												var body = string.Join("<br>", dic_data.ToList());
												var result_ticket = await _sobotBaseService.AddUserTicket(_localizer["完成购课提醒"], body);
												if (result_ticket.StatusCode == 0)
												{
													model_UserCourse.BuyTicketId = result_ticket.Data;
													model_UserCourse.IsBuyTicket = 1;
												}
												else
												{
													_logger.LogError(_localizer["完成购课提醒"] + "," + result_ticket.Message);
												}

												//触发邮件
												var dic_email = new Dictionary<string, string>();
												dic_email.Add("name", model_User.FirstName + " " + model_User.LastName);
												string head_img = string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg;
												dic_email.Add("head_img", PicHelper.ImageToBase64(_commonBaseService.ResourceDomain(head_img)));
												dic_email.Add("course_name", model_Order.Title);
												dic_email.Add("user_url", $"{Appsettings.app("Web:Host")}/user/course");
												dic_email.Add("order_url", $"{Appsettings.app("Web:Host")}/user/order");
												dic_email.Add("order_sn", model_Order.OrderSn);
												dic_email.Add("url", $"{string.Format(Appsettings.app("Web:ServiceUrl"), _userManage.Lang)}");
												await _messageBaseService.AddEmailTask("Buy", model_User.Email, dic_email, model_User.Lang);
											}
										}
										user_courseid = _context.Insertable(model_UserCourse).ExecuteReturnBigIdentity();

										var result_menke_course = await _courseBaseService.UserCourseArranging(user_courseid);
										if (result_menke_course.StatusCode != 0)
										{
											_logger.LogError("支付回调时创建拓课云课程[user_courseid=" + user_courseid + "]异常:" + result_menke_course.Message);
										}
										
                                    }
									return "success";
                                }
							}
						}
						else if (stripeEvent.Type == Events.ChargeFailed)
                        {
                            var charge = stripeEvent.Data.Object as Charge;
                            var dic_meta = charge.Metadata;
                            if (dic_meta.ContainsKey("order_sn") && !string.IsNullOrEmpty(dic_meta["order_sn"]))
                            {
                                model_Order = _context.Queryable<WebOrder>().First(u => u.OrderSn == dic_meta["order_sn"]);
                                if (model_Order != null)
                                {
                                    model_Order.PayStatus = 3;
                                    model_Order.Status = 0;
                                    _context.Updateable(model_Order).ExecuteCommand();
                                    return "success";
                                }
                            }
                        }
						else
						{
							Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
						}

                        return "success";
					}
					catch (Exception ex)
					{
						return "error";
					}
					break;
				default://BananaPay
					XmlDocument xmlDoc = new XmlDocument();
					try
					{
						xmlDoc.LoadXml(_userManage.RequestBody);
						string str_code = "";
						var nodeList = xmlDoc.ChildNodes.Item(0);
						foreach (XmlNode node in nodeList)
						{
							dic_data.Add(node.Name, node.InnerText);
						}
						if (!dic_data.ContainsKey("out_trade_no") || !dic_data.ContainsKey("sign") || !dic_data.ContainsKey("trade_status") || !dic_data.ContainsKey("transaction_id"))
						{
							_logger.LogInformation("回调参数异常");
							return "fail";
						}
						string order_sn = dic_data["out_trade_no"].ToString();
						if (dic_data["trade_status"].ToString().ToUpper() == "TRADE_SUCCESS")
						{
							var arr_data = new List<string>();
							arr_data = dic_data.Keys.ToList();
							arr_data.Sort(string.CompareOrdinal);
							foreach (var key in arr_data)
							{
								if (key == "sign") continue;
								if (arr_data.IndexOf(key) > 0) str_code += "&";
								str_code += $"{key}={dic_data[key]}";
							}
							str_code += "&key=" + _pubConfigBaseService.GetConfig("bananapay_notify_key");
							string sign = MD5Helper.MD5Encrypt32(str_code);//md5签名
							if (dic_data["sign"].ToString() != sign)
							{
								//签名较验异常
								_logger.LogInformation("签名较验异常");
								return "fail";
							}
							model_Order = _context.Queryable<WebOrder>().First(u => SqlFunc.MergeString(u.OrderSn,u.OrderSnExt) == order_sn);
							if (model_Order != null)
							{
								model_Order.PayStatus = 2;
								model_Order.Remark = $"[{DateTime.Now}]回调检查支付成功<hr>" + model_Order.Remark;
								model_Order.Status = 1;
								if (dic_data.ContainsKey("cash_fee"))
									model_Order.Payment = decimal.Parse(dic_data["cash_fee"].ToString());
								if (dic_data.ContainsKey("cash_fee_type"))
									model_Order.CurrencyCode = dic_data["cash_fee_type"].ToString();
								if (dic_data.ContainsKey("transaction_id"))
									model_Order.TransactionId = dic_data["transaction_id"].ToString();

								_context.Updateable(model_Order).ExecuteCommand();

								//2023-3-6,新增兼容直播课，仅众语课与直播课需要创建“我的课程”
								if (!_context.Queryable<WebUserCourse>().Any(u => u.OrderSn == model_Order.OrderSn && u.Status == 1) && (model_Order.Type == 0 || model_Order.Type == 1))
								{
									var model_UserCourse = new WebUserCourse();
									model_UserCourse.OrderSn = model_Order.OrderSn;
									model_UserCourse.Orderid = model_Order.Orderid;
									model_UserCourse.Userid = model_Order.Userid;
									model_UserCourse.Courseid = model_Order.Courseid;
									model_UserCourse.MenkeCourseId = 0;// model_Order.MenkeCourseId;不能再用课程关联ID，需要重新分配
									model_UserCourse.Title = model_Order.Title;
									model_UserCourse.Img = model_Order.Img;
									model_UserCourse.Message = model_Order.Message;
									model_UserCourse.ClassHour = model_Order.ClassHour;
									model_UserCourse.Status = 1;
									model_UserCourse.SkuTypeid = model_Order.SkuTypeid;


									model_UserCourse.Type = model_Order.Type;//2023-3-6 新增兼容直播课支付订单回调
									model_UserCourse.OnlineCourseid = model_Order.OnlineCourseid;
									if (model_Order.Type == 0)  //2023-3-6 仅众语直营课触发
									{
										//工单场景：完成购买
										dic_data.Clear();
										var model_User = _context.Queryable<WebUser>().InSingle(model_Order.Userid);
										if (model_User != null)
										{
											var model_skutype = _context.Queryable<WebSkuTypeLang>().First(u => u.SkuTypeid == model_Order.SkuTypeid && u.Lang == model_User.Lang);
											dic_data.Add(_localizer["购买人"], model_User.FirstName + " " + model_User.LastName);
											dic_data.Add(_localizer["购买人手机"], model_User.MobileCode + "-" + model_User.Mobile);
											dic_data.Add(_localizer["购买人邮箱"], model_User.Email);
											dic_data.Add(_localizer["所在地时区"], model_User.Utc);
											dic_data.Add(_localizer["所选课程"], model_Order.Title + "");
											dic_data.Add(_localizer["上课方式"], model_skutype?.Title + "");
											dic_data.Add(_localizer["购买课时"], model_Order.ClassHour);
											dic_data.Add(_localizer["购买时间"], model_Order.Paytime + "");
											dic_data.Add(_localizer["售价"], model_Order.Payment);
											dic_data.Add(_localizer["币种"], model_Order.CurrencyCode);
											dic_data.Add(_localizer["付款方式"], $"BananaPay-{(dic_data.ContainsKey("pay_way") ? dic_data["pay_way"] : "")}");

											var body = string.Join("<br>", dic_data.ToList());
											var result_ticket = await _sobotBaseService.AddUserTicket(_localizer["完成购买提醒"], body);
											if (result_ticket.StatusCode == 0)
											{
												model_UserCourse.BuyTicketId = result_ticket.Data;
												model_UserCourse.IsBuyTicket = 1;
											}
											else
											{
												_logger.LogError(_localizer["完成购课提醒"] + "," + result_ticket.Message);
											}
											//触发邮件
											var dic_email = new Dictionary<string, string>();
											dic_email.Add("name", model_User.FirstName + " " + model_User.LastName);
											string head_img = string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg;
											dic_email.Add("head_img", PicHelper.ImageToBase64(_commonBaseService.ResourceDomain(head_img)));
											dic_email.Add("course_name", model_Order.Title);
											dic_email.Add("user_url", $"{Appsettings.app("Web:Host")}/user/course");
											dic_email.Add("order_url", $"{Appsettings.app("Web:Host")}/user/order");
											dic_email.Add("order_sn", model_Order.OrderSn);
											dic_email.Add("url", $"{string.Format(Appsettings.app("Web:ServiceUrl"), _userManage.Lang)}");
											await _messageBaseService.AddEmailTask("Buy", model_User.Email, dic_email, model_User.Lang);
										}
									}
                                    user_courseid = _context.Insertable(model_UserCourse).ExecuteReturnBigIdentity();
                                    var result_menke_course = await _courseBaseService.UserCourseArranging(user_courseid);
                                    if (result_menke_course.StatusCode != 0)
                                    {
                                        _logger.LogError("支付回调时创建拓课云课程[user_courseid=" + user_courseid + "]异常:" + result_menke_course.Message);
                                    }

                                }

                                return "success";
							}
						}
						else
						{
							model_Order = _context.Queryable<WebOrder>().First(u => SqlFunc.MergeString(u.OrderSn, u.OrderSnExt) == order_sn);
							if (model_Order != null)
							{
								switch (dic_data["trade_status"].ToString().ToUpper()) {
									case "TRADE_CLOSED":
                                        model_Order.PayStatus = 2;
                                        model_Order.Status = 2;
                                        break;
									case "TRADE_FAILED":
                                        model_Order.PayStatus = 3;
                                        model_Order.Status = 2;
                                        break;
                                }
								
								_context.Updateable(model_Order).ExecuteCommand();
								return "success";
							}
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, _userManage.RequestBody);
					}
					break;
			}
            return "";
        }
		#endregion

		#region 查询订单状态
		/// <summary>
		/// 查询订单状态
		/// </summary>
		/// <param name="order_sn">订单号</param>
		/// <param name="orderid">订单id</param>
		/// <returns></returns>
		public async Task<ApiResult<OrderStatusDto>> GetOrderStatus(string order_sn,long orderid)
		{
			var result = new ApiResult<OrderStatusDto>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			if (string.IsNullOrEmpty(order_sn) && orderid == 0)
			{
				result.StatusCode = 4003;
				result.Message = _localizer["订单ID与订单号均不存在"];
				return result;
			}
			var model_Order = _context.Queryable<WebOrder>().First(u => (u.Orderid == orderid || u.OrderSn == order_sn) && u.Userid == _userManage.Userid);
			if (model_Order != null)
			{
				result.Data = new OrderStatusDto();
				result.Data.Status = model_Order.Status;
			}
			else
			{
				result.StatusCode = 4009;
				result.Message = _localizer["订单不存在"];
			}
			return result;
		}
		#endregion

		#region 订单列表（众语课）
		/// <summary>
		/// 订单列表（众语课）
		/// </summary>
		/// <param name="payStatus">支付状态:0全部，1待支付，2已支付</param>
		/// <returns></returns>
		public async Task<ApiResult<Pages<OrderDetailDto>>> OrderList(int payStatus=0, int page = 1, int pageSize = 10)
		{
			var result = new ApiResult<Pages<OrderDetailDto>>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			result.Data = new Pages<OrderDetailDto>();
			int total = 0;
			var list_Order = _context.Queryable<WebOrder>().Where(u => u.Status != -1 && u.UserDeleted==0 && u.Userid== _userManage.Userid && u.Type==0)
				.WhereIF(payStatus > 0, u => u.Status == (payStatus - 1))
				.OrderBy(u => u.Dtime, OrderByType.Desc)
				.ToPageList(page,pageSize,ref total);
			result.Data.Total = total;
			var list_Course = _context.Queryable<WebCourse>().LeftJoin<WebCourseLang>((l,r)=>l.Courseid==r.Courseid).Where((l,r) => list_Order.Select(s => s.Courseid).Contains(l.Courseid) && r.Lang == _userManage.Lang)
				.Select((l,r)=>new { l.Courseid,l.Img,r.Title,r.Message}).ToList();
			var list_SkuType = _context.Queryable<WebSkuTypeLang>().Where(u => list_Order.Select(s => s.SkuTypeid).Contains(u.SkuTypeid)).Select(u => new { u.SkuTypeid, u.Title }).ToList();
			var list_Paytype = _context.Queryable<WebPaytype>().ToList();
			var list_Currency = _context.Queryable<PubCurrency>().Where(u=>u.Status==1).ToList();


			foreach (var model_Order in list_Order) { 
				var order = new OrderDetailDto();
				order.Orderid= model_Order.Orderid;
				order.OrderSn= model_Order.OrderSn;
				order.Status = model_Order.Status;
				var model_Course = list_Course.FirstOrDefault(u => u.Courseid == model_Order.Courseid);
				if (model_Course != null)
				{
					order.Img = _commonBaseService.ResourceDomain(model_Course.Img);
					order.Title = model_Course.Title;
					order.Message = model_Course.Message;
				}
				var model_SkuType = list_SkuType.FirstOrDefault(u => u.SkuTypeid == model_Order.SkuTypeid);
				if(model_SkuType!=null) order.Type = model_SkuType.Title; 
				order.SkuTypeid = model_Order.SkuTypeid;
				order.ClassHour=model_Order.ClassHour;
				order.MarketPrice = model_Order.MarketPrice;
				var model_Currency = list_Currency.FirstOrDefault(u => u.CurrencyCode == model_Order.CurrencyCode);
				order.Ico = model_Currency?.Ico ?? "$";
				if (model_Order.Status == 1)
				{
					order.Price = model_Order.Payment;
					order.Paytime = DateHelper.ConvertDateTimeInt(model_Order.Paytime);
					order.PayType.Paytypeid = model_Order.Paytypeid;
				}
				else
				{
					order.Price = model_Order.Amount;
				}
				var model_Paytype = list_Paytype.FirstOrDefault(u => u.Paytypeid == model_Order.Paytypeid);
				if (model_Paytype != null)
				{
					order.PayType.Ico = _commonBaseService.ResourceDomain(model_Paytype.Ico);
					order.PayType.PayName = _localizer[model_Paytype.Title];
					order.PayType.isScan = model_Paytype.IsScan;
					order.PayType.isWeb = model_Paytype.IsWeb;
				}
				result.Data.List.Add(order);
			}
			return result;
		}
		#endregion

		#region 订单详情（众语课）
		/// <summary>
		/// 订单列表（众语课）
		/// </summary>
		/// <param name="orderid">订单id</param>
		/// <returns></returns>
		public async Task<ApiResult<OrderDetailDto>> OrderDetail(long orderid)
		{
			var result = new ApiResult<OrderDetailDto>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0) {
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			var model_Order = _context.Queryable<WebOrder>().First(u => u.Orderid == orderid && u.Userid == _userManage.Userid && u.Status!=-1 && u.UserDeleted == 0);
			if (model_Order != null)
			{
				var order = new OrderDetailDto();
				order.Orderid = model_Order.Orderid;
				order.OrderSn = model_Order.OrderSn;
				order.Status = model_Order.Status;

				//从订单表中取的是快照信息，原课程可以已删
				var model_Course = _context.Queryable<WebCourse>().LeftJoin<WebCourseLang>((l, r) => l.Courseid == r.Courseid).Where((l, r) => l.Courseid == model_Order.Courseid && r.Lang==_userManage.Lang)
					.Select((l, r) => new { l.Courseid,l.Img, r.Title, r.Message }).First();
				if (model_Course != null)
				{
					order.Img = _commonBaseService.ResourceDomain(model_Course.Img);
					order.Title = model_Course.Title;
					order.Message = model_Course.Message;
				}
				else
                {
                    order.Img = _commonBaseService.ResourceDomain(model_Order.Img + "");
                    order.Title = model_Order.Title;
                    order.Message = model_Order.Message;
                }
                var model_SkuType = _context.Queryable<WebSkuTypeLang>().First(u => u.SkuTypeid == model_Order.SkuTypeid);
				if (model_SkuType != null) order.Type = model_SkuType.Title;
				order.ClassHour = model_Order.ClassHour;
				order.MarketPrice = model_Order.MarketPrice;
				var model_Currency = _context.Queryable<PubCurrency>().First(u => u.CurrencyCode == model_Order.CurrencyCode);
				order.Ico = model_Currency?.Ico??"$";

				if (model_Order.Payment > 0)
				{
					order.Price = model_Order.Payment;
					order.Paytime = DateHelper.ConvertUtcDateTimeInt(model_Order.Paytime.ToUniversalTime());//给前端是UTC时间戳
				}
				else
				{
					order.Price = model_Order.Amount;
				}
				order.PayType.Paytypeid = model_Order.Paytypeid;
				var model_Paytype = _context.Queryable<WebPaytype>().First(u => u.Paytypeid == model_Order.Paytypeid);
				if (model_Paytype != null)
				{
					order.PayType.Ico = _commonBaseService.ResourceDomain(model_Paytype.Ico);
					order.PayType.PayName = _localizer[model_Paytype.Title];
					order.PayType.isScan = model_Paytype.IsScan;
					order.PayType.isWeb = model_Paytype.IsWeb;
				}
				result.Data = order;
			}
			else {
				result.StatusCode = 4009;
				result.Message = _localizer["订单不存在"];
			}
			return result;
		}
		#endregion

		#region 订单列表（直播课）
		/// <summary>
		/// 订单列表（直播课）
		/// </summary>
		/// <param name="payStatus">支付状态:0全部，1待支付，2已支付</param>
		/// <returns></returns>
		public async Task<ApiResult<Pages<OnlineOrderDetailDto>>> OnlineOrderList(int payStatus = 0, int page = 1, int pageSize = 10)
		{
			var result = new ApiResult<Pages<OnlineOrderDetailDto>>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			result.Data = new Pages<OnlineOrderDetailDto>();
			int total = 0;
			var list_Order = _context.Queryable<WebOrder>().Where(u => u.Status != -1 && u.UserDeleted == 0 && u.Userid == _userManage.Userid && u.Type == 1)
				.WhereIF(payStatus > 0, u => u.Status == (payStatus - 1))
				.OrderBy(u => u.Dtime, OrderByType.Desc)
				.ToPageList(page, pageSize, ref total);
			result.Data.Total = total;
			var list_Course = _context.Queryable<WebOnlineCourse>().LeftJoin<WebOnlineCourseLang>((l, r) => l.OnlineCourseid == r.OnlineCourseid).Where((l, r) => list_Order.Select(s => s.OnlineCourseid).Contains(l.OnlineCourseid) && r.Lang == _userManage.Lang)
				.Select((l, r) => new { l.OnlineCourseid,l.LessonCount,l.LessonStart,l.StudentCount, l.Img, r.Title, r.Message }).ToList();
			var list_Paytype = _context.Queryable<WebPaytype>().ToList();
			var list_Currency = _context.Queryable<PubCurrency>().Where(u => u.Status == 1).ToList();


			foreach (var model_Order in list_Order)
			{
				var order = new OnlineOrderDetailDto();
				order.Orderid = model_Order.Orderid;
				order.OrderSn = model_Order.OrderSn;
				order.Status = model_Order.Status;
				var model_Course = list_Course.FirstOrDefault(u => u.OnlineCourseid == model_Order.OnlineCourseid);
				if (model_Course != null)
				{
					order.Img = _commonBaseService.ResourceDomain(model_Course.Img);
					order.Title = model_Course.Title;
					order.Message = model_Course.Message;
					order.LessonStart=model_Course.LessonStart;
					order.LessonCount= model_Course.LessonCount;
					order.StudentCount= model_Course.StudentCount;
				}
				order.MarketPrice = model_Order.MarketPrice;
				var model_Currency = list_Currency.FirstOrDefault(u => u.CurrencyCode == model_Order.CurrencyCode);
				order.Ico = model_Currency?.Ico ??"$";
				if (model_Order.Status == 1)
				{
					order.Price = model_Order.Payment;
					order.Paytime = DateHelper.ConvertDateTimeInt(model_Order.Paytime);
					order.PayType.Paytypeid = model_Order.Paytypeid;
				}
				else
				{
					order.Price = model_Order.Amount;
				}
				var model_Paytype = list_Paytype.FirstOrDefault(u => u.Paytypeid == model_Order.Paytypeid);
				if (model_Paytype != null)
				{
					order.PayType.Ico = _commonBaseService.ResourceDomain(model_Paytype.Ico);
					order.PayType.PayName = _localizer[model_Paytype.Title];
					order.PayType.isScan = model_Paytype.IsScan;
					order.PayType.isWeb = model_Paytype.IsWeb;
				}
				result.Data.List.Add(order);
			}
			return result;
		}
		#endregion

		#region 订单详情（直播课）
		/// <summary>
		/// 订单列表（直播课）
		/// </summary>
		/// <param name="orderid">订单id</param>
		/// <returns></returns>
		public async Task<ApiResult<OnlineOrderDetailDto>> OnlineOrderDetail(long orderid)
		{
			var result = new ApiResult<OnlineOrderDetailDto>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			var model_Order = _context.Queryable<WebOrder>().First(u => u.Orderid == orderid && u.Userid == _userManage.Userid && u.Status != -1 && u.UserDeleted == 0);
			if (model_Order != null)
			{
				var order = new OnlineOrderDetailDto();
				order.Orderid = model_Order.Orderid;
				order.OrderSn = model_Order.OrderSn;
				order.Status = model_Order.Status;

				//从订单表中取的是快照信息，原课程可以已删
				var model_Course = _context.Queryable<WebOnlineCourse>().LeftJoin<WebOnlineCourseLang>((l, r) => l.OnlineCourseid == r.OnlineCourseid)
					.Where((l, r) => l.OnlineCourseid == model_Order.OnlineCourseid && r.Lang == _userManage.Lang)
					.Select((l, r) => new { l.OnlineCourseid, l.Img,l.LessonCount,l.LessonStart,l.StudentCount, r.Title, r.Message }).First();
				if (model_Course != null)
				{
					order.Img = _commonBaseService.ResourceDomain(model_Course.Img);
					order.Title = model_Course.Title;
					order.Message = model_Course.Message;
					order.LessonStart = model_Course.LessonStart;
					order.LessonCount = model_Course.LessonCount;
					order.StudentCount = model_Course.StudentCount;
				}
				else
				{
					order.Img = _commonBaseService.ResourceDomain(model_Order.Img + "");
					order.Title = model_Order.Title;
					order.Message = model_Order.Message;
				}
				order.MarketPrice = model_Order.MarketPrice;
				var model_Currency = _context.Queryable<PubCurrency>().First(u => u.CurrencyCode == model_Order.CurrencyCode);
				order.Ico = model_Currency?.Ico ?? "$";

				if (model_Order.Payment > 0)
				{
					order.Price = model_Order.Payment;
					order.Paytime = DateHelper.ConvertUtcDateTimeInt(model_Order.Paytime.ToUniversalTime());//给前端是UTC时间戳
				}
				else
				{
					order.Price = model_Order.Amount;
				}
				order.PayType.Paytypeid = model_Order.Paytypeid;
				var model_Paytype = _context.Queryable<WebPaytype>().First(u => u.Paytypeid == model_Order.Paytypeid);
				if (model_Paytype != null)
				{
					order.PayType.Ico = _commonBaseService.ResourceDomain(model_Paytype.Ico);
					order.PayType.PayName = _localizer[model_Paytype.Title];
					order.PayType.isScan = model_Paytype.IsScan;
					order.PayType.isWeb = model_Paytype.IsWeb;
				}
				result.Data = order;
			}
			else
			{
				result.StatusCode = 4009;
				result.Message = _localizer["订单不存在"];
			}
			return result;
		}
		#endregion

		#region 订单列表（录播课）
		/// <summary>
		/// 订单列表（录播课）
		/// </summary>
		/// <param name="payStatus">支付状态:0全部，1待支付，2已支付</param>
		/// <returns></returns>
		public async Task<ApiResult<Pages<RecordOrderDetailDto>>> RecordOrderList(int payStatus = 0, int page = 1, int pageSize = 10)
		{
			var result = new ApiResult<Pages<RecordOrderDetailDto>>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			result.Data = new Pages<RecordOrderDetailDto>();
			int total = 0;
			var list_Order = _context.Queryable<WebOrder>().Where(u => u.Status != -1 && u.UserDeleted == 0 && u.Userid == _userManage.Userid && u.Type == 2)
				.WhereIF(payStatus > 0, u => u.Status == (payStatus - 1))
				.OrderBy(u => u.Dtime, OrderByType.Desc)
				.ToPageList(page, pageSize, ref total);
			result.Data.Total = total;
			var list_Course = _context.Queryable<WebRecordCourse>().LeftJoin<WebRecordCourseLang>((l, r) => l.RecordCourseid == r.RecordCourseid).Where((l, r) => list_Order.Select(s => s.RecordCourseid).Contains(l.RecordCourseid) && r.Lang == _userManage.Lang)
				.Select((l, r) => new { l.RecordCourseid, l.LessonCount, l.StudentCount, l.Img, r.Title, r.Message }).ToList();
			var list_Paytype = _context.Queryable<WebPaytype>().ToList();
			var list_Currency = _context.Queryable<PubCurrency>().Where(u => u.Status == 1).ToList();


			foreach (var model_Order in list_Order)
			{
				var order = new RecordOrderDetailDto();
				order.Orderid = model_Order.Orderid;
				order.OrderSn = model_Order.OrderSn;
				order.Status = model_Order.Status;
				var model_Course = list_Course.FirstOrDefault(u => u.RecordCourseid == model_Order.RecordCourseid);
				if (model_Course != null)
				{
					order.Img = _commonBaseService.ResourceDomain(model_Course.Img);
					order.Title = model_Course.Title;
					order.Message = model_Course.Message;
					order.LessonCount = model_Course.LessonCount;
					order.StudentCount = model_Course.StudentCount;
				}
				order.MarketPrice = model_Order.MarketPrice;
				var model_Currency = list_Currency.FirstOrDefault(u => u.CurrencyCode == model_Order.CurrencyCode);
				order.Ico = model_Currency?.Ico??"$";
				if (model_Order.Status == 1)
				{
					order.Price = model_Order.Payment;
					order.Paytime = DateHelper.ConvertDateTimeInt(model_Order.Paytime);
					order.PayType.Paytypeid = model_Order.Paytypeid;
				}
				else
				{
					order.Price = model_Order.Amount;
				}
				var model_Paytype = list_Paytype.FirstOrDefault(u => u.Paytypeid == model_Order.Paytypeid);
				if (model_Paytype != null)
				{
					order.PayType.Ico = _commonBaseService.ResourceDomain(model_Paytype.Ico);
					order.PayType.PayName = _localizer[model_Paytype.Title];
					order.PayType.isScan = model_Paytype.IsScan;
					order.PayType.isWeb = model_Paytype.IsWeb;
				}
				result.Data.List.Add(order);
			}
			return result;
		}
		#endregion

		#region 订单详情（录播课）
		/// <summary>
		/// 订单列表（录播课）
		/// </summary>
		/// <param name="orderid">订单id</param>
		/// <returns></returns>
		public async Task<ApiResult<RecordOrderDetailDto>> RecordOrderDetail(long orderid)
		{
			var result = new ApiResult<RecordOrderDetailDto>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			var model_Order = _context.Queryable<WebOrder>().First(u => u.Orderid == orderid && u.Userid == _userManage.Userid && u.Status != -1 && u.UserDeleted == 0);
			if (model_Order != null)
			{
				var order = new RecordOrderDetailDto();
				order.Orderid = model_Order.Orderid;
				order.OrderSn = model_Order.OrderSn;
				order.Status = model_Order.Status;

				//从订单表中取的是快照信息，原课程可以已删
				var model_Course = _context.Queryable<WebRecordCourse>().LeftJoin<WebRecordCourseLang>((l, r) => l.RecordCourseid == r.RecordCourseid)
					.Where((l, r) => l.RecordCourseid == model_Order.RecordCourseid && r.Lang == _userManage.Lang)
					.Select((l, r) => new { l.RecordCourseid, l.Img, l.LessonCount, l.StudentCount, r.Title, r.Message }).First();
				if (model_Course != null)
				{
					order.Img = _commonBaseService.ResourceDomain(model_Course.Img);
					order.Title = model_Course.Title;
					order.Message = model_Course.Message;
					order.LessonCount = model_Course.LessonCount;
					order.StudentCount = model_Course.StudentCount;
				}
				else
				{
					order.Img = _commonBaseService.ResourceDomain(model_Order.Img + "");
					order.Title = model_Order.Title;
					order.Message = model_Order.Message;
				}
				order.MarketPrice = model_Order.MarketPrice;
				var model_Currency = _context.Queryable<PubCurrency>().First(u => u.CurrencyCode == model_Order.CurrencyCode);
				order.Ico = model_Currency?.Ico + "$";

				if (model_Order.Payment > 0)
				{
					order.Price = model_Order.Payment;
					order.Paytime = DateHelper.ConvertUtcDateTimeInt(model_Order.Paytime.ToUniversalTime());//给前端是UTC时间戳
				}
				else
				{
					order.Price = model_Order.Amount;
				}
				order.PayType.Paytypeid = model_Order.Paytypeid;
				var model_Paytype = _context.Queryable<WebPaytype>().First(u => u.Paytypeid == model_Order.Paytypeid);
				if (model_Paytype != null)
				{
					order.PayType.Ico = _commonBaseService.ResourceDomain(model_Paytype.Ico);
					order.PayType.PayName = _localizer[model_Paytype.Title];
					order.PayType.isScan = model_Paytype.IsScan;
					order.PayType.isWeb = model_Paytype.IsWeb;
				}
				result.Data = order;
			}
			else
			{
				result.StatusCode = 4009;
				result.Message = _localizer["订单不存在"];
			}
			return result;
		}
        #endregion

        #region 订单列表（线下课）
        /// <summary>
        /// 订单列表（线下课）
        /// </summary>
        /// <param name="payStatus">支付状态:0全部，1待支付，2已支付</param>
        /// <returns></returns>
        public async Task<ApiResult<Pages<OfflineOrderDetailDto>>> OfflineOrderList(int payStatus = 0, int page = 1, int pageSize = 10)
		{
			var result = new ApiResult<Pages<OfflineOrderDetailDto>>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			result.Data = new Pages<OfflineOrderDetailDto>();
			int total = 0;
			var list_Order = _context.Queryable<WebOrder>().Where(u => u.Status != -1 && u.UserDeleted == 0 && u.Userid == _userManage.Userid && u.Type == 3)
				.WhereIF(payStatus > 0, u => u.Status == (payStatus - 1))
				.OrderBy(u => u.Dtime, OrderByType.Desc)
				.ToPageList(page, pageSize, ref total);
			result.Data.Total = total;
			var list_Course = _context.Queryable<WebOfflineCourse>().LeftJoin<WebOfflineCourseLang>((l, r) => l.OfflineCourseid == r.OfflineCourseid).Where((l, r) => list_Order.Select(s => s.OfflineCourseid).Contains(l.OfflineCourseid) && r.Lang == _userManage.Lang)
				.Select((l, r) => new { l.OfflineCourseid, l.LessonCount, l.LessonStart, l.StudentCount, l.Img, r.Title, r.Message }).ToList();
			var list_Paytype = _context.Queryable<WebPaytype>().ToList();
			var list_Currency = _context.Queryable<PubCurrency>().Where(u => u.Status == 1).ToList();


			foreach (var model_Order in list_Order)
			{
				var order = new OfflineOrderDetailDto();
				order.Orderid = model_Order.Orderid;
				order.OrderSn = model_Order.OrderSn;
				order.Status = model_Order.Status;
				var model_Course = list_Course.FirstOrDefault(u => u.OfflineCourseid == model_Order.OfflineCourseid);
				if (model_Course != null)
				{
					order.Img = _commonBaseService.ResourceDomain(model_Course.Img);
					order.Title = model_Course.Title;
					order.Message = model_Course.Message;
					order.LessonStart = model_Course.LessonStart;
					order.LessonCount = model_Course.LessonCount;
					order.StudentCount = model_Course.StudentCount;
				}
				order.MarketPrice = model_Order.MarketPrice;
				var model_Currency = list_Currency.FirstOrDefault(u => u.CurrencyCode == model_Order.CurrencyCode);
				order.Ico = model_Currency?.Ico??"$";
				if (model_Order.Status == 1)
				{
					order.Price = model_Order.Payment;
					order.Paytime = DateHelper.ConvertDateTimeInt(model_Order.Paytime);
					order.PayType.Paytypeid = model_Order.Paytypeid;
				}
				else
				{
					order.Price = model_Order.Amount;
				}
				var model_Paytype = list_Paytype.FirstOrDefault(u => u.Paytypeid == model_Order.Paytypeid);
				if (model_Paytype != null)
				{
					order.PayType.Ico = _commonBaseService.ResourceDomain(model_Paytype.Ico);
					order.PayType.PayName = _localizer[model_Paytype.Title];
					order.PayType.isScan = model_Paytype.IsScan;
					order.PayType.isWeb = model_Paytype.IsWeb;
				}
				result.Data.List.Add(order);
			}
			return result;
		}
        #endregion

        #region 订单详情（线下课）
        /// <summary>
        /// 订单列表（线下课）
        /// </summary>
        /// <param name="orderid">订单id</param>
        /// <returns></returns>
        public async Task<ApiResult<OfflineOrderDetailDto>> OfflineOrderDetail(long orderid)
		{
			var result = new ApiResult<OfflineOrderDetailDto>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			var model_Order = _context.Queryable<WebOrder>().First(u => u.Orderid == orderid && u.Userid == _userManage.Userid && u.Status != -1 && u.UserDeleted == 0);
			if (model_Order != null)
			{
				var order = new OfflineOrderDetailDto();
				order.Orderid = model_Order.Orderid;
				order.OrderSn = model_Order.OrderSn;
				order.Status = model_Order.Status;

				//从订单表中取的是快照信息，原课程可以已删
				var model_Course = _context.Queryable<WebOfflineCourse>().LeftJoin<WebOfflineCourseLang>((l, r) => l.OfflineCourseid == r.OfflineCourseid)
					.Where((l, r) => l.OfflineCourseid == model_Order.OfflineCourseid && r.Lang == _userManage.Lang)
					.Select((l, r) => new { l.OfflineCourseid, l.Img, l.LessonCount, l.LessonStart, l.StudentCount, r.Title, r.Message }).First();
				if (model_Course != null)
				{
					order.Img = _commonBaseService.ResourceDomain(model_Course.Img);
					order.Title = model_Course.Title;
					order.Message = model_Course.Message;
					order.LessonStart = model_Course.LessonStart;
					order.LessonCount = model_Course.LessonCount;
					order.StudentCount = model_Course.StudentCount;
				}
				else
				{
					order.Img = _commonBaseService.ResourceDomain(model_Order.Img + "");
					order.Title = model_Order.Title;
					order.Message = model_Order.Message;
				}
				order.MarketPrice = model_Order.MarketPrice;
				var model_Currency = _context.Queryable<PubCurrency>().First(u => u.CurrencyCode == model_Order.CurrencyCode);
				order.Ico = model_Currency?.Ico + "$";

				if (model_Order.Payment > 0)
				{
					order.Price = model_Order.Payment;
					order.Paytime = DateHelper.ConvertUtcDateTimeInt(model_Order.Paytime.ToUniversalTime());//给前端是UTC时间戳
				}
				else
				{
					order.Price = model_Order.Amount;
				}
				order.PayType.Paytypeid = model_Order.Paytypeid;
				var model_Paytype = _context.Queryable<WebPaytype>().First(u => u.Paytypeid == model_Order.Paytypeid);
				if (model_Paytype != null)
				{
					order.PayType.Ico = _commonBaseService.ResourceDomain(model_Paytype.Ico);
					order.PayType.PayName = _localizer[model_Paytype.Title];
					order.PayType.isScan = model_Paytype.IsScan;
					order.PayType.isWeb = model_Paytype.IsWeb;
				}
				result.Data = order;
			}
			else
			{
				result.StatusCode = 4009;
				result.Message = _localizer["订单不存在"];
			}
			return result;
		}
		#endregion

		#region 取消订单
		/// <summary>
		/// 取消订单原因
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<List<string>>> CancelOrderKeys()
		{
			var result = new ApiResult<List<string>>();
			var list_Key = _context.Queryable<PubKeys>().LeftJoin<PubKeysLang>((l, r) => l.Keysid == r.Keysid)
				.Where((l, r) => l.Status == 1 && l.Sty == 3 && r.Lang == _userManage.Lang)
				.OrderBy(l => l.Sort).OrderBy(l => l.Keysid, OrderByType.Desc)
				.Select((l, r) => r.Title)
				.ToList();
			result.Data = list_Key;
			return result;
		}
		/// <summary>
		/// 取消订单
		/// </summary>
		/// <param name="orderid">订单id</param>
		/// <param name="keys">原因标签</param>
		/// <param name="message">其它原因</param>
		/// <returns></returns>
		public async Task<ApiResult> CancelOrder(long orderid,List<string> keys, string message)
		{
			var result = new ApiResult<OrderDetailDto>();
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid <= 0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户登录超时"];
				return result;
			}
			var model_Order = _context.Queryable<WebOrder>().First(u => u.Orderid == orderid && u.Userid == _userManage.Userid && u.Status!=-1 && u.UserDeleted == 0);
			if (model_Order != null)
			{
				if (model_Order.Status == 0)
				{
					var model_Sku = _context.Queryable<WebCourseSku>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid && r.Lang == _userManage.Lang)
									.Where((l, r) => l.CourseSkuid == model_Order.CourseSkuid).Select((l, r) => new { r.Title, l.ClassHour, l.CourseSkuid }).First();
					if(keys!=null && keys.Count>0) model_Order.CancelKeys = string.Join(",", keys);
					model_Order.CancelMessage = message;
					model_Order.CancelTime = DateTime.Now;
					model_Order.UserDeleted = 1;

					//工单场景:取消订单
					var dic_data = new Dictionary<string, object>();
					dic_data.Add(_localizer["订单号"], model_Order.OrderSn);
                    dic_data.Add(_localizer["订单日期"], model_Order.Dtime.ToString());
                    dic_data.Add(_localizer["课程"], model_Order.Title);
					if (model_Sku != null)
					{
						dic_data.Add(_localizer["上课类型"], model_Sku.Title);
						dic_data.Add(_localizer["课时"], model_Sku.ClassHour);
					}
					dic_data.Add(_localizer["币种"], model_Order.CurrencyCode);
					dic_data.Add(_localizer["售价"], model_Order.Amount);
					var model_User = _context.Queryable<WebUser>().InSingle(model_Order.Userid);
					if (model_User != null)
                    {
                        dic_data.Add(_localizer["购买人"], model_User.FirstName + " " + model_User.LastName);
						dic_data.Add(_localizer["购买人手机"], model_User.MobileCode + "-" + model_User.Mobile);
                        dic_data.Add(_localizer["购买人邮箱"], model_User.Email);
                        dic_data.Add(_localizer["所在地时区"], model_User.Utc);
                    }
					if (keys.Count > 0) message += "," + string.Join(",", keys);
					dic_data.Add(_localizer["取消原因"], message);
					var body = string.Join("<br>", dic_data.ToList());
					var result_ticket = await _sobotBaseService.AddUserTicket(_localizer["申请取消订单"], body);
					if (result_ticket.StatusCode == 0)
					{
						model_Order.IsTicket = 1;
						model_Order.TicketTime = DateTime.Now;
						model_Order.Ticketid = result_ticket.Data;
					}
					else
					{
						_logger.LogError(_localizer["申请取消订单"] + "," + result_ticket.Message);
					}


					_context.Updateable(model_Order).UpdateColumns(u => new {u.UserDeleted, u.CancelKeys, u.CancelMessage, u.CancelTime }).ExecuteCommand();
				}
				else
				{
					result.StatusCode = 4007;
					result.Message = _localizer["仅未支付订单可以取消"];
				}
			}
			else
			{
				result.StatusCode = 4007;
				result.Message = _localizer["您没有操作权限"];
			}
			return result;
		}
        #endregion

        #region 付款超时提醒工单
        /// <summary>
        /// 付款超时提醒工单
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult> PaymentReminder()
		{
			var result = new ApiResult<OrderDetailDto>();
			var dtime = DateTime.Now.AddMinutes(-60);
			var list_Order = _context.Queryable<WebOrder>().Where(u => u.Status == 0 && u.Dtime< dtime && u.IsTicket==0).ToList();
			var list_Sku = _context.Queryable<WebCourseSku>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid && r.Lang == _userManage.Lang)
				.Where((l, r) => list_Order.Select(s => s.CourseSkuid).Contains(l.CourseSkuid))
				.Select((l, r) => new { r.Title, l.ClassHour, l.CourseSkuid }).ToList();
			var list_User = _context.Queryable<WebUser>().Where(u => list_Order.Select(s => s.Userid).Contains(u.Userid)).ToList();

            foreach (var model_Order in list_Order)
            {
				var model_User = list_User.FirstOrDefault(u => u.Userid == model_Order.Userid);
                var model_Sku = list_Sku.FirstOrDefault(u => u.CourseSkuid == model_Order.CourseSkuid);
				if (model_Sku == null || model_User==null) continue;
                //创建工单提醒
                var dic_data = new Dictionary<string, object>();
                dic_data.Add(_localizer["订单号"], model_Order.OrderSn);
                dic_data.Add(_localizer["课程"], model_Order.Title);
                dic_data.Add(_localizer["上课类型"], model_Sku.Title);
                dic_data.Add(_localizer["课时"], model_Sku.ClassHour);
				dic_data.Add(_localizer["币种"], model_Order.CurrencyCode);
				dic_data.Add(_localizer["售价"], model_Order.Amount);
                dic_data.Add(_localizer["购买人"], model_User.FirstName + " " + model_User.LastName);
                dic_data.Add(_localizer["购买人手机"], model_User.MobileCode + "-" + model_User.Mobile);
                var body = string.Join("<br>", dic_data.ToList());
                var result_ticket = await _sobotBaseService.AddUserTicket(_localizer["请跟进超时未付款订单"], body);
				if (result_ticket.StatusCode == 0)
				{
					model_Order.IsTicket = 1;
					model_Order.TicketTime = DateTime.Now;
					model_Order.Ticketid = result_ticket.Data;
                }
				else
				{
					_logger.LogError(_localizer["请跟进超时未付款订单"] + "," + result_ticket.Message);
				}
            }
			_context.Updateable(list_Order).UpdateColumns(u => new { u.IsTicket, u.TicketTime ,u.Ticketid }).ExecuteCommand();
			return result;
		}
		#endregion

		#region 定时检查支付状态
		/// <summary>
		/// 定时检查支付状态
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult> CheckPayStatus()
		{
			var result = new ApiResult();
			var list_PayType = _context.Queryable<WebPaytype>().Where(u=>u.Status==1).ToList();
			//2小时内的订单重复调
			var list_Order = _context.Queryable<WebOrder>().Where(u => u.Status == 0 && u.PayStatus == 1 && u.Dtime.AddHours(2)>DateTime.Now).ToList();
			string bananapay_url = Appsettings.app("APIS:Cashier.Payment.QueryOrder");
			var dic_config = _pubConfigBaseService.GetConfigs("bananapay_access_key,bananapay_sign_key,bananapay_notify_key,stripe_apikey");
			var dic_data=new Dictionary<string, object>();
			var arr_data = new List<string>();
			string str_code = "",json="";
			JObject o = null;
			long user_courseid = 0;


            foreach (var model_Order in list_Order) {
				var model_PayType = list_PayType.FirstOrDefault(u => u.Paytypeid == model_Order.Paytypeid);
				if (model_PayType == null) continue;
				switch (model_PayType.PayGateway.ToLower()) {
					case "bananapay":
						string bananapay_access_key = dic_config.ContainsKey("bananapay_access_key") ? dic_config["bananapay_access_key"] : "";
						string bananapay_sign_key = dic_config.ContainsKey("bananapay_sign_key") ? dic_config["bananapay_sign_key"] : "";
						string bananapay_notify_key = dic_config.ContainsKey("bananapay_notify_key") ? dic_config["bananapay_notify_key"] : "";
						if (string.IsNullOrEmpty(bananapay_access_key) || string.IsNullOrEmpty(bananapay_sign_key) || string.IsNullOrEmpty(bananapay_notify_key))
						{
							result.StatusCode = 4009;
							result.Message = _localizer["支付参数不完整"];
							return result;
						}
						dic_data.Clear();
						dic_data.Add("access_key", bananapay_access_key);
						dic_data.Add("pay_way", model_PayType.Code.ToLower());//支付方式：“weixin”、“alipay”、“unionpay”、”gcashpay”
						dic_data.Add("out_trade_no", model_Order.OrderSn + model_Order.OrderSnExt);
						dic_data.Add("ts", DateHelper.ConvertDateTimeInt(DateTime.Now));//时间戳
						arr_data = dic_data.Keys.ToList();
						str_code = "";
						arr_data.Sort(string.CompareOrdinal);
						foreach (var key in arr_data)
						{
							if (arr_data.IndexOf(key) > 0) str_code += "&";
							str_code += $"{key}={dic_data[key]}";
						}
						str_code += "&key=" + bananapay_sign_key;
						dic_data.Add("sign", MD5Helper.MD5Encrypt32(str_code));//md5签名

						json = GetRemoteHelper.HttpWebRequestUrl(bananapay_url, JsonConvert.SerializeObject(dic_data), "utf-8", "post", null, "application/json");
						o = JObject.Parse(json);
						if (o != null && o["errno"] != null)
						{
							if (o["errno"].ToString() == "0" && o["results"] != null)
							{
								//state 和 ispay 都等于 1 时，表示支付成功
								if (o["results"]["state"] != null && o["results"]["ispay"] != null && o["results"]["state"].ToString() == "1" && o["results"]["ispay"].ToString() == "1")
								{
									model_Order.PayStatus = 2;
									model_Order.Status = 1;
									model_Order.Remark = $"[{DateTime.Now}]系统定时检查支付成功<hr>" + model_Order.Remark;
									if (o["results"]["transaction_id"] != null && !string.IsNullOrEmpty(o["results"]["transaction_id"].ToString())) 
										model_Order.TransactionId = o["results"]["transaction_id"].ToString();

                                    if (!_context.Queryable<WebUserCourse>().Any(u => u.OrderSn == model_Order.OrderSn && u.Status == 1))
                                    {
                                        var model_UserCourse = new WebUserCourse();
                                        model_UserCourse.OrderSn = model_Order.OrderSn;
                                        model_UserCourse.Orderid = model_Order.Orderid;
                                        model_UserCourse.Userid = model_Order.Userid;
                                        model_UserCourse.Courseid = model_Order.Courseid;
                                        model_UserCourse.MenkeCourseId = model_Order.MenkeCourseId;
                                        model_UserCourse.Title = model_Order.Title;
                                        model_UserCourse.Img = model_Order.Img;
                                        model_UserCourse.Message = model_Order.Message;
                                        model_UserCourse.ClassHour = model_Order.ClassHour;
                                        model_UserCourse.Status = 1;
                                        model_UserCourse.SkuTypeid = model_Order.SkuTypeid;

                                        //工单场景：完成购买
                                        dic_data.Clear();
                                        var model_User = _context.Queryable<WebUser>().InSingle(model_Order.Userid);
                                        if (model_User != null)
                                        {
                                            var model_skutype = _context.Queryable<WebSkuTypeLang>().First(u => u.SkuTypeid == model_Order.SkuTypeid && u.Lang == model_User.Lang);
                                            dic_data.Add(_localizer["购买人"], model_User.FirstName + " " + model_User.LastName);
                                            dic_data.Add(_localizer["购买人手机"], model_User.MobileCode + "-" + model_User.Mobile);
                                            dic_data.Add(_localizer["购买人邮箱"], model_User.Email);
                                            dic_data.Add(_localizer["所在地时区"], model_User.Utc);
                                            dic_data.Add(_localizer["所选课程"], model_Order.Title + "");
                                            dic_data.Add(_localizer["上课方式"], model_skutype?.Title + "");
                                            dic_data.Add(_localizer["购买课时"], model_Order.ClassHour);
                                            dic_data.Add(_localizer["购买时间"], model_Order.Paytime + "");
                                            dic_data.Add(_localizer["售价"], model_Order.Payment);
                                            dic_data.Add(_localizer["币种"], model_Order.CurrencyCode);
                                            dic_data.Add(_localizer["付款方式"], $"BananaPay-{(dic_data.ContainsKey("pay_way") ? dic_data["pay_way"] : "")}");

                                            var body = string.Join("<br>", dic_data.ToList());
                                            var result_ticket = await _sobotBaseService.AddUserTicket(_localizer["完成购买提醒"], body);
                                            if (result_ticket.StatusCode == 0)
                                            {
                                                model_UserCourse.BuyTicketId = result_ticket.Data;
                                                model_UserCourse.IsBuyTicket = 1;
                                            }
                                            else
                                            {
                                                _logger.LogError(_localizer["完成购课提醒"] + "," + result_ticket.Message);
                                            }
                                            //触发邮件
                                            var dic_email = new Dictionary<string, string>();
                                            dic_email.Add("name", model_User.FirstName + " " + model_User.LastName);
                                            string head_img = string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg;
                                            dic_email.Add("head_img", PicHelper.ImageToBase64(_commonBaseService.ResourceDomain(head_img)));
                                            dic_email.Add("course_name", model_Order.Title);
                                            dic_email.Add("user_url", $"{Appsettings.app("Web:Host")}/user/course");
                                            dic_email.Add("order_url", $"{Appsettings.app("Web:Host")}/user/order");
                                            dic_email.Add("order_sn", model_Order.OrderSn);
                                            dic_email.Add("url", $"{string.Format(Appsettings.app("Web:ServiceUrl"), _userManage.Lang)}");
                                            await _messageBaseService.AddEmailTask("Buy", model_User.Email, dic_email, model_User.Lang);
                                        }

                                        user_courseid = _context.Insertable(model_UserCourse).ExecuteReturnBigIdentity();
                                        var result_menke_course = await _courseBaseService.UserCourseArranging(user_courseid);
                                        if (result_menke_course.StatusCode != 0)
                                        {
                                            _logger.LogError("支付回调时创建拓课云课程[user_courseid=" + user_courseid + "]异常:" + result_menke_course.Message);
                                        }

                                    }
                                }
								else
								{
									model_Order.Remark = $"[{DateTime.Now}]系统定时检查支付状态失败.<hr>" + model_Order.Remark;
								}
							}
							else if (o["errno"].ToString() == "3" || o["errno"].ToString() == "5")
							{
								model_Order.PayStatus = 3;//errno=3 或 5 时，订单已关闭无法支付
								model_Order.Status = 2;
								model_Order.Remark = $"[{DateTime.Now}]系统检查付款单已关闭，状态errno={o["errno"]}<hr>" + model_Order.Remark;
							}
							else
							{
								_logger.LogInformation("bananapay系统定时检查支付状态:" + json);
								model_Order.Remark = $"[{DateTime.Now}]系统定时检查支付状态失败<hr>" + model_Order.Remark;
							}
						}
						break;
					case "stripe":
						string stripe_apikey = dic_config.ContainsKey("stripe_apikey") ? dic_config["stripe_apikey"] : "";
						if (string.IsNullOrEmpty(stripe_apikey))
						{
							result.StatusCode = 4009;
							result.Message = _localizer["支付参数不完整"];
							return result;
						}
						StripeConfiguration.ApiKey = stripe_apikey;
						var options = new PaymentIntentSearchOptions
						{
							Query = "metadata['orderid']:'" + model_Order.Orderid + "'",
						};
						var service = new PaymentIntentService();
						var sea = service.Search(options);
						if (sea.Data != null && sea.Data.Count>0) {

							var sea_model = sea.Data.FirstOrDefault();
							model_Order.TransactionId = String.Join(",", sea.Data.Select(u => u.Id).ToList());
							switch (sea_model.Status)
							{
								case "canceled":
									model_Order.PayStatus = 3;//errno=3 或 5 时，订单已关闭无法支付
									model_Order.Status = 2;
									model_Order.Remark = $"[{DateTime.Now}]系统用户取消支付，状态[canceled]<hr>" + model_Order.Remark;
									break;
								case "succeeded":
									model_Order.PayStatus = 2;
									model_Order.Status = 1;
									model_Order.Remark = $"[{DateTime.Now}]系统定时检查支付成功<hr>" + model_Order.Remark;

                                    if (!_context.Queryable<WebUserCourse>().Any(u => u.OrderSn == model_Order.OrderSn && u.Status == 1))
                                    {
                                        var model_UserCourse = new WebUserCourse();
                                        model_UserCourse.OrderSn = model_Order.OrderSn;
                                        model_UserCourse.Orderid = model_Order.Orderid;
                                        model_UserCourse.Userid = model_Order.Userid;
                                        model_UserCourse.Courseid = model_Order.Courseid;
                                        model_UserCourse.MenkeCourseId = model_Order.MenkeCourseId;
                                        model_UserCourse.Title = model_Order.Title;
                                        model_UserCourse.Img = model_Order.Img;
                                        model_UserCourse.Message = model_Order.Message;
                                        model_UserCourse.ClassHour = model_Order.ClassHour;
                                        model_UserCourse.Status = 1;
                                        model_UserCourse.SkuTypeid = model_Order.SkuTypeid;

                                        //工单场景：完成购买
                                        dic_data.Clear();
                                        var model_User = _context.Queryable<WebUser>().InSingle(model_Order.Userid);
                                        if (model_User != null)
                                        {
                                            var model_skutype = _context.Queryable<WebSkuTypeLang>().First(u => u.SkuTypeid == model_Order.SkuTypeid && u.Lang == model_User.Lang);
                                            dic_data.Add(_localizer["购买人"], model_User.FirstName + " " + model_User.LastName);
                                            dic_data.Add(_localizer["购买人手机"], model_User.MobileCode + "-" + model_User.Mobile);
                                            dic_data.Add(_localizer["购买人邮箱"], model_User.Email);
                                            dic_data.Add(_localizer["所在地时区"], model_User.Utc);
                                            dic_data.Add(_localizer["所选课程"], model_Order.Title + "");
                                            dic_data.Add(_localizer["上课方式"], model_skutype?.Title + "");
                                            dic_data.Add(_localizer["购买课时"], model_Order.ClassHour);
                                            dic_data.Add(_localizer["购买时间"], model_Order.Paytime + "");
                                            dic_data.Add(_localizer["售价"], model_Order.Payment);
                                            dic_data.Add(_localizer["币种"], model_Order.CurrencyCode);
                                            dic_data.Add(_localizer["付款方式"], "Stripe");

                                            var body = string.Join("<br>", dic_data.ToList());
                                            var result_ticket = await _sobotBaseService.AddUserTicket(_localizer["完成购课提醒"], body);
                                            if (result_ticket.StatusCode == 0)
                                            {
                                                model_UserCourse.BuyTicketId = result_ticket.Data;
                                                model_UserCourse.IsBuyTicket = 1;
                                            }
                                            else
                                            {
                                                _logger.LogError(_localizer["完成购课提醒"] + "," + result_ticket.Message);
                                            }

                                            //触发邮件
                                            var dic_email = new Dictionary<string, string>();
                                            dic_email.Add("name", model_User.FirstName + " " + model_User.LastName);
                                            string head_img = string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg;
                                            dic_email.Add("head_img", PicHelper.ImageToBase64(_commonBaseService.ResourceDomain(head_img)));
                                            dic_email.Add("course_name", model_Order.Title);
                                            dic_email.Add("user_url", $"{Appsettings.app("Web:Host")}/user/course");
                                            dic_email.Add("order_url", $"{Appsettings.app("Web:Host")}/user/order");
                                            dic_email.Add("order_sn", model_Order.OrderSn);
                                            dic_email.Add("url", $"{string.Format(Appsettings.app("Web:ServiceUrl"), _userManage.Lang)}");
                                            await _messageBaseService.AddEmailTask("Buy", model_User.Email, dic_email, model_User.Lang);
                                        }
                                        user_courseid = _context.Insertable(model_UserCourse).ExecuteReturnBigIdentity();
                                        var result_menke_course = await _courseBaseService.UserCourseArranging(user_courseid);
                                        if (result_menke_course.StatusCode != 0)
                                        {
                                            _logger.LogError("支付回调时创建拓课云课程[user_courseid=" + user_courseid + "]异常:" + result_menke_course.Message);
                                        }
                                    }
                                    break;
								default:
                                    _logger.LogInformation("stripe系统定时检查支付状态:" + JsonConvert.SerializeObject(sea));
                                    model_Order.Remark = $"[{DateTime.Now}]系统定时检查支付状态失败<hr>" + model_Order.Remark;
									//processing//处理
									//requires_action//要求_操作
									//requires_capture//要求_捕获
									//requires_confirmation//要求_确认
									//requires_payment_method//要求付款方法
									break;
							}
						}
						break;
				}
				_context.Updateable(model_Order).UpdateColumns(u => new { u.PayStatus, u.Status, u.TransactionId, u.Remark }).ExecuteCommand();
			}
			return result;
		}
		#endregion
	}
}