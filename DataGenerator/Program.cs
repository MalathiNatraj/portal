using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using Diebold.DAO.NH.Infrastructure;
using Diebold.DAO.NH.Repositories;
using Diebold.Domain.Entities;
using Diebold.Platform.Proxies.Impl;
using Diebold.Services.Config;
using Diebold.Services.Impl;
using Diebold.Services.Infrastructure;
using NHibernate;
using Ninject;
using Ninject.Modules;
using Action = Diebold.Domain.Entities.Action;

namespace DataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DATA LOADER");
            Console.WriteLine("===========");
            Console.WriteLine();

            TestDataLoader.LoadData();

            Console.WriteLine();
            Console.WriteLine("Data loading process ended.");

            Console.Read();
        }
    }

    public class TestDataLoader
    {
        public static IValidationProvider ValidationProvider { get; set; }
        public static ISessionFactory SessionFactory { get; set; }

        public static void Initialize()
        {
            var settings = new NinjectSettings { LoadExtensions = false };
            var kernel = new StandardKernel(settings);
            kernel.Load(new List<NinjectModule> { new ValidationModule() });
            ValidationProvider = kernel.Get<IValidationProvider>();

            var connectionString = ConfigurationManager.ConnectionStrings["DieboldDB"].ConnectionString;
            var helper = new NHibernateHelper(connectionString);

            //Set up schema
            helper.CreateSchema();

            SessionFactory = helper.SessionFactory;
        }

        #region private methods

        private static void Execute(Action<NHUnitOfWork> action)
        {
            using (var unitOfWork = new NHUnitOfWork(SessionFactory))
            {
                action(unitOfWork);
            }
        }

        private static void ExecuteFor(int count, Action<NHUnitOfWork, int> action)
        {
            for (var i = 0; i < count; i++)
            {
                using (var unitOfWork = new NHUnitOfWork(SessionFactory))
                {
                    action(unitOfWork, i);
                }
            }
        }

        private static void ExecuteGlobal(Action<NHUnitOfWork> action)
        {
            using (var unitOfWork = new GlobalUnitOfWork(SessionFactory))
            {
                action(unitOfWork);

                unitOfWork.FinalCommit();
            }
        }

        #endregion

        public static void LoadData()
        {
            Initialize();

            Company company1 = null;
            Company company2 = null;

            CompanyGrouping1Level grouping1Level = null;
            CompanyGrouping2Level grouping2Level = null;

            Site siteA = null;
            Site siteB = null;

            var gateways = new Gateway[5];

            var devices = new Device[50];

            Role adminRole = null;
            Role userRole = null;

            try
            {


                ExecuteGlobal(unitOfWork =>
                                  {
                                      var companyRepository = new BaseIntKeyedRepository<Company>(unitOfWork);
                                      var companyGrouping1LevelRepository =
                                          new BaseIntKeyedRepository<CompanyGrouping1Level>(unitOfWork);
                                      var companyGrouping2LevelRepository =
                                          new BaseIntKeyedRepository<CompanyGrouping2Level>(unitOfWork);
                                      var companyService = new CompanyService(companyRepository, unitOfWork,
                                                                              ValidationProvider,
                                                                              companyGrouping1LevelRepository,
                                                                              companyGrouping2LevelRepository);

                                      var siteRepository = new BaseIntKeyedRepository<Site>(unitOfWork);
                                      var siteService = new SiteService(siteRepository, unitOfWork, ValidationProvider);

                                      //Companies
                                      company1 = new Company
                                                     {
                                                         Name = "Company A",
                                                         FirstLevelGrouping = "District",
                                                         SecondLevelGrouping = "Area",
                                                         ThirdLevelGrouping = "Site",
                                                         FourthLevelGrouping = "Device",
                                                         PrimaryContactName = "ContactName",
                                                         PrimaryContactOffice = "ContactOffice",
                                                         PrimaryContactEmail = "contact@mail.com",
                                                         ReportingFrequency = ReportingFrequency.Daily,
                                                         ExternalCompanyId = 1234,
                                                     };
                                      company2 = new Company
                                                     {
                                                         Name = "Company B",
                                                         FirstLevelGrouping = "District",
                                                         SecondLevelGrouping = "Area",
                                                         ThirdLevelGrouping = "Site",
                                                         FourthLevelGrouping = "Device",
                                                         PrimaryContactName = "ContactName",
                                                         PrimaryContactOffice = "ContactOffice",
                                                         PrimaryContactEmail = "contact@mail.com",
                                                         ReportingFrequency = ReportingFrequency.Daily,
                                                         ExternalCompanyId = 1235,
                                                     };

                                      //Company Groupings
                                      var companyGrouping1LevelA = new CompanyGrouping1Level()
                                                                       {
                                                                           Company = company1,
                                                                           Name = "Company Group A"
                                                                       };

                                      var companyGrouping1LevelB = new CompanyGrouping1Level()
                                                                       {
                                                                           Company = company2,
                                                                           Name = "Company Group B"
                                                                       };

                                      company1.CompanyGrouping1Levels.Add(companyGrouping1LevelA);
                                      company2.CompanyGrouping1Levels.Add(companyGrouping1LevelB);


                                      var companyGrouping2LevelA = new CompanyGrouping2Level()
                                                                       {
                                                                           CompanyGrouping1Level =
                                                                               companyGrouping1LevelA,
                                                                           Name = "Company Group 2A",
                                                                       };

                                      companyGrouping1LevelA.CompanyGrouping2Levels.Add(companyGrouping2LevelA);

                                      var companyGrouping2LevelB = new CompanyGrouping2Level()
                                                                       {
                                                                           CompanyGrouping1Level =
                                                                               companyGrouping1LevelB,
                                                                           Name = "Company Group 2B"
                                                                       };

                                      companyGrouping1LevelB.CompanyGrouping2Levels.Add(companyGrouping2LevelB);

                                      grouping1Level = companyGrouping1LevelA;
                                      grouping2Level = companyGrouping2LevelA;

                                      companyService.Create(company1);
                                      companyService.Create(company2);

                                      //Sites
                                      siteA = new Site
                                                  {
                                                      Name = "Site A",
                                                      City = "city",
                                                      State = "state",
                                                      Zip = "zip",
                                                      Country = "country",
                                                      County = "county",
                                                      DieboldName = "asd",
                                                      Address1 = "Address x",
                                                      Address2 = "Address y",
                                                      SharepointURL = "www.abc.com",
                                                      ParentAssociation = "ParentAssosciation",
                                                      CCMFStatus = "1",
                                                      CompanyGrouping2Level = companyGrouping2LevelA
                                                  };

                                      companyGrouping2LevelA.Sites.Add(siteA);

                                      siteService.Create(siteA);

                                      siteB = new Site
                                                  {
                                                      Name = "Site B",
                                                      City = "city",
                                                      State = "state",
                                                      Zip = "zip",
                                                      Country = "country",
                                                      County = "county",
                                                      DieboldName = "asd",
                                                      Address1 = "Address x",
                                                      Address2 = "Address y",
                                                      SharepointURL = "www.bcd.com",
                                                      ParentAssociation = "ParentAssosciation",
                                                      CCMFStatus = "1",
                                                      CompanyGrouping2Level = companyGrouping2LevelB
                                                  };
                                      companyGrouping2LevelB.Sites.Add(siteB);

                                      siteService.Create(siteB);
                                  });

                Console.WriteLine("Companies and Sites created.");

                //Gateways

                IList<string> macAddress = null;

                ExecuteFor(gateways.Length, (unitOfWork, index) =>
                                                {
                                                    var gatewayRepository =
                                                        new BaseIntKeyedRepository<Gateway>(unitOfWork);
                                                    var gatewayApi = new GatewayApi();
                                                    var deviceApi = new DeviceApi();
                                                    var gatewayService = new GatewayService(gatewayRepository,
                                                                                            unitOfWork,
                                                                                            ValidationProvider,
                                                                                            gatewayApi, deviceApi);

                                                    if (macAddress == null)
                                                    {
                                                        macAddress = gatewayService.GetEnabledMacAddress();
                                                    }

                                                    if (macAddress.Count > index)
                                                    {
                                                        gateways[index] = new Gateway
                                                                              {
                                                                                  Company = company1,
                                                                                  EMCId = 1234,
                                                                                  MacAddress = macAddress[index],
                                                                                  Name = "Gateway " + index,
                                                                                  Site = siteA,
                                                                                  Protocol = 1,
                                                                                  TimeZone = "UTC",
                                                                              };

                                                        gatewayService.Create(gateways[index]);
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("NOT ENOUGH UNASSIGNED SLOTS for creating gateways. Please add new slots.");
                                                    }
                                                });

                Console.WriteLine("Gateways created.");

                // General Alarms Configuration

                ExecuteGlobal(unitOfWork =>
                                  {
                                      var alarmConfigurationRepository =
                                          new BaseIntKeyedRepository<AlarmConfiguration>(unitOfWork);

                                      //Costar

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.DaysRecorded,
                                                                               DeviceType = DeviceType.Costar,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Integer,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.IsNotRecording,
                                                                               DeviceType = DeviceType.Costar,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Boolean,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.NetworkDown,
                                                                               DeviceType = DeviceType.Costar,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Boolean,
                                                                           });
                                      /*
                    alarmConfigurationRepository.Add(new AlarmConfiguration
                    {
                        AlarmType = AlarmType.SMART,
                        DeviceType = DeviceType.Costar,
                        Operator = AlarmOperator.Equals,
                        Severity = AlarmSeverity.Low,
                        Device = null,
                        Threshold = "0",
                        DataType = DataType.Boolean,
                    });

                    alarmConfigurationRepository.Add(new AlarmConfiguration
                    {
                        AlarmType = AlarmType.DriveTemperature,
                        DeviceType = DeviceType.Costar,
                        Operator = AlarmOperator.Equals,
                        Severity = AlarmSeverity.Low,
                        Device = null,
                        Threshold = "0",
                        DataType = DataType.Integer,
                    });

                    alarmConfigurationRepository.Add(new AlarmConfiguration
                    {
                        AlarmType = AlarmType.RaidStatus,
                        DeviceType = DeviceType.Costar,
                        Operator = AlarmOperator.Equals,
                        Severity = AlarmSeverity.Low,
                        Device = null,
                        Threshold = "0",
                        DataType = DataType.Boolean,
                    });
                    */
                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.VideoLoss,
                                                                               DeviceType = DeviceType.Costar,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Object,
                                                                           });

                                      //IpConfigure

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.DaysRecorded,
                                                                               DeviceType = DeviceType.IpConfigure,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Integer,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.IsNotRecording,
                                                                               DeviceType = DeviceType.IpConfigure,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Boolean,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.NetworkDown,
                                                                               DeviceType = DeviceType.IpConfigure,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Boolean,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.SMART,
                                                                               DeviceType = DeviceType.IpConfigure,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Boolean,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.DriveTemperature,
                                                                               DeviceType = DeviceType.IpConfigure,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Integer,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.RaidStatus,
                                                                               DeviceType = DeviceType.IpConfigure,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Boolean,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.VideoLoss,
                                                                               DeviceType = DeviceType.IpConfigure,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Object,
                                                                           });

                                      //VerintEdgeVr200

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.DaysRecorded,
                                                                               DeviceType = DeviceType.VerintEdgeVr200,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Integer,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.IsNotRecording,
                                                                               DeviceType = DeviceType.VerintEdgeVr200,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Boolean,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.NetworkDown,
                                                                               DeviceType = DeviceType.VerintEdgeVr200,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Boolean,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.SMART,
                                                                               DeviceType = DeviceType.VerintEdgeVr200,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Boolean,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.DriveTemperature,
                                                                               DeviceType = DeviceType.VerintEdgeVr200,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Integer,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.RaidStatus,
                                                                               DeviceType = DeviceType.VerintEdgeVr200,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Boolean,
                                                                           });

                                      alarmConfigurationRepository.Add(new AlarmConfiguration
                                                                           {
                                                                               AlarmType = AlarmType.VideoLoss,
                                                                               DeviceType = DeviceType.VerintEdgeVr200,
                                                                               Operator = AlarmOperator.Equals,
                                                                               Severity = AlarmSeverity.Severe,
                                                                               Device = null,
                                                                               Threshold = "0",
                                                                               DataType = DataType.Object,
                                                                           });
                                  });

                Console.WriteLine("Alarm Configuration created.");

                var j = 0;
                foreach (var gateway in gateways)
                {
                    ExecuteFor(10, (unitOfWork, index) =>
                                       {
                                           var userMonitorGroupRepository = new UserMonitorGroupRepository(unitOfWork);
                                           var userDeviceMonitorRepository = new UserDeviceMonitorRepository(unitOfWork);
                                           var gatewayRepository = new BaseIntKeyedRepository<Gateway>(unitOfWork);
                                           var deviceRepository = new BaseIntKeyedRepository<Device>(unitOfWork);
                                           var alarmConfigurationRepository =
                                               new BaseIntKeyedRepository<AlarmConfiguration>(unitOfWork);
                                           var alertStatusRepository = new AlertStatusRepository(unitOfWork);

                                           var deviceApi = new DeviceApi();
                                           var deviceService = new DeviceService(deviceRepository, gatewayRepository,
                                                                                 unitOfWork,
                                                                                 ValidationProvider, deviceApi,
                                                                                 alertStatusRepository,
                                                                                 userMonitorGroupRepository,
                                                                                 userDeviceMonitorRepository);
                                           var alarmConfigurationService =
                                               new AlarmConfigurationService(alarmConfigurationRepository,
                                                                             unitOfWork, ValidationProvider);

                                           const int camerasCount = 10;

                                           var deviceType = DeviceType.VerintEdgeVr200;
                                           switch (index%3)
                                           {
                                               case 0:
                                                   deviceType = DeviceType.VerintEdgeVr200;
                                                   break;
                                               case 1:
                                                   deviceType = DeviceType.Costar;
                                                   break;
                                               case 2:
                                                   deviceType = DeviceType.IpConfigure;
                                                   break;
                                           }

                                           var cameras = new List<Camera>();
                                           for (var i = 0; i < camerasCount; i++)
                                           {
                                               cameras.Add(new Camera
                                                               {
                                                                   Name = "Camera " + i,
                                                                   Channel = i.ToString(),
                                                                   Active = i%2 == 0
                                                               });
                                           }

                                           var device = new Device
                                                            {
                                                                Name = gateway.Name + " - Device " + index,
                                                                DeviceKey = GenerateCode(10),
                                                                Company = company1,
                                                                AlarmConfigurations = null,
                                                                Cameras = cameras,
                                                                TimeZone = "GMT-3",
                                                                Gateway = gateway,
                                                                HealthCheckVersion = HealthCheckVersion.Version1,
                                                                DeviceType = deviceType,
                                                                HostName = "192.168." + (1 + j) + "." + (100 + index),
                                                                NumberOfCameras = camerasCount,
                                                                PollingFrequency = PollingFrequency.OneMinute,
                                                                ZoneNumber = "1",
                                                                ReportBufferSize = 1
                                                            };

                                           device.AlarmConfigurations =
                                               alarmConfigurationService.GetAlarmConfigurationForCreate(deviceType, company1);

                                           devices[10*j + index] = device;

                                           deviceService.Create(device);
                                       });

                    j++;
                }

                Console.WriteLine("Devices created.");

                //Roles

                Execute(unitOfWork =>
                            {
                                var roleRepository = new BaseIntKeyedRepository<Role>(unitOfWork);
                                var roleService = new RoleService(roleRepository, unitOfWork, ValidationProvider);

                                adminRole = new Role
                                                {
                                                    Name = "General Administrator",
                                                    Actions = new List<Action>
                                                                  {
                                                                      Action.ManageUsers,
                                                                      Action.MonitorAssignment,
                                                                      Action.ManageCompanies,
                                                                      Action.ManageSites,
                                                                      Action.ManageGateways,
                                                                      Action.ManageDevices,
                                                                      Action.ViewReports,
                                                                  }
                                                };

                                roleService.Create(adminRole);
                            });

                Execute(unitOfWork =>
                            {
                                var roleRepository = new BaseIntKeyedRepository<Role>(unitOfWork);
                                var roleService = new RoleService(roleRepository, unitOfWork, ValidationProvider);

                userRole = new Role
                {
                    Name = "Customer User",
                    Actions = new List<Action>
                    {
                        Action.ViewDashboard,
                        Action.ViewVideo,
                        Action.ManageUsers,
                        Action.MonitorAssignment,
                        Action.ViewDiagnostics
                    }
                };

                                roleService.Create(userRole);
                            });

                Console.WriteLine("Roles created.");


                ExecuteGlobal(unitOfWork =>
                                  {
                                      var userRepository = new UserRepository(unitOfWork);
                                      var userMonitorGroupRepository = new UserMonitorGroupRepository(unitOfWork);
                                      var userDeviceMonitorRepository = new UserDeviceMonitorRepository(unitOfWork);
                                      var alertStatusRepository = new AlertStatusRepository(unitOfWork);

                                      var deviceRepository = new BaseIntKeyedRepository<Device>(unitOfWork);
                                      var gatewayRepository = new BaseIntKeyedRepository<Gateway>(unitOfWork);
                                      var deviceApiService = new DeviceApi();

                                      var deviceService = new DeviceService(deviceRepository, gatewayRepository,
                                                                            unitOfWork, ValidationProvider,
                                                                            deviceApiService, alertStatusRepository,
                                                                            userMonitorGroupRepository,
                                                                            userDeviceMonitorRepository);

                                      var userService = new UserService(userRepository, unitOfWork, ValidationProvider,
                                                                        userMonitorGroupRepository,
                                                                        deviceService, userDeviceMonitorRepository);

                                      userService.Create(new User
                                                             {
                                                                 Company = company1,
                                                                 Role = adminRole,
                                                                 FirstName = "General",
                                                                 LastName = "Administrator",
                                                                 Username = "admin",
                                                                 Email = "admin@diebold.com",
                                                                 Phone = "123456",
                                                                 OfficePhone = "123456",
                                                                 Extension = "123",
                                                                 Mobile = "15123456",
                                                                 Title = "aaa 123",
                                                                 Text1 = "aaa",
                                                                 Text2 = "bbb",
                                                                 PreferredContact = PreferredContact.Mobile,
                                                                 TimeZone = "UTC"
                                                             });

                                      userService.Create(new User
                                                             {
                                                                 Company = company1,
                                                                 Role = userRole,
                                                                 FirstName = "Customer",
                                                                 LastName = "User",
                                                                 Username = "user",
                                                                 Email = "user@customer.com",
                                                                 Phone = "123457",
                                                                 OfficePhone = "123457",
                                                                 Extension = "124",
                                                                 Mobile = "15123457",
                                                                 Title = "aaa 124",
                                                                 Text1 = "aab",
                                                                 Text2 = "bbc",
                                                                 PreferredContact = PreferredContact.Mobile,
                                                                 TimeZone = "UTC"
                                                             });
                                  });

                Console.WriteLine("Users created.");

                ExecuteGlobal(unitOfWork =>
                                  {
                                      var userRepository = new UserRepository(unitOfWork);
                                      var userMonitorGroupRepository = new UserMonitorGroupRepository(unitOfWork);
                                      var userDeviceMonitorRepository = new UserDeviceMonitorRepository(unitOfWork);
                                      var alertStatusRepository = new AlertStatusRepository(unitOfWork);

                                      var deviceRepository = new BaseIntKeyedRepository<Device>(unitOfWork);
                                      var gatewayRepository = new BaseIntKeyedRepository<Gateway>(unitOfWork);

                                      var deviceApiService = new DeviceApi();

                                      var deviceService = new DeviceService(deviceRepository, gatewayRepository,
                                                                            unitOfWork, ValidationProvider,
                                                                            deviceApiService, alertStatusRepository,
                                                                            userMonitorGroupRepository,
                                                                            userDeviceMonitorRepository);

                                      var userService = new UserService(userRepository, unitOfWork, ValidationProvider,
                                                                        userMonitorGroupRepository,
                                                                        deviceService, userDeviceMonitorRepository);

                                      var user = userService.GetUserByUserName("user@customer.com");

                                      var monitorGroup = new List<UserMonitorGroup>
                                                             {
                                                                 new UserMonitorGroup
                                                                     {
                                                                         FirstGroupLevel = grouping1Level,
                                                                         SecondGroupLevel = grouping2Level,
                                                                         Site = siteA
                                                                     }
                                                             };

                                      userService.EditUserAndMonitoredGroupOfDevices(user, monitorGroup,
                                                                              new List<UserMonitorGroup>());

                                  });

                Console.WriteLine("User Monitor Groups created.");

            }
            catch(Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("======================================================");
                Console.WriteLine("");
                Console.WriteLine("An error occurred while creating data:");
                Console.WriteLine("");
                Console.WriteLine(ex.Message);
            }
        }

        protected static string GenerateCode(int codeLength)
        {
            const string a = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var chars = a.ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[codeLength];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(codeLength);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            return result.ToString();
        }

    }
}
