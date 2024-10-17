using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Api.Dto.Order;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto.Common;
using WeTalk.Models.Dto.Order;
using static WeTalk.Api.HttpHeaderFilter;

namespace WeTalk.Api
{
    /// <summary>
    /// 订单
    /// </summary>
    [Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
    public partial class OrderController : Base.WebApiController
	{
		private readonly IOrderService _orderService;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IHttpContextAccessor accessor, IOrderService orderService, ILogger<OrderController> logger) :base(accessor)
		{
			_orderService = orderService;
            _accessor = accessor;
            _logger = logger;
    }

        #region "支付方式列表"
        /// <summary>
        /// 支付方式列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Currency]
        public async Task<ApiResult<List<PayType>>> PayTypes()
        {
            try
            {
                return await _orderService.PayTypes();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<List<PayType>>() { StatusCode = 4012, Message = ex.Message };
            }
        }
        #endregion

        #region "加载确认订单商品信息"
        /// <summary>
        /// 加载确认订单商品信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Currency]
        public async Task<ApiResult<ConfirmOrderDto>> ConfirmOrder([FromBody] ConfirmOrderRequestDto data)
		{
            try
            {
                return await _orderService.ConfirmOrder(data.courseSkuid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<ConfirmOrderDto>() { StatusCode = 4012, Message = ex.Message };
            }
        }
        #endregion

        #region "创建订单"
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [UserToken(Power = "")]
        [Currency]
        public async Task<ApiResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequestDto data)
		{
            return await _orderService.CreateOrder(data.courseSkuid);
        }
        #endregion

        #region "确认支付"
        /// <summary>
        /// 确认支付
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [UserToken(Power = "")]
        [Currency]
        public async Task<ApiResult<OrderPayDto>> OrderPay([FromBody] OrderPayRequestDto data)
        {
            return await _orderService.OrderPay(data.orderid, data.payTypeid, data.type);
        }
        #endregion

        #region "查询订单状态"
        /// <summary>
        /// 查询订单状态
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [UserToken(Power = "")]
        [Currency]
        public async Task<ApiResult<OrderStatusDto>> GetOrderStatus([FromBody] GetOrderStatusRequestDto data)
        {
            try
            {
                return await _orderService.GetOrderStatus(data.orderSn, data.orderid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<OrderStatusDto>() { StatusCode = 4012, Message = ex.Message };
            }
        }
        #endregion

        #region "支付回调"
        /// <summary>
        /// 支付回调
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<string> NotifyUrl(string action="")
        {
			//只允许8001端口调用
			if (_accessor.HttpContext.Connection.LocalPort != 8001)
            {
                _logger.LogError("非法请求,拒绝响应");
                return "非法请求";
            }
            return await _orderService.NotifyUrl(action);
        }
		#endregion

		#region "订单列表（众语课）"
		/// <summary>
		/// 订单列表（众语课）
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<Pages<OrderDetailDto>>> OrderList([FromBody] OrderListRequestDto data)
		{
            try
            {
                return await _orderService.OrderList(data.payStatus, data.page, data.pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<Pages<OrderDetailDto>>() { StatusCode = 4012, Message = ex.Message };
            }
        }
		#endregion

		#region "订单详情（众语课）"
		/// <summary>
		/// 订单详情（众语课）
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[UserToken(Power = "")]
		public async Task<ApiResult<OrderDetailDto>> OrderDetail([FromBody] OrderDetailRequestDto data)
		{
            try
            {
                return await _orderService.OrderDetail(data.orderid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<OrderDetailDto>() { StatusCode = 4012, Message = ex.Message };
            }
        }
		#endregion

		#region "订单列表（直播课）"
		/// <summary>
		/// 订单列表（直播课）
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[UserToken(Power = "")]
		public async Task<ApiResult<Pages<OnlineOrderDetailDto>>> OnlineOrderList([FromBody] OrderListRequestDto data)
		{
			try
			{
				return await _orderService.OnlineOrderList(data.payStatus, data.page, data.pageSize);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<Pages<OnlineOrderDetailDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "订单详情（直播课）"
		/// <summary>
		/// 订单详情（直播课）
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[UserToken(Power = "")]
		public async Task<ApiResult<OnlineOrderDetailDto>> OnlineOrderDetail([FromBody] OrderDetailRequestDto data)
		{
			try
			{
				return await _orderService.OnlineOrderDetail(data.orderid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<OnlineOrderDetailDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "订单列表（录播课）"
		/// <summary>
		/// 订单列表（录播课）
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[UserToken(Power = "")]
		public async Task<ApiResult<Pages<RecordOrderDetailDto>>> RecordOrderList([FromBody] OrderListRequestDto data)
		{
			try
			{
				return await _orderService.RecordOrderList(data.payStatus, data.page, data.pageSize);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<Pages<RecordOrderDetailDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "订单详情（录播课）"
		/// <summary>
		/// 订单详情（录播课）
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[UserToken(Power = "")]
		public async Task<ApiResult<RecordOrderDetailDto>> RecordOrderDetail([FromBody] OrderDetailRequestDto data)
		{
			try
			{
				return await _orderService.RecordOrderDetail(data.orderid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<RecordOrderDetailDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "订单列表（线下课）"
		/// <summary>
		/// 订单列表（线下课）
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[UserToken(Power = "")]
		public async Task<ApiResult<Pages<OfflineOrderDetailDto>>> OfflineOrderList([FromBody] OrderListRequestDto data)
		{
			try
			{
				return await _orderService.OfflineOrderList(data.payStatus, data.page, data.pageSize);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<Pages<OfflineOrderDetailDto>>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "订单详情（线下课）"
		/// <summary>
		/// 订单详情（线下课）
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[UserToken(Power = "")]
		public async Task<ApiResult<OfflineOrderDetailDto>> OfflineOrderDetail([FromBody] OrderDetailRequestDto data)
		{
			try
			{
				return await _orderService.OfflineOrderDetail(data.orderid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<OfflineOrderDetailDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "取消订单"
		/// <summary>
		/// 取消订单原因标签
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult<List<string>>> CancelOrderKeys()
        {
            try
            {
                return await _orderService.CancelOrderKeys();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult<List<string>>() { StatusCode = 4012, Message = ex.Message };
            }
        }
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult> CancelOrder([FromBody] CancelOrderRequestDto data)
		{
            try
            {
                return await _orderService.CancelOrder(data.orderid, data.keys, data.message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
                return new ApiResult() { StatusCode = 4012, Message = ex.Message };
            }
        }
		#endregion
	}
}
