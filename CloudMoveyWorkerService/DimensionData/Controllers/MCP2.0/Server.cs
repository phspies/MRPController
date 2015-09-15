using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.CaaS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CloudMoveyWorkerService.CaaS2
{
    class MCP2WorkloadObject : Core
    {
        public MCP2WorkloadObject(DimensionData _dimensiondata) : base(_dimensiondata) { }

        // Paging/Ordering Optional Parameters:
        // [&pageSize=]
        // [&pageNumber=]
        // [&orderBy=]
        //Filter Optional Parameters:
        // [&id=]
        // [&datacenterId=]
        // [&networkDomainId=]
        // [&networkId=]
        // [&vlanId=]
        // [&sourceImageId=]
        // [&deployed=]
        // [&name=]
        // [&createTime=]
        // [&state=]
        // [&started=]
        // [&operatingSystemId=]
        // [&ipv6=]
        // [&privateIpv4=]
        public ServerListType listworkloads(List<Option> options = null)
        {
            orgendpoint2("/server/server");
            urloptions = options;
            ServerListType workloads = get<ServerListType>(null, true) as ServerListType;
            return workloads;
        }
        public ResponseType deployworkload(DeployServerType _workload)
        {
            orgendpoint2("/server/deployServer");
            ResponseType response = post<ResponseType>(_workload, false) as ResponseType;
            return response;
        }
        public ServerType getworkload(String _workload_id)
        {
            orgendpoint2(String.Format("/server/server/{0}", _workload_id));
            ServerType workload = get<ServerType>(null, true) as ServerType;
            return workload;
        }
        public ResponseType deleteworkload(DeleteServerType _workload)
        {
            orgendpoint2("/server/deleteServer");
            ResponseType response = post<ResponseType>(_workload, false) as ResponseType;
            return response;
        }
        public ResponseType workloadaddnic(AddNicType _nic)
        {
            orgendpoint2("/server/addNic");
            ResponseType response = post<ResponseType>(_nic, false) as ResponseType;
            return response;
        }
        public ResponseType workloadremovenic(RemoveNicType _nic)
        {
            orgendpoint2("/server/removeNic");
            ResponseType response = post<ResponseType>(_nic, false) as ResponseType;
            return response;
        }
        public ResponseType workloadIpChange(NotifyNicIpChangeType _ipchange)
        {
            orgendpoint2("/server/notifyNicIpChange");
            ResponseType response = post<ResponseType>(_ipchange, false) as ResponseType;
            return response;
        }
        public ResponseType workloadCleanFailedDeploy(CleanServerType _workload)
        {
            orgendpoint2("/server/cleanWorkload");
            ResponseType response = post<ResponseType>(_workload, false) as ResponseType;
            return response;
        }
        public ResponseType workloadEnableMonitoring(EnableServerMonitoringType _monitor)
        {
            orgendpoint2("/server/enableServerMonitoring");
            _monitor.servicePlan = "ESSENTIALS";
            ResponseType response = post<ResponseType>(_monitor, false) as ResponseType;
            return response;
        }
        public ResponseType workloadDisableMonitoring(DisableServerMonitoringType _workload)
        {
            orgendpoint2("/server/disableServerMonitoring");
            ResponseType response = post<ResponseType>(_workload, false) as ResponseType;
            return response;
        }
    }
}



