﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.6.81.0.
// 
namespace DD.CBU.Compute.Api.Contracts.Network20
{
    using System.Xml.Serialization;

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class createSecurityGroup
    {

        private string vlanIdField;

        private string nameField;

        private string descriptionField;

        /// <remarks/>
        public string vlanId
        {
            get { return this.vlanIdField; }
            set { this.vlanIdField = value; }
        }

        /// <remarks/>
        public string name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        /// <remarks/>
        public string description
        {
            get { return this.descriptionField; }
            set { this.descriptionField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute("deleteSecurityGroup", Namespace = "urn:didata.com:api:cloud:types",
        IsNullable = false)]
    public partial class DeleteSecurityGroup
    {

        private string idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class addNicToSecurityGroup
    {

        private string nicIdField;

        private string securityGroupIdField;

        /// <remarks/>
        public string nicId
        {
            get { return this.nicIdField; }
            set { this.nicIdField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string securityGroupId
        {
            get { return this.securityGroupIdField; }
            set { this.securityGroupIdField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class removeNicFromSecurityGroup
    {

        private string nicIdField;

        private string securityGroupIdField;

        /// <remarks/>
        public string nicId
        {
            get { return this.nicIdField; }
            set { this.nicIdField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string securityGroupId
        {
            get { return this.securityGroupIdField; }
            set { this.securityGroupIdField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute("securityGroup", Namespace = "urn:didata.com:api:cloud:types",
        IsNullable = false)]
    public partial class SecurityGroupType
    {

        private string nameField;

        private string descriptionField;

        private string stateField;

        private SecurityGroupTypeNic[] nicField;

        private string vlanIdField;

        private System.DateTime createTimeField;

        private string idField;

        private string datacenterIdField;

        /// <remarks/>
        public string name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        /// <remarks/>
        public string description
        {
            get { return this.descriptionField; }
            set { this.descriptionField = value; }
        }

        /// <remarks/>
        public string state
        {
            get { return this.stateField; }
            set { this.stateField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("nic")]
        public SecurityGroupTypeNic[] nic
        {
            get { return this.nicField; }
            set { this.nicField = value; }
        }

        /// <remarks/>
        public string vlanId
        {
            get { return this.vlanIdField; }
            set { this.vlanIdField = value; }
        }

        /// <remarks/>
        public System.DateTime createTime
        {
            get { return this.createTimeField; }
            set { this.createTimeField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string datacenterId
        {
            get { return this.datacenterIdField; }
            set { this.datacenterIdField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    public partial class SecurityGroupTypeNic
    {

        private SecurityGroupTypeNicServer serverField;

        private string idField;

        private string ipv4AddressField;

        private string ipv6AddressField;

        private bool primaryField;

        /// <remarks/>
        public SecurityGroupTypeNicServer server
        {
            get { return this.serverField; }
            set { this.serverField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ipv4Address
        {
            get { return this.ipv4AddressField; }
            set { this.ipv4AddressField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ipv6Address
        {
            get { return this.ipv6AddressField; }
            set { this.ipv6AddressField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool primary
        {
            get { return this.primaryField; }
            set { this.primaryField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    public partial class SecurityGroupTypeNicServer
    {

        private string nameField;

        private string idField;

        /// <remarks/>
        public string name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class securityGroups
    {

        private SecurityGroupType[] securityGroupField;

        private int pageNumberField;

        private bool pageNumberFieldSpecified;

        private int pageCountField;

        private bool pageCountFieldSpecified;

        private int totalCountField;

        private bool totalCountFieldSpecified;

        private int pageSizeField;

        private bool pageSizeFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("securityGroup")]
        public SecurityGroupType[] securityGroup
        {
            get { return this.securityGroupField; }
            set { this.securityGroupField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int pageNumber
        {
            get { return this.pageNumberField; }
            set { this.pageNumberField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool pageNumberSpecified
        {
            get { return this.pageNumberFieldSpecified; }
            set { this.pageNumberFieldSpecified = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int pageCount
        {
            get { return this.pageCountField; }
            set { this.pageCountField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool pageCountSpecified
        {
            get { return this.pageCountFieldSpecified; }
            set { this.pageCountFieldSpecified = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int totalCount
        {
            get { return this.totalCountField; }
            set { this.totalCountField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool totalCountSpecified
        {
            get { return this.totalCountFieldSpecified; }
            set { this.totalCountFieldSpecified = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int pageSize
        {
            get { return this.pageSizeField; }
            set { this.pageSizeField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool pageSizeSpecified
        {
            get { return this.pageSizeFieldSpecified; }
            set { this.pageSizeFieldSpecified = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:didata.com:api:cloud:types")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:didata.com:api:cloud:types", IsNullable = false)]
    public partial class editSecurityGroup
    {

        private string nameField;

        private string descriptionField;

        private string idField;

        /// <remarks/>
        public string name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string description
        {
            get { return this.descriptionField; }
            set { this.descriptionField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }
    }
}
