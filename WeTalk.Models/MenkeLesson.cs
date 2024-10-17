using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///拓课云-课节列表
    ///</summary>
    [SugarTable("menke_lesson")]
    public partial class MenkeLesson
    {
           public MenkeLesson(){

            this.MenkeLessonId =0;
            this.MenkeCourseId =0;
            this.MenkeState =0;
            this.MenkeStarttime =0;
            this.MenkeEndtime =0;
            this.MenkeUpdateTime =0;
            this.MenkeDeleteTime =0;
            this.Dtime =DateTime.Now;
            this.Status =0;
            this.Istrial =0;
            this.IsTeacherEmail =0;
            this.TeacherCodeTime =DateTime.Now;
            this.Isexe =0;
            this.ExeTime =DateTime.Now;

           }
           /// <summary>
           /// Desc:关联拓课云-课节ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_lesson_id")]
           public int MenkeLessonId {get;set;}

           /// <summary>
           /// Desc:关联拓课云-门课ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_course_id")]
           public int MenkeCourseId {get;set;}

           /// <summary>
           /// Desc:课节名称
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
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_state")]
           public int MenkeState {get;set;}

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
           /// Desc:教室号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_live_serial")]
           public string MenkeLiveSerial {get;set;}

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
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="json")]
           public string Json {get;set;}

           /// <summary>
           /// Desc:是否试听
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="istrial")]
           public int Istrial {get;set;}

           /// <summary>
           /// Desc:是否发给老师填报告的MAIL
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="is_teacher_email")]
           public int IsTeacherEmail {get;set;}

           /// <summary>
           /// Desc:老师发布课堂报告的临时授权码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="teacher_code")]
           public string TeacherCode {get;set;}

           /// <summary>
           /// Desc:临时授权码发布时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="teacher_code_time")]
           public DateTime TeacherCodeTime {get;set;}

           /// <summary>
           /// Desc:是否拆解执行JSON
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="isexe")]
           public int Isexe {get;set;}

           /// <summary>
           /// Desc:执行时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="exe_time")]
           public DateTime ExeTime {get;set;}

           /// <summary>
           /// Desc:学生数组
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="students")]
           public string Students {get;set;}

    }
}
