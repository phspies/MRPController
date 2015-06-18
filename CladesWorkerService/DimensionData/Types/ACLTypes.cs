using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CladesWorkerService.CaaS.Models
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/network", IsNullable = false)]
    public partial class AclRuleList
    {

        private string nameField;

        private List<AclRuleListAclRule> aclRuleField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AclRule")]
        public List<AclRuleListAclRule> AclRule
        {
            get
            {
                return this.aclRuleField;
            }
            set
            {
                this.aclRuleField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    public partial class AclRuleListAclRule
    {

        private string idField;

        private string nameField;

        private string statusField;

        private ushort positionField;

        private string actionField;

        private string protocolField;

        private object sourceIpRangeField;

        private object destinationIpRangeField;

        private AclRuleListAclRulePortRange portRangeField;

        private string typeField;

        /// <remarks/>
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public ushort position
        {
            get
            {
                return this.positionField;
            }
            set
            {
                this.positionField = value;
            }
        }

        /// <remarks/>
        public string action
        {
            get
            {
                return this.actionField;
            }
            set
            {
                this.actionField = value;
            }
        }

        /// <remarks/>
        public string protocol
        {
            get
            {
                return this.protocolField;
            }
            set
            {
                this.protocolField = value;
            }
        }

        /// <remarks/>
        public object sourceIpRange
        {
            get
            {
                return this.sourceIpRangeField;
            }
            set
            {
                this.sourceIpRangeField = value;
            }
        }

        /// <remarks/>
        public object destinationIpRange
        {
            get
            {
                return this.destinationIpRangeField;
            }
            set
            {
                this.destinationIpRangeField = value;
            }
        }

        /// <remarks/>
        public AclRuleListAclRulePortRange portRange
        {
            get
            {
                return this.portRangeField;
            }
            set
            {
                this.portRangeField = value;
            }
        }

        /// <remarks/>
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    public partial class AclRuleListAclRulePortRange
    {

        private string typeField;

        private ushort port1Field;

        private bool port1FieldSpecified;

        /// <remarks/>
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public ushort port1
        {
            get
            {
                return this.port1Field;
            }
            set
            {
                this.port1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool port1Specified
        {
            get
            {
                return this.port1FieldSpecified;
            }
            set
            {
                this.port1FieldSpecified = value;
            }
        }
    }


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/network", IsNullable = false)]
    public partial class AclRule
    {

        private string nameField;

        private byte positionField;

        private string actionField;

        private string protocolField;

        private AclRuleSourceIpRange sourceIpRangeField;

        private AclRuleDestinationIpRange destinationIpRangeField;

        private AclRulePortRange portRangeField;

        private string typeField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public byte position
        {
            get
            {
                return this.positionField;
            }
            set
            {
                this.positionField = value;
            }
        }

        /// <remarks/>
        public string action
        {
            get
            {
                return this.actionField;
            }
            set
            {
                this.actionField = value;
            }
        }

        /// <remarks/>
        public string protocol
        {
            get
            {
                return this.protocolField;
            }
            set
            {
                this.protocolField = value;
            }
        }

        /// <remarks/>
        public AclRuleSourceIpRange sourceIpRange
        {
            get
            {
                return this.sourceIpRangeField;
            }
            set
            {
                this.sourceIpRangeField = value;
            }
        }

        /// <remarks/>
        public AclRuleDestinationIpRange destinationIpRange
        {
            get
            {
                return this.destinationIpRangeField;
            }
            set
            {
                this.destinationIpRangeField = value;
            }
        }

        /// <remarks/>
        public AclRulePortRange portRange
        {
            get
            {
                return this.portRangeField;
            }
            set
            {
                this.portRangeField = value;
            }
        }

        /// <remarks/>
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    public partial class AclRuleSourceIpRange
    {

        private string ipAddressField;

        private string netmaskField;

        /// <remarks/>
        public string ipAddress
        {
            get
            {
                return this.ipAddressField;
            }
            set
            {
                this.ipAddressField = value;
            }
        }

        /// <remarks/>
        public string netmask
        {
            get
            {
                return this.netmaskField;
            }
            set
            {
                this.netmaskField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    public partial class AclRuleDestinationIpRange
    {

        private string ipAddressField;

        private string netmaskField;

        /// <remarks/>
        public string ipAddress
        {
            get
            {
                return this.ipAddressField;
            }
            set
            {
                this.ipAddressField = value;
            }
        }

        /// <remarks/>
        public string netmask
        {
            get
            {
                return this.netmaskField;
            }
            set
            {
                this.netmaskField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    public partial class AclRulePortRange
    {

        private string typeField;

        private ushort port1Field;

        private ushort port2Field;

        /// <remarks/>
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public ushort port1
        {
            get
            {
                return this.port1Field;
            }
            set
            {
                this.port1Field = value;
            }
        }

        /// <remarks/>
        public ushort port2
        {
            get
            {
                return this.port2Field;
            }
            set
            {
                this.port2Field = value;
            }
        }
    }


}
