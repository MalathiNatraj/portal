﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;
using Diebold.Services.Impl;
using Diebold.Platform.Proxies.DTO;

namespace Diebold.Services.Contracts
{
    public interface IFireAlarmService 
    {
        List<ReportsDTO> FireAlarmReport(string AccountNumber);
    }
}
