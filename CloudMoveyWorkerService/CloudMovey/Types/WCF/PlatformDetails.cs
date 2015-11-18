using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.Portal.Sqlite.Models;
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
            List<CaaS.Models.Option> _dcoptions = new List<CaaS.Models.Option>();
            List<CaaS.Models.Option> _workloadoptions = new List<CaaS.Models.Option>();
            List<CaaS.Models.Option> _networkdomainoptions = new List<CaaS.Models.Option>();
            List<CaaS.Models.Option> _vlanoptions = new List<CaaS.Models.Option>();


            _dcoptions.Add(new CaaS.Models.Option() { option = "id", value = _datacenterId });
            DatacenterType _dc = ((DatacenterListType)_caas.datacenter().datacenters(_dcoptions)).datacenter[0];
            if (_dc.type == "MCP 2.0")
            {
                _workloadoptions.Add(new CaaS.Models.Option() { option = "datacenterId", value = _datacenterId });
                _networkdomainoptions.Add(new CaaS.Models.Option() { option = "datacenterId", value = _datacenterId });

                NetworkDomains = _caas.mcp2networkdomain().networkdomainlist(_networkdomainoptions).networkDomain;
                Workloads = _caas.mcp2workloads().listworkloads(_workloadoptions).server;
                Networks = _caas.mcp2vlans().listvlan(_vlanoptions).vlan;
            }
            else
            {
                _vlanoptions.Add(new CaaS.Models.Option() { option = "location", value = _datacenterId });
                _workloadoptions.Add(new CaaS.Models.Option() { option = "location", value = _datacenterId });

                NetworkDomains.Add(new NetworkDomainType() { description = "Default Network Domain", id = "0" });
                foreach (var workload in _caas.workload().platformworkloads(_workloadoptions).server)
                {
                    Workloads.Add(new ServerType()
                    {
                        id = workload.id,
                        description = workload.description.ToString(),
                        name = workload.name,
                        cpuCount = workload.cpuCount,
                        memoryGb = workload.memoryMb,
                        operatingSystem = new OperatingSystemType() { displayName = workload.operatingSystem.displayName, family = workload.operatingSystem.type, id = workload.operatingSystem.id },
                        datacenterId = workload.location,
                        started = workload.isStarted,
                        deployed = workload.isDeployed,
                        sourceImageId = workload.sourceImageId,
                        createTime = workload.created,
                        disk = workload.disk.Select(x => new ServerTypeDisk { id = x.id, scsiId = x.scsiId, sizeGb = x.sizeGb, speed = x.speed, state = x.state }).ToList(),
                    });
                }
                foreach (var network in _caas.network().networklist(_datacenterId).network)
                {
                    Networks.Add(new VlanType()
                    {
                        name = network.name,
                        datacenterId = network.location,
                        id = network.id,
                        privateIpv4Range = new IpRangeCidrType() { address = network.privateNet, prefixSize = 24 },
                        description = network.description
                    });
                }
                

            }

        }
    }
}
