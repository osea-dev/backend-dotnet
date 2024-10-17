using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///录播课价格表
    ///</summary>
    [SugarTable("web_record_course_price")]
    public partial class WebRecordCoursePrice
    {
           public WebRecordCoursePrice(){

            this.RecordCourseid =0L;
            this.PubCurrencyid =0L;
            this.MarketPrice =0.00M;
            this.Price =0.00M;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="record_course_priceid")]
           public long RecordCoursePriceid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="record_courseid")]
           public long RecordCourseid {get;set;}

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
