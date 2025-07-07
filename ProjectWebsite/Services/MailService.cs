using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProjectWebsite.Services
{
    public class MailService
    {
        private readonly IConfiguration _config;

        public MailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var mailSettings = _config.GetSection("MailSettings");
            var fromEmail = mailSettings["Mail"];
            var password = mailSettings["Password"];
            var displayName = mailSettings["DisplayName"];
            var host = mailSettings["Host"];
            var port = int.Parse(mailSettings["Port"]);

            var fromAddress = new MailAddress(fromEmail, displayName);
            var toAddress = new MailAddress(toEmail);

            var smtp = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, password)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true // Cho phép gửi mail dạng HTML
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
    }
}