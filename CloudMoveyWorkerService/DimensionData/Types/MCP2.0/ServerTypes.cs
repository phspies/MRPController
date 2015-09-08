namespace CloudMoveyWorkerService.CaaS
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;
    using System.Collections.Generic;

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute("server", Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class ServerType
    {

        private string nameField;
        private string descriptionField;
        private OperatingSystemType operatingSystemField;
        private int cpuCountField;
        private int memoryGbField;
        private List<ServerTypeDisk> diskField;
        private ServerTypeNetworkInfo networkInfoField;
        private ServerNic nicField;
        private ServerTypeBackup backupField;
        private ServerTypeMonitoring monitoringField;
        private List<string> softwareLabelField;
        private string sourceImageIdField;
        private System.DateTime createTimeField;
        private bool deployedField;
        private bool startedField;
        private string stateField;
        private ProgressType progressField;
        private List<ServerTypeMachineStatus> machineStatusField;
        private string idField;
        private string datacenterIdField;
        public ServerType()
        {
            this.machineStatusField = new List<ServerTypeMachineStatus>();
            this.progressField = new ProgressType();
            this.softwareLabelField = new List<string>();
            this.networkInfoField = new ServerTypeNetworkInfo();
            this.monitoringField = new ServerTypeMonitoring();
            this.backupField = new ServerTypeBackup();
            this.diskField = new List<ServerTypeDisk>();
            this.operatingSystemField = new OperatingSystemType();
        }

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

        public OperatingSystemType operatingSystem
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

        public int cpuCount
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

        public int memoryGb
        {
            get
            {
                return this.memoryGbField;
            }
            set
            {
                this.memoryGbField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("disk")]
        public List<ServerTypeDisk> disk
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
        public ServerNic nic
        {
            get
            {
                return this.nicField;
            }
            set
            {
                this.nicField = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("networkInfo", typeof(ServerTypeNetworkInfo))]
        public ServerTypeNetworkInfo networkInfo
        {
            get
            {
                return this.networkInfoField;
            }
            set
            {
                this.networkInfoField = value;
            }
        }

        public ServerTypeBackup backup
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

        public ServerTypeMonitoring monitoring
        {
            get
            {
                return this.monitoringField;
            }
            set
            {
                this.monitoringField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("softwareLabel")]
        public List<string> softwareLabel
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

        public System.DateTime createTime
        {
            get
            {
                return this.createTimeField;
            }
            set
            {
                this.createTimeField = value;
            }
        }

        public bool deployed
        {
            get
            {
                return this.deployedField;
            }
            set
            {
                this.deployedField = value;
            }
        }

        public bool started
        {
            get
            {
                return this.startedField;
            }
            set
            {
                this.startedField = value;
            }
        }

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

        public ProgressType progress
        {
            get
            {
                return this.progressField;
            }
            set
            {
                this.progressField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("machineStatus")]
        public List<ServerTypeMachineStatus> machineStatus
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

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string datacenterId
        {
            get
            {
                return this.datacenterIdField;
            }
            set
            {
                this.datacenterIdField = value;
            }
        }
    }
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    public partial class ServerNic
    {

        private string idField;

        private string privateIpv4Field;

        private string networkIdField;

        private string networkNameField;

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
        public string privateIpv4
        {
            get
            {
                return this.privateIpv4Field;
            }
            set
            {
                this.privateIpv4Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string networkName
        {
            get
            {
                return this.networkNameField;
            }
            set
            {
                this.networkNameField = value;
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    public partial class ServerTypeDisk
    {

        private string idField;

        private int scsiIdField;

        private int sizeGbField;

        private string speedField;

        private string stateField;

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

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int scsiId
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

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int sizeGb
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

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    public partial class ServerTypeNetworkInfo
    {

        private NicType primaryNicField;

        private List<NicType> additionalNicField;

        private string networkDomainIdField;

        public ServerTypeNetworkInfo()
        {
            this.additionalNicField = new List<NicType>();
            this.primaryNicField = new NicType();
        }

        public NicType primaryNic
        {
            get
            {
                return this.primaryNicField;
            }
            set
            {
                this.primaryNicField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("additionalNic")]
        public List<NicType> additionalNic
        {
            get
            {
                return this.additionalNicField;
            }
            set
            {
                this.additionalNicField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string networkDomainId
        {
            get
            {
                return this.networkDomainIdField;
            }
            set
            {
                this.networkDomainIdField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:didata.com:api:cloud:types", IsNullable = true)]
    public partial class NicType
    {

        private string idField;

        private string privateIpv4Field;

        private string ipv6Field;

        private string vlanIdField;

        private string vlanNameField;

        private string macAddressField;

        private string stateField;

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

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string privateIpv4
        {
            get
            {
                return this.privateIpv4Field;
            }
            set
            {
                this.privateIpv4Field = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ipv6
        {
            get
            {
                return this.ipv6Field;
            }
            set
            {
                this.ipv6Field = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string vlanId
        {
            get
            {
                return this.vlanIdField;
            }
            set
            {
                this.vlanIdField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string vlanName
        {
            get
            {
                return this.vlanNameField;
            }
            set
            {
                this.vlanNameField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string macAddress
        {
            get
            {
                return this.macAddressField;
            }
            set
            {
                this.macAddressField = value;
            }
        }

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

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    public partial class ServerTypeNic
    {

        private string idField;

        private string privateIpv4Field;

        private string networkIdField;

        private string networkNameField;

        private string macAddressField;

        private string stateField;

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

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string privateIpv4
        {
            get
            {
                return this.privateIpv4Field;
            }
            set
            {
                this.privateIpv4Field = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
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

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string networkName
        {
            get
            {
                return this.networkNameField;
            }
            set
            {
                this.networkNameField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string macAddress
        {
            get
            {
                return this.macAddressField;
            }
            set
            {
                this.macAddressField = value;
            }
        }

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

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    public partial class ServerTypeBackup
    {

        private string assetIdField;

        private string servicePlanField;

        private string stateField;

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

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    public partial class ServerTypeMonitoring
    {

        private string monitoringIdField;

        private string servicePlanField;

        private string stateField;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string monitoringId
        {
            get
            {
                return this.monitoringIdField;
            }
            set
            {
                this.monitoringIdField = value;
            }
        }

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

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    public partial class ServerTypeMachineStatus
    {

        private string nameField;

        private string valueField;

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

        [System.Xml.Serialization.XmlAttributeAttribute()]
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
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute("servers", Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class ServerListType
    {

        private List<ServerType> serverField;

        private int pageNumberField;

        private bool pageNumberFieldSpecified;

        private int pageCountField;

        private bool pageCountFieldSpecified;

        private int totalCountField;

        private bool totalCountFieldSpecified;

        private int pageSizeField;

        private bool pageSizeFieldSpecified;

        public ServerListType()
        {
            this.serverField = new List<ServerType>();
        }

        [System.Xml.Serialization.XmlElementAttribute("Server")]
        public List<ServerType> server
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

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int pageNumber
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool pageNumberSpecified
        {
            get
            {
                return this.pageNumberFieldSpecified;
            }
            set
            {
                this.pageNumberFieldSpecified = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int pageCount
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool pageCountSpecified
        {
            get
            {
                return this.pageCountFieldSpecified;
            }
            set
            {
                this.pageCountFieldSpecified = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int totalCount
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool totalCountSpecified
        {
            get
            {
                return this.totalCountFieldSpecified;
            }
            set
            {
                this.totalCountFieldSpecified = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int pageSize
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool pageSizeSpecified
        {
            get
            {
                return this.pageSizeFieldSpecified;
            }
            set
            {
                this.pageSizeFieldSpecified = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute("deployServer", Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class DeployServerType
    {

        private string nameField;

        private string descriptionField;

        private string imageIdField;

        private bool startField;

        private string administratorPasswordField;

        private object itemField;

        private List<DeployServerTypeDisk> diskField;

        public DeployServerType()
        {
            this.diskField = new List<DeployServerTypeDisk>();
        }

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

        public string imageId
        {
            get
            {
                return this.imageIdField;
            }
            set
            {
                this.imageIdField = value;
            }
        }

        public bool start
        {
            get
            {
                return this.startField;
            }
            set
            {
                this.startField = value;
            }
        }

        public string administratorPassword
        {
            get
            {
                return this.administratorPasswordField;
            }
            set
            {
                this.administratorPasswordField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("network", typeof(DeployServerTypeNetwork))]
        [System.Xml.Serialization.XmlElementAttribute("networkInfo", typeof(DeployServerTypeNetworkInfo))]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("disk")]
        public List<DeployServerTypeDisk> disk
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
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    public partial class DeployServerTypeNetwork
    {

        private string itemField;

        private ItemChoiceType itemElementNameField;

        [System.Xml.Serialization.XmlElementAttribute("networkId", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("privateIpv4", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public string Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType ItemElementName
        {
            get
            {
                return this.itemElementNameField;
            }
            set
            {
                this.itemElementNameField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types", IncludeInSchema = false)]
    public enum ItemChoiceType
    {

        /// <remarks/>
        networkId,

        /// <remarks/>
        privateIpv4,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    public partial class DeployServerTypeNetworkInfo
    {

        private VlanIdOrPrivateIpType primaryNicField;

        private List<VlanIdOrPrivateIpType> additionalNicField;

        private string networkDomainIdField;

        public DeployServerTypeNetworkInfo()
        {
            this.additionalNicField = new List<VlanIdOrPrivateIpType>();
            this.primaryNicField = new VlanIdOrPrivateIpType();
        }

        public VlanIdOrPrivateIpType primaryNic
        {
            get
            {
                return this.primaryNicField;
            }
            set
            {
                this.primaryNicField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("additionalNic")]
        public List<VlanIdOrPrivateIpType> additionalNic
        {
            get
            {
                return this.additionalNicField;
            }
            set
            {
                this.additionalNicField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string networkDomainId
        {
            get
            {
                return this.networkDomainIdField;
            }
            set
            {
                this.networkDomainIdField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:didata.com:api:cloud:types", IsNullable = true)]
    public partial class VlanIdOrPrivateIpType
    {

        private string itemField;

        private ItemChoiceType1 itemElementNameField;

        [System.Xml.Serialization.XmlElementAttribute("privateIpv4", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("vlanId", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public string Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType1 ItemElementName
        {
            get
            {
                return this.itemElementNameField;
            }
            set
            {
                this.itemElementNameField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types", IncludeInSchema = false)]
    public enum ItemChoiceType1
    {

        /// <remarks/>
        privateIpv4,

        /// <remarks/>
        vlanId,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    public partial class DeployServerTypeDisk
    {

        private ushort scsiIdField;

        private string speedField;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort scsiId
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
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute("notifyNicIpChange", Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class NotifyNicIpChangeType
    {

        private string privateIpv4Field;

        private string ipv6Field;

        private string nicIdField;

        public string privateIpv4
        {
            get
            {
                return this.privateIpv4Field;
            }
            set
            {
                this.privateIpv4Field = value;
            }
        }

        public string ipv6
        {
            get
            {
                return this.ipv6Field;
            }
            set
            {
                this.ipv6Field = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string nicId
        {
            get
            {
                return this.nicIdField;
            }
            set
            {
                this.nicIdField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute("deleteServer", Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class DeleteServerType
    {

        private string idField;

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

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute("cleanServer", Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class CleanServerType
    {

        private string idField;

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

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute("removeNic", Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class RemoveNicType
    {

        private string idField;

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

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute("addNic", Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class AddNicType
    {

        private string serverIdField;

        private VlanIdOrPrivateIpType nicField;

        public AddNicType()
        {
            this.nicField = new VlanIdOrPrivateIpType();
        }

        public string serverId
        {
            get
            {
                return this.serverIdField;
            }
            set
            {
                this.serverIdField = value;
            }
        }

        public VlanIdOrPrivateIpType nic
        {
            get
            {
                return this.nicField;
            }
            set
            {
                this.nicField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute("enableServerMonitoring", Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class EnableServerMonitoringType
    {

        private string servicePlanField;

        private string idField;

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

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute("disableServerMonitoring", Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class DisableServerMonitoringType
    {

        private string idField;

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
