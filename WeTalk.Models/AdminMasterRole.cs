using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("admin_master_role")]
    public partial class AdminMasterRole
    {
           public AdminMasterRole(){

            this.AdminMasterid =0L;
            this.AdminRoleid =0L;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="admin_master_roleid")]
           public long AdminMasterRoleid {get;set;}

           /// <summary>
           /// Desc:管理用户ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="admin_masterid")]
           public long AdminMasterid {get;set;}

           /// <summary>
           /// Desc:角色ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="admin_roleid")]
           public long AdminRoleid {get;set;}

    }
}
