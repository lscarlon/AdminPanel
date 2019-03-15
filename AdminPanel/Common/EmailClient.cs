using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//https://dotnetcoretutorials.com/2017/11/02/using-mailkit-send-receive-email-asp-net-core/

namespace AdminPanel.Common
{
    public class EmailAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class EmailMessage
    {
        public EmailMessage()
        {
            FromAddress = new EmailAddress();
            ToAddresses = new List<EmailAddress>();
            CcAddresses = new List<EmailAddress>();
            BccAddresses = new List<EmailAddress>();
        }
        public EmailAddress FromAddress { get; set; }
        public List<EmailAddress> ToAddresses { get; set; }
        public List<EmailAddress> CcAddresses { get; set; }
        public List<EmailAddress> BccAddresses { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }

    public interface IEmailSettings
    {
        string SmtpServer { get; }
        int SmtpPort { get; }
        bool SmtpSSL { get; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }
    }

    public class EmailSettings : IEmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpSSL { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
    }

    public interface IEmailService
    {
        bool Send(EmailMessage emailMessage, out string response);
    }

    public class EmailService : IEmailService
    {
        private readonly IEmailSettings _emailSettings;

        public EmailService(IEmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public bool Send(EmailMessage emailMessage, out string response)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(emailMessage.FromAddress.Name, emailMessage.FromAddress.Address));
                message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
                message.Cc.AddRange(emailMessage.CcAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
                message.Bcc.AddRange(emailMessage.BccAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

                message.Subject = emailMessage.Subject;

                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = emailMessage.Content
                };

                using (var emailClient = new SmtpClient())
                {
                    emailClient.Connect(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.SmtpSSL);
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                    if (_emailSettings.SmtpUsername != "" && _emailSettings.SmtpPassword != "")
                        emailClient.Authenticate(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                    emailClient.Send(message);
                    emailClient.Disconnect(true);
                }
                response = "Message sent successfully";
                return true;
            }
            catch (Exception ex)
            {
                response = ex.ToString();
                return false;
            }
        }
    }
}
