using GSCrm.Data;
using GSCrm.Models;
using MailKit.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using Task = System.Threading.Tasks.Task;

namespace GSCrm.Notifications.Services
{
    /// <summary>
    /// Отпарвка уведомлений по email
    /// </summary>
    public class EmailNotificationService
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string smtpAddress = "smtp.gmail.com";
        private readonly int smtpPort = 587;
        #endregion

        public EmailNotificationService(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            _configuration = serviceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
        }

        /// <summary>
        /// Метод ассинхронно отправляет уведомление адресату
        /// </summary>
        /// <param name="notification">Сформированное уведомление</param>
        /// <param name="targetUser">Пользователь, которому его необходимо отправить</param>
        public async Task SendAsync(EmailNotification notification, User targetUser)
        {
            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(smtpAddress, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration["SMTPSender"], _configuration["SMTPSenderPassword"]);
                await client.SendAsync(GetMimeMessage(notification, targetUser));
                await client.DisconnectAsync(false);
            }
            catch(Exception ex)
            {
                
            }
        }

        /// <summary>
        /// Метод синхронно отправляет уведомление адресату
        /// </summary>
        /// <param name="notification">Сформированное уведомление</param>
        /// <param name="targetUser">Пользователь, которому его необходимо отправить</param>
        public void Send(EmailNotification notification, User targetUser)
        {
            try
            {
                using var client = new SmtpClient();
                client.Connect(smtpAddress, smtpPort, SecureSocketOptions.StartTls);
                client.Send(GetMimeMessage(notification, targetUser));
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Метод возвращает сообщение для отправки по email
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        private MimeMessage GetMimeMessage(EmailNotification notification, User targetUser)
        {
            MimeMessage emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(notification.Header, _configuration["SMTPSender"]));
            emailMessage.To.Add(new MailboxAddress(targetUser.UserName, targetUser.Email));
            emailMessage.Subject = notification.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = notification.Content
            };
            return emailMessage;
        }
    }
}
