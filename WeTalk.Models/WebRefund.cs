using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///退款单
    ///</summary>
    [SugarTable("web_refund")]
    public partial class WebRefund
    {
           public WebRefund(){

            this.Userid =0L;
            this.Orderid =0L;
            this.Amount =0.00M;
            this.Dtime =DateTime.Now;
            this.Payment =0.00M;
            this.RefundTime =DateTime.Now;
            this.Status =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="refundid")]
           public long Refundid {get;set;}

           /// <summary>
           /// Desc:申请人ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="userid")]
           public long Userid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="orderid")]
           public long Orderid {get;set;}

           /// <summary>
           /// Desc:订单号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="order_sn")]
           public string OrderSn {get;set;}

           /// <summary>
           /// Desc:退款单号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="refund_sn")]
           public string RefundSn {get;set;}

           /// <summary>
           /// Desc:申请退款金额
           /// Default:0.00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="amount")]
           public decimal Amount {get;set;}

           /// <summary>
           /// Desc:申请时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:实际退款金额
           /// Default:0.00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="payment")]
           public decimal Payment {get;set;}

           /// <summary>
           /// Desc:退款时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="refund_time")]
           public DateTime RefundTime {get;set;}

           /// <summary>
           /// Desc:0未退款，1全退款，2部份退款
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="remark")]
           public string Remark {get;set;}

    }
}
