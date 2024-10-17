using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///课节报告
    ///</summary>
    [SugarTable("web_user_lesson_report")]
    public partial class WebUserLessonReport
    {
           public WebUserLessonReport(){

            this.UserLessonid =0L;
            this.Userid =0L;
            this.MenkeUserid =0L;
            this.Teacherid =0L;
            this.Homework =0;
            this.Attention =0;
            this.Enthusiasm =0;
            this.Hear =0;
            this.Say =0;
            this.Read =0;
            this.Write =0;
            this.Thinking =0;
            this.EmotionalQuotient =0;
            this.LoveQuotient =0;
            this.InverseQuotient =0;
            this.Performance =0;
            this.Dtime =DateTime.Now;
            this.MenkeCourseId =0;
            this.MenkeLessonId =0;
            this.Level =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="user_lesson_reportid")]
           public long UserLessonReportid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="user_lessonid")]
           public long UserLessonid {get;set;}

           /// <summary>
           /// Desc:学生ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="userid")]
           public long Userid {get;set;}

           /// <summary>
           /// Desc:拓课云学生ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_userid")]
           public long MenkeUserid {get;set;}

           /// <summary>
           /// Desc:老师ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="teacherid")]
           public long Teacherid {get;set;}

           /// <summary>
           /// Desc:老师评语
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="message")]
           public string Message {get;set;}

           /// <summary>
           /// Desc:作业完成情况
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="homework")]
           public int Homework {get;set;}

           /// <summary>
           /// Desc:注意力
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="attention")]
           public int Attention {get;set;}

           /// <summary>
           /// Desc:积极性
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="enthusiasm")]
           public int Enthusiasm {get;set;}

           /// <summary>
           /// Desc:听
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="hear")]
           public int Hear {get;set;}

           /// <summary>
           /// Desc:说
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="say")]
           public int Say {get;set;}

           /// <summary>
           /// Desc:读
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="read")]
           public int Read {get;set;}

           /// <summary>
           /// Desc:写
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="write")]
           public int Write {get;set;}

           /// <summary>
           /// Desc:思
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="thinking")]
           public int Thinking {get;set;}

           /// <summary>
           /// Desc:情商
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="emotional_quotient")]
           public int EmotionalQuotient {get;set;}

           /// <summary>
           /// Desc:爱商
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="love_quotient")]
           public int LoveQuotient {get;set;}

           /// <summary>
           /// Desc:逆商
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="inverse_quotient")]
           public int InverseQuotient {get;set;}

           /// <summary>
           /// Desc:综合表现
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="performance")]
           public int Performance {get;set;}

           /// <summary>
           /// Desc:创建报告时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

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
           /// Desc:等级定级
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="level")]
           public int Level {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="remarks")]
           public string Remarks {get;set;}

    }
}
