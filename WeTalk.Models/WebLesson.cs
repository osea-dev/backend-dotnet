using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///课节列表(预约)
    ///</summary>
    [SugarTable("web_lesson")]
    public partial class WebLesson
    {
           public WebLesson(){

            this.UserCourseid =0L;
            this.Courseid =0L;
            this.MenkeStarttime =0;
            this.MenkeEndtime =0;
            this.MenkeUpdateTime =0;
            this.MenkeDeleteTime =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="lessonid")]
           public long Lessonid {get;set;}

           /// <summary>
           /// Desc:用户所属课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="user_courseid")]
           public long UserCourseid {get;set;}

           /// <summary>
           /// Desc:课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="courseid")]
           public long Courseid {get;set;}

           /// <summary>
           /// Desc:关联拓课云-门课ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_course_id")]
           public string MenkeCourseId {get;set;}

           /// <summary>
           /// Desc:关联拓课云-课节ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_id")]
           public string MenkeId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_name")]
           public string MenkeName {get;set;}

           /// <summary>
           /// Desc:教师名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_teacher_name")]
           public string MenkeTeacherName {get;set;}

           /// <summary>
           /// Desc:教师手机号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_teacher_mobile")]
           public string MenkeTeacherMobile {get;set;}

           /// <summary>
           /// Desc:教师手机区号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_teacher_code")]
           public string MenkeTeacherCode {get;set;}

           /// <summary>
           /// Desc:助教信息，json数组
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_helpers")]
           public string MenkeHelpers {get;set;}

           /// <summary>
           /// Desc:课次状态（1未开始2进行中3已结课4已过期）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_status")]
           public int? MenkeStatus {get;set;}

           /// <summary>
           /// Desc:课次开始时间戳
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_starttime")]
           public int MenkeStarttime {get;set;}

           /// <summary>
           /// Desc:课次结束时间戳
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_endtime")]
           public int MenkeEndtime {get;set;}

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

    }
}
