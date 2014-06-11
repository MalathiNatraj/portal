using Diebold.Domain.Entities;
using NHibernate.Type;
using NHibernate.Mapping.ByCode;

namespace Diebold.DAO.NH.Maps
{
    public class AlarmConfigurationMap : IntKeyedEntityMapping<AlarmConfiguration>
    {

        public AlarmConfigurationMap()
	    {
            Property(u => u.Sms, c => c.NotNullable(true));

            Property(u => u.Email, c => c.NotNullable(true));

            Property(u => u.Emc, c => c.NotNullable(true));

            Property(u => u.Log, c => c.NotNullable(true));

            Property(u => u.Ack, c => c.NotNullable(true));

            Property(u => u.Display, c => c.NotNullable(true));

            Property(u => u.Threshold, c =>
            {
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.CompanyId, c =>
            {
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.Severity, c =>
            {
                c.Type<EnumStringType<AlarmSeverity>>();
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.AlarmType, c =>
            {
                c.Type<EnumStringType<AlarmType>>();
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.AlarmParentType, c =>
            {
                c.Type<EnumStringType<AlarmParentType>>();
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.DeviceType, c =>
            {
                c.Type<EnumStringType<DeviceType>>();
                c.NotNullable(false);
                c.Length(32);
            });

            Property(u => u.Operator, c =>
            {
                c.Type<EnumStringType<AlarmOperator>>();
                c.NotNullable(true);
                c.Length(32);
            });

            Property(u => u.DataType, c =>
            {
                c.Type<EnumStringType<DataType>>();
                c.NotNullable(true);
                c.Length(32);
            });


            ManyToOne(x => x.Device, mapping =>
            {
                mapping.Fetch(FetchKind.Join);
                mapping.Column("DeviceId");
                mapping.NotNullable(false);
            });

            //ManyToOne(x => x.Company, mapping =>
            //{
            //    mapping.Fetch(FetchKind.Join);
            //    mapping.Column("CompanyId");
            //    mapping.NotNullable(false);
            //});
	    }
    }
}
