using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("web_teacher_lang")]
    public partial class WebTeacherLang
    {
           public WebTeacherLang(){

            this.Dtime =DateTime.Now;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="teacher_langid")]
           public long TeacherLangid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="teacherid")]
           public long Teacherid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lang")]
           public string Lang {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="name")]
           public string Name {get;set;}

           /// <summary>
           /// Desc:标签组
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="keys")]
           public string Keys {get;set;}

           /// <summary>
           /// Desc:简介
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="message")]
           public string Message {get;set;}

           /// <summary>
           /// Desc:来自
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="comefrom")]
           public string Comefrom {get;set;}

           /// <summary>
           /// Desc:教育
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="edu")]
           public string Edu {get;set;}

           /// <summary>
           /// Desc:宗教信仰
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="religion")]
           public string Religion {get;set;}

           /// <summary>
           /// Desc:数字标
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="advantage")]
           public string Advantage {get;set;}

           /// <summary>
           /// Desc:座右铭
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="motto")]
           public string Motto {get;set;}

           /// <summary>
           /// Desc:哲学修养
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="philosophy")]
           public string Philosophy {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

    }
}
