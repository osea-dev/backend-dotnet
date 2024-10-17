using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///国家城市表
    ///</summary>
    [SugarTable("pub_currency")]
    public partial class PubCurrency
    {
           public PubCurrency(){

            this.Status =1;
            this.Dtime =DateTime.Now;
            this.Sort =100;
            this.Isuse =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="pub_currencyid")]
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
           /// Default:CURRENT_TIMESTAMP
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

           /// <summary>
           /// Desc:币种符号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ico")]
           public string Ico {get;set;}

    }
}
