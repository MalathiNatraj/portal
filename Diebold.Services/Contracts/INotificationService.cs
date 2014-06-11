using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface INotificationService
    {
        void Notify(Notification notification);
        void ValidateEmcAccount(string emcAccountNumber);
        void SendEmcNotification(Notification notification);
        void SendMailNotification(Notification notification);
    }
}
