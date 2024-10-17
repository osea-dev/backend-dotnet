using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///VIEW
    ///</summary>
    [SugarTable("view_pub_currency")]
    public partial class ViewPubCurrency
    {
           public ViewPubCurrency(){

            this.PubCurrencyid =0L;
            this.Status =1;
            this.Sort =100;
            this.Isuse =0;

           }
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

           /// <summary>
           /// Desc:0未启用，1启用
           /// Default:1
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0000-00-00 00:00:00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:同级的排序
           /// Default:100
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sort")]
           public int Sort {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="currency_code")]
           public string CurrencyCode {get;set;}

           /// <summary>
           /// Desc:国旗图标
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="flag")]
           public string Flag {get;set;}

           /// <summary>
           /// Desc:国家代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="country_code")]
           public string CountryCode {get;set;}

           /// <summary>
           /// Desc:是否启用
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="isuse")]
           public int Isuse {get;set;}

    }
}
