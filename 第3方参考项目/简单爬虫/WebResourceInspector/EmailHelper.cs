using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Threading;

namespace WebResourceInspector
{
    /// <summary>
    /// 邮件帮助类
    /// </summary>
    public class EmailHelper
    {
        #region 私有对象

        private SmtpClient smtpClient = null;   //设置SMTP协议
        private MailAddress mailAddressFrom = null; //设置发信人地址  当然还需要密码
        private MailMessage mailMessage = null;
        private string filePath = null;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public EmailHelper()
        {
            try
            {
                smtpClient = new SmtpClient();
                //指定SMTP服务名  例如QQ邮箱为 smtp.qq.com 新浪cn邮箱为 smtp.sina.cn等
                smtpClient.Host = ConfigurationManager.AppSettings["SmtpServerHost"];
                //指定端口号
                smtpClient.Port = int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]);
                //超时时间
                smtpClient.Timeout = int.Parse(ConfigurationManager.AppSettings["Timeout"]);
                //附件地址
                filePath = ConfigurationManager.AppSettings["EmailFilePath"];
                //邮件对象
                mailMessage = new MailMessage();
                //创建服务器认证
                string sendEmailAddress = ConfigurationManager.AppSettings["SendEmailAddress"];
                string sendEmailPassword = ConfigurationManager.AppSettings["SendEmailPassword"];
                string displayEmailName = ConfigurationManager.AppSettings["DisplayEmailName"];
                NetworkCredential networkCredential = new NetworkCredential(sendEmailAddress, displayEmailName);
                //实例化发件人地址
                mailAddressFrom = new System.Net.Mail.MailAddress(sendEmailAddress, sendEmailAddress);
                //指定发件人信息  包括邮箱地址和邮箱密码
                smtpClient.Credentials = new System.Net.NetworkCredential(mailAddressFrom.Address, sendEmailPassword);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        public void Send()
        {
            try
            {
                if (mailMessage != null)
                {
                    mailMessage.To.Clear();
                }
                string[] targetEmail = ConfigurationManager.AppSettings["TargetEmailAddress"].Split(',');

                if (targetEmail.Length > 0)
                {
                    foreach (var email in targetEmail)
                    {
                        mailMessage.To.Add(new MailAddress(email));
                    }
                    //发件人邮箱
                    mailMessage.From = mailAddressFrom;
                    //邮件主题
                    mailMessage.Subject = "网站地址可访问性验证结果";
                    mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                    //邮件正文
                    mailMessage.Body = "详细信息请看附件！";
                    mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                    //清空历史附件  以防附件重复发送
                    mailMessage.Attachments.Clear();
                    //添加附件
                    mailMessage.Attachments.Add(new Attachment(filePath, MediaTypeNames.Application.Octet));
                    //开始发送邮件
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
