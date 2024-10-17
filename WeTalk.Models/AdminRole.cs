using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("admin_role")]
    public partial class AdminRole
    {
           public AdminRole(){

            this.Sort =100;
            this.Dtime =DateTime.Now;
            this.Status =1;
            this.Isagent =0;
            this.Isadmin =0;
            this.Appletsid =0L;
            this.AppletsCompanyid =0L;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="admin_roleid")]
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
           /// Default:CURRENT_TIMESTAMP
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

    }
}
