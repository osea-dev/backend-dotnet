using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("web_course_group_info_lang")]
    public partial class WebCourseGroupInfoLang
    {
           public WebCourseGroupInfoLang(){

            this.CourseGroupInfoid =0L;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="course_group_info_langid")]
           public long CourseGroupInfoLangid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="course_group_infoid")]
           public long CourseGroupInfoid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lang")]
           public string Lang {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="title")]
           public string Title {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="message")]
           public string Message {get;set;}

           /// <summary>
           /// Desc:课程介绍
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="intro")]
           public string Intro {get;set;}

           /// <summary>
           /// Desc:课程目标
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="objectives")]
           public string Objectives {get;set;}

           /// <summary>
           /// Desc:适合人群
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="crowd")]
           public string Crowd {get;set;}

           /// <summary>
           /// Desc:课程特色
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="merit")]
           public string Merit {get;set;}

           /// <summary>
           /// Desc:开课时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="begintime")]
           public string Begintime {get;set;}

           /// <summary>
           /// Desc:课程目录
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="catalog")]
           public string Catalog {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="keys")]
           public string Keys {get;set;}

    }
}
