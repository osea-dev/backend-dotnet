using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///菜单功能控制项
    ///</summary>
    [SugarTable("admin_menu_function")]
    public partial class AdminMenuFunction
    {
           public AdminMenuFunction(){

            this.AdminMenuid =0L;
            this.Sort =100;
            this.Status =1;
            this.Dtime =DateTime.Now;
            this.Fid =0L;
            this.Depth =1;
            this.Rootid =0L;
            this.Ispage =false;
            this.Isadmin =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="admin_menu_functionid")]
           public long AdminMenuFunctionid {get;set;}

           /// <summary>
           /// Desc:菜单ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="admin_menuid")]
           public long AdminMenuid {get;set;}

           /// <summary>
           /// Desc:对应方法名称，用来判断是否有执行该方法的权限
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="function_name")]
           public string FunctionName {get;set;}

           /// <summary>
           /// Desc:对应控件ID，用来控制是否显示此控件ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="control_id")]
           public string ControlId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:100
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sort")]
           public int Sort {get;set;}

           /// <summary>
           /// Desc:
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
           /// Desc:后台文件路径/不含参数
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="url")]
           public string Url {get;set;}

           /// <summary>
           /// Desc:后台路径的参数
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="parameter")]
           public string Parameter {get;set;}

           /// <summary>
           /// Desc:数据绑定路径
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="data_url")]
           public string DataUrl {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="fid")]
           public long Fid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:1
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="depth")]
           public int Depth {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="path")]
           public string Path {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="rootid")]
           public long Rootid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:b'0'
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="ispage")]
           public bool Ispage {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="isadmin")]
           public int Isadmin {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="intro")]
           public string Intro {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="area")]
           public string Area {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="controller")]
           public string Controller {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="action")]
           public string Action {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="method")]
           public string Method {get;set;}

           /// <summary>
           /// Desc:语言标识字串数组
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="langs")]
           public string Langs {get;set;}

    }
}
