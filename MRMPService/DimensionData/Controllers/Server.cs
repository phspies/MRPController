using CloudMoveyWorkerService.CaaS;
using System;
using System.Collections.Generic;


namespace CloudMoveyWorkerService.CaaS2
{
    class WorkloadObject : Core
    {
        public WorkloadObject(DimensionData _dimensiondata) : base(_dimensiondata) { }
        public ServersType listworkloads(List<Option> options = null)
        {
            orgendpoint("/server/server");
            urloptions = options;
            urloptions.Add(new Option() { option = "pageSize", value = "1000" });
            ServersType workloads = get<ServersType>(null, true) as ServersType;
            return workloads;
        }
        public ResponseType deployworkload(DeployServerType _workload)
        {
            orgendpoint("/server/deployServer");
            ResponseType response = post<ResponseType>(_workload, false) as ResponseType;
            return response;
        }
        public ServerType getworkload(String _workload_id)
        {
            orgendpoint(String.Format("/server/server/{0}", _workload_id));
            ServerType workload = get<ServerType>(null, true) as ServerType;
            return workload;
        }
        public ResponseType deleteworkload(DeleteServerType _workload)
        {
            orgendpoint("/server/deleteServer");
            ResponseType response = post<ResponseType>(_workload, false) as ResponseType;
            return response;
        }
        public ResponseType workloadaddnic(AddNicType _nic)
        {
            orgendpoint("/server/addNic");
            ResponseType response = post<ResponseType>(_nic, false) as ResponseType;
            return response;
        }
        public ResponseType workloadremovenic(RemoveNicType _nic)
        {
            orgendpoint("/server/removeNic");
            ResponseType response = post<ResponseType>(_nic, false) as ResponseType;
            return response;
        }
        public ResponseType workloadIpChange(NotifyNicIpChangeType _ipchange)
        {
            orgendpoint("/server/notifyNicIpChange");
            ResponseType response = post<ResponseType>(_ipchange, false) as ResponseType;
            return response;
        }
    }
}



