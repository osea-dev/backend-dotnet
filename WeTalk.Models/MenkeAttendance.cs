using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///考勤表
    ///</summary>
    [SugarTable("menke_attendance")]
    public partial class MenkeAttendance
    {
           public MenkeAttendance(){

            this.MenkeLessonId =0;
            this.MenkeStarttime =0;
            this.MenkeEndtime =0;
            this.MenkeAttendanceLate =0;
            this.MenkeAttendanceLeaveEarly =0;
            this.MenkeAttendanceAbsent =0;
            this.Dtime =DateTime.Now;
            this.Status =0;
            this.Istrial =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="menke_attendanceid")]
           public long MenkeAttendanceid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_lesson_id")]
           public int MenkeLessonId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_starttime")]
           public int MenkeStarttime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_endtime")]
           public int MenkeEndtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_attendance_mobile")]
           public string MenkeAttendanceMobile {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_attendance_mobile_code")]
           public string MenkeAttendanceMobileCode {get;set;}

           /// <summary>
           /// Desc:账号类型（7教师8学生）
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_attendance_userroleid")]
           public int MenkeAttendanceUserroleid {get;set;}

           /// <summary>
           /// Desc:进出记录，json数组
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_attendance_access_record")]
           public string MenkeAttendanceAccessRecord {get;set;}

           /// <summary>
           /// Desc:是否迟到（0否1是）
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_attendance_late")]
           public int MenkeAttendanceLate {get;set;}

           /// <summary>
           /// Desc:是否早退（0否1是）
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_attendance_leave_early")]
           public int MenkeAttendanceLeaveEarly {get;set;}

           /// <summary>
           /// Desc:是否缺勤（0否1是）
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_attendance_absent")]
           public int MenkeAttendanceAbsent {get;set;}

           /// <summary>
           /// Desc:上下课记录，json数组
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_timeinfo")]
           public string MenkeTimeinfo {get;set;}

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
           /// Desc:0未处理，1已处理
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
