using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("web_works_category_lang")]
    public partial class WebWorksCategoryLang
    {
           public WebWorksCategoryLang(){

            this.Type =0;
            this.WorksCategoryid =0L;
            this.Dtime =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="works_category_langid")]
           public long WorksCategoryLangid {get;set;}

           /// <summary>
           /// Desc:类型:0公共，1短视频，2录播课，3电子文档
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="type")]
           public int Type {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="works_categoryid")]
           public long WorksCategoryid {get;set;}

           /// <summary>
           /// Desc:课程分类
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="title")]
           public string Title {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lang")]
           public string Lang {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public int Dtime {get;set;}

    }
}
