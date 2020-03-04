using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDataCms.Storage.Interfaces
{
    public interface ICmsItemBase
    {
        bool HandlesType(string cmsType);
    }
}
