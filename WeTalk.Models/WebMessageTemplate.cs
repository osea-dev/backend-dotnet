using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///消息模板表
    ///</summary>
    [SugarTable("web_message_template")]
    public partial class WebMessageTemplate
    {
           public WebMessageTemplate(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,ColumnName="message_templateid")]
           public long MessageTemplateid {get;set;}

           /// <summary>
           /// Desc:消息模板code
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="template_code")]
           public string TemplateCode {get;set;}

           /// <summary>
           /// Desc:模板标题
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="title")]
           public string Title {get;set;}

           /// <summary>
           /// Desc:模板
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="content")]
           public string Content {get;set;}

    }
}
