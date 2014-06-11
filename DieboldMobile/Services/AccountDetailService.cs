using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DieboldMobile.Models;

namespace DieboldMobile.Services
{
    public class AccountDetailService
    {
        public AccountDetailModel GetAccountDetails()
        {
            AccountDetailModel objAccountDetailModel = new AccountDetailModel();
            objAccountDetailModel.CompanyName = "DIEBOLD";
            objAccountDetailModel.SiteCount = 15;
            objAccountDetailModel.DeviceCount = 12;
            objAccountDetailModel.IntrusionDevices = 9;
            objAccountDetailModel.HealthDevices = 3;
            objAccountDetailModel.AccessDevices = 8;
            return objAccountDetailModel;
        }
    }
}