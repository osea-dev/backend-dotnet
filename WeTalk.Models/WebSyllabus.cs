using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///课程大纲表
    ///</summary>
    [SugarTable("web_syllabus")]
    public partial class WebSyllabus
    {
           public WebSyllabus(){

            this.Courseid =0L;
            this.Title ="";
            this.Dtime =DateTime.Now;
            this.Sort =100;
            this.Sendtime =DateTime.Now;
            this.Status =0;
            this.Fid =0L;
            this.Depth =1;
            this.Rootid =0L;
            this.Tryout =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="syllabusid")]
           public long Syllabusid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="courseid")]
           public long Courseid {get;set;}

           /// <summary>
           /// Desc:标题
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="title")]
           public string Title {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

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
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="fid")]
           public long Fid {get;set;}

           /// <summary>
           /// Desc:纵深级别，默认1级
           /// Default:1
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="depth")]
           public int Depth {get;set;}

           /// <summary>
           /// Desc:纵深ID路径
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="path")]
           public string Path {get;set;}

           /// <summary>
           /// Desc:根菜单ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="rootid")]
           public long Rootid {get;set;}

           /// <summary>
           /// Desc:试听
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="tryout")]
           public int Tryout {get;set;}

           /// <summary>
           /// Desc:视频播放时长
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="time")]
           public string Time {get;set;}

           /// <summary>
           /// Desc:视频链接
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="link")]
           public string Link {get;set;}

    }
}
