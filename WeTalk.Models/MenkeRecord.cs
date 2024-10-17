using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///课堂回放表
    ///</summary>
    [SugarTable("menke_record")]
    public partial class MenkeRecord
    {
           public MenkeRecord(){

            this.MenkeLessonId =0;
            this.MenkeRecordid ="0";
            this.MenkeStarttime =0;
            this.MenkeDuration =0;
            this.MenkeSize =0L;
            this.Dtime =DateTime.Now;
            this.Status =0;

           }
           /// <summary>
           /// Desc:关联拓课云-课节ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_lesson_id")]
           public int MenkeLessonId {get;set;}

           /// <summary>
           /// Desc:录制件ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_recordid")]
           public string MenkeRecordid {get;set;}

           /// <summary>
           /// Desc:录制件标题
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_recordtitle")]
           public string MenkeRecordtitle {get;set;}

           /// <summary>
           /// Desc:开始时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_starttime")]
           public int MenkeStarttime {get;set;}

           /// <summary>
           /// Desc:录制件时长（毫秒）
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_duration")]
           public int MenkeDuration {get;set;}

           /// <summary>
           /// Desc:录制件大小（字节）
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_size")]
           public long MenkeSize {get;set;}

           /// <summary>
           /// Desc:录制件类型 0常规、5MP4、6微录课mp4
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_type")]
           public string MenkeType {get;set;}

           /// <summary>
           /// Desc:回放地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menke_url")]
           public string MenkeUrl {get;set;}

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
