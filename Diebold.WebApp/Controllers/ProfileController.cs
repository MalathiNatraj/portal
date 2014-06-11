using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diebold.WebApp.Models;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Data;
using Diebold.Domain.Entities;
using Diebold.Services.Contracts;
using Diebold.Domain.Contracts.Infrastructure;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace Diebold.WebApp.Controllers
{
    public class ProfileController : BaseCRUDTrackeableController<User, UserViewModel>
    {
        //
        // GET: /Profile/

        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ICompanyService _companyService;
        private readonly IDvrService _deviceService;
        private readonly ICurrentUserProvider _currentUserProvider;

        public ProfileController(IUserService userService,
            IRoleService roleService, ICompanyService companyService,
            IDvrService deviceService, ICurrentUserProvider currentUserProvider)
            : base(userService)
        {
            this._userService = userService;
            this._roleService = roleService;
            this._companyService = companyService;
            this._deviceService = deviceService;
            this._currentUserProvider = currentUserProvider;
        }       

        public ActionResult LogOn()
        {
            return new EmptyResult();
        }
       

        protected override UserViewModel MapEntity(User item)
        {
            return new UserViewModel(item);
        }       
       
    }
}


