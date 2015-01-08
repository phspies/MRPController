using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleTakeProxyService.DimensionData.Models
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/datacenter")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/datacenter", IsNullable = false)]
    public partial class DatacentersWithMaintenanceStatus
    {

        private List<DatacentersWithMaintenanceStatusDatacenter> datacenterField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("datacenter")]
        public List<DatacentersWithMaintenanceStatusDatacenter> datacenter
        {
            get
            {
                return this.datacenterField;
            }
            set
            {
                this.datacenterField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/datacenter")]
    public partial class DatacentersWithMaintenanceStatusDatacenter
    {

        private string displayNameField;

        private string cityField;

        private string stateField;

        private string countryField;

        private string vpnUrlField;

        private DatacentersWithMaintenanceStatusDatacenterNetworking networkingField;

        private DatacentersWithMaintenanceStatusDatacenterHypervisor hypervisorField;

        private DatacentersWithMaintenanceStatusDatacenterBackup backupField;

        private bool defaultField;

        private string locationField;

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
        public string city
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public string vpnUrl
        {
            get
            {
                return this.vpnUrlField;
            }
            set
            {
                this.vpnUrlField = value;
            }
        }

        /// <remarks/>
        public DatacentersWithMaintenanceStatusDatacenterNetworking networking
        {
            get
            {
                return this.networkingField;
            }
            set
            {
                this.networkingField = value;
            }
        }

        /// <remarks/>
        public DatacentersWithMaintenanceStatusDatacenterHypervisor hypervisor
        {
            get
            {
                return this.hypervisorField;
            }
            set
            {
                this.hypervisorField = value;
            }
        }

        /// <remarks/>
        public DatacentersWithMaintenanceStatusDatacenterBackup backup
        {
            get
            {
                return this.backupField;
            }
            set
            {
                this.backupField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public bool @default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/datacenter")]
    public partial class DatacentersWithMaintenanceStatusDatacenterNetworking
    {

        private DatacentersWithMaintenanceStatusDatacenterNetworkingProperty propertyField;

        private string typeField;

        private string maintenanceStatusField;

        /// <remarks/>
        public DatacentersWithMaintenanceStatusDatacenterNetworkingProperty property
        {
            get
            {
                return this.propertyField;
            }
            set
            {
                this.propertyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
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
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string maintenanceStatus
        {
            get
            {
                return this.maintenanceStatusField;
            }
            set
            {
                this.maintenanceStatusField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/datacenter")]
    public partial class DatacentersWithMaintenanceStatusDatacenterNetworkingProperty
    {

        private string nameField;

        private byte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/datacenter")]
    public partial class DatacentersWithMaintenanceStatusDatacenterHypervisor
    {

        private List<DatacentersWithMaintenanceStatusDatacenterHypervisorDiskSpeed> diskSpeedField;

        private List<DatacentersWithMaintenanceStatusDatacenterHypervisorProperty> propertyField;

        private string typeField;

        private string maintenanceStatusField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("diskSpeed")]
        public List<DatacentersWithMaintenanceStatusDatacenterHypervisorDiskSpeed> diskSpeed
        {
            get
            {
                return this.diskSpeedField;
            }
            set
            {
                this.diskSpeedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("property")]
        public List<DatacentersWithMaintenanceStatusDatacenterHypervisorProperty> property
        {
            get
            {
                return this.propertyField;
            }
            set
            {
                this.propertyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
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
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string maintenanceStatus
        {
            get
            {
                return this.maintenanceStatusField;
            }
            set
            {
                this.maintenanceStatusField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/datacenter")]
    public partial class DatacentersWithMaintenanceStatusDatacenterHypervisorDiskSpeed
    {

        private string displayNameField;

        private string abbreviationField;

        private string descriptionField;

        private string idField;

        private bool defaultField;

        private bool availableField;

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
        public string abbreviation
        {
            get
            {
                return this.abbreviationField;
            }
            set
            {
                this.abbreviationField = value;
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
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
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
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public bool @default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public bool available
        {
            get
            {
                return this.availableField;
            }
            set
            {
                this.availableField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/datacenter")]
    public partial class DatacentersWithMaintenanceStatusDatacenterHypervisorProperty
    {

        private string nameField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/datacenter")]
    public partial class DatacentersWithMaintenanceStatusDatacenterBackup
    {

        private string typeField;

        private string maintenanceStatusField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
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
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string maintenanceStatus
        {
            get
            {
                return this.maintenanceStatusField;
            }
            set
            {
                this.maintenanceStatusField = value;
            }
        }
    }


}
