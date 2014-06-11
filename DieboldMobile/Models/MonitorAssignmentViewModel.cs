using AutoMapper;
using Diebold.Domain.Entities;

namespace DieboldMobile.Models
{
    public class MonitorAssignmentViewModel : BaseMappeableViewModel<UserMonitorGroup>
    {
        public bool Selected { get; set; }

        public bool IsNew { get; set; }

        public int? FirstGroupLevelId { get; set; }

        public string FirstGroupLevelName { get; set; }

        public int? SecondGroupLevelId { get; set; }

        public string SecondGroupLevelName { get; set; }

        public int? SiteId { get; set; }

        public string SiteName { get; set; }

        public int? DeviceId { get; set; }

        public string DeviceName { get; set; }

        public string FirstLevelName { get; set; }

        public string SecondLevelName { get; set; }

        public string SelectedGroupLevelType { get; set; }

        public int SelectedGroupLevelId { get; set; }

        public string SelectedGroupLevelName { get; set; }

        public string GroupName
        {
            get
            {
                if (DeviceId.HasValue)
                    return string.Format("Device: {0}", DeviceName);
                else if (SiteId.HasValue)
                    return string.Format("Site: {0}", SiteName);
                else if (SecondGroupLevelId.HasValue)
                    return string.Format("{0}: {1}", SecondLevelName, SecondGroupLevelName);
                else
                    return string.Format("{0}: {1}", FirstLevelName, FirstGroupLevelName);
            }
        }

        static MonitorAssignmentViewModel()
        {

            Mapper.CreateMap<UserMonitorGroup, MonitorAssignmentViewModel>();
            Mapper.CreateMap<MonitorAssignmentViewModel, UserMonitorGroup>()
                .ForMember(dest => dest.FirstGroupLevel,
                           opt =>
                           opt.MapFrom(
                               src =>
                               src.FirstGroupLevelId.HasValue
                                   ? new CompanyGrouping1Level { Id = src.FirstGroupLevelId.Value }
                                   : null))
                .ForMember(dest => dest.SecondGroupLevel,
                           opt =>
                           opt.MapFrom(
                               src =>
                               src.SecondGroupLevelId.HasValue
                                   ? new CompanyGrouping2Level { Id = src.SecondGroupLevelId.Value }
                                   : null))
                .ForMember(dest => dest.Site,
                           opt =>
                           opt.MapFrom(
                               src =>
                               src.SiteId.HasValue
                               ? new Site { Id = src.SiteId.Value }
                               : null))
                .ForMember(dest => dest.Device,
                           opt =>
                           opt.MapFrom(
                               src =>
                               src.DeviceId.HasValue
                               ? new Dvr { Id = src.DeviceId.Value }
                               : null));
        }

        public MonitorAssignmentViewModel(UserMonitorGroup group)
        {
            Mapper.Map(group, this);

            this.Selected = true;
            this.IsNew = false;
        }

        public MonitorAssignmentViewModel()
        {
        }

    }
}