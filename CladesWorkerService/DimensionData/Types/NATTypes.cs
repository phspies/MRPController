using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CladesWorkerService.DimensionData.Models
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/network", IsNullable = false)]
    public partial class NatRules
    {

        private List<NatRulesNatRule> natRuleField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("NatRule")]
        public List<NatRulesNatRule> NatRule
        {
            get
            {
                return this.natRuleField;
            }
            set
            {
                this.natRuleField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    public partial class NatRulesNatRule
    {

        private string idField;

        private string nameField;

        private string natIpField;

        private string sourceIpField;

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
        public string natIp
        {
            get
            {
                return this.natIpField;
            }
            set
            {
                this.natIpField = value;
            }
        }

        /// <remarks/>
        public string sourceIp
        {
            get
            {
                return this.sourceIpField;
            }
            set
            {
                this.sourceIpField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/network", IsNullable = false)]
    public partial class NatRule
    {

        private string nameField;

        private string sourceIpField;

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
        public string sourceIp
        {
            get
            {
                return this.sourceIpField;
            }
            set
            {
                this.sourceIpField = value;
            }
        }
    }



}
