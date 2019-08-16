﻿using QMS.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Storage.Interfaces
{
    public interface IWriteCmsItem
    {
        Task Write(CmsItem item, string cmsType, string id, string lang);
    }
}
