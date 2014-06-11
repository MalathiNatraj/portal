using System;
using System.Collections.Generic;
using Diebold.Platform.Proxies.DTO;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.Contracts
{
    public interface ISiteApiService
    {
        string getGeoCoordinates(String address);

        // To Get the Site Weather Alerts
        string GetWeatherAlertbyStateandCity(string State, string City);
    }
}
