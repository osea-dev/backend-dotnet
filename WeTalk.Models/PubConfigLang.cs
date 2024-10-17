using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("pub_config_lang")]
    public partial class PubConfigLang
    {
           public PubConfigLang(){

            this.PubConfigid =0L;
            this.Dtime =DateTime.Now;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="pub_config_langid")]
           public long PubConfigLangid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="pub_configid")]
           public long PubConfigid {get;set;}

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

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:语言标识
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lang")]
           public string Lang {get;set;}

    }
}
