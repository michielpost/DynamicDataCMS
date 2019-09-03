using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QMS.Core
{
    public class CmsBuilder
    {
        private List<string> _namespaces = new List<string>();

        public bool HasDataPackages => _namespaces.Any();

        public void AddNamespace(string name)
        {
            _namespaces.Add(name);
        }

        public string GetHostingStartupAssembliesKey()
        {
            return string.Join(";", _namespaces);
        }

    }
}
