using Diebold.Domain.Entities;

namespace Diebold.DAO.NH.Maps
{
    public class DeviceMediaMap : IntKeyedEntityMapping<DeviceMedia>
    {
        public DeviceMediaMap()
        {
            Property(u => u.MediaType, c =>
            {
                c.NotNullable(true);
                c.Length(10);
            });

            Property(u => u.MediaId, c =>
            {
                c.NotNullable(true);
                c.Length(50);
                c.Unique(true);
            });

            Property(u => u.Title, c =>
            {
                c.NotNullable(true);
                c.Length(50);
            });

            Property(u => u.Description, c =>
            {
                c.NotNullable(true);
                c.Length(250);
            });

            Property(u => u.Notes, c =>
            {
                c.NotNullable(true);
                c.Length(5000);
            });

            Property(u => u.Status, c =>
            {
                c.NotNullable(true);
                c.Length(50);
            });

            Property(u => u.FileName, c =>
            {
                c.NotNullable(true);
                c.Length(50);
            });

            Property(u => u.CreatedDate, c =>
            {
                c.NotNullable(true);
            });
        }
    }
}