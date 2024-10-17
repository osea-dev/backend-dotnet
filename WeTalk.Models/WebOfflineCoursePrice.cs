using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///线下课程价格表
    ///</summary>
    [SugarTable("web_offline_course_price")]
    public partial class WebOfflineCoursePrice
    {
           public WebOfflineCoursePrice(){

            this.OfflineCourseid =0L;
            this.PubCurrencyid =0L;
            this.MarketPrice =0.00M;
            this.Price =0.00M;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="offline_course_priceid")]
           public long OfflineCoursePriceid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="offline_courseid")]
           public long OfflineCourseid {get;set;}

           /// <summary>
           /// Desc:所属货币ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="pub_currencyid")]
           public long PubCurrencyid {get;set;}

           /// <summary>
           /// Desc:市场价
           /// Default:0.00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="market_price")]
           public decimal MarketPrice {get;set;}

           /// <summary>
           /// Desc:价格
           /// Default:0.00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="price")]
           public decimal Price {get;set;}

           /// <summary>
           /// Desc:国家代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="country_code")]
           public string CountryCode {get;set;}

           /// <summary>
           /// Desc:货币代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="currency_code")]
           public string CurrencyCode {get;set;}

    }
}
