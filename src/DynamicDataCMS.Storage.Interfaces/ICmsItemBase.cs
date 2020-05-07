using DynamicDataCMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDataCMS.Storage.Interfaces
{
    public interface ICmsItemBase
    {
        bool HandlesType(CmsType cmsType);
    }
}
