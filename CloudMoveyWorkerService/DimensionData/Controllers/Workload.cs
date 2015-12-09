using CloudMoveyWorkerService.CaaS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CaaS
{

    class WorkloadObject : Core
    {
        public WorkloadObject(DimensionData _dimensiondata) : base(_dimensiondata) { }

        public ResponseType deploy(DeployServerType _workload)
        {
            orgendpoint("/server/deployServer");
            ResponseType status = post<ResponseType>(_workload, false) as ResponseType;
            return status;
        }
        public ServersType list(List<Option> options = null)
        {
            orgendpoint("/server/server");
            urloptions = options;
            return get<ServersType>(null, true) as ServersType;
        }
        public ServerType get(string _workload_id)
        {
            orgendpoint(String.Format("/server/server/?", _workload_id));
            return get<ServerType>(null, true) as ServerType;
        }
        public ResponseType delete(DeleteServerType workload)
        {
            orgendpoint("/server/deleteServer");
            return post<ResponseType>(workload, false) as ResponseType;
        }
        public ResponseType start(StartServerType workload)
        {
            orgendpoint("/server/startServer");
            return post<ResponseType>(workload, false) as ResponseType;
        }
        public ResponseType shutdown(ShutdownServerType workload)
        {
            orgendpoint("/server/shutdownServer");
            return post<ResponseType>(workload, false) as ResponseType;
        }
        public ResponseType reboot(RebootServerType workload)
        {
            orgendpoint("/server/rebootServer");
            return post<ResponseType>(workload, false) as ResponseType;
        }
        public ResponseType reset(ResetServerType workload)
        {
            orgendpoint("/server/resetServer");
            return post<ResponseType>(workload, false) as ResponseType;
        }
        public ResponseType poweroff(PowerOffServerType workload)
        {
            orgendpoint("/server/powerOffServer");
            return post<ResponseType>(workload, false) as ResponseType;
        }
        public ResponseType reconfigure(ReconfigureServerType workload)
        {
            orgendpoint("/server/reconfigureServer");
            return post<ResponseType>(workload, false) as ResponseType;
        }
        public ResponseType updateVmwareTools(UpdateVmwareToolsType workload)
        {
            orgendpoint("/server/updateVmwareTools");
            return post<ResponseType>(workload, false) as ResponseType;
        }
        public ResponseType addnic(AddNicType workload)
        {
            orgendpoint("/server/addNic");
            return post<ResponseType>(workload, false) as ResponseType;
        }
        public ResponseType removenic(RemoveNicType workload)
        {
            orgendpoint("/server/removeNic");
            return post<ResponseType>(workload, false) as ResponseType;
        }
        public ResponseType notifyNicIpChange(NotifyNicIpChangeType workload)
        {
            orgendpoint("/server/notifyNicIpChange");
            return post<ResponseType>(workload, false) as ResponseType;
        }
        public ResponseType cleanserver(CleanServerType workload)
        {
            orgendpoint("/server/cleanServer");
            return post<ResponseType>(workload, false) as ResponseType;
        }
    }
}
