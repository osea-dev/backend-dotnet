using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///课程申请表
    ///</summary>
    [SugarTable("web_course_apply")]
    public partial class WebCourseApply
    {
           public WebCourseApply(){

            this.Sty =0;
            this.Courseid =0L;
            this.UserCourseid =0L;
            this.Dtime =DateTime.Now;
            this.Status =0;
            this.MenkeCourseId =0;
            this.MenkeLessonId =0;
            this.Userid =0L;
            this.IsTicket =0;
            this.TicketTime =DateTime.Now;
            this.Age =0;
            this.Ischinese =0;
            this.Gender =0;
            this.SkuTypeid =0L;
            this.ClassHour =0;
            this.CourseGroupInfoid =0L;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="course_applyid")]
           public long CourseApplyid {get;set;}

           /// <summary>
           /// Desc:0试听申请，1普通留言，2排课申请
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sty")]
           public int Sty {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="courseid")]
           public long Courseid {get;set;}

           /// <summary>
           /// Desc:已购课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="user_courseid")]
           public long UserCourseid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="contact_mobile_code")]
           public string ContactMobileCode {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="contact_mobile")]
           public string ContactMobile {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="contact_email")]
           public string ContactEmail {get;set;}

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
           [SugarColumn(ColumnName="ip")]
           public string Ip {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="remark")]
           public string Remark {get;set;}

           /// <summary>
           /// Desc:0申请，1同意，2缺课，3拒绝
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
           [SugarColumn(ColumnName="message")]
           public string Message {get;set;}

           /// <summary>
           /// Desc:上课语言(做废)
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lecture_lang")]
           public string LectureLang {get;set;}

           /// <summary>
           /// Desc:试听课程名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="course_name")]
           public string CourseName {get;set;}

           /// <summary>
           /// Desc:拓课云-课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_course_id")]
           public int MenkeCourseId {get;set;}

           /// <summary>
           /// Desc:拓课云-课节ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_lesson_id")]
           public int MenkeLessonId {get;set;}

           /// <summary>
           /// Desc:拓课云-课程名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_course_name")]
           public string MenkeCourseName {get;set;}

           /// <summary>
           /// Desc:拓课云-课节名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_lesson_name")]
           public string MenkeLessonName {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="teacher")]
           public string Teacher {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="teacher_mobile_code")]
           public string TeacherMobileCode {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="teacher_mobile")]
           public string TeacherMobile {get;set;}

           /// <summary>
           /// Desc:申请人ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="userid")]
           public long Userid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="is_ticket")]
           public int IsTicket {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="ticket_time")]
           public DateTime TicketTime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ticket_id")]
           public string TicketId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="age")]
           public int Age {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="birthdate")]
           public string Birthdate {get;set;}

           /// <summary>
           /// Desc:是否有汉语基础
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="ischinese")]
           public int Ischinese {get;set;}

           /// <summary>
           /// Desc:性别:0女，1男
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="gender")]
           public int Gender {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sku_typeid")]
           public long SkuTypeid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="class_hour")]
           public int ClassHour {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="course_group_infoid")]
           public long CourseGroupInfoid {get;set;}

    }
}
