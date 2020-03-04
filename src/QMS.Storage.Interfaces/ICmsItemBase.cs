using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Storage.Interfaces
{
    public interface ICmsItemBase
    {
        bool HandlesType(string cmsType);
    }
}
