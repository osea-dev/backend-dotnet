using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///VIEW
    ///</summary>
    [SugarTable("view_pub_config")]
    public partial class ViewPubConfig
    {
           public ViewPubConfig(){

            this.PubConfigid =0L;
            this.Status =1;
            this.Isadmin =false;
            this.Islang =0;

           }
           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="pub_configid")]
           public long PubConfigid {get;set;}

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
           /// Default:0000-00-00 00:00:00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:字段标题
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="title")]
           public string Title {get;set;}

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
           /// Desc:语言标识
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lang")]
           public string Lang {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="val")]
           public string Val {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="longval")]
           public string Longval {get;set;}

    }
}
