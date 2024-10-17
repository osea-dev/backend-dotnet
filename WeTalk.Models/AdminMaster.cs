using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("admin_master")]
    public partial class AdminMaster
    {
           public AdminMaster(){

            this.Nums =0;
            this.Lasttime =DateTime.Now;
            this.Dtime =DateTime.Now;
            this.SmsTime =DateTime.Now;
            this.Loginnum =0;
            this.Status =0;
            this.Isadmin =0;
            this.Onlyone =false;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="admin_masterid")]
           public long AdminMasterid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="username")]
           public string Username {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="userpwd")]
           public string Userpwd {get;set;}

           /// <summary>
           /// Desc:登录次数超5次错，限制登录
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="nums")]
           public int Nums {get;set;}

           /// <summary>
           /// Desc:最后一次登录IP
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ip")]
           public string Ip {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="lasttime")]
           public DateTime Lasttime {get;set;}

           /// <summary>
           /// Desc:角色ID集合
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="admin_roleids")]
           public string AdminRoleids {get;set;}

           /// <summary>
           /// Desc:拥有的菜单权限，每次登录时更新存储
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="admin_menuids")]
           public string AdminMenuids {get;set;}

           /// <summary>
           /// Desc:真实姓名
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="name")]
           public string Name {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="email")]
           public string Email {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="mobile")]
           public string Mobile {get;set;}

           /// <summary>
           /// Desc:头像
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="photo")]
           public string Photo {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="sms_code")]
           public string SmsCode {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sms_time")]
           public DateTime SmsTime {get;set;}

           /// <summary>
           /// Desc:登录次数
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="loginnum")]
           public int Loginnum {get;set;}

           /// <summary>
           /// Desc:拥有的菜单控制项权限，每次登录时更新存储
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="admin_menu_functionids")]
           public string AdminMenuFunctionids {get;set;}

           /// <summary>
           /// Desc:0不启用，1启用
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

           /// <summary>
           /// Desc:是否渠道超级管理，不允许删除
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="isadmin")]
           public int Isadmin {get;set;}

           /// <summary>
           /// Desc:cookie授权
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="token")]
           public string Token {get;set;}

           /// <summary>
           /// Desc:是否只允许一人登录
           /// Default:b'0'
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="onlyone")]
           public bool Onlyone {get;set;}

           /// <summary>
           /// Desc:最后一次的语言包
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="last_lang")]
           public string LastLang {get;set;}

    }
}
