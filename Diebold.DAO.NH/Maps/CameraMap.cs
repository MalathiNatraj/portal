using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;

namespace Diebold.DAO.NH.Maps
{
    public class CameraMap : TrackeableEntityMapping<Camera>
    {
        public CameraMap()
        {
            Property(u => u.Name, c =>
                        {
                            c.NotNullable(true);
                            c.Length(32);
                        });

            Property(u => u.Channel, c =>
                        {
                            c.NotNullable(true);
                            c.Length(32);
                        });

            Property(u => u.Active, c => c.NotNullable(true));
        }
    }
}
