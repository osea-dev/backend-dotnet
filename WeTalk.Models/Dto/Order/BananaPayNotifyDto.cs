
using System;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.Order
{
    /// <summary>
    ///  BananaPay回调
    /// </summary>
    public class BananaPayNotifyDto
    {
        /// <summary>
        /// 支付方式
        /// </summary>
        public String pay_way { get; set; }
		/// <summary>
		/// 订单号
		/// </summary>
		public string out_trade_no { get; set; }
        /// <summary>
        /// trade_status_sync
        /// </summary>
		public string notify_type { get; set; }
        /// <summary>
        /// 回调时间（时间戳）
        /// </summary>
		public int notify_time { get; set; }
        /// <summary>
        /// 支付时间（时间戳）
        /// </summary>
		public int pay_time { get; set; }
        /// <summary>
        /// “TRADE_SUCCESS”支付成功,”TRADE_CLOSED”订单关闭，“TRADE_FAILED”支付失败
        /// </summary>
        public string trade_status { get; set; }
        /// <summary>
        /// 交易单号
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public string total_fee { get; set; }
        /// <summary>
        /// 货币种类
        /// </summary>
        public string fee_type { get; set; }
        /// <summary>
        /// 付款银行
        /// </summary>
        public string pay_bank { get; set; }
        /// <summary>
        /// 代金券金额（元）
        /// </summary>
        public string coupon_fee { get; set; }
        /// <summary>
        /// 现金支付金额（元）
        /// </summary>
        public string cash_fee { get; set; }
        /// <summary>
        /// 现金支付货币类型(CNY)
        /// </summary>
        public string cash_fee_type { get; set; }
        /// <summary>
        /// 业务结果 SUCCESS
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 业务结果 SUCCESS
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 订单中一些内容 JSON 格式(详见 9.1.3)
        /// </summary>
        public string order_info { get; set; }
        /// <summary>
        /// 自定义附加内容，长度 500 以内 JSON 字符串
        /// </summary>
        public string attach { get; set; }
        /// <summary>
        /// md5 签
        /// </summary>
        public string sign { get; set; }
    }
}
