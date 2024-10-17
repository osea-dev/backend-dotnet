namespace WeTalk.Api.Dto.Order
{
    public class GetOrderStatusRequestDto
    {
		/// <summary>
		/// 订单ID
		/// </summary>
		public long orderid { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderSn { get; set; }

    }
}
