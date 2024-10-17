using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///上传文件管理表
    ///</summary>
    [SugarTable("web_file")]
    public partial class WebFile
    {
           public WebFile(){

            this.Dtime =DateTime.Now;
            this.Status =1;
            this.Width =0;
            this.Height =0;
            this.Size =0;
            this.Fid =0L;
            this.Userid =0L;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="fileid")]
           public long Fileid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="fileurl")]
           public string Fileurl {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:1
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="width")]
           public double Width {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="height")]
           public double Height {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="size")]
           public double Size {get;set;}

           /// <summary>
           /// Desc:同table_name字段共用，对应ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="fid")]
           public long Fid {get;set;}

           /// <summary>
           /// Desc:对应ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="table_name")]
           public string TableName {get;set;}

           /// <summary>
           /// Desc:备注说明
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
           [SugarColumn(ColumnName="userid")]
           public long Userid {get;set;}

           /// <summary>
           /// Desc:视频首图
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="img")]
           public string Img {get;set;}

           /// <summary>
           /// Desc:云存储类型，如qiniu
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="cloud_type")]
           public string CloudType {get;set;}

           /// <summary>
           /// Desc:子文件，相关文件
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="fileurl_sub")]
           public string FileurlSub {get;set;}

    }
}
