using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("admin_role_menu_function")]
    public partial class AdminRoleMenuFunction
    {
           public AdminRoleMenuFunction(){

            this.AdminMenuFunctionid =0L;
            this.AdminRoleid =0L;
            this.AdminMenuid =0L;
            this.Dtime =DateTime.Now;
            this.Status =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="admin_role_menu_functionid")]
           public long AdminRoleMenuFunctionid {get;set;}

           /// <summary>
           /// Desc:菜单功能项ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="admin_menu_functionid")]
           public long AdminMenuFunctionid {get;set;}

           /// <summary>
           /// Desc:角色ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="admin_roleid")]
           public long AdminRoleid {get;set;}

           /// <summary>
           /// Desc:菜单ID，冗余项
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="admin_menuid")]
           public long AdminMenuid {get;set;}

           /// <summary>
           /// Desc:菜单后台路径/不含参数（冗余字段）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="menu_url")]
           public string MenuUrl {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

    }
}
