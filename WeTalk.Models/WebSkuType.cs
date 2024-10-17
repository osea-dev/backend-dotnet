using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///课程SKU类型含年龄限制
    ///</summary>
    [SugarTable("web_sku_type")]
    public partial class WebSkuType
    {
           public WebSkuType(){

            this.AgeMin =0;
            this.AgeMax =0;
            this.Status =0;
            this.Dtime =DateTime.Now;
            this.Sort =100;
            this.MenkeTemplateId =0;
            this.MenkeType =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="sku_typeid")]
           public long SkuTypeid {get;set;}

           /// <summary>
           /// Desc:图标字
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ico_font")]
           public string IcoFont {get;set;}

           /// <summary>
           /// Desc:限制年龄最小值，0不限
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="age_min")]
           public int AgeMin {get;set;}

           /// <summary>
           /// Desc:限制年龄最大值，0不限
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="age_max")]
           public int AgeMax {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime? Dtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:100
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sort")]
           public int Sort {get;set;}

           /// <summary>
           /// Desc:对应上课教室ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_template_id")]
           public int MenkeTemplateId {get;set;}

           /// <summary>
           /// Desc:教室类型教室类型（一对一0、一对多3、大直播4）
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_type")]
           public int MenkeType {get;set;}

    }
}
