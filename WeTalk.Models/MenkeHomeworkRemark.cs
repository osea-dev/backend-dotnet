using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///作业点评表
    ///</summary>
    [SugarTable("menke_homework_remark")]
    public partial class MenkeHomeworkRemark
    {
           public MenkeHomeworkRemark(){

            this.MenkeSubmitId =0;
            this.MenkeHomeworkId =0;
            this.MenkeIsPass =0;
            this.MenkeRemarkTime =0;
            this.MenkeRemarkDelTime =0;
            this.MenkeRank =0;
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
           [SugarColumn(ColumnName="menke_teacher_code")]
           public string MenkeTeacherCode {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_teacher_mobile")]
           public string MenkeTeacherMobile {get;set;}

           /// <summary>
           /// Desc:是否通过（1是0否）
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_is_pass")]
           public int MenkeIsPass {get;set;}

           /// <summary>
           /// Desc:点评时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_remark_time")]
           public int MenkeRemarkTime {get;set;}

           /// <summary>
           /// Desc:点评记录删除时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_remark_del_time")]
           public int MenkeRemarkDelTime {get;set;}

           /// <summary>
           /// Desc:点评内容
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_remark_content")]
           public string MenkeRemarkContent {get;set;}

           /// <summary>
           /// Desc:点评资源jarray
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_remark_files")]
           public string MenkeRemarkFiles {get;set;}

           /// <summary>
           /// Desc:评级
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_rank")]
           public int MenkeRank {get;set;}

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
