using MimeKit;
using MailKit.Net.Smtp;
using GSCrm.Models;
using Task = System.Threading.Tasks.Task;
using GSCrm.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using GSCrm.Localization;

namespace GSCrm.Services
{
    public class EmailSender
    {
        private readonly IConfiguration configuration;
        private readonly ResManager resManager;
        public EmailSender(IConfiguration configuration, ResManager resManager)
        {
            this.configuration = configuration;
            this.resManager = resManager;
        }

        /// <summary>
        /// Подтверждение регистрации в приложении
        /// </summary>
        /// <param name="model"></param>
        /// <param name="confirmEmailUrl">Ссылка для подтверждения регистрации</param>
        /// <returns></returns>
        public async Task SendRegisterConfirmationEmail(UserModel model, string confirmEmailUrl)
        {
            EmailNotification notification = new EmailNotification()
            {
                RecipientAddress = model.Email,
                RecipientName = model.UserName,
                Sender = configuration["SMTPSender"],
                SenderPassword = configuration["SMTPSenderPassword"],
                Subject = resManager.GetString("RegisterConfirmationSubject"),
                Message = $"Для подтверждения регистрации перейдите по следующей ссылке: <a href='{confirmEmailUrl}'>Подтверждение регистрации</a>.",
                Header = resManager.GetString("EmailDefaultHeader")
            };
            await SendEmailAsync(notification);
        }

        /// <summary>
        /// Сброс пароля
        /// </summary>
        /// <param name="model"></param>
        /// <param name="confirmEmailUrl">Ссылка для сброса пароля</param>
        /// <returns></returns>
        public async Task SendResetPasswordEmail(UserModel model, string resetPasswordUrl)
        {
            EmailNotification notification = new EmailNotification()
            {
                RecipientAddress = model.Email,
                RecipientName = model.UserName,
                Sender = configuration["SMTPSender"],
                SenderPassword = configuration["SMTPSenderPassword"],
                Subject = resManager.GetString("ResetPasswordSubject"),
                Message = $"Для сброса пароля перейдите по следующей ссылке: <a href='{resetPasswordUrl}'>Сброс пароля</a>.",
                Header = resManager.GetString("EmailDefaultHeader")
            };
            await SendEmailAsync(notification);
        }

        /// <summary>
        /// Рассылка уведомлений через smtp
        /// </summary>
        /// <param name="notification">Модель, хранящая поля, настраивающие уведомление</param>
        /// <returns></returns>
        private async Task SendEmailAsync(EmailNotification notification)
        {
            MimeMessage emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(notification.Header, notification.Sender));
            emailMessage.To.Add(new MailboxAddress(notification.RecipientName, notification.RecipientAddress));
            emailMessage.Subject = notification.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = notification.Message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync(notification.Sender, notification.SenderPassword);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
