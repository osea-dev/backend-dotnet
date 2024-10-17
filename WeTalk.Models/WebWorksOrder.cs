using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///作品订单表
    ///</summary>
    [SugarTable("web_works_order")]
    public partial class WebWorksOrder
    {
           public WebWorksOrder(){

            this.Worksid =0L;
            this.Businessid =0L;
            this.Userid =0L;
            this.Price =0.00M;
            this.MarketPrice =0.00M;
            this.Amount =0.00M;
            this.Payment =0.00M;
            this.Dtime =DateTime.Now;
            this.Status =0;
            this.PayStatus =0;
            this.Paytime =DateTime.Now;
            this.Paytypeid =0L;
            this.CancelTime =DateTime.Now;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="works_orderid")]
           public long WorksOrderid {get;set;}

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
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="worksid")]
           public long Worksid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="businessid")]
           public long Businessid {get;set;}

           /// <summary>
           /// Desc:老师ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="userid")]
           public long Userid {get;set;}

           /// <summary>
           /// Desc:作品标题
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="title")]
           public string Title {get;set;}

           /// <summary>
           /// Desc:作品缩略图
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="img")]
           public string Img {get;set;}

           /// <summary>
           /// Desc:作品简介
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="message")]
           public string Message {get;set;}

           /// <summary>
           /// Desc:市场原价
           /// Default:0.00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="price")]
           public decimal Price {get;set;}

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
           /// Desc:支付平台的交易单号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="transaction_id")]
           public string TransactionId {get;set;}

    }
}
