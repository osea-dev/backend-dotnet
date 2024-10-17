using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///作业列表
    ///</summary>
    [SugarTable("menke_homework")]
    public partial class MenkeHomework
    {
           public MenkeHomework(){

            this.MenkeHomeworkId =0;
            this.MenkeSubmitWay =0;
            this.MenkeIsDraft =0;
            this.MenkeLessonId =0;
            this.MenkeDeleteTime =0;
            this.MenkeUpdateTime =0;
            this.MenkeCreateTime =0;
            this.MenkeEndDate =0;
            this.Dtime =DateTime.Now;
            this.Status =0;

           }
           /// <summary>
           /// Desc:作业ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_homework_id")]
           public int MenkeHomeworkId {get;set;}

           /// <summary>
           /// Desc:日期
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_day")]
           public string MenkeDay {get;set;}

           /// <summary>
           /// Desc:标题
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_title")]
           public string MenkeTitle {get;set;}

           /// <summary>
           /// Desc:内容
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_content")]
           public string MenkeContent {get;set;}

           /// <summary>
           /// Desc:资源
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_resources")]
           public string MenkeResources {get;set;}

           /// <summary>
           /// Desc:提交方式0不限制反馈方式1图片2视频3录音
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_submit_way")]
           public int MenkeSubmitWay {get;set;}

           /// <summary>
           /// Desc:是否草稿 0否 1是
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_is_draft")]
           public int MenkeIsDraft {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_lesson_id")]
           public int MenkeLessonId {get;set;}

           /// <summary>
           /// Desc:删除时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_delete_time")]
           public int MenkeDeleteTime {get;set;}

           /// <summary>
           /// Desc:更新时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_update_time")]
           public int MenkeUpdateTime {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_create_time")]
           public int MenkeCreateTime {get;set;}

           /// <summary>
           /// Desc:截止时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_end_date")]
           public int MenkeEndDate {get;set;}

           /// <summary>
           /// Desc:账号（手机号或自定义账号）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_account")]
           public string MenkeAccount {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_msg")]
           public string MenkeMsg {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_logid")]
           public string MenkeLogid {get;set;}

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
