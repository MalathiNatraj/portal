using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using Diebold.Domain.Entities;
using System.Web.Mvc;
using System.ComponentModel;

namespace Diebold.WebApp.Models
{
    public class SiteInventoryViewModel
    {
        public string InventoryKey { get; set; }
        public string InventoryValue { get; set; }
        public int? InventoryKeyId { get; set; }
        public bool isInventoryViewable { get; set; }
        public bool isInventoryEditable { get; set; }
        public bool isInventoryDeleteable { get; set; }
        public int? ExternalCompanyId { get; set; }
        public int? SiteId { get; set; }
    }
}