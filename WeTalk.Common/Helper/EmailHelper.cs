using System;
using System.Linq;
using System.Net.Mail;

namespace WeTalk.Common.Helper
{
	public class EmailHelper
	{

		#region 发送邮件
		/// <summary>
		/// 发送邮件
		/// </summary>
		/// <param name="Subject">标题</param>
		/// <param name="body">内容</param>
		/// <param name="FromName">发件人名称</param>
		/// <param name="FromEmail">发件人地址</param>
		/// <param name="ToEmail">收件人地址</param>
		/// <param name="ToEmailCC">抄送人地址</param>
		/// <param name="StmpServer">发送方服务器</param>
		/// <param name="MailServerUsername">发送方邮件</param>
		/// <param name="MailServerPassword">发送方密码</param>
		public static (bool isok, int errcode,string msg) Sentmail(string Subject, string body, string FromName, string FromEmail, string ToEmail, string ToEmailCC, string StmpServer, string MailServerUsername, string MailServerPassword)
		{
            if (ToEmail.Trim().Length > 5)
			{
				if (ToEmail.Contains(";")) {
					var arr = ToEmail.Split(';');
					if (arr.Length > 0) ToEmail = arr[0];
					if (arr.Length > 1) {
						if (ToEmailCC.Length > 5)
						{
							ToEmailCC += (";" + string.Join(";", arr));
						}
						else
                        {
                            ToEmailCC = string.Join(";", arr);
                        }
					}
				}

				DateTime t = DateTime.Now;
				int Port = 0;
				if (StmpServer.Contains(":")) {
					var arr = StmpServer.Split(":");
                    StmpServer=arr[0];
					Port = int.Parse(arr[1]);
                }

				SmtpClient client = new SmtpClient();
				client.Host = StmpServer;// "smtp.163.com";
				if(Port>0) client.Port = Port;
                client.UseDefaultCredentials = false;
				client.Credentials = new System.Net.NetworkCredential(MailServerUsername, MailServerPassword);
				client.DeliveryMethod = SmtpDeliveryMethod.Network;
				client.EnableSsl= true;
				client.Timeout = 20000;//毫秒单位
                MailMessage message = new MailMessage(FromEmail, ToEmail);
				if (ToEmailCC.Trim().Length > 0)
				{
					string[] strArray = ToEmailCC.Trim().Split(';');
					foreach (string str in strArray)
					{
						message.CC.Add(str);
					}
				}
				message.Subject = Subject;
				message.Body = body + t.ToString();
				message.BodyEncoding = System.Text.Encoding.UTF8;
				message.IsBodyHtml = true;
				message.Priority = MailPriority.High;
				try
				{

					client.Send(message);
				}
				catch(System.Net.Mail.SmtpException ex) {
                    
                    var code = ex.StatusCode;
					code = SmtpStatusCode.Ok;
                    return (false, 4002, ex.Message+";错误码:" + code.ToString());
                }
                return (true, 0, "");
            }
			else {
				return (false,4002,"");
			}
		}
		#endregion

	}


}
