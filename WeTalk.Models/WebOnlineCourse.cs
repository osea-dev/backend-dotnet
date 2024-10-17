using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///在线直播课
    ///</summary>
    [SugarTable("web_online_course")]
    public partial class WebOnlineCourse
    {
           public WebOnlineCourse(){

            this.OnlineCategoryid =0L;
            this.Type =0;
            this.Dtime =DateTime.Now;
            this.Status =0;
            this.Sort =100;
            this.Sendtime =DateTime.Now;
            this.Masterid =0L;
            this.Hits =0;
            this.Lasttime =DateTime.Now;
            this.MarketPrice =0.00M;
            this.Price =0.00M;
            this.Teacherid =0L;
            this.LessonCount =0;
            this.StudentCount =0;
            this.Recommend =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="online_courseid")]
           public long OnlineCourseid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="online_categoryid")]
           public long OnlineCategoryid {get;set;}

           /// <summary>
           /// Desc:课程类型1录播，2直播，3视频，4线下
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="type")]
           public int Type {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="img")]
           public string Img {get;set;}

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
           /// Desc:
           /// Default:100
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sort")]
           public int Sort {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sendtime")]
           public DateTime Sendtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="masterid")]
           public long Masterid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="remark")]
           public string Remark {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="hits")]
           public int Hits {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="lasttime")]
           public DateTime Lasttime {get;set;}

           /// <summary>
           /// Desc:最后访问IP
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ip")]
           public string Ip {get;set;}

           /// <summary>
           /// Desc:缩略图
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="banner_h5")]
           public string BannerH5 {get;set;}

           /// <summary>
           /// Desc:缩略图
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="banner")]
           public string Banner {get;set;}

           /// <summary>
           /// Desc:默认原价，美元
           /// Default:0.00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="market_price")]
           public decimal MarketPrice {get;set;}

           /// <summary>
           /// Desc:默认价格，美元
           /// Default:0.00
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="price")]
           public decimal Price {get;set;}

           /// <summary>
           /// Desc:关联老师
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="teacherid")]
           public long Teacherid {get;set;}

           /// <summary>
           /// Desc:课节数
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="lesson_count")]
           public int LessonCount {get;set;}

           /// <summary>
           /// Desc:上课时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lesson_start")]
           public string LessonStart {get;set;}

           /// <summary>
           /// Desc:学习人次
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="student_count")]
           public int StudentCount {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="recommend")]
           public int Recommend {get;set;}

    }
}
