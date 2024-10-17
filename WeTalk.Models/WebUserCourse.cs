using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///我的课程(众语|直播)
    ///</summary>
    [SugarTable("web_user_course")]
    public partial class WebUserCourse
    {
           public WebUserCourse(){

            this.Orderid =0L;
            this.Userid =0L;
            this.Courseid =0L;
            this.OnlineCourseid =0L;
            this.MenkeCourseId =0;
            this.ClassHour =0;
            this.Classes =0;
            this.ScheduleClasses =0;
            this.Dtime =DateTime.Now;
            this.Status =0;
            this.SkuTypeid =0L;
            this.IsResidueEmail =0;
            this.ResidueEmailTime =DateTime.Now;
            this.IsTicket =0;
            this.TicketTime =DateTime.Now;
            this.IsBuyTicket =0;
            this.BuyTicketTime =DateTime.Now;
            this.Lastchanged =0;
            this.Istrial =0;
            this.Type =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="user_courseid")]
           public long UserCourseid {get;set;}

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
           /// Desc:众语课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="courseid")]
           public long Courseid {get;set;}

           /// <summary>
           /// Desc:在线直播课
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="online_courseid")]
           public long OnlineCourseid {get;set;}

           /// <summary>
           /// Desc:关联拓课云-门课ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_course_id")]
           public int MenkeCourseId {get;set;}

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
           /// Desc:课时数
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="class_hour")]
           public int ClassHour {get;set;}

           /// <summary>
           /// Desc:已完成的课节数
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="classes")]
           public int Classes {get;set;}

           /// <summary>
           /// Desc:已排课节数
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="schedule_classes")]
           public int ScheduleClasses {get;set;}

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
           /// Desc:上课类型ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sku_typeid")]
           public long SkuTypeid {get;set;}

           /// <summary>
           /// Desc:是否发送过剩余课时提醒
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="is_residue_email")]
           public int IsResidueEmail {get;set;}

           /// <summary>
           /// Desc:剩余课时提醒时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="residue_email_time")]
           public DateTime ResidueEmailTime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="remarks")]
           public string Remarks {get;set;}

           /// <summary>
           /// Desc:是否发出续课提醒工单
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="is_ticket")]
           public int IsTicket {get;set;}

           /// <summary>
           /// Desc:续课工单提醒时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="ticket_time")]
           public DateTime TicketTime {get;set;}

           /// <summary>
           /// Desc:续课工单ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ticket_id")]
           public string TicketId {get;set;}

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

           /// <summary>
           /// Desc:是否试听课
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="istrial")]
           public int Istrial {get;set;}

           /// <summary>
           /// Desc:0众语课程,1直播课
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="type")]
           public int Type {get;set;}

    }
}
