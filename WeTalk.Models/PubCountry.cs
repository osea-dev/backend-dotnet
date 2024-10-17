using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("pub_country")]
    public partial class PubCountry
    {
           public PubCountry(){

            this.Number =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="countryid")]
           public long Countryid {get;set;}

           /// <summary>
           /// Desc:iso_3166_1国家代码（2位）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="code")]
           public string Code {get;set;}

           /// <summary>
           /// Desc:国家代码（3位）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="code_3")]
           public string Code3 {get;set;}

           /// <summary>
           /// Desc:国家代码（数字）
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="number")]
           public int Number {get;set;}

           /// <summary>
           /// Desc:iso_3166_2标准2位代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="iso_3166_2_code")]
           public string Iso31662Code {get;set;}

           /// <summary>
           /// Desc:国家名（英文）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="country_en")]
           public string CountryEn {get;set;}

           /// <summary>
           /// Desc:国家名
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="country")]
           public string Country {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="country_hk")]
           public string CountryHk {get;set;}

           /// <summary>
           /// Desc:国家名（台湾常用）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="country_tw")]
           public string CountryTw {get;set;}

           /// <summary>
           /// Desc:国家名（香港常用）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="notes")]
           public string Notes {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="mobile_code")]
           public string MobileCode {get;set;}

    }
}
