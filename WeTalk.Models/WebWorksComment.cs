using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///作品评论表
    ///</summary>
    [SugarTable("web_works_comment")]
    public partial class WebWorksComment
    {
           public WebWorksComment(){

            this.Dtime =0;
            this.Status =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="works_commentid")]
           public long WorksCommentid {get;set;}

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
           /// Desc:引用上级评论ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="fid")]
           public long? Fid {get;set;}

           /// <summary>
           /// Desc:评论人机构ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="comment_businessid")]
           public long? CommentBusinessid {get;set;}

           /// <summary>
           /// Desc:评论人
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="comment_userid")]
           public long? CommentUserid {get;set;}

           /// <summary>
           /// Desc:评论内容
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="message")]
           public string Message {get;set;}

           /// <summary>
           /// Desc:
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
           /// Desc:0未审核，1正常
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
           /// Desc:回复此ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="to_commentid")]
           public long? ToCommentid {get;set;}

    }
}
