using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleTakeProxyService.DimensionData.Models
{

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://oec.api.opsource.net/schemas/server")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://oec.api.opsource.net/schemas/server", IsNullable=false)]
public partial class ImagesWithDiskSpeed {
    
    private List<ImagesWithDiskSpeedImage> imageField;
    
    private byte pageNumberField;
    
    private byte pageCountField;
    
    private byte totalCountField;
    
    private byte pageSizeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("image")]
    public List<ImagesWithDiskSpeedImage> image {
        get {
            return this.imageField;
        }
        set {
            this.imageField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte pageNumber {
        get {
            return this.pageNumberField;
        }
        set {
            this.pageNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte pageCount {
        get {
            return this.pageCountField;
        }
        set {
            this.pageCountField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte totalCount {
        get {
            return this.totalCountField;
        }
        set {
            this.totalCountField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte pageSize {
        get {
            return this.pageSizeField;
        }
        set {
            this.pageSizeField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://oec.api.opsource.net/schemas/server")]
public partial class ImagesWithDiskSpeedImage {
    
    private string nameField;
    
    private string descriptionField;
    
    private ImagesWithDiskSpeedImageOperatingSystem operatingSystemField;
    
    private byte cpuCountField;
    
    private ushort memoryMbField;
    
    private List<ImagesWithDiskSpeedImageDisk> diskField;
    
    private List<String> softwareLabelField;
    
    private ImagesWithDiskSpeedImageSource sourceField;
    
    private System.DateTime createdField;
    
    private string stateField;
    
    private string idField;
    
    private string locationField;
    
    /// <remarks/>
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    public string description {
        get {
            return this.descriptionField;
        }
        set {
            this.descriptionField = value;
        }
    }
    
    /// <remarks/>
    public ImagesWithDiskSpeedImageOperatingSystem operatingSystem {
        get {
            return this.operatingSystemField;
        }
        set {
            this.operatingSystemField = value;
        }
    }
    
    /// <remarks/>
    public byte cpuCount {
        get {
            return this.cpuCountField;
        }
        set {
            this.cpuCountField = value;
        }
    }
    
    /// <remarks/>
    public ushort memoryMb {
        get {
            return this.memoryMbField;
        }
        set {
            this.memoryMbField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("disk")]
    public List<ImagesWithDiskSpeedImageDisk> disk {
        get {
            return this.diskField;
        }
        set {
            this.diskField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("softwareLabel")]
    public List<String> softwareLabel {
        get {
            return this.softwareLabelField;
        }
        set {
            this.softwareLabelField = value;
        }
    }
    
    /// <remarks/>
    public ImagesWithDiskSpeedImageSource source {
        get {
            return this.sourceField;
        }
        set {
            this.sourceField = value;
        }
    }
    
    /// <remarks/>
    public System.DateTime created {
        get {
            return this.createdField;
        }
        set {
            this.createdField = value;
        }
    }
    
    /// <remarks/>
    public string state {
        get {
            return this.stateField;
        }
        set {
            this.stateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string location {
        get {
            return this.locationField;
        }
        set {
            this.locationField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://oec.api.opsource.net/schemas/server")]
public partial class ImagesWithDiskSpeedImageOperatingSystem {
    
    private string idField;
    
    private string displayNameField;
    
    private string typeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string displayName {
        get {
            return this.displayNameField;
        }
        set {
            this.displayNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://oec.api.opsource.net/schemas/server")]
public partial class ImagesWithDiskSpeedImageDisk {
    
    private string idField;
    
    private byte scsiIdField;
    
    private ushort sizeGbField;
    
    private string speedField;
    
    private string stateField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte scsiId {
        get {
            return this.scsiIdField;
        }
        set {
            this.scsiIdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public ushort sizeGb {
        get {
            return this.sizeGbField;
        }
        set {
            this.sizeGbField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string speed {
        get {
            return this.speedField;
        }
        set {
            this.speedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string state {
        get {
            return this.stateField;
        }
        set {
            this.stateField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://oec.api.opsource.net/schemas/server")]
public partial class ImagesWithDiskSpeedImageSource {
    
    private string typeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
}




/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/server", IsNullable = false)]
public partial class ImageNameExists
{

    private string locationField;

    private string imageNameField;

    /// <remarks/>
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

    /// <remarks/>
    public string imageName
    {
        get
        {
            return this.imageNameField;
        }
        set
        {
            this.imageNameField = value;
        }
    }
}



/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/server", IsNullable = false)]
public partial class ServerImageWithState
{

    private string idField;

    private string locationField;

    private string nameField;

    private string descriptionField;

    private ServerImageWithStateOperatingSystem operatingSystemField;

    private byte cpuCountField;

    private ushort memoryMbField;

    private byte osStorageGbField;

    private List<ServerImageWithStateAdditionalDisk> additionalDiskField;

    private List<string> softwareLabelField;

    private ServerImageWithStateSource sourceField;

    private string stateField;

    private System.DateTime deployedTimeField;

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
    public ServerImageWithStateOperatingSystem operatingSystem
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
    public byte osStorageGb
    {
        get
        {
            return this.osStorageGbField;
        }
        set
        {
            this.osStorageGbField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("additionalDisk")]
    public List<ServerImageWithStateAdditionalDisk> additionalDisk
    {
        get
        {
            return this.additionalDiskField;
        }
        set
        {
            this.additionalDiskField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("softwareLabel")]
    public List<String> softwareLabel
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
    public ServerImageWithStateSource source
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
    public System.DateTime deployedTime
    {
        get
        {
            return this.deployedTimeField;
        }
        set
        {
            this.deployedTimeField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
public partial class ServerImageWithStateOperatingSystem
{

    private string typeField;

    private string displayNameField;

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
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
public partial class ServerImageWithStateAdditionalDisk
{

    private string idField;

    private byte scsiIdField;

    private ushort diskSizeGbField;

    private string stateField;

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
    public ushort diskSizeGb
    {
        get
        {
            return this.diskSizeGbField;
        }
        set
        {
            this.diskSizeGbField = value;
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
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
public partial class ServerImageWithStateSource
{

    private string typeField;

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
[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/server", IsNullable = false)]
public partial class ModifyServerImage
{

    private string descriptionField;

    private List<ModifyServerImageDisk> diskField;

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
    [System.Xml.Serialization.XmlElementAttribute("disk")]
    public List<ModifyServerImageDisk> disk
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

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
public partial class ModifyServerImageDisk
{

    private byte scsiIdField;

    private string speedField;

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


/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/server", IsNullable = false)]
public partial class DeployServer
{

    private string nameField;

    private string descriptionField;

    private string imageIdField;

    private bool startField;

    private string administratorPasswordField;

    private string privateIpField;

    private string networkIdField;

    private List<DeployServerDisk> diskField;

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

    /// <remarks/>
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

    /// <remarks/>
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
    [System.Xml.Serialization.XmlElementAttribute("disk")]
    public List<DeployServerDisk> disk
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

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
public partial class DeployServerDisk
{

    private byte scsiIdField;

    private string speedField;

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

public partial class ModifyServer
{
    private string nameField;
    private string descriptionField;
    private string cpuCountField;
    private string memoryField;
    private string privateIpField;
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
    public string cpuCount
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
    public string memory
    {
        get
        {
            return this.memoryField;
        }
        set
        {
            this.memoryField = value;
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

}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/server", IsNullable = false)]
public partial class ChangeDiskSpeed
{

    private string speedField;

    /// <remarks/>
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


/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/server")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/server", IsNullable = false)]
public partial class ChangeDiskSize
{

    private byte newSizeGbField;

    /// <remarks/>
    public byte newSizeGb
    {
        get
        {
            return this.newSizeGbField;
        }
        set
        {
            this.newSizeGbField = value;
        }
    }
}




}
