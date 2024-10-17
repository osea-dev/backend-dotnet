using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("web_business")]
    public partial class WebBusiness
    {
           public WebBusiness(){

            this.Timezoneid =0L;
            this.ContactSmsTime =DateTime.Now;
            this.ContactEmailTime =0;
            this.Dtime =0;
            this.Lasttime =0;
            this.Status =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="businessid")]
           public long Businessid {get;set;}

           /// <summary>
           /// Desc:商户类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="type")]
           public string Type {get;set;}

           /// <summary>
           /// Desc:营业执照
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="license")]
           public string License {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="reg_address")]
           public string RegAddress {get;set;}

           /// <summary>
           /// Desc:credit code
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="credit_code")]
           public string CreditCode {get;set;}

           /// <summary>
           /// Desc:时区ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="timezoneid")]
           public long Timezoneid {get;set;}

           /// <summary>
           /// Desc:UTC时区分差（纯记录）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="utc_sec")]
           public int? UtcSec {get;set;}

           /// <summary>
           /// Desc:时区名（纯记录）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="logo")]
           public string Logo {get;set;}

           /// <summary>
           /// Desc:时区名（纯记录）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="utc")]
           public string Utc {get;set;}

           /// <summary>
           /// Desc:公司或组织名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="company_short")]
           public string CompanyShort {get;set;}

           /// <summary>
           /// Desc:公司或组织名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="company")]
           public string Company {get;set;}

           /// <summary>
           /// Desc:开户行
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="bank")]
           public string Bank {get;set;}

           /// <summary>
           /// Desc:银行卡号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="bank_card")]
           public string BankCard {get;set;}

           /// <summary>
           /// Desc:法人
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="legal")]
           public string Legal {get;set;}

           /// <summary>
           /// Desc:手机国家代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="legalt_mobile_code")]
           public string LegaltMobileCode {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="legal_mobile")]
           public string LegalMobile {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="legal_email")]
           public string LegalEmail {get;set;}

           /// <summary>
           /// Desc:联系人
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="contact_name")]
           public string ContactName {get;set;}

           /// <summary>
           /// Desc:手机国家代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="contact_mobile_code")]
           public string ContactMobileCode {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="contact_mobile")]
           public string ContactMobile {get;set;}

           /// <summary>
           /// Desc:短信验证码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="contact_sms_code")]
           public string ContactSmsCode {get;set;}

           /// <summary>
           /// Desc:短信验证码发送时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="contact_sms_time")]
           public DateTime ContactSmsTime {get;set;}

           /// <summary>
           /// Desc: 联系人身份证号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="contact_idcard")]
           public string ContactIdcard {get;set;}

           /// <summary>
           /// Desc: 联系人邮箱
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="contact_email")]
           public string ContactEmail {get;set;}

           /// <summary>
           /// Desc:邮箱验证码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="contact_email_code")]
           public string ContactEmailCode {get;set;}

           /// <summary>
           /// Desc:邮箱验证码发送时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="contact_email_time")]
           public int ContactEmailTime {get;set;}

           /// <summary>
           /// Desc:品牌官网
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="website")]
           public string Website {get;set;}

           /// <summary>
           /// Desc:社交账号,JSON对象
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="sns_account")]
           public string SnsAccount {get;set;}

           /// <summary>
           /// Desc:商户环境,json数组
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="imgs")]
           public string Imgs {get;set;}

           /// <summary>
           /// Desc:商户介绍
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="intro")]
           public string Intro {get;set;}

           /// <summary>
           /// Desc:注册时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public int Dtime {get;set;}

           /// <summary>
           /// Desc:最后登录时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="lasttime")]
           public int Lasttime {get;set;}

           /// <summary>
           /// Desc:最后登录IP
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lastip")]
           public string Lastip {get;set;}

           /// <summary>
           /// Desc:
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
