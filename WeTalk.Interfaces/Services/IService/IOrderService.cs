using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models.Dto;
using WeTalk.Models;
using System;
using Microsoft.AspNetCore.Http;
using WeTalk.Models.Dto.Order;
using WeTalk.Models.Dto.Common;

namespace WeTalk.Interfaces.Services
{
	public partial interface IOrderService : IBaseService
	{
		Task<ApiResult> CancelOrder(long orderid, List<string> keys, string message);
        Task<ApiResult<List<string>>> CancelOrderKeys();
        Task<ApiResult> CheckPayStatus();
        Task<ApiResult<ConfirmOrderDto>> ConfirmOrder(long course_skuid);
        Task<ApiResult<OrderDto>> CreateOrder(long courseSkuid);
        Task<ApiResult<OrderStatusDto>> GetOrderStatus(string order_sn, long orderid);
        Task<string> NotifyUrl(string action);
		Task<ApiResult<OfflineOrderDetailDto>> OfflineOrderDetail(long orderid);
		Task<ApiResult<Pages<OfflineOrderDetailDto>>> OfflineOrderList(int payStatus = 0, int page = 1, int pageSize = 10);
		Task<ApiResult<OnlineOrderDetailDto>> OnlineOrderDetail(long orderid);
		Task<ApiResult<Pages<OnlineOrderDetailDto>>> OnlineOrderList(int payStatus = 0, int page = 1, int pageSize = 10);
		Task<ApiResult<OrderDetailDto>> OrderDetail(long orderid);
		Task<ApiResult<Pages<OrderDetailDto>>> OrderList(int payStatus = 0, int page = 1, int pageSize = 10);
        Task<ApiResult<OrderPayDto>> OrderPay(long orderid, long payTypeid, int type = 1);
        Task<ApiResult> PaymentReminder();
        Task<ApiResult<List<PayType>>> PayTypes();
		Task<ApiResult<RecordOrderDetailDto>> RecordOrderDetail(long orderid);
		Task<ApiResult<Pages<RecordOrderDetailDto>>> RecordOrderList(int payStatus = 0, int page = 1, int pageSize = 10);
	}
}