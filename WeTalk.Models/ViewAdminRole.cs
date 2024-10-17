using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///VIEW
    ///</summary>
    [SugarTable("view_admin_role")]
    public partial class ViewAdminRole
    {
           public ViewAdminRole(){

            this.AdminRoleid =0L;
            this.Sort =100;
            this.Status =1;
            this.Isagent =0;
            this.Isadmin =0;
            this.Appletsid =0L;
            this.AppletsCompanyid =0L;

           }
           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="admin_roleid")]
           public long AdminRoleid {get;set;}

           /// <summary>
           /// Desc:菜单ID集合
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menuids")]
           public string Menuids {get;set;}

           /// <summary>
           /// Desc:排序
           /// Default:100
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sort")]
           public int Sort {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0000-00-00 00:00:00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:状态，0未启用，1启用
           /// Default:1
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:代理层级：1第三方平台，2渠道代理，3普通代理
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="isagent")]
           public int Isagent {get;set;}

           /// <summary>
           /// Desc:是否不可删
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="isadmin")]
           public int Isadmin {get;set;}

           /// <summary>
           /// Desc:所属第三方平台ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="appletsid")]
           public long Appletsid {get;set;}

           /// <summary>
           /// Desc:所属小程序ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="applets_companyid")]
           public long AppletsCompanyid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="remark")]
           public string Remark {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="langs")]
           public string Langs {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lang")]
           public string Lang {get;set;}

           /// <summary>
           /// Desc:角色名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="title")]
           public string Title {get;set;}

    }
}
