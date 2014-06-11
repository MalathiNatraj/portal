using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Type;
using Diebold.Domain.Entities;

namespace Diebold.DAO.NH.Helpers
{
    public class ActionStringType : EnumStringType
    {
        public ActionStringType()
            : base(typeof(Diebold.Domain.Entities.Action), 25)
        {
        }
    }
}
