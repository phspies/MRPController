using DD.CBU.Compute.Api.Client;
using DD.CBU.Compute.Api.Contracts.Network20;
using MRPService.LocalDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace MRPService.Portal.Models
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
            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_url), new NetworkCredential(_credential.username, _credential.password));

            NetworkDomains = CaaS.Networking.NetworkDomain.GetNetworkDomains(new DD.CBU.Compute.Api.Contracts.Requests.Network20.NetworkDomainListOptions() { DatacenterId = _datacenterId }).Result.ToList();
            Workloads = CaaS.ServerManagement.Server.GetServers(new DD.CBU.Compute.Api.Contracts.Requests.Server20.ServerListOptions() { DatacenterId = _datacenterId }).Result.ToList();
            Networks = CaaS.Networking.Vlan.GetVlans(new DD.CBU.Compute.Api.Contracts.Requests.Network20.VlanListOptions() { DatacenterId = _datacenterId }).Result.ToList();

        }
    }
}
