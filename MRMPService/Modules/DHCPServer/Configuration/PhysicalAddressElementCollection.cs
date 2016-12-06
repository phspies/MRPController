using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Modules.DHCPServer.Library.Configuration
{
    [ConfigurationCollection(typeof(PhysicalAddressElement), AddItemName = "macAddress")]
    public class PhysicalAddressElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PhysicalAddressElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PhysicalAddressElement)element).PhysicalAddress;
        }
    }
}
