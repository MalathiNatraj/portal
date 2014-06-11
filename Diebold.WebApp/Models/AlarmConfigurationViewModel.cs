using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Diebold.Domain.Entities;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diebold.WebApp.Models
{
    public class AlarmConfigurationViewModel : BaseMappeableViewModel<AlarmConfiguration>
    {
        static AlarmConfigurationViewModel()
        {
            Mapper.CreateMap<AlarmConfiguration, AlarmConfigurationViewModel>();

            Mapper.CreateMap<AlarmConfigurationViewModel, AlarmConfiguration>()
                  .ForMember(dest => dest.Device, opt => opt.MapFrom( src => src.DeviceId.HasValue ? new Dvr { Id = src.DeviceId.Value } : null))
                  .ForAllMembers(dest => dest.Condition(src => !src.IsSourceValueNull));
        }

        public AlarmConfigurationViewModel()
        {
        }

        public AlarmConfigurationViewModel(AlarmConfiguration alert)
        {
            Mapper.Map(alert, this);
        }
       
        public string AlarmType { get; set; }
        
        public string Severity { get; set; }

        [StringLength(32)]
        public string Threshold { get; set; }
        
        public int ThresholdValue { get; set; }
      
        public string Operator { get; set; }

        [DisplayName("SMS")]
        public bool Sms { get; set; }

        [DisplayName("E-mail")]
        public bool Email { get; set; }

        [DisplayName("EMC")]
        public bool Emc { get; set; }

        [DisplayName("Log")]
        public bool Log { get; set; }
        
        public int? DeviceId { get; set; }

        public string DeviceType { get; set; }

        public string DataType { get; set; }

        public SelectList AvailableAlarmSeverities { get; protected set; }
        public IList<string> AvailableAlarmSeverityList
        {
            set
            {
                var availableAlarmSeverities = value.Select(alarmSeverity => new SelectListItem
                                                                {
                                                                    Text = alarmSeverity,
                                                                    Value = alarmSeverity
                                                                }).ToList();

                AvailableAlarmSeverities = new SelectList(availableAlarmSeverities, "Value", "Text");
            }
        }

        public SelectList AvailableAlarmOperators { get; protected set; }
        public IList<string> AvailableAlarmOperatorList
        {
            set
            {
                var availableAlarmOperators = new List<SelectListItem>();
                foreach (var alarmOperator in value)
                {
                    var sb = new System.Text.StringBuilder();
                    foreach (var c in alarmOperator)
                    {
                        if (Char.IsUpper(c))
                            sb.Append(' ');
                        sb.Append(c);
                    }

                    availableAlarmOperators.Add(new SelectListItem
                    {
                        Text = sb.ToString(),
                        Value = alarmOperator
                    });
                }
                AvailableAlarmOperators = new SelectList(availableAlarmOperators, "Value", "Text");
            }
        }

        public SelectList AvailableAlarmParentType { get; protected set; }
        public IList<string> AvailableAlarmParentTypeList
        {
            set
            {
                var availableAlarmParentAlarms = new List<SelectListItem>();
                foreach (var alarmOperator in value)
                {
                    var sb = new System.Text.StringBuilder();
                    foreach (var c in alarmOperator)
                    {
                        if (Char.IsUpper(c))
                            sb.Append(' ');
                        sb.Append(c);
                    }

                    availableAlarmParentAlarms.Add(new SelectListItem
                    {
                        Text = sb.ToString(),
                        Value = alarmOperator
                    });
                }
                AvailableAlarmOperators = new SelectList(availableAlarmParentAlarms, "Value", "Text");
            }
        }

        [DisplayName("Ack")]
        public bool Ack { get; set; }

        [DisplayName("Display")]
        public bool Display { get; set; }

        public int CompanyId { get; set; }
        public string AlarmParentType { get; set; }
    }
}