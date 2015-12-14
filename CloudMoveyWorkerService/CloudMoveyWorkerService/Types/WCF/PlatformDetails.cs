using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CloudMoveyWorkerService.Portal.Models
{
    [DataContract]
    public class PlatformDetails
    {
        [DataMember]
        public List<ServerType> Workloads { get; set; }
        [DataMember]
        public List<NetworkDomainType> NetworkDomains { get; set; }
        [DataMember] 
        public List<VlanType> Networks { get; set; }
        public PlatformDetails(String _datacenterId, String _url, Credential _credential)
        {
            DimensionData _caas = new DimensionData(_url, _credential.username, _credential.password);
            List<Option> _dcoptions = new List<Option>();
            List<Option> _workloadoptions = new List<Option>();
            List<Option> _networkdomainoptions = new List<Option>();
            List<Option> _vlanoptions = new List<Option>();


            _dcoptions.Add(new Option() { option = "id", value = _datacenterId });
            DatacenterType _dc = ((DatacenterListType)_caas.datacenter().datacenters(_dcoptions)).datacenter[0];
            _workloadoptions.Add(new Option() { option = "datacenterId", value = _datacenterId });
            _networkdomainoptions.Add(new Option() { option = "datacenterId", value = _datacenterId });
      
            NetworkDomains = _caas.networkdomain().list(_networkdomainoptions).networkDomain;
            Workloads = _caas.workloads().list(_workloadoptions).server;
            Networks = _caas.vlans().list(_vlanoptions).vlan;

        }
    }
}
