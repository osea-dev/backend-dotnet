using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///众语课程表
    ///</summary>
    [SugarTable("web_course")]
    public partial class WebCourse
    {
           public WebCourse(){

            this.Dtime =DateTime.Now;
            this.Status =0;
            this.Sort =100;
            this.Sendtime =DateTime.Now;
            this.Masterid =0L;
            this.Hits =0;
            this.Lasttime =DateTime.Now;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="courseid")]
           public long Courseid {get;set;}

           /// <summary>
           /// Desc:标签组
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="keysids")]
           public string Keysids {get;set;}

           /// <summary>
           /// Desc:缩略图
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="img")]
           public string Img {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime? Dtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:
           /// Default:100
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sort")]
           public int Sort {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sendtime")]
           public DateTime Sendtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="masterid")]
           public long Masterid {get;set;}

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
           [SugarColumn(ColumnName="hits")]
           public int Hits {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lasttime")]
           public DateTime? Lasttime {get;set;}

           /// <summary>
           /// Desc:最后访问IP
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ip")]
           public string Ip {get;set;}

           /// <summary>
           /// Desc:缩略图
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="banner_h5")]
           public string BannerH5 {get;set;}

           /// <summary>
           /// Desc:缩略图
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="banner")]
           public string Banner {get;set;}

    }
}
