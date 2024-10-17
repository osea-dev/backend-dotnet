using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///老师表
    ///</summary>
    [SugarTable("web_teacher")]
    public partial class WebTeacher
    {
           public WebTeacher(){

            this.TeacherCategoryid =0L;
            this.Status =0;
            this.Sort =100;
            this.Sendtime =DateTime.Now;
            this.Masterid =0L;
            this.Likes =0;
            this.Dtime =DateTime.Now;
            this.MenkeUserId =0;
            this.Gender =0;
            this.Birthdate =DateTime.Now;
            this.Sty =0;
            this.Timezoneid =0L;
            this.TeacherUtcSec =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="teacherid")]
           public long Teacherid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="teacher_categoryid")]
           public long TeacherCategoryid {get;set;}

           /// <summary>
           /// Desc:缩略图
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="img")]
           public string Img {get;set;}

           /// <summary>
           /// Desc:照片
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="photo")]
           public string Photo {get;set;}

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
           /// Desc:点赞数
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="likes")]
           public int Likes {get;set;}

           /// <summary>
           /// Desc:
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="mobile")]
           public string Mobile {get;set;}

           /// <summary>
           /// Desc:国家区号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="mobile_code")]
           public string MobileCode {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_user_id")]
           public int MenkeUserId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="email")]
           public string Email {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="country_code")]
           public string CountryCode {get;set;}

           /// <summary>
           /// Desc:头像
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="head_img")]
           public string HeadImg {get;set;}

           /// <summary>
           /// Desc:性别:0女，1男
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="gender")]
           public int Gender {get;set;}

           /// <summary>
           /// Desc:生日
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="birthdate")]
           public DateTime Birthdate {get;set;}

           /// <summary>
           /// Desc:0前台展示老师，1不在前台展示老师
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sty")]
           public int Sty {get;set;}

           /// <summary>
           /// Desc:老师习惯用的语言
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="teacher_lang")]
           public string TeacherLang {get;set;}

           /// <summary>
           /// Desc:老师所在时区ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="timezoneid")]
           public long Timezoneid {get;set;}

           /// <summary>
           /// Desc:老师常驻地时区
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="teacher_utc")]
           public string TeacherUtc {get;set;}

           /// <summary>
           /// Desc:时区分差
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="teacher_utc_sec")]
           public int TeacherUtcSec {get;set;}

    }
}
