using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///拓课云-课程列表
    ///</summary>
    [SugarTable("menke_course")]
    public partial class MenkeCourse
    {
           public MenkeCourse(){

            this.MenkeCourseId =0;
            this.MenkeFirstStartTime =0;
            this.MenkeLastestEndTime =0;
            this.MenkeUpdateTime =0;
            this.MenkeDeleteTime =0;
            this.Dtime =DateTime.Now;
            this.Status =0;
            this.Istrial =0;

           }
           /// <summary>
           /// Desc:关联拓课云-课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_course_id")]
           public int MenkeCourseId {get;set;}

           /// <summary>
           /// Desc:课程名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_name")]
           public string MenkeName {get;set;}

           /// <summary>
           /// Desc:课程简介
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_intro")]
           public string MenkeIntro {get;set;}

           /// <summary>
           /// Desc:第一次课节的开始时间戳
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_first_start_time")]
           public int MenkeFirstStartTime {get;set;}

           /// <summary>
           /// Desc:最后一次课节的结束时间戳
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_lastest_end_time")]
           public int MenkeLastestEndTime {get;set;}

           /// <summary>
           /// Desc:更新时间戳
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_update_time")]
           public int MenkeUpdateTime {get;set;}

           /// <summary>
           /// Desc:删除时间戳
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_delete_time")]
           public int MenkeDeleteTime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_msg")]
           public string MenkeMsg {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

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
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:是否试听
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="istrial")]
           public int Istrial {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="json")]
           public string Json {get;set;}

    }
}
