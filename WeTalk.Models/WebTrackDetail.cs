using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///行为轨迹明细表
    ///</summary>
    [SugarTable("web_track_detail")]
    public partial class WebTrackDetail
    {
           public WebTrackDetail(){

            this.Dtime =0;
            this.Status =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="track_detailid")]
           public long TrackDetailid {get;set;}

           /// <summary>
           /// Desc:操作者所属机构
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="operator_businessid")]
           public long? OperatorBusinessid {get;set;}

           /// <summary>
           /// Desc:操作者
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="operator_userid")]
           public long? OperatorUserid {get;set;}

           /// <summary>
           /// Desc:被操作者所属机构
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="businessid")]
           public long? Businessid {get;set;}

           /// <summary>
           /// Desc:被操作者
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="userid")]
           public long? Userid {get;set;}

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
           /// Desc:操作类型:访问，播放，分享等
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="type")]
           public string Type {get;set;}

           /// <summary>
           /// Desc:注册时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public int Dtime {get;set;}

           /// <summary>
           /// Desc:0草稿，1正式
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

           /// <summary>
           /// Desc:对象ID，配合table_name用
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="fid")]
           public long? Fid {get;set;}

           /// <summary>
           /// Desc:对象表
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="table_name")]
           public string TableName {get;set;}

    }
}
