using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    /// 我的线下课
    ///</summary>
    [SugarTable("web_user_offline_course")]
    public partial class WebUserOfflineCourse
    {
           public WebUserOfflineCourse(){

            this.Orderid =0L;
            this.Userid =0L;
            this.OfflineCourseid =0L;
            this.Dtime =DateTime.Now;
            this.Status =0;
            this.IsBuyTicket =0;
            this.BuyTicketTime =DateTime.Now;
            this.Lastchanged =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="user_offline_courseid")]
           public long UserOfflineCourseid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="order_sn")]
           public string OrderSn {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="orderid")]
           public long Orderid {get;set;}

           /// <summary>
           /// Desc:
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
           [SugarColumn(ColumnName="offline_courseid")]
           public long OfflineCourseid {get;set;}

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
           /// Desc:创建时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:0未启用锁定，1正常，-1删除
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
           [SugarColumn(ColumnName="remarks")]
           public string Remarks {get;set;}

           /// <summary>
           /// Desc:是否发出购课提醒工单
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="is_buy_ticket")]
           public int IsBuyTicket {get;set;}

           /// <summary>
           /// Desc:购课工单提醒时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="buy_ticket_time")]
           public DateTime BuyTicketTime {get;set;}

           /// <summary>
           /// Desc:购课工单ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="buy_ticket_id")]
           public string BuyTicketId {get;set;}

           /// <summary>
           /// Desc:网站操作的最后更新时间(UTC时间)
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="lastchanged")]
           public int Lastchanged {get;set;}

    }
}
