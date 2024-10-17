using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///课程下分组明细表
    ///</summary>
    [SugarTable("web_course_group_info")]
    public partial class WebCourseGroupInfo
    {
           public WebCourseGroupInfo(){

            this.CourseGroupid =0L;
            this.Courseid =0L;
            this.Sort =100;
            this.Status =0;
            this.Dtime =DateTime.Now;
            this.Sendtime =DateTime.Now;
            this.Hits =0;
            this.Lasttime =DateTime.Now;
            this.AgeMin =0;
            this.AgeMax =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="course_group_infoid")]
           public long CourseGroupInfoid {get;set;}

           /// <summary>
           /// Desc:子课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="course_groupid")]
           public long CourseGroupid {get;set;}

           /// <summary>
           /// Desc:课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="courseid")]
           public long Courseid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:100
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sort")]
           public int Sort {get;set;}

           /// <summary>
           /// Desc:
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
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sendtime")]
           public DateTime Sendtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="img")]
           public string Img {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="keysids")]
           public string Keysids {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="hits")]
           public int Hits {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lasttime")]
           public DateTime? Lasttime {get;set;}

           /// <summary>
           /// Desc:最后访问IP
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ip")]
           public string Ip {get;set;}

           /// <summary>
           /// Desc:限制年龄最小值，0不限
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="age_min")]
           public int AgeMin {get;set;}

           /// <summary>
           /// Desc:限制年龄最大值，0不限
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="age_max")]
           public int AgeMax {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="level")]
           public string Level {get;set;}

    }
}
