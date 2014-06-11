using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Linq;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.Domain.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Diebold.Services.Infrastructure;
using Diebold.Platform.Proxies.Contracts;
using System.Linq.Dynamic;
using Diebold.Services.Extensions;
using System;
using Diebold.Platform.Proxies.DTO;
using System.Transactions;
using NHibernate;
using NHibernate.Linq;

namespace Diebold.Services.Impl
{
    public class SiteService : BaseCRUDTrackeableService<Site>, ISiteService
    {
        private readonly IReadOnlyRepository<CompanyGrouping2Level> _companyGrouping2LevelRepository;
        private readonly ISiteApiService _siteApiService;
        private readonly IUserMonitorGroupRepository _monitorGroupRepository;
        private readonly ICompanyService _companyService;
        private readonly IIntKeyedRepository<SiteAccountNumber> _siteAccountNumberRepository;
        private readonly IIntKeyedRepository<SiteLogoDetails> _siteLogoDetailsRepository;
        protected readonly IUserDefaultsService _userDefaultService;
        private readonly IIntKeyedRepository<UserDefaults> _userDefaultsRepository;
        private readonly IIntKeyedRepository<AlarmConfiguration> _alarmRepository;
        private readonly ISiteRepository _siteRepository;

        static SiteService() 
        { 
            
        }

        public SiteService(ISiteRepository repository,
            IReadOnlyRepository<CompanyGrouping2Level> companyGrouping2LevelRepository,
            IUnitOfWork unitOfWork,
            IValidationProvider validationProvider, ILogService logService, IUserMonitorGroupRepository monitorGroupRepository, ICompanyService companyService, ISiteApiService siteApiService, IIntKeyedRepository<SiteAccountNumber> siteAccountNumberRepository
            , IIntKeyedRepository<SiteLogoDetails> siteLogoDetailsRepository,IUserDefaultsService userDefaultService, IIntKeyedRepository<UserDefaults> userDefaultRepository,IIntKeyedRepository<AlarmConfiguration> alarmRepository, ISiteRepository siteRepository)
            : base(repository, unitOfWork, validationProvider, logService)
        {
            _companyGrouping2LevelRepository = companyGrouping2LevelRepository;
            this._monitorGroupRepository = monitorGroupRepository;
            this._companyService = companyService;
            this._siteApiService = siteApiService;
            _siteAccountNumberRepository = siteAccountNumberRepository;
            _siteLogoDetailsRepository = siteLogoDetailsRepository;
            _userDefaultsRepository = userDefaultRepository;
            _userDefaultService = userDefaultService;
            _alarmRepository = alarmRepository;
            _siteRepository = siteRepository;
        }

        public Page<Site> GetPage(int pageNumber, int pageSize, string sortBy, bool ascending, string whereCondition)
        {
            var sites = _monitorGroupRepository.All().Where(x => x.User.Id == 1).Select(y => y.Site).ToList();

            var query = _repository.All().Where(x => x.DeletedKey == null);

            if (whereCondition.Trim().Length > 0)
            {
                query = query.Where(x =>
                    x.Name.Contains(whereCondition) ||
                    x.SiteId.ToString().Contains(whereCondition) ||
                    x.CompanyGrouping2Level.CompanyGrouping1Level.Company.Name.Contains(whereCondition) ||
                    x.Address1.Contains(whereCondition) ||
                    x.Address2.Contains(whereCondition));
            }

            string orderBy = string.Empty;
            if (sortBy.Contains(','))
            {
                string[] sortCriterias = sortBy.Split(new char[] { ',' });

                foreach (string s in sortCriterias)
                {
                    orderBy += ((orderBy.Length > 0) ? ", " : string.Empty) +
                        string.Format("{0} {1}", s, (ascending ? string.Empty : "DESC"));
                }
            }
            else
            {
                orderBy = string.Format("{0} {1}", sortBy, (ascending ? string.Empty : "DESC"));
            }

            return query.OrderBy(orderBy).ToPage(pageNumber, pageSize);
        }
        
        public IList<Site> GetSitesByCompanyGrouping2Level(int companyGrouping2LevelId)
        {
            return _repository.FilterBy(x => x.CompanyGrouping2Level.Id == companyGrouping2LevelId && x.DeletedKey == null).OrderBy(x=>x.Name).ToList();
        }
        
        public IList<Site> GetSitesByCompany(int companyId, bool showDisabled)
        {
            var query = _repository.All().Where(x => x.DeletedKey == null && x.CompanyGrouping2Level.CompanyGrouping1Level.Company.Id == companyId);

            if (!showDisabled)
                query = query.Where(x => x.IsDisabled == false);

            return query.OrderBy(x=>x.Name).ToList();
        }

        public IList<Site> GetGatewayByCompany(int companyId, bool showDisabled)
        {
            var query = _repository.All().Where(x => x.DeletedKey == null && x.CompanyGrouping2Level.CompanyGrouping1Level.Company.Id == companyId);

            if (!showDisabled)
                query = query.Where(x => x.IsDisabled == false);

            return query.OrderBy(x=>x.Name).ToList();
        }

        public Site getGeoCoordinates(Site objSite)
        {
            string strCommaWithSpace = ", ";
            String address = objSite.Address1 + strCommaWithSpace + objSite.City + strCommaWithSpace + objSite.State + strCommaWithSpace + objSite.Country + strCommaWithSpace + objSite.Zip;
            string sitePlatformResponse = _siteApiService.getGeoCoordinates(address);

            if (sitePlatformResponse != "[]")
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                dynamic obj = js.Deserialize<dynamic>(sitePlatformResponse);
                if (obj != null && obj["status"] == "OK")
                {
                    dynamic result = obj["results"][0];
                    dynamic location = result["geometry"]["location"];
                    objSite.Latitude = location["lat"].ToString();
                    objSite.Longitude = location["lng"].ToString();
                }
            }
            return objSite;
        }

        public override void LogOperation(LogAction action, Site item)
        {
            var site = item;
            var companyGrouping2 = _companyGrouping2LevelRepository.FindBy(x => x.Id == item.CompanyGrouping2Level.Id);
            var companyGrouping1 = companyGrouping2.CompanyGrouping1Level;

            _logService.Log(action, item.ToString(), companyGrouping1, companyGrouping2, site);
        }

        public bool ValidateSiteId(int siteId)
        {
            try
            {
                var validate = _repository.FindBy(x => x.SiteId == siteId && x.DeletedKey == null);
                return (validate == null);
            }
            catch (InvalidOperationException)
            {
                return true;
            }
        }
        public bool ValidateSiteIdForSiteInfo(int siteId)
        {
            try
            {
                var validate = _repository.FindBy(x => x.Id == siteId && x.DeletedKey == null);
                return (validate == null);
            }
            catch (InvalidOperationException)
            {
                return true;
            }
        }
        // To get next suggested Site ID
        public int GetMaxSiteId()
        {            
            var query = _repository.All().Max(x => x.SiteId);            
            return query;
        }

       // public List<Site> GetSitewithFireMonitoringAccNumber(int CurrentUserId)
       public List<SiteAccountNumber> GetSitewithFireMonitoringAccNumber(int CurrentUserId)
        {
            try
            {
                IList<int> selectedSiteId = new List<int>();
                var SitesAssociatedwithCurrentUser = GetSitesByUser(CurrentUserId);
               // var query = SitesAssociatedwithCurrentUser.Where(x => x.FireMonitoringAccNumber != null && x.DeletedKey == null);

                var query = SitesAssociatedwithCurrentUser.Where(x => x.DeletedKey == null).Select(y => y.SiteId);
                selectedSiteId = query.ToList();                
                var SitesAssociatedwithFA = GetSitesAssociatedwithFA(CurrentUserId).Where(x => selectedSiteId.Contains(x.siteId)).ToList();
                return SitesAssociatedwithFA.ToList();

                //return SitesAssociatedwithFA.ToList().Select(c => new 
                //{
                //    Id = c.Id,
                //    Name = _siteService.Get(c.Id),
                //    //Site SiteDetail = _siteService.Get(SiteId);                
                //    // Name = _siteService.Get(Id).Name,
                //    AccNumber = c.AccountNumber
                //});

                //IList<int> selectedSiteId = new List<int>();
                //var SitesAssociatedwithCurrentUser = GetSitesByUser(CurrentUserId);
                ////var query = SitesAssociatedwithCurrentUser.Where(x => x.FireMonitoringAccNumber != null && x.DeletedKey == null);
                //// var sites = _monitorGroupRepository.All().Where(x => x.User.Id == 1).Select(y => y.Site).ToList();

                //var query = SitesAssociatedwithCurrentUser.Where(x => x.DeletedKey == null).Select(y => y.Id);
                //selectedSiteId = query.ToList();
                //var SitesAssociatedwithFA = GetSitesAssociatedwithFA(CurrentUserId).Where(x => selectedSiteId.Contains(x.siteId)).ToList();
                //return SitesAssociatedwithFA.ToList();
                //return query.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public WeatherAlertsDTO GetWeatherAlerts(string State, string City)
        {
            string WeatherAlerts = _siteApiService.GetWeatherAlertbyStateandCity(State, City);
            var Weatheralert = new JavaScriptSerializer().Deserialize<WeatherAlertsDTO>(WeatherAlerts);
            if (Weatheralert.alerts != null && Weatheralert.alerts.Count() > 0)
            {
                return Weatheralert;
            }
            else
            {
                return null;
            }
           // return lstString;
        }

        #region ISiteService Members


        public IList<Site> GetSitesByUser(int userId)
        {
            IList<UserMonitorGroup> usetMonitorGroup = _monitorGroupRepository.GetUserMonitorGroupByUser(userId);

            IDictionary<int, Site> siteMap = new Dictionary<int, Site>();
            foreach (UserMonitorGroup group in usetMonitorGroup)
            {
                if (group.Site != null)
                {
                    siteMap[group.Site.Id] = group.Site;
                }
                else if (group.SecondGroupLevel != null)
                {
                    foreach (var site in GetSitesByCompanyGrouping2Level(group.SecondGroupLevel.Id))
                    {
                        siteMap[site.Id] = site;
                    }
                }
                else if (group.FirstGroupLevel != null)
                {
                    foreach (var secondGroupLevel in _companyService.GetGrouping2LevelsByGrouping1LevelId(group.FirstGroupLevel.Id))
                    {
                        foreach (var site in GetSitesByCompanyGrouping2Level(secondGroupLevel.Id))
                        {
                            siteMap[site.Id] = site;
                        }
                    }
                }
            }
            return siteMap.Values.ToList();
        }

        public List<SiteAccountNumber> GetSitesByUserForMonitoring(int CurrentUserId)
        {
            try
            {
                IList<int> selectedSiteId = new List<int>();
                var SitesAssociatedwithCurrentUser = GetSitesByUser(CurrentUserId).Where(x => x.DeletedKey == null).Distinct();

                var query = SitesAssociatedwithCurrentUser.Where(x => x.DeletedKey == null).Select(y => y.SiteId);
                selectedSiteId = query.ToList();

                //var SitesAssociatedwithFA = GetSitesAccountNumbers(CurrentUserId).Where(x => selectedSiteId.Contains(x.siteId)).ToList();
                var SitesAssociatedwithFA = GetSitesAccountNumbers().Where(x => selectedSiteId.Contains(x.siteId)).ToList();
                return SitesAssociatedwithFA.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<SiteAccountNumber> GetSitesAssociatedwithFA(int userId)
        {
            IList<UserMonitorGroup> usetMonitorGroup = _monitorGroupRepository.GetUserMonitorGroupByUser(userId);
            IList<SiteAccountNumber> lstsiteAccNumber = _siteAccountNumberRepository.All().Where(x => x.DeletedKey == null && x.IsAssociatedWithFA == true).ToList();           
            return lstsiteAccNumber.ToList();
        }
        //public IList<SiteAccountNumber> GetSitesAccountNumbers(int userId)
        public IList<SiteAccountNumber> GetSitesAccountNumbers()
        {
            //IList<SiteAccountNumber> lstsiteAccNumber = _siteAccountNumberRepository.All().Where(x => x.DeletedKey == null && x.User.Id == userId).ToList();
            IList<SiteAccountNumber> lstsiteAccNumber = _siteAccountNumberRepository.All().Where(x => x.DeletedKey == null).ToList();
            return lstsiteAccNumber.ToList();
        }        
        public Site GetSitesBySiteId(int siteId)
        {         
            var query = _repository.All().Where(x => x.SiteId == siteId).SingleOrDefault();         
            return query;
        }
        public void CreateSiteDetails(Site item,SiteLogoDetails siteLogo)
        {
            try
            {
                _logger.Debug("Creating a new site started");
                using (var scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    this._repository.Add(item);
                    _logger.Debug("Creating a new site completed");
                    siteLogo.siteId = item.Id;
                    _logger.Debug("Creating a new site logo started");
                    _siteLogoDetailsRepository.Add(siteLogo);
                    _logger.Debug("Creating a new site logo completed");
                    LogOperation(LogAction.SiteCreate, item);
                    _unitOfWork.Commit();
                    scope.Complete();
                    _logger.Debug("Commit Operation Completed in Create Site Details");
                }
            }            
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw e;
            }
        }

        public void UpdateSiteDetails(Site item, SiteLogoDetails siteLogo)
        {
            try
            {
                _logger.Debug("Updating a site started");
                using (var scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    // Simple merge will do here. Update will cause error as the entity is already associated in 
                    this._repository.Merge(item);
                    LogOperation(LogAction.SiteEdit, item);
                    _siteLogoDetailsRepository.Update(siteLogo);
                    _unitOfWork.Commit();
                    scope.Complete();
                    _logger.Debug("Commit Operation Completed in Update Site Details");
                }
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw e;
            }
        }       
        public void DeleteSiteDetails(Site item, int userId, SiteLogoDetails siteLogo)
        {
            try
            {
                _logger.Debug("Updating a site started");
                using (var scope = new TransactionScope(TransactionScopeOption.Required))
                {

                    IList<UserDefaults> lstUserDefaults = _userDefaultService.GetUserDefaultsUserandPortlet(userId, "SITEINFORMATION");
                    if (lstUserDefaults.Count() > 0)
                    {
                        _logger.Debug("Started Delete User Defaults for Site Information");
                        _userDefaultsRepository.Delete(lstUserDefaults.FirstOrDefault());
                        _logger.Debug("Completed Delete User Defaults for Site Information");
                    }
                    IList<UserDefaults> lstUserDefaultsFA = _userDefaultService.GetUserDefaultsUserandPortlet(userId, "FIREALARM");
                    if (lstUserDefaultsFA.Count() > 0)
                    {
                        _logger.Debug("Started Delete User Defaults for Fire Alarm");
                        _userDefaultsRepository.Delete(lstUserDefaultsFA.FirstOrDefault());
                        _logger.Debug("Completed Delete User Defaults for Fire Alarm");
                    }
                    IList<UserDefaults> lstUserDefaultsSiteMap = _userDefaultService.GetUserDefaultsUserandPortlet(userId, "SITEMAP");
                    if (lstUserDefaultsSiteMap.Count() > 0)
                    {
                        _logger.Debug("Started Delete User Defaults for Site Map");
                        _userDefaultsRepository.Delete(lstUserDefaultsSiteMap.FirstOrDefault());
                        _logger.Debug("Completed Delete User Defaults for Site Map");
                    } 
                    _logger.Debug("Started Delete Site Details");                    
                    item.DeletedKey = item.Id;
                    this._repository.Update(item);
                    _logger.Debug("Completed Delete Site Details");
                    _logger.Debug("Started Delete Site Logo Details");  
                    siteLogo.DeletedKey = siteLogo.Id;
                    _siteLogoDetailsRepository.Update(siteLogo);
                    _logger.Debug("Completed Delete Site Logo Details");  
                    _unitOfWork.Commit();
                    scope.Complete();
                    _logger.Debug("Commit Operation Completed in Delete Site Details");
                }
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw e;
            }
        }
        public void DeleteDuplicateAlarmConfigurations()
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    IList<AlarmConfiguration> lstAlarmConfig = _alarmRepository.All().Where(x => x.AlarmParentType.ToString().ToLower() == "deleted").ToList();
                    if (lstAlarmConfig.Count() > 0)
                    {
                        //AlarmConfig.Device = null;
                        _alarmRepository.Delete(lstAlarmConfig);
                        _unitOfWork.Commit();
                        scope.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw e;
            }

        }
        public int GetSitesCount()
        {
            var query = _siteRepository.GetSitesCount();
            return query;
        }
        public IList<Site> GetSitesPerPage(int pageIndex, int rowCount)
        {
            var query = _siteRepository.GetSitesPerPage(pageIndex, rowCount).ToList();           
            return query;
        }        
        #endregion
    }


}
