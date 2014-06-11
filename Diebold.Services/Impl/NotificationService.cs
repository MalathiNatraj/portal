using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Domain.Entities;
using Diebold.Platform.Proxies.Contracts;
using Diebold.Platform.Proxies.DTO;
using Diebold.RemoteService.Proxies.EMC.Contracts;
using Diebold.RemoteService.Proxies.EMC.Impl;
using Diebold.Services.Contracts;
using Diebold.Services.Exceptions;
using Diebold.Services.Extensions;
using Diebold.Services.Infrastructure;

namespace Diebold.Services.Impl
{
    public class NotificationService : BaseService, INotificationService
    {
        private readonly IEmcService _emcService;
        private readonly IUtilitiesApiService _utilitiesApiService;
        private readonly IUserService _userService;

        public NotificationService(IUnitOfWork unitOfWork, IEmcService emcService, IUtilitiesApiService utilitiesApiService, IUserService userService)
            : base(unitOfWork)
        {
            this._emcService = emcService;
            this._utilitiesApiService = utilitiesApiService;
            this._userService = userService;
        }

        /// <summary>
        /// Send notification based on alarm configuration
        /// </summary>
        /// <param name="notification"></param>
        public void Notify(Notification notification)
        {
            // Each task is async and independent

            if (notification.SendToEmail)
            {
                SendMailNotification(notification);
            }

            if (notification.SendToEmc)
            {
                SendEmcNotification(notification);
            }

            if (notification.SendToLog)
            {
                
            }
        }

        public void ValidateEmcAccount(string emcAccountNumber)
        {
            bool isValid = false;
            try
            {
                string CurrentEnv = ConfigurationManager.AppSettings["Environment"];
                if (CurrentEnv.Equals("Development", StringComparison.InvariantCultureIgnoreCase))
                {
                    isValid = true;
                }
                else
                {
                    isValid = _emcService.ValidateEmcAccount(emcAccountNumber);
                }
            }
            catch(SocketException socketException)
            {
                _logger.Debug(socketException.Message);
                var validationR = new List<ValidationResult>();
                validationR.Add(new ValidationResult("EmcNumber", "The EMC Service is not available"));
                throw new ValidationException(validationR);
            }
            catch (Exception ex)
            {
                _logger.Debug("EMC Poll Message Error: " + ex.Message);
            }

            if (!isValid)
            {
                var validationR = new List<ValidationResult>();
                validationR.Add(new ValidationResult("EmcNumber", "The EMC Account number is not valid."));
                throw new ValidationException(validationR);
            }

        }

        public void SendEmcNotification(Notification notification)
        {
            try
            {
                _emcService.SendAlarm(notification.EmcAccontNumber, notification.EmcDevicezone);
            }
            catch (System.IO.IOException ioEx)
            {
                _logger.Debug("Send EMC Notification Error: " + ioEx.Message);
                throw new ServiceException("Could not establish connection with EMC");
            }
            catch (Exception ex)
            {
                _logger.Debug("Send EMC Notification Error: " + ex.Message);
            }
        }

        public void SendMailNotification(Notification notification)
        {
            try
            {
                var deviceId = notification.DeviceId;
                var users = _userService.GetUsersMonitoringDevice(deviceId);
                var sender = ConfigurationManager.AppSettings["NotificationSender"];
                var subject = string.Format("{0} - {1}", notification.DeviceName, notification.AlarmName);
                
                var mailBody = new StringBuilder();
                mailBody.AppendFormat("{0} {1}: {2} {3} {4}", notification.DateOccur, notification.TimeZone, notification.AlarmName, 
                                                              (notification.AlertCleared) ? "alert cleared on" : "is in alert on",
                                                              notification.DeviceName);
                mailBody.AppendLine();
                mailBody.AppendLine();
                mailBody.AppendFormat("Site Information: \r\n{0}\r\n{1}\r\n{2}", notification.SiteName, notification.SiteAddress1, notification.SiteAddress2);

                foreach (var user in users)
                {
                    var mail = new MailDTO()
                                   {
                                       From = sender,
                                       To = user.Email,
                                       Subject = subject,
                                       Body = mailBody.ToString()
                                   };

                    _utilitiesApiService.SendMail(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("Send Email Notification Error: " + ex.Message);
            }
        }
    }
}
