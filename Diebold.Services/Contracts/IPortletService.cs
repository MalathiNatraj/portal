using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface IPortletService : ICRUDService<Portlets>
    {
        IList<Portlets> GetAllUserPortlet();
        Portlets GetById(int intId);
        Portlets GetByInternalName(string strInternalName);
    }
}
