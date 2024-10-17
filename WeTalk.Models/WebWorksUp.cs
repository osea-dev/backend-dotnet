using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///作品点赞表
    ///</summary>
    [SugarTable("web_works_up")]
    public partial class WebWorksUp
    {
           public WebWorksUp(){

            this.Dtime =0;
            this.Status =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="works_uptid")]
           public long WorksUptid {get;set;}

           /// <summary>
           /// Desc:作品
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="worksid")]
           public long? Worksid {get;set;}

           /// <summary>
           /// Desc:机构
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="businessid")]
           public long? Businessid {get;set;}

           /// <summary>
           /// Desc:发布者
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="userid")]
           public long? Userid {get;set;}

           /// <summary>
           /// Desc:评论人机构ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="up_businessid")]
           public long? UpBusinessid {get;set;}

           /// <summary>
           /// Desc:评论人
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="up_userid")]
           public long? UpUserid {get;set;}

           /// <summary>
           /// Desc:注册时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public int Dtime {get;set;}

           /// <summary>
           /// Desc:0取消，1正常
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
           [SugarColumn(ColumnName="remark")]
           public string Remark {get;set;}

    }
}
