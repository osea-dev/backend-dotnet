using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("tmp_time_zone")]
    public partial class TmpTimeZone
    {
           public TmpTimeZone(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="zone_name")]
           public string ZoneName {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="country_code")]
           public string CountryCode {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="abbreviation")]
           public string Abbreviation {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="time_start")]
           public decimal TimeStart {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="gmt_offset")]
           public int GmtOffset {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dst")]
           public string Dst {get;set;}

    }
}
