using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///课前准备信息
    ///</summary>
    [SugarTable("menke_lesson_url")]
    public partial class MenkeLessonUrl
    {
           public MenkeLessonUrl(){

            this.MenkeLessonId =0;
            this.MenkeUserId =0;
            this.Dtime =DateTime.Now;

           }
           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_lesson_id")]
           public int MenkeLessonId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_teacher_userpassword")]
           public string MenkeTeacherUserpassword {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_teacher_enterurl")]
           public string MenkeTeacherEnterurl {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_teacher_entryurl")]
           public string MenkeTeacherEntryurl {get;set;}

           /// <summary>
           /// Desc:助教密码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_assistant_userpassword")]
           public string MenkeAssistantUserpassword {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_assistant_enterurl")]
           public string MenkeAssistantEnterurl {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_assistant_entryurl")]
           public string MenkeAssistantEntryurl {get;set;}

           /// <summary>
           /// Desc:学生密码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_student_userpassword")]
           public string MenkeStudentUserpassword {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_student_enterurl")]
           public string MenkeStudentEnterurl {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_student_entryurl")]
           public string MenkeStudentEntryurl {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_tour_userpassword")]
           public string MenkeTourUserpassword {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_tour_enterurl")]
           public string MenkeTourEnterurl {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_tour_entryurl")]
           public string MenkeTourEntryurl {get;set;}

           /// <summary>
           /// Desc:巡课密码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_auditors_userpassword")]
           public string MenkeAuditorsUserpassword {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_auditors_enterurl")]
           public string MenkeAuditorsEnterurl {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_auditors_entryurl")]
           public string MenkeAuditorsEntryurl {get;set;}

           /// <summary>
           /// Desc:学生ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_user_id")]
           public int MenkeUserId {get;set;}

           /// <summary>
           /// Desc:进入教室的名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_user_name")]
           public string MenkeUserName {get;set;}

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
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="json")]
           public string Json {get;set;}

    }
}
