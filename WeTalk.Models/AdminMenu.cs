using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///后台菜单表
    ///</summary>
    [SugarTable("admin_menu")]
    public partial class AdminMenu
    {
           public AdminMenu(){

            this.Fid =0L;
            this.Depth =1;
            this.Isadmin =false;
            this.Rootid =0L;
            this.Status =1;
            this.Dtime =DateTime.Now;
            this.Sort =100;
            this.Sty =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="admin_menuid")]
           public long AdminMenuid {get;set;}

           /// <summary>
           /// Desc:父菜单ID
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
           /// Desc:
           /// Default:b'0'
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="isadmin")]
           public bool Isadmin {get;set;}

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
           /// Desc:同级菜单的排序
           /// Default:100
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sort")]
           public int Sort {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ico")]
           public string Ico {get;set;}

           /// <summary>
           /// Desc:0开发者专用，1普通
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sty")]
           public int Sty {get;set;}

           /// <summary>
           /// Desc:语言标识字串数组
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="langs")]
           public string Langs {get;set;}

    }
}
