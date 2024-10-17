using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///通知信息表
    ///</summary>
    [SugarTable("web_message")]
    public partial class WebMessage
    {
           public WebMessage(){

            this.SendUserid =0L;
            this.AccUserid =0L;
            this.Sendtime =DateTime.Now;
            this.Acctime =DateTime.Now;
            this.SendStatus =0;
            this.AccStatus =0;
            this.Fid =0L;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="messageid")]
           public long Messageid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="send_userid")]
           public long SendUserid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="acc_userid")]
           public long AccUserid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="message")]
           public string Message {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sendtime")]
           public DateTime Sendtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="acctime")]
           public DateTime Acctime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="send_status")]
           public int SendStatus {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="acc_status")]
           public int AccStatus {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="remark")]
           public string Remark {get;set;}

           /// <summary>
           /// Desc:消息模板code
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="template_code")]
           public string TemplateCode {get;set;}

           /// <summary>
           /// Desc:对象ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="fid")]
           public long Fid {get;set;}

           /// <summary>
           /// Desc:对像表名
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="table_name")]
           public string TableName {get;set;}

    }
}
