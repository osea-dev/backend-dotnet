using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///作品播放记录表
    ///</summary>
    [SugarTable("web_works_pay")]
    public partial class WebWorksPay
    {
           public WebWorksPay(){

            this.Dtime =0;
            this.Status =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="works_payid")]
           public long WorksPayid {get;set;}

           /// <summary>
           /// Desc:作品
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="worksid")]
           public long? Worksid {get;set;}

           /// <summary>
           /// Desc:机构
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="businessid")]
           public long? Businessid {get;set;}

           /// <summary>
           /// Desc:发布者
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="userid")]
           public long? Userid {get;set;}

           /// <summary>
           /// Desc:所属人的机构
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="pay_businessid")]
           public long? PayBusinessid {get;set;}

           /// <summary>
           /// Desc:所属人
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="pay_userid")]
           public long? PayUserid {get;set;}

           /// <summary>
           /// Desc:来源机构
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="from_businessid")]
           public long? FromBusinessid {get;set;}

           /// <summary>
           /// Desc:来源人，推荐人
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="from_userid")]
           public long? FromUserid {get;set;}

           /// <summary>
           /// Desc:来源类型:如邮件，微信分享等
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="from_type")]
           public string FromType {get;set;}

           /// <summary>
           /// Desc:注册时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public int Dtime {get;set;}

           /// <summary>
           /// Desc:0取消，1正常
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="remark")]
           public string Remark {get;set;}

    }
}
