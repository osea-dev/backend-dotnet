using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///在线直播课语言扩展表
    ///</summary>
    [SugarTable("web_online_course_lang")]
    public partial class WebOnlineCourseLang
    {
           public WebOnlineCourseLang(){

            this.OnlineCourseid =0L;
            this.Dtime =DateTime.Now;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="online_course_langid")]
           public long OnlineCourseLangid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="online_courseid")]
           public long OnlineCourseid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lang")]
           public string Lang {get;set;}

           /// <summary>
           /// Desc:标题
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="title")]
           public string Title {get;set;}

           /// <summary>
           /// Desc:简单介绍
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="message")]
           public string Message {get;set;}

           /// <summary>
           /// Desc:详情
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="intro")]
           public string Intro {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:标签
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="keys")]
           public string Keys {get;set;}

    }
}
