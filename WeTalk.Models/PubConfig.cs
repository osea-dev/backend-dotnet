using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("pub_config")]
    public partial class PubConfig
    {
           public PubConfig(){

            this.Dtime =DateTime.Now;
            this.Status =1;
            this.Isadmin =false;
            this.Islang =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="pub_configid")]
           public long PubConfigid {get;set;}

           /// <summary>
           /// Desc:字段标题
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="title")]
           public string Title {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="key")]
           public string Key {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="description")]
           public string Description {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:-1删除，1正常，0冻结
           /// Default:1
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:是否系统级参数(不可删)
           /// Default:b'0'
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="isadmin")]
           public bool Isadmin {get;set;}

           /// <summary>
           /// Desc:是否启用多语言，0取本表，1取语言表
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="islang")]
           public int Islang {get;set;}

           /// <summary>
           /// Desc:功能项名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="val")]
           public string Val {get;set;}

           /// <summary>
           /// Desc:功能项名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="longval")]
           public string Longval {get;set;}

    }
}
