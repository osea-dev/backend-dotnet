using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///标签库
    ///</summary>
    [SugarTable("pub_keys")]
    public partial class PubKeys
    {
           public PubKeys(){

            this.Sty =0;
            this.Sort =100;
            this.Status =0;
            this.Hits =0;
            this.Dtime =DateTime.Now;
            this.Lasttime =DateTime.Now;
            this.WorksCategoryid =0L;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="keysid")]
           public long Keysid {get;set;}

           /// <summary>
           /// Desc:图标字
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ico_font")]
           public string IcoFont {get;set;}

           /// <summary>
           /// Desc:0关键词，1课程标签，2课节取消原因，3订单取消原因，4短视频标签，5录播课标签，6直播课标签，7线下课标签
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sty")]
           public int Sty {get;set;}

           /// <summary>
           /// Desc:
           /// Default:100
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sort")]
           public int Sort {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

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
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="lasttime")]
           public DateTime Lasttime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ip")]
           public string Ip {get;set;}

           /// <summary>
           /// Desc:作品分类ID，配合sty=4时使用
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="works_categoryid")]
           public long WorksCategoryid {get;set;}

    }
}
