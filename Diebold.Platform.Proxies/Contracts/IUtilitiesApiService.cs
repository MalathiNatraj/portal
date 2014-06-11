using Diebold.Platform.Proxies.DTO;

namespace Diebold.Platform.Proxies.Contracts
{
    public interface IUtilitiesApiService
    {
        void SendMail(MailDTO mail);
    }
}
