using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("web_online_category")]
    public partial class WebOnlineCategory
    {
           public WebOnlineCategory(){

            this.Fid =0L;
            this.Depth =1;
            this.Rootid =0L;
            this.Status =1;
            this.Dtime =DateTime.Now;
            this.Sort =100;
            this.Sendtime =DateTime.Now;
            this.Isadmin =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="online_categoryid")]
           public long OnlineCategoryid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="fid")]
           public long Fid {get;set;}

           /// <summary>
           /// Desc:纵深级别，默认1级
           /// Default:1
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="depth")]
           public int Depth {get;set;}

           /// <summary>
           /// Desc:纵深ID路径
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="path")]
           public string Path {get;set;}

           /// <summary>
           /// Desc:根菜单ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="rootid")]
           public long Rootid {get;set;}

           /// <summary>
           /// Desc:0未启用，1启用
           /// Default:1
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:同级的排序
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
           /// Desc:1锁定分类，0开放分类
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="isadmin")]
           public int Isadmin {get;set;}

           /// <summary>
           /// Desc:图
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="img")]
           public string Img {get;set;}

    }
}
