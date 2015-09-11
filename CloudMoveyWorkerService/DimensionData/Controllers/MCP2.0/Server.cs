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
        public WorkloadListType listworkloads(List<Option> options = null)
        {
            orgendpoint2("/workload/workload");
            urloptions = options;
            WorkloadListType workloads = get<WorkloadListType>(null, true) as WorkloadListType;
            return workloads;
        }
        public ResponseType deployworkload(DeployWorkloadType _workload)
        {
            orgendpoint2("/workload/deployWorkload");
            ResponseType response = post<ResponseType>(_workload, false) as ResponseType;
            return response;
        }
        public WorkloadType getworkload(String _workload_id)
        {
            orgendpoint2(String.Format("/workload/workload/{0}",_workload_id));
            WorkloadType workload = get<WorkloadType>(null, true) as WorkloadType;
            return workload;
        }
        public ResponseType deleteworkload(DeleteWorkloadType _workload)
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
        public ResponseType workloadCleanFailedDeploy(CleanWorkloadType _workload)
        {
            orgendpoint2("/workload/cleanWorkload");
            ResponseType response = post<ResponseType>(_workload, false) as ResponseType;
            return response;
        }
        public ResponseType workloadEnableMonitoring(EnableWorkloadMonitoringType _monitor)
        {
            orgendpoint2("/workload/enableWorkloadMonitoring");
            _monitor.servicePlan = "ESSENTIALS";
            ResponseType response = post<ResponseType>(_monitor, false) as ResponseType;
            return response;
        }
        public ResponseType workloadDisableMonitoring(DisableWorkloadMonitoringType _workload)
        {
            orgendpoint2("/workload/disableWorkloadMonitoring");
            ResponseType response = post<ResponseType>(_workload, false) as ResponseType;
            return response;
        }
    }
}



