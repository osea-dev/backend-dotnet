using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///邮件任务队列
    ///</summary>
    [SugarTable("web_task_email")]
    public partial class WebTaskEmail
    {
           public WebTaskEmail(){

            this.Dtime =DateTime.Now;
            this.Status =0;
            this.Count =0;
            this.Lasttime =DateTime.Now;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="task_emailid")]
           public long TaskEmailid {get;set;}

           /// <summary>
           /// Desc:解发者
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="userid")]
           public long Userid {get;set;}

           /// <summary>
           /// Desc:模板代码 
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="code")]
           public string Code {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="subject")]
           public string Subject {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="body")]
           public string Body {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime? Dtime {get;set;}

           /// <summary>
           /// Desc:0待发，1成功，2失败
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:失败重发次数（最多3次）
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="count")]
           public int Count {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lasttime")]
           public DateTime? Lasttime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="remarks")]
           public string Remarks {get;set;}

           /// <summary>
           /// Desc:接收者邮箱
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="email")]
           public string Email {get;set;}

    }
}
