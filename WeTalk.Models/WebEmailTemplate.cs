using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("web_email_template")]
    public partial class WebEmailTemplate
    {
           public WebEmailTemplate(){

            this.Status =0;
            this.Dtime =DateTime.Now;
            this.Sort =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="email_templateid")]
           public long EmailTemplateid {get;set;}

           /// <summary>
           /// Desc:模板唯一代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="code")]
           public string Code {get;set;}

           /// <summary>
           /// Desc:语言
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lang")]
           public string Lang {get;set;}

           /// <summary>
           /// Desc:模板名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="template_name")]
           public string TemplateName {get;set;}

           /// <summary>
           /// Desc:邮件标题
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="title")]
           public string Title {get;set;}

           /// <summary>
           /// Desc:模板内容
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="content")]
           public string Content {get;set;}

           /// <summary>
           /// Desc:提示简介
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="message")]
           public string Message {get;set;}

           /// <summary>
           /// Desc:状态
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime? Dtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="remark")]
           public string Remark {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sort")]
           public int Sort {get;set;}

    }
}
