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
            orgendpoint2("/workload/workload");
            urloptions = options;
            ServerListType workloads = get<ServerListType>(null, true) as ServerListType;
            return workloads;
        }
        public ResponseType deployworkload(DeployServerType _workload)
        {
            orgendpoint2("/workload/deployWorkload");
            ResponseType response = post<ResponseType>(_workload, false) as ResponseType;
            return response;
        }
        public ServerType getworkload(String _workload_id)
        {
            orgendpoint2(String.Format("/workload/workload/{0}",_workload_id));
            ServerType workload = get<ServerType>(null, true) as ServerType;
            return workload;
        }
        public ResponseType deleteworkload(DeleteServerType _workload)
        {
            orgendpoint2("/workload/deleteWorkload");
            ResponseType response = post<ResponseType>(_workload, false) as ResponseType;
            return response;
        }
        public ResponseType workloadaddnic(AddNicType _nic)
        {
            orgendpoint2("/workload/addNic");
            ResponseType response = post<ResponseType>(_nic, false) as ResponseType;
            return response;
        }
        public ResponseType workloadremovenic(RemoveNicType _nic)
        {
            orgendpoint2("/workload/removeNic");
            ResponseType response = post<ResponseType>(_nic, false) as ResponseType;
            return response;
        }
        public ResponseType workloadIpChange(NotifyNicIpChangeType _ipchange)
        {
            orgendpoint2("/workload/notifyNicIpChange");
            ResponseType response = post<ResponseType>(_ipchange, false) as ResponseType;
            return response;
        }
        public ResponseType workloadCleanFailedDeploy(CleanServerType _workload)
        {
            orgendpoint2("/workload/cleanWorkload");
            ResponseType response = post<ResponseType>(_workload, false) as ResponseType;
            return response;
        }
        public ResponseType workloadEnableMonitoring(EnableServerMonitoringType _monitor)
        {
            orgendpoint2("/workload/enableWorkloadMonitoring");
            _monitor.servicePlan = "ESSENTIALS";
            ResponseType response = post<ResponseType>(_monitor, false) as ResponseType;
            return response;
        }
        public ResponseType workloadDisableMonitoring(DisableServerMonitoringType _workload)
        {
            orgendpoint2("/workload/disableWorkloadMonitoring");
            ResponseType response = post<ResponseType>(_workload, false) as ResponseType;
            return response;
        }
    }
}



