using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace WeTalk.Models
{
    ///<summary>
    ///付款类型表
    ///</summary>
    [SugarTable("web_paytype")]
    public partial class WebPaytype
    {
           public WebPaytype(){

            this.PayType =0;
            this.Sort =0;
            this.Status =0;
            this.IsScan =0;
            this.IsWeb =0;

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="paytypeid")]
           public long Paytypeid {get;set;}

           /// <summary>
           /// Desc:支付类型代码
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="code")]
           public string Code {get;set;}

           /// <summary>
           /// Desc:支付显示名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="title")]
           public string Title {get;set;}

           /// <summary>
           /// Desc:0线下转款，1微信，2支付宝，3Paypal,4Stripe
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="pay_type")]
           public int PayType {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="appsecret")]
           public string Appsecret {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="appid")]
           public string Appid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="key")]
           public string Key {get;set;}

           /// <summary>
           /// Desc:收款商户号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="mch_id")]
           public string MchId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sort")]
           public int Sort {get;set;}

           /// <summary>
           /// Desc:回调地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="redirecturl")]
           public string Redirecturl {get;set;}

           /// <summary>
           /// Desc:图标
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ico")]
           public string Ico {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="status")]
           public int Status {get;set;}

           /// <summary>
           /// Desc:备注
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="message")]
           public string Message {get;set;}

           /// <summary>
           /// Desc:是否支持扫码支付
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="is_scan")]
           public int IsScan {get;set;}

           /// <summary>
           /// Desc:是否支持网页支付
           /// Default:0
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="is_web")]
           public int IsWeb {get;set;}

           /// <summary>
           /// Desc:支付网关，不填为直接支付
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="pay_gateway")]
           public string PayGateway {get;set;}

    }
}
