using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("pub_currency_lang")]
    public partial class PubCurrencyLang
    {
           public PubCurrencyLang(){

            this.PubCurrencyid =0L;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="pub_currency_langid")]
           public long PubCurrencyLangid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lang")]
           public string Lang {get;set;}

           /// <summary>
           /// Desc:六大州
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="states")]
           public string States {get;set;}

           /// <summary>
           /// Desc:国家
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="country")]
           public string Country {get;set;}

           /// <summary>
           /// Desc:币种
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="currency")]
           public string Currency {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="pub_currencyid")]
           public long PubCurrencyid {get;set;}

    }
}
