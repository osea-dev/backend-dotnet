using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///学生提交的作业记录
    ///</summary>
    [SugarTable("menke_homework_submit")]
    public partial class MenkeHomeworkSubmit
    {
           public MenkeHomeworkSubmit(){

            this.MenkeSubmitId =0;
            this.MenkeHomeworkId =0;
            this.MenkeSubmitTime =0;
            this.MenkeSubmitDelTime =0;
            this.Dtime =DateTime.Now;
            this.Status =0;

           }
           /// <summary>
           /// Desc:提交记录ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_submit_id")]
           public int MenkeSubmitId {get;set;}

           /// <summary>
           /// Desc:作业ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_homework_id")]
           public int MenkeHomeworkId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_student_code")]
           public string MenkeStudentCode {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_student_mobile")]
           public string MenkeStudentMobile {get;set;}

           /// <summary>
           /// Desc:提交时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_submit_time")]
           public int MenkeSubmitTime {get;set;}

           /// <summary>
           /// Desc:提交记录删除时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_submit_del_time")]
           public int MenkeSubmitDelTime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_submit_content")]
           public string MenkeSubmitContent {get;set;}

           /// <summary>
           /// Desc:提交资源附件jarray
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_submit_files")]
           public string MenkeSubmitFiles {get;set;}

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

    }
}
