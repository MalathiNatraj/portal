using System.Collections.ObjectModel;
using Diebold.Domain.Entities;
using Diebold.Services.Extensions;
using NHibernate;

namespace Diebold.Test.InitialDbData
{
    public class DataCreator
    {
        private readonly ISession _session;
        private Role[] _roles;
        private User[] _users;
        private Company[] _companies;
        private Site[] _sites;

        public DataCreator(ISession session)
        {
            this._session = session;
        }

        public void Create()
        {
            CreateRoles();
            CreateUsers();
            //CreateCompany();
            //CreateSite();
        }

        private void CreateCompany()
        {
            _companies = new[]
                             {
                                 new Company() { Name = "Company A" },
                                 new Company() { Name = "Company B" },
                                 new Company() { Name = "Company C" },
                                 new Company() { Name = "Company D" } 
                             };
            _companies.ForEach(x => _session.Save(x));
        }

        private void CreateSite()
        {
            _sites = new[]
                         {
                             new Site() { Name = "Site A", City = "city", State = "state", Zip = "zip",Country = "country", County = "county", DieboldName = "asd", Address1 = "Address x", Address2 = "Address y" }, 
                             new Site() { Name = "Site B", City = "city", State = "state", Zip = "zip",Country = "country", County = "county", DieboldName = "asd", Address1 = "Address x", Address2 = "Address y" }, 
                             new Site() { Name = "Site C", City = "city", State = "state", Zip = "zip",Country = "country", County = "county", DieboldName = "asd", Address1 = "Address x", Address2 = "Address y" }, 
                             new Site() { Name = "Site D", City = "city", State = "state", Zip = "zip",Country = "country", County = "county", DieboldName = "asd", Address1 = "Address x", Address2 = "Address y" }
                         };
            _sites.ForEach(x => _session.Save(x));
        }

        private void CreateUsers()
        {
            _users = new[]
                        {
                             new User()
                                {
                                    FirstName = "TestAdmin",
                                    LastName = "testAdmin",
                                    Email = "admin@admin.com",
                                    Extension = "1111",
                                    Mobile = "2222",
                                    OfficePhone = "3333",
                                    Phone = "4444",
                                    Title = "sample",
                                    Text1 = "text1",
                                    Text2 = "text2",
                                    Role = _roles[0]
                                },
                                new User()
                                {
                                    FirstName = "TestCSUport",
                                    LastName = "test",
                                    Email = "support1@admin.com",
                                    Extension = "1111",
                                    Mobile = "2222",
                                    OfficePhone = "3333",
                                    Phone = "4444",
                                    Title = "sample",
                                    Text1 = "text1",
                                    Text2 = "text2",
                                    Role = _roles[1]
                                },
                                new User()
                                {
                                    FirstName = "TestUSuppor",
                                    LastName = "test",
                                    Email = "support2@admin.com",
                                    Extension = "1111",
                                    Mobile = "2222",
                                    OfficePhone = "3333",
                                    Phone = "4444",
                                    Title = "sample",
                                    Text1 = "text1",
                                    Text2 = "text2",
                                    Role = _roles[2]
                                },
                                new User()
                                {
                                    FirstName = "TestCUsert",
                                    LastName = "test",
                                    Email = "user@admin.com",
                                    Extension = "1111",
                                    Mobile = "2222",
                                    OfficePhone = "3333",
                                    Phone = "4444",
                                    Title = "sample",
                                    Text1 = "text1",
                                    Text2 = "text2",
                                    Role = _roles[3]
                                }
                        };
            _users.ForEach(x => _session.Save(x));
        }

        private void CreateRoles()
        {
            _roles = new[]
                         {
                            new Role()
                            {
                                Name = "General Administrator", 
                                Actions = new Collection<Action>()
                                              {
                                                  Action.ManageUsers,
                                                  Action.ManageDevices,
                                                  Action.ManageGateways,
                                                  Action.ManageRoles,
                                                  Action.ManageSites,
                                                  Action.ViewReports,
                                                  Action.TakeActionsOverAlerts,
                                                  Action.ViewDashboard
                                              }
                            },
                            new Role() { Name = "Customer Support" },
                            new Role() { Name = "Tech Support" },
                            new Role() { Name = "Customer User" }
                         };
            _roles.ForEach(x => _session.Save(x));
        }
    }
}