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
    public partial class ServersWithBackup
    {

        private ServersWithBackupServer[] serverField;

        private byte pageNumberField;

        private byte pageCountField;

        private byte totalCountField;

        private byte pageSizeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("server")]
        public ServersWithBackupServer[] server
        {
            get
            {
                return this.serverField;
            }
            set
            {
                this.serverField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte pageNumber
        {
            get
            {
                return this.pageNumberField;
            }
            set
            {
                this.pageNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte pageCount
        {
            get
            {
                return this.pageCountField;
            }
            set
            {
                this.pageCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte totalCount
        {
            get
            {
                return this.totalCountField;
            }
            set
            {
                this.totalCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte pageSize
        {
            get
            {
                return this.pageSizeField;
            }
            set
            {
                this.pageSizeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
    public partial class ServersWithBackupServer
    {

        private string nameField;

        private string descriptionField;

        private ServersWithBackupServerOperatingSystem operatingSystemField;

        private byte cpuCountField;

        private ushort memoryMbField;

        private ServersWithBackupServerDisk[] diskField;

        private string softwareLabelField;

        private string sourceImageIdField;

        private string networkIdField;

        private string machineNameField;

        private string privateIpField;

        private string publicIpField;

        private System.DateTime createdField;

        private bool isDeployedField;

        private bool isStartedField;

        private string stateField;

        private ServersWithBackupServerMachineStatus[] machineStatusField;

        private ServersWithBackupServerBackup backupField;

        private string idField;

        private string locationField;

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
        public ServersWithBackupServerOperatingSystem operatingSystem
        {
            get
            {
                return this.operatingSystemField;
            }
            set
            {
                this.operatingSystemField = value;
            }
        }

        /// <remarks/>
        public byte cpuCount
        {
            get
            {
                return this.cpuCountField;
            }
            set
            {
                this.cpuCountField = value;
            }
        }

        /// <remarks/>
        public ushort memoryMb
        {
            get
            {
                return this.memoryMbField;
            }
            set
            {
                this.memoryMbField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("disk")]
        public ServersWithBackupServerDisk[] disk
        {
            get
            {
                return this.diskField;
            }
            set
            {
                this.diskField = value;
            }
        }

        /// <remarks/>
        public string softwareLabel
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

        /// <remarks/>
        public string sourceImageId
        {
            get
            {
                return this.sourceImageIdField;
            }
            set
            {
                this.sourceImageIdField = value;
            }
        }

        /// <remarks/>
        public string networkId
        {
            get
            {
                return this.networkIdField;
            }
            set
            {
                this.networkIdField = value;
            }
        }

        /// <remarks/>
        public string machineName
        {
            get
            {
                return this.machineNameField;
            }
            set
            {
                this.machineNameField = value;
            }
        }

        /// <remarks/>
        public string privateIp
        {
            get
            {
                return this.privateIpField;
            }
            set
            {
                this.privateIpField = value;
            }
        }

        /// <remarks/>
        public string publicIp
        {
            get
            {
                return this.publicIpField;
            }
            set
            {
                this.publicIpField = value;
            }
        }

        /// <remarks/>
        public System.DateTime created
        {
            get
            {
                return this.createdField;
            }
            set
            {
                this.createdField = value;
            }
        }

        /// <remarks/>
        public bool isDeployed
        {
            get
            {
                return this.isDeployedField;
            }
            set
            {
                this.isDeployedField = value;
            }
        }

        /// <remarks/>
        public bool isStarted
        {
            get
            {
                return this.isStartedField;
            }
            set
            {
                this.isStartedField = value;
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
        [System.Xml.Serialization.XmlElementAttribute("machineStatus")]
        public ServersWithBackupServerMachineStatus[] machineStatus
        {
            get
            {
                return this.machineStatusField;
            }
            set
            {
                this.machineStatusField = value;
            }
        }

        /// <remarks/>
        public ServersWithBackupServerBackup backup
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
    public partial class ServersWithBackupServerOperatingSystem
    {

        private string idField;

        private string displayNameField;

        private string typeField;

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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
    public partial class ServersWithBackupServerDisk
    {

        private string idField;

        private byte scsiIdField;

        private byte sizeGbField;

        private string speedField;

        private string stateField;

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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte scsiId
        {
            get
            {
                return this.scsiIdField;
            }
            set
            {
                this.scsiIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte sizeGb
        {
            get
            {
                return this.sizeGbField;
            }
            set
            {
                this.sizeGbField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string speed
        {
            get
            {
                return this.speedField;
            }
            set
            {
                this.speedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
    public partial class ServersWithBackupServerMachineStatus
    {

        private string valueField;

        private string nameField;

        /// <remarks/>
        public string value
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
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
    public partial class ServersWithBackupServerBackup
    {

        private string assetIdField;

        private string stateField;

        private string servicePlanField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string assetId
        {
            get
            {
                return this.assetIdField;
            }
            set
            {
                this.assetIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string servicePlan
        {
            get
            {
                return this.servicePlanField;
            }
            set
            {
                this.servicePlanField = value;
            }
        }
    }



   
}


