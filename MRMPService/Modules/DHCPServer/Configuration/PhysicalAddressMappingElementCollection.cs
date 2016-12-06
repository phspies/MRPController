using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Modules.DHCPServer.Library.Configuration
{
    public class PhysicalAddressMappingElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PhysicalAddressMappingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PhysicalAddressMappingElement)element).PhysicalAddress;
        }
    }
}
