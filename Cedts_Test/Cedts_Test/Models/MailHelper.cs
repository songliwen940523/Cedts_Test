using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.ComponentModel;
using System.Net.Mime;
using System.IO;
using System.Configuration;

namespace Cedts_Test.Models
{
    /// <summary>
    /// 邮件发送类
    /// </summary>
    public class MailHelper
    {
        /// <summary>
        /// 方法：发送邮件
        /// </summary>
        /// <param name="obj">model</param>
        public void SendMail(object obj)
        {
            MailModel model = obj as MailModel;
            MailAddress from = new MailAddress(model.Senderemail, model.Sendername);
            MailAddress to = new MailAddress(model.Receiveremail, model.Receivername);

            MailMessage message = new MailMessage(from, to);

            message.Sender = new MailAddress(model.Senderemail);

            message.Subject ="今日("+ DateTime.Now.ToString("yyyy-MM-dd")+")AEE密码找回邮件提醒";
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            message.Body = model.Sendcontent;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            //Attachment content = new Attachment(@"c:\附件.txt", MediaTypeNames.Text.Plain);
            //message.Attachments.Add(content);

            message.Priority = MailPriority.High;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            SmtpClient client = new SmtpClient();
            client.Host = ConfigurationManager.AppSettings["SMTPSERVER"];
            client.Port = Int32.Parse(ConfigurationManager.AppSettings["SMTPSERVERPORT"]);
            
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MAILUSER"]))
            {
                client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MAILUSER"], ConfigurationManager.AppSettings["MAILUSERPW"]);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
            }
            else
            {
                client.Credentials = CredentialCache.DefaultNetworkCredentials;
            }
            client.Send(message);
            //client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            //client.SendAsync(message,"send");           
            message.Dispose();
        }
    }

    /// <summary>
    /// 邮件发送实体类
    /// </summary>
    public class MailModel
    {
        private string _sendername;
        /// <summary>
        /// 发送人姓名
        /// </summary>
        public string Sendername
        {
            get { return _sendername; }
            set { _sendername = value; }
        }

        private string _senderemail;
        /// <summary>
        /// 发送人邮件地址
        /// </summary>
        public string Senderemail
        {
            get { return _senderemail; }
            set { _senderemail = value; }
        }

        private string _receivername;
        /// <summary>
        /// 接收人姓名
        /// </summary>
        public string Receivername
        {
            get { return _receivername; }
            set { _receivername = value; }
        }

        private string _receiveremail;
        /// <summary>
        /// 接收人邮件地址
        /// </summary>
        public string Receiveremail
        {
            get { return _receiveremail; }
            set { _receiveremail = value; }
        }

        private string _sendcontent;
        /// <summary>
        /// 发送内容
        /// </summary>
        public string Sendcontent
        {
            get { return _sendcontent; }
            set { _sendcontent = value; }
        }
    }
}
