using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace Diebold.WebApp.Models
{
    public class DiagnosticConfigurationViewModel
    {
        #region Devices

        [DisplayName("Company")]
        public string CompanyId { get; set; }

        [DisplayName("Site")]
        public string SiteId { get; set; }

        [DisplayName("Gateway")]
        public string GatewayId { get; set; }

        public SelectList AvailableCompanies { get; protected set; }
        public IList<CompanyPair> AvailableCompaniesList
        {
            set
            {
                var companies = value.Select(company => new SelectListItem { Text = company.CompanyName, Value = company.CompanyId.ToString() }).ToList();
                AvailableCompanies = new SelectList(companies, "Value", "Text");
            }
        }

        public SelectList AvailableSites { get; protected set; }
        public IList<SitePair> AvailableSitesList
        {
            set
            {
                var sites = value.Select(site => new SelectListItem { Text = site.SiteName, Value = site.SiteId.ToString() }).ToList();
                AvailableSites = new SelectList(sites, "Value", "Text");
            }
        }

        public SelectList AvailableGateways { get; protected set; }
        public IList<GatewayPair> AvailableGatewaysList
        {
            set
            {
                var gateways = value.Select(gateway => new SelectListItem { Text = gateway.GatewayName, Value = gateway.GatewayId.ToString() }).ToList();
                AvailableGateways = new SelectList(gateways, "Value", "Text");
            }
        }

        #endregion

        #region Gateways

        public DiagnosticConfigurationViewModel() 
        { 
            InitialSignature();
        }

        private void InitialSignature()
        {
            var intialSignatureRaw = new List<SelectListItem>
            {
                new SelectListItem { Text = "Enabled", Value = "false"},
                new SelectListItem { Text = "Disabled", Value = "true"}
            };
            AvailableGatewayStatus = new SelectList(intialSignatureRaw, "Value", "Text");
        }

        public SelectList AvailableGatewayStatus { get; protected set; }
        public string GatewayStatus { get; set; }

        #endregion
    }

    public class CompanyPair
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
    }

    public class SitePair
    {
        public int SiteId { get; set; }
        public string SiteName { get; set; }
    }

    public class GatewayPair
    {
        public int GatewayId { get; set; }
        public string GatewayName { get; set; }
    }
}