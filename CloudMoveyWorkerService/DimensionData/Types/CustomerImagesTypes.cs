using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CaaS.Models
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
    [System.Xml.Serialization.XmlRootAttribute("ImagesWithDiskSpeed", Namespace = "http://oec.api.opsource.net/schemas/server", IsNullable = false)]
    public partial class CustomerImagesWithDiskSpeed
    {

        private List<CustomerImagesWithDiskSpeedImage> imageField;

        private byte pageNumberField;

        private byte pageCountField;

        private byte totalCountField;

        private byte pageSizeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("image")]
        public List<CustomerImagesWithDiskSpeedImage> image
        {
            get
            {
                return this.imageField;
            }
            set
            {
                this.imageField = value;
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
    public partial class CustomerImagesWithDiskSpeedImage
    {

        private string nameField;

        private string descriptionField;

        private CustomerImagesWithDiskSpeedImageOperatingSystem operatingSystemField;

        private byte cpuCountField;

        private uint memoryMbField;

        private List<CustomerImagesWithDiskSpeedImageDisk> diskField;

        private CustomerImagesWithDiskSpeedImageSource sourceField;

        private System.DateTime createdField;

        private string stateField;

        private CustomerImagesWithDiskSpeedImageStatus statusField;

        private List<CustomerImagesWithDiskSpeedImageMachineStatus> machineStatusField;

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
        public CustomerImagesWithDiskSpeedImageOperatingSystem operatingSystem
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
        public uint memoryMb
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
        public List<CustomerImagesWithDiskSpeedImageDisk> disk
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
        public CustomerImagesWithDiskSpeedImageSource source
        {
            get
            {
                return this.sourceField;
            }
            set
            {
                this.sourceField = value;
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
        public CustomerImagesWithDiskSpeedImageStatus status
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
        [System.Xml.Serialization.XmlElementAttribute("machineStatus")]
        public List<CustomerImagesWithDiskSpeedImageMachineStatus> machineStatus
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
    public partial class CustomerImagesWithDiskSpeedImageOperatingSystem
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
    public partial class CustomerImagesWithDiskSpeedImageDisk
    {

        private string idField;

        private byte scsiIdField;

        private ushort sizeGbField;

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
        public ushort sizeGb
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
    public partial class CustomerImagesWithDiskSpeedImageSource
    {

        private List<CustomerImagesWithDiskSpeedImageSourceArtifact> artifactField;

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("artifact")]
        public List<CustomerImagesWithDiskSpeedImageSourceArtifact> artifact
        {
            get
            {
                return this.artifactField;
            }
            set
            {
                this.artifactField = value;
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
    public partial class CustomerImagesWithDiskSpeedImageSourceArtifact
    {

        private string typeField;

        private string valueField;

        private System.DateTime dateField;

        private bool dateFieldSpecified;

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

        /// <remarks/>
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool dateSpecified
        {
            get
            {
                return this.dateFieldSpecified;
            }
            set
            {
                this.dateFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
    public partial class CustomerImagesWithDiskSpeedImageStatus
    {

        private string actionField;

        private System.DateTime requestTimeField;

        private string userNameField;

        private byte numberOfStepsField;

        private System.DateTime updateTimeField;

        private CustomerImagesWithDiskSpeedImageStatusStep stepField;

        private string failureReasonField;

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
        public System.DateTime requestTime
        {
            get
            {
                return this.requestTimeField;
            }
            set
            {
                this.requestTimeField = value;
            }
        }

        /// <remarks/>
        public string userName
        {
            get
            {
                return this.userNameField;
            }
            set
            {
                this.userNameField = value;
            }
        }

        /// <remarks/>
        public byte numberOfSteps
        {
            get
            {
                return this.numberOfStepsField;
            }
            set
            {
                this.numberOfStepsField = value;
            }
        }

        /// <remarks/>
        public System.DateTime updateTime
        {
            get
            {
                return this.updateTimeField;
            }
            set
            {
                this.updateTimeField = value;
            }
        }

        /// <remarks/>
        public CustomerImagesWithDiskSpeedImageStatusStep step
        {
            get
            {
                return this.stepField;
            }
            set
            {
                this.stepField = value;
            }
        }

        /// <remarks/>
        public string failureReason
        {
            get
            {
                return this.failureReasonField;
            }
            set
            {
                this.failureReasonField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
    public partial class CustomerImagesWithDiskSpeedImageStatusStep
    {

        private string nameField;

        private byte numberField;

        private byte percentCompleteField;

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
        public byte number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }

        /// <remarks/>
        public byte percentComplete
        {
            get
            {
                return this.percentCompleteField;
            }
            set
            {
                this.percentCompleteField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
    public partial class CustomerImagesWithDiskSpeedImageMachineStatus
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


}
