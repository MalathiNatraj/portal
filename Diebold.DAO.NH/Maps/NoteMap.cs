using NHibernate.Mapping.ByCode;
using Diebold.Domain.Entities;

namespace Diebold.DAO.NH.Maps
{
    public class NoteMap : TrackeableEntityMapping<Note>
    {
        public NoteMap()
        {
            Property(u => u.Date, c =>
            {
                c.NotNullable(true);
            });

            Property(u => u.Text, c =>
            {
                c.NotNullable(true);
                c.Length(500);
            });

            ManyToOne(u => u.Device, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("DeviceId");
            });

            ManyToOne(u => u.User, mto =>
            {
                mto.Fetch(FetchKind.Join);
                mto.NotNullable(true);
                mto.Column("UserId");
            });


        }
    }
}
