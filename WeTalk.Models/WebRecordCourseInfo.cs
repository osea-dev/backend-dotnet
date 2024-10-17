using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///录播课明细表
    ///</summary>
    [SugarTable("web_record_course_info")]
    public partial class WebRecordCourseInfo
    {
           public WebRecordCourseInfo(){

            this.RecordCourseid =0L;
            this.Sort =0;
            this.Dtime =DateTime.Now;
            this.ViewCount =0;
            this.Status =0;
            this.Hits =0;
            this.Lasttime =DateTime.Now;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="record_course_infoid")]
           public long RecordCourseInfoid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="record_courseid")]
           public long RecordCourseid {get;set;}

           /// <summary>
           /// Desc:视频URL
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="video")]
           public string Video {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sort")]
           public int Sort {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:时长
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="duration")]
           public string Duration {get;set;}

           /// <summary>
           /// Desc:观看人次
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="view_count")]
           public int ViewCount {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

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
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="lasttime")]
           public DateTime Lasttime {get;set;}

           /// <summary>
           /// Desc:最后访问IP
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ip")]
           public string Ip {get;set;}

    }
}
