using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///课程订单表
    ///</summary>
    [SugarTable("web_order")]
    public partial class WebOrder
    {
           public WebOrder(){

            this.Type =0;
            this.OfflineCourseid =0L;
            this.RecordCourseid =0L;
            this.OnlineCourseid =0L;
            this.Courseid =0L;
            this.CourseSkuid =0L;
            this.Userid =0L;
            this.SkuTypeid =0L;
            this.ClassHour =0;
            this.MenkeCourseId =0;
            this.MarketPrice =0.00M;
            this.Amount =0.00M;
            this.Payment =0.00M;
            this.Dtime =DateTime.Now;
            this.Status =0;
            this.UserDeleted =0;
            this.PayStatus =0;
            this.Paytime =DateTime.Now;
            this.Paytypeid =0L;
            this.CancelTime =DateTime.Now;
            this.IsTicket =0;
            this.TicketTime =DateTime.Now;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="orderid")]
           public long Orderid {get;set;}

           /// <summary>
           /// Desc:0众语课程，1直播课，2录播课，3线下课
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="type")]
           public int Type {get;set;}

           /// <summary>
           /// Desc:订单号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="order_sn")]
           public string OrderSn {get;set;}

           /// <summary>
           /// Desc:订单号的随机字串（bananapay不允许重复外部订单号）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="order_sn_ext")]
           public string OrderSnExt {get;set;}

           /// <summary>
           /// Desc:线下课ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="offline_courseid")]
           public long OfflineCourseid {get;set;}

           /// <summary>
           /// Desc:录播课ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="record_courseid")]
           public long RecordCourseid {get;set;}

           /// <summary>
           /// Desc:在线直播课ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="online_courseid")]
           public long OnlineCourseid {get;set;}

           /// <summary>
           /// Desc:众语课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="courseid")]
           public long Courseid {get;set;}

           /// <summary>
           /// Desc:SKU对应价格
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="course_sku_priceid")]
           public long CourseSkuPriceid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="course_skuid")]
           public long CourseSkuid {get;set;}

           /// <summary>
           /// Desc:购课人ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="userid")]
           public long Userid {get;set;}

           /// <summary>
           /// Desc:课程标题
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="title")]
           public string Title {get;set;}

           /// <summary>
           /// Desc:课程缩略图
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="img")]
           public string Img {get;set;}

           /// <summary>
           /// Desc:课程简介
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="message")]
           public string Message {get;set;}

           /// <summary>
           /// Desc:上课类型ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sku_typeid")]
           public long SkuTypeid {get;set;}

           /// <summary>
           /// Desc:课时数
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="class_hour")]
           public int ClassHour {get;set;}

           /// <summary>
           /// Desc:关联拓课云课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_course_id")]
           public int MenkeCourseId {get;set;}

           /// <summary>
           /// Desc:市场原价
           /// Default:0.00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="market_price")]
           public decimal MarketPrice {get;set;}

           /// <summary>
           /// Desc:应付款
           /// Default:0.00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="amount")]
           public decimal Amount {get;set;}

           /// <summary>
           /// Desc:实付款
           /// Default:0.00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="payment")]
           public decimal Payment {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:0未付款，1已付款，2付款异常,-1删除
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:0正常，1用户删除
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="user_deleted")]
           public int UserDeleted {get;set;}

           /// <summary>
           /// Desc:0未付款，1提交付款等回调，2回调成功，3回调失败
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="pay_status")]
           public int PayStatus {get;set;}

           /// <summary>
           /// Desc:付款时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="paytime")]
           public DateTime Paytime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="remark")]
           public string Remark {get;set;}

           /// <summary>
           /// Desc:购买者下单时的IP
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ip")]
           public string Ip {get;set;}

           /// <summary>
           /// Desc:货币代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="currency_code")]
           public string CurrencyCode {get;set;}

           /// <summary>
           /// Desc:国家代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="country_code")]
           public string CountryCode {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="paytypeid")]
           public long Paytypeid {get;set;}

           /// <summary>
           /// Desc:取消原因，JSON数组
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="cancel_keys")]
           public string CancelKeys {get;set;}

           /// <summary>
           /// Desc:其它原因
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="cancel_message")]
           public string CancelMessage {get;set;}

           /// <summary>
           /// Desc:取消时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="cancel_time")]
           public DateTime? CancelTime {get;set;}

           /// <summary>
           /// Desc:付款超时提醒，是否创建工单
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="is_ticket")]
           public int IsTicket {get;set;}

           /// <summary>
           /// Desc:创建工单时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="ticket_time")]
           public DateTime TicketTime {get;set;}

           /// <summary>
           /// Desc:付款超时提醒，工单ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ticketid")]
           public string Ticketid {get;set;}

           /// <summary>
           /// Desc:支付平台的交易单号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="transaction_id")]
           public string TransactionId {get;set;}

    }
}
