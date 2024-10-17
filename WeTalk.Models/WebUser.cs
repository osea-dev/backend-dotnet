using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///用户表
    ///</summary>
    [SugarTable("web_user")]
    public partial class WebUser
    {
           public WebUser(){

            this.Email ="";
            this.Gender =0;
            this.Regtime =DateTime.Now;
            this.Birthdate =DateTime.Now;
            this.Timezoneid =0L;
            this.Dtime =DateTime.Now;
            this.Lasttime =DateTime.Now;
            this.Status =0;
            this.UtcSec =0;
            this.Updatetime =DateTime.Now;
            this.Expiresin =7200;
            this.MenkeUserId =0;
            this.MobileSmstime =DateTime.Now;
            this.EmailCodetime =DateTime.Now;
            this.IsTicket =0;
            this.TicketTime =DateTime.Now;
            this.Type =0;
            this.Bussinessid =0L;
            this.MessageTime =0L;
            this.MessageUpTime =0L;
            this.MessageFavTime =0L;
            this.MessageNoticeTime =0L;
            this.MessageSysTime =0L;
            this.MessageCommentTime =0L;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="userid")]
           public long Userid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="username")]
           public string Username {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="userpwd")]
           public string Userpwd {get;set;}

           /// <summary>
           /// Desc:姓
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="first_name")]
           public string FirstName {get;set;}

           /// <summary>
           /// Desc:名
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="last_name")]
           public string LastName {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="email")]
           public string Email {get;set;}

           /// <summary>
           /// Desc:电话区号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="mobile_code")]
           public string MobileCode {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="mobile")]
           public string Mobile {get;set;}

           /// <summary>
           /// Desc:手机所属国家代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="mobile_country")]
           public string MobileCountry {get;set;}

           /// <summary>
           /// Desc:性别:0女，1男
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="gender")]
           public int Gender {get;set;}

           /// <summary>
           /// Desc:注册时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="regtime")]
           public DateTime Regtime {get;set;}

           /// <summary>
           /// Desc:生日
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="birthdate")]
           public DateTime Birthdate {get;set;}

           /// <summary>
           /// Desc:出生地，国家代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="native")]
           public string Native {get;set;}

           /// <summary>
           /// Desc:居住地时区ID
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="timezoneid")]
           public long Timezoneid {get;set;}

           /// <summary>
           /// Desc:教育
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="education")]
           public string Education {get;set;}

           /// <summary>
           /// Desc:头像
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="head_img")]
           public string HeadImg {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="facebook_id")]
           public string FacebookId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="google_id")]
           public int? GoogleId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="wechat_id")]
           public int? WechatId {get;set;}

           /// <summary>
           /// Desc:注册时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="dtime")]
           public DateTime Dtime {get;set;}

           /// <summary>
           /// Desc:最后登录时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="lasttime")]
           public DateTime Lasttime {get;set;}

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

           /// <summary>
           /// Desc:最后切换展示货币代码（纯记录）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="currency_code")]
           public string CurrencyCode {get;set;}

           /// <summary>
           /// Desc:最后使用语言（纯记录）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="lang")]
           public string Lang {get;set;}

           /// <summary>
           /// Desc:UTC时区秒差（纯记录）
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="utc_sec")]
           public int UtcSec {get;set;}

           /// <summary>
           /// Desc:时区名（纯记录）
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="utc")]
           public string Utc {get;set;}

           /// <summary>
           /// Desc:token更新时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="updatetime")]
           public DateTime Updatetime {get;set;}

           /// <summary>
           /// Desc:token有效时长
           /// Default:7200
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="expiresin")]
           public int Expiresin {get;set;}

           /// <summary>
           /// Desc:会员TOKEN
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="token")]
           public string Token {get;set;}

           /// <summary>
           /// Desc:手机验证码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="mobile_smscode")]
           public string MobileSmscode {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="menke_user_id")]
           public int MenkeUserId {get;set;}

           /// <summary>
           /// Desc:手机验证码最后发出时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="mobile_smstime")]
           public DateTime MobileSmstime {get;set;}

           /// <summary>
           /// Desc:邮箱临时授权码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="email_code")]
           public string EmailCode {get;set;}

           /// <summary>
           /// Desc:邮箱临时授权码最后发出时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="email_codetime")]
           public DateTime EmailCodetime {get;set;}

           /// <summary>
           /// Desc:监护人姓名
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="guardian_name")]
           public string GuardianName {get;set;}

           /// <summary>
           /// Desc:监护人手机区号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="guardian_mobile_code")]
           public string GuardianMobileCode {get;set;}

           /// <summary>
           /// Desc:监护人手机
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="guardian_mobile")]
           public string GuardianMobile {get;set;}

           /// <summary>
           /// Desc:监护人关系
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="guardianship_fee")]
           public string GuardianshipFee {get;set;}

           /// <summary>
           /// Desc:注册提醒，是否创建工单
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="is_ticket")]
           public int IsTicket {get;set;}

           /// <summary>
           /// Desc:创建注册提醒工单时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="ticket_time")]
           public DateTime TicketTime {get;set;}

           /// <summary>
           /// Desc:注册提醒，工单ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ticket_id")]
           public string TicketId {get;set;}

           /// <summary>
           /// Desc:会员类型:0学生，1普通，2老师
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="type")]
           public int Type {get;set;}

           /// <summary>
           /// Desc:所属机构
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="bussinessid")]
           public long Bussinessid {get;set;}

           /// <summary>
           /// Desc:最后阅读私信时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="message_time")]
           public long MessageTime {get;set;}

           /// <summary>
           /// Desc:点赞消息最后阅读时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="message_up_time")]
           public long MessageUpTime {get;set;}

           /// <summary>
           /// Desc:收藏消息最后阅读时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="message_fav_time")]
           public long MessageFavTime {get;set;}

           /// <summary>
           /// Desc:公告消息最后阅读时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="message_notice_time")]
           public long MessageNoticeTime {get;set;}

           /// <summary>
           /// Desc:系统消息最后阅读时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="message_sys_time")]
           public long MessageSysTime {get;set;}

           /// <summary>
           /// Desc:评论消息最后阅读时间
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="message_comment_time")]
           public long MessageCommentTime {get;set;}

    }
}
