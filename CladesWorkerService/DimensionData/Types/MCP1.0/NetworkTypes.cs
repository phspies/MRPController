using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CaaS.Models
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/network", IsNullable = false)]
    public partial class NewNetworkWithLocation
    {

        private string nameField;

        private string descriptionField;

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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/network", IsNullable = false)]
    public partial class Network
    {

        private string idField;

        private string resourcePathField;

        private string nameField;

        private string descriptionField;

        private bool multicastField;

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
        public string resourcePath
        {
            get
            {
                return this.resourcePathField;
            }
            set
            {
                this.resourcePathField = value;
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
        public bool multicast
        {
            get
            {
                return this.multicastField;
            }
            set
            {
                this.multicastField = value;
            }
        }
    }


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://oec.api.opsource.net/schemas/network", IsNullable = false)]
    public partial class NetworkWithLocations
    {

        private List<NetworkWithLocationsNetwork> networkField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("network")]
        public List<NetworkWithLocationsNetwork> network
        {
            get
            {
                return this.networkField;
            }
            set
            {
                this.networkField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://oec.api.opsource.net/schemas/network")]
    public partial class NetworkWithLocationsNetwork
    {

        private string idField;

        private string nameField;

        private string descriptionField;

        private string locationField;

        private string privateNetField;

        private bool multicastField;

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
        public string privateNet
        {
            get
            {
                return this.privateNetField;
            }
            set
            {
                this.privateNetField = value;
            }
        }

        /// <remarks/>
        public bool multicast
        {
            get
            {
                return this.multicastField;
            }
            set
            {
                this.multicastField = value;
            }
        }
    }
    public partial class ModifyNetwork
    {
        private string nameField;
        private string descriptionField;
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

    }

}
