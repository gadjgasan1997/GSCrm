using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Notifications.Params;
using GSCrm.Notifications.Services;
using System;
using Task = System.Threading.Tasks.Task;

namespace GSCrm.Notifications.Factories.NotFactories
{
    public class UserRegisterNotFacory : NotificationFactory<UserRegisterParams>
    {
        public UserRegisterNotFacory(IServiceProvider serviceProvider, ApplicationDbContext context, UserRegisterParams notificationParams)
            : base(serviceProvider, context, notificationParams)
        { }

        public async Task SendAsync(User newUser)
        {
            EmailNotificationService emailNotificationService = new EmailNotificationService(serviceProvider, context);
            await emailNotificationService.SendAsync((EmailNotification)Create(NotificationTarget.Email), newUser);
        }

        protected override Notification Create(NotificationTarget notificationTarget)
            => new EmailNotification()
            {
                Id = Guid.NewGuid(),
                Content = GetEmailTemplate(),
                SourceId = string.Empty,
                NotificationSource = NotificationSource.GSCrm,
                Header = resManager.GetString("UserRegisterHeader"),
                Subject = resManager.GetString("UserRegisterSubject"),
                NotificationType = NotificationType.Register
            };

        protected override string GetEmailTemplate()
            => $"Для подтверждения регистрации перейдите по следующей ссылке: <a href='{notificationParams.ConfirmEmailUrl}'>Подтверждение регистрации</a>.";
    }
}
