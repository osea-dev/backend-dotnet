using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("web_course_group_lang")]
    public partial class WebCourseGroupLang
    {
           public WebCourseGroupLang(){

            this.CourseGroupid =0L;
            this.Dtime =DateTime.Now;

           }
           /// <summary>
           /// Desc:课程下的分组表
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="course_group_langid")]
           public long CourseGroupLangid {get;set;}

           /// <summary>
           /// Desc:课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="course_groupid")]
           public long CourseGroupid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lang")]
           public string Lang {get;set;}

           /// <summary>
           /// Desc:分组名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="group_name")]
           public string GroupName {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

    }
}
