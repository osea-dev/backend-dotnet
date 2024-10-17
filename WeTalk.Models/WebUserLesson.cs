using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///我的课节表（关联拓课云课节）
    ///</summary>
    [SugarTable("web_user_lesson")]
    public partial class WebUserLesson
    {
           public WebUserLesson(){

            this.UserCourseid =0L;
            this.MenkeUserId =0;
            this.Userid =0L;
            this.Teacherid =0L;
            this.OnlineCourseid =0L;
            this.Courseid =0L;
            this.CourseSkuid =0L;
            this.SkuTypeid =0L;
            this.MenkeCourseId =0;
            this.MenkeLessonId =0;
            this.MenkeState =0;
            this.MenkeStarttime =0;
            this.MenkeEndtime =0;
            this.MenkeUpdateTime =0;
            this.MenkeDeleteTime =0;
            this.Dtime =DateTime.Now;
            this.Status =0;
            this.Score =0;
            this.ScoreTime =DateTime.Now;
            this.ScoreFirsttime =DateTime.Now;
            this.Istrial =0;
            this.IsTrialEmail =0;
            this.TrialEmailTime =DateTime.Now;
            this.IsTrialCompleteEmail =0;
            this.TrialCompleteEmailTime =DateTime.Now;
            this.IsBeginEmail =0;
            this.BeginEmailTime =DateTime.Now;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="user_lessonid")]
           public long UserLessonid {get;set;}

           /// <summary>
           /// Desc:同步时关联
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="user_courseid")]
           public long UserCourseid {get;set;}

           /// <summary>
           /// Desc:拓课云中的用户ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_user_id")]
           public int MenkeUserId {get;set;}

           /// <summary>
           /// Desc:用户ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="userid")]
           public long Userid {get;set;}

           /// <summary>
           /// Desc:老师ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="teacherid")]
           public long Teacherid {get;set;}

           /// <summary>
           /// Desc:在线直播课
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="online_courseid")]
           public long OnlineCourseid {get;set;}

           /// <summary>
           /// Desc:课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="courseid")]
           public long Courseid {get;set;}

           /// <summary>
           /// Desc:课程SKUID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="course_skuid")]
           public long CourseSkuid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="sku_typeid")]
           public long? SkuTypeid {get;set;}

           /// <summary>
           /// Desc:关联拓课云-课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_course_id")]
           public int MenkeCourseId {get;set;}

           /// <summary>
           /// Desc:关联拓课云-课节ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_lesson_id")]
           public int MenkeLessonId {get;set;}

           /// <summary>
           /// Desc:课节名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_lesson_name")]
           public string MenkeLessonName {get;set;}

           /// <summary>
           /// Desc:学生姓名
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_student_name")]
           public string MenkeStudentName {get;set;}

           /// <summary>
           /// Desc:学生手机号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_student_mobile")]
           public string MenkeStudentMobile {get;set;}

           /// <summary>
           /// Desc:学生手机区号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_student_code")]
           public string MenkeStudentCode {get;set;}

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
           /// Desc:0禁用，1正常
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:教室入口免密URL
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_entryurl")]
           public string MenkeEntryurl {get;set;}

           /// <summary>
           /// Desc:对老师的评分
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="score")]
           public int Score {get;set;}

           /// <summary>
           /// Desc:最后评分时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="score_time")]
           public DateTime ScoreTime {get;set;}

           /// <summary>
           /// Desc:首次评分时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="score_firsttime")]
           public DateTime ScoreFirsttime {get;set;}

           /// <summary>
           /// Desc:是否试听
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="istrial")]
           public int Istrial {get;set;}

           /// <summary>
           /// Desc:是否发送过试听预约成功邮件
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="is_trial_email")]
           public int IsTrialEmail {get;set;}

           /// <summary>
           /// Desc:试听预约成功邮件提醒时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="trial_email_time")]
           public DateTime TrialEmailTime {get;set;}

           /// <summary>
           /// Desc:是否发送过试听完成邮件
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="is_trial_complete_email")]
           public int IsTrialCompleteEmail {get;set;}

           /// <summary>
           /// Desc:试听完成邮件提醒时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="trial_complete_email_time")]
           public DateTime TrialCompleteEmailTime {get;set;}

           /// <summary>
           /// Desc:是否课前提醒
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="is_begin_email")]
           public int IsBeginEmail {get;set;}

           /// <summary>
           /// Desc:课前提醒时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="begin_email_time")]
           public DateTime BeginEmailTime {get;set;}

    }
}
