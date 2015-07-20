using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CaaS.Models
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/server", IsNullable = false)]
    public partial class SoftwareLabels
    {

        private List<SoftwareLabelsSoftwareLabel> softwareLabelField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("softwareLabel")]
        public List<SoftwareLabelsSoftwareLabel> softwareLabel
        {
            get
            {
                return this.softwareLabelField;
            }
            set
            {
                this.softwareLabelField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
    public partial class SoftwareLabelsSoftwareLabel
    {

        private string displayNameField;

        private string descriptionField;

        private string pricedPerField;

        private ushort runningUnitsField;

        private ushort stoppedUnitsField;

        private string idField;

        /// <remarks/>
        public string displayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public string pricedPer
        {
            get
            {
                return this.pricedPerField;
            }
            set
            {
                this.pricedPerField = value;
            }
        }

        /// <remarks/>
        public ushort runningUnits
        {
            get
            {
                return this.runningUnitsField;
            }
            set
            {
                this.runningUnitsField = value;
            }
        }

        /// <remarks/>
        public ushort stoppedUnits
        {
            get
            {
                return this.stoppedUnitsField;
            }
            set
            {
                this.stoppedUnitsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
    }


}
