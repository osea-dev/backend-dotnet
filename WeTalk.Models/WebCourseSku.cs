using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///课程SKU表
    ///</summary>
    [SugarTable("web_course_sku")]
    public partial class WebCourseSku
    {
           public WebCourseSku(){

            this.Courseid =0L;
            this.SkuTypeid =0L;
            this.ClassHour =0;
            this.MenkeCourseId =0;
            this.Dtime =DateTime.Now;
            this.Status =0;
            this.Sort =0;
            this.Price =0.00M;
            this.MarketPrice =0.00M;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="course_skuid")]
           public long CourseSkuid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="courseid")]
           public long Courseid {get;set;}

           /// <summary>
           /// Desc:图标字
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ico_font")]
           public string IcoFont {get;set;}

           /// <summary>
           /// Desc:SKU名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="sku_name")]
           public string SkuName {get;set;}

           /// <summary>
           /// Desc:上课类型ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sku_typeid")]
           public long SkuTypeid {get;set;}

           /// <summary>
           /// Desc:课时数
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="class_hour")]
           public int ClassHour {get;set;}

           /// <summary>
           /// Desc:关联拓课云课程ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_course_id")]
           public int MenkeCourseId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime? Dtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:排序字段
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sort")]
           public int Sort {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="remark")]
           public string Remark {get;set;}

           /// <summary>
           /// Desc:默认价格，美元
           /// Default:0.00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="price")]
           public decimal Price {get;set;}

           /// <summary>
           /// Desc:默认原价，美元
           /// Default:0.00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="market_price")]
           public decimal MarketPrice {get;set;}

    }
}
