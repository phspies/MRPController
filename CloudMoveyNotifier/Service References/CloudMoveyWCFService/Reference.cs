﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CloudMoveyNotifier.CloudMoveyWCFService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CloudMoveyNotifier_WCFInterface", Namespace="http://schemas.datacontract.org/2004/07/CloudMoveyWorkerService.WCF")]
    [System.SerializableAttribute()]
    public partial class CloudMoveyNotifier_WCFInterface : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string agentIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int currentJobsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string versionNumberField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string agentId {
            get {
                return this.agentIdField;
            }
            set {
                if ((object.ReferenceEquals(this.agentIdField, value) != true)) {
                    this.agentIdField = value;
                    this.RaisePropertyChanged("agentId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int currentJobs {
            get {
                return this.currentJobsField;
            }
            set {
                if ((this.currentJobsField.Equals(value) != true)) {
                    this.currentJobsField = value;
                    this.RaisePropertyChanged("currentJobs");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string versionNumber {
            get {
                return this.versionNumberField;
            }
            set {
                if ((object.ReferenceEquals(this.versionNumberField, value) != true)) {
                    this.versionNumberField = value;
                    this.RaisePropertyChanged("versionNumber");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="platform", Namespace="http://schemas.datacontract.org/2004/07/CloudMoveyWorkerService.CloudMovey.Sqlite" +
        ".Models")]
    [System.SerializableAttribute()]
    public partial class platform : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string datacenterField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string descriptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string idField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int max_coresField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int max_disk_sizeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int max_disksField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int max_memoryField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int max_networksField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string passwordField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte typeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string urlField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string usernameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string vendorField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string datacenter {
            get {
                return this.datacenterField;
            }
            set {
                if ((object.ReferenceEquals(this.datacenterField, value) != true)) {
                    this.datacenterField = value;
                    this.RaisePropertyChanged("datacenter");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string description {
            get {
                return this.descriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.descriptionField, value) != true)) {
                    this.descriptionField = value;
                    this.RaisePropertyChanged("description");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string id {
            get {
                return this.idField;
            }
            set {
                if ((object.ReferenceEquals(this.idField, value) != true)) {
                    this.idField = value;
                    this.RaisePropertyChanged("id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int max_cores {
            get {
                return this.max_coresField;
            }
            set {
                if ((this.max_coresField.Equals(value) != true)) {
                    this.max_coresField = value;
                    this.RaisePropertyChanged("max_cores");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int max_disk_size {
            get {
                return this.max_disk_sizeField;
            }
            set {
                if ((this.max_disk_sizeField.Equals(value) != true)) {
                    this.max_disk_sizeField = value;
                    this.RaisePropertyChanged("max_disk_size");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int max_disks {
            get {
                return this.max_disksField;
            }
            set {
                if ((this.max_disksField.Equals(value) != true)) {
                    this.max_disksField = value;
                    this.RaisePropertyChanged("max_disks");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int max_memory {
            get {
                return this.max_memoryField;
            }
            set {
                if ((this.max_memoryField.Equals(value) != true)) {
                    this.max_memoryField = value;
                    this.RaisePropertyChanged("max_memory");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int max_networks {
            get {
                return this.max_networksField;
            }
            set {
                if ((this.max_networksField.Equals(value) != true)) {
                    this.max_networksField = value;
                    this.RaisePropertyChanged("max_networks");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string password {
            get {
                return this.passwordField;
            }
            set {
                if ((object.ReferenceEquals(this.passwordField, value) != true)) {
                    this.passwordField = value;
                    this.RaisePropertyChanged("password");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte type {
            get {
                return this.typeField;
            }
            set {
                if ((this.typeField.Equals(value) != true)) {
                    this.typeField = value;
                    this.RaisePropertyChanged("type");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string url {
            get {
                return this.urlField;
            }
            set {
                if ((object.ReferenceEquals(this.urlField, value) != true)) {
                    this.urlField = value;
                    this.RaisePropertyChanged("url");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string username {
            get {
                return this.usernameField;
            }
            set {
                if ((object.ReferenceEquals(this.usernameField, value) != true)) {
                    this.usernameField = value;
                    this.RaisePropertyChanged("username");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string vendor {
            get {
                return this.vendorField;
            }
            set {
                if ((object.ReferenceEquals(this.vendorField, value) != true)) {
                    this.vendorField = value;
                    this.RaisePropertyChanged("vendor");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CloudMoveyWCFService.ICloudMoveyService")]
    public interface ICloudMoveyService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICloudMoveyService/CollectionInformation", ReplyAction="http://tempuri.org/ICloudMoveyService/CollectionInformationResponse")]
        CloudMoveyNotifier.CloudMoveyWCFService.CloudMoveyNotifier_WCFInterface CollectionInformation();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICloudMoveyService/CollectionInformation", ReplyAction="http://tempuri.org/ICloudMoveyService/CollectionInformationResponse")]
        System.Threading.Tasks.Task<CloudMoveyNotifier.CloudMoveyWCFService.CloudMoveyNotifier_WCFInterface> CollectionInformationAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICloudMoveyService/ListPlatforms", ReplyAction="http://tempuri.org/ICloudMoveyService/ListPlatformsResponse")]
        CloudMoveyNotifier.CloudMoveyWCFService.platform[] ListPlatforms();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICloudMoveyService/ListPlatforms", ReplyAction="http://tempuri.org/ICloudMoveyService/ListPlatformsResponse")]
        System.Threading.Tasks.Task<CloudMoveyNotifier.CloudMoveyWCFService.platform[]> ListPlatformsAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICloudMoveyService/AddPlatform", ReplyAction="http://tempuri.org/ICloudMoveyService/AddPlatformResponse")]
        CloudMoveyNotifier.CloudMoveyWCFService.platform AddPlatform(CloudMoveyNotifier.CloudMoveyWCFService.platform _addplatform);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICloudMoveyService/AddPlatform", ReplyAction="http://tempuri.org/ICloudMoveyService/AddPlatformResponse")]
        System.Threading.Tasks.Task<CloudMoveyNotifier.CloudMoveyWCFService.platform> AddPlatformAsync(CloudMoveyNotifier.CloudMoveyWCFService.platform _addplatform);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICloudMoveyService/UpdatePlatform", ReplyAction="http://tempuri.org/ICloudMoveyService/UpdatePlatformResponse")]
        CloudMoveyNotifier.CloudMoveyWCFService.platform UpdatePlatform(CloudMoveyNotifier.CloudMoveyWCFService.platform _updateplatform);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICloudMoveyService/UpdatePlatform", ReplyAction="http://tempuri.org/ICloudMoveyService/UpdatePlatformResponse")]
        System.Threading.Tasks.Task<CloudMoveyNotifier.CloudMoveyWCFService.platform> UpdatePlatformAsync(CloudMoveyNotifier.CloudMoveyWCFService.platform _updateplatform);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICloudMoveyService/DestroyPlatform", ReplyAction="http://tempuri.org/ICloudMoveyService/DestroyPlatformResponse")]
        bool DestroyPlatform(CloudMoveyNotifier.CloudMoveyWCFService.platform _destroyplatform);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICloudMoveyService/DestroyPlatform", ReplyAction="http://tempuri.org/ICloudMoveyService/DestroyPlatformResponse")]
        System.Threading.Tasks.Task<bool> DestroyPlatformAsync(CloudMoveyNotifier.CloudMoveyWCFService.platform _destroyplatform);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICloudMoveyServiceChannel : CloudMoveyNotifier.CloudMoveyWCFService.ICloudMoveyService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CloudMoveyServiceClient : System.ServiceModel.ClientBase<CloudMoveyNotifier.CloudMoveyWCFService.ICloudMoveyService>, CloudMoveyNotifier.CloudMoveyWCFService.ICloudMoveyService {
        
        public CloudMoveyServiceClient() {
        }
        
        public CloudMoveyServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CloudMoveyServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CloudMoveyServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CloudMoveyServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public CloudMoveyNotifier.CloudMoveyWCFService.CloudMoveyNotifier_WCFInterface CollectionInformation() {
            return base.Channel.CollectionInformation();
        }
        
        public System.Threading.Tasks.Task<CloudMoveyNotifier.CloudMoveyWCFService.CloudMoveyNotifier_WCFInterface> CollectionInformationAsync() {
            return base.Channel.CollectionInformationAsync();
        }
        
        public CloudMoveyNotifier.CloudMoveyWCFService.platform[] ListPlatforms() {
            return base.Channel.ListPlatforms();
        }
        
        public System.Threading.Tasks.Task<CloudMoveyNotifier.CloudMoveyWCFService.platform[]> ListPlatformsAsync() {
            return base.Channel.ListPlatformsAsync();
        }
        
        public CloudMoveyNotifier.CloudMoveyWCFService.platform AddPlatform(CloudMoveyNotifier.CloudMoveyWCFService.platform _addplatform) {
            return base.Channel.AddPlatform(_addplatform);
        }
        
        public System.Threading.Tasks.Task<CloudMoveyNotifier.CloudMoveyWCFService.platform> AddPlatformAsync(CloudMoveyNotifier.CloudMoveyWCFService.platform _addplatform) {
            return base.Channel.AddPlatformAsync(_addplatform);
        }
        
        public CloudMoveyNotifier.CloudMoveyWCFService.platform UpdatePlatform(CloudMoveyNotifier.CloudMoveyWCFService.platform _updateplatform) {
            return base.Channel.UpdatePlatform(_updateplatform);
        }
        
        public System.Threading.Tasks.Task<CloudMoveyNotifier.CloudMoveyWCFService.platform> UpdatePlatformAsync(CloudMoveyNotifier.CloudMoveyWCFService.platform _updateplatform) {
            return base.Channel.UpdatePlatformAsync(_updateplatform);
        }
        
        public bool DestroyPlatform(CloudMoveyNotifier.CloudMoveyWCFService.platform _destroyplatform) {
            return base.Channel.DestroyPlatform(_destroyplatform);
        }
        
        public System.Threading.Tasks.Task<bool> DestroyPlatformAsync(CloudMoveyNotifier.CloudMoveyWCFService.platform _destroyplatform) {
            return base.Channel.DestroyPlatformAsync(_destroyplatform);
        }
    }
}
