using DD.CBU.Compute.Api.Client;
using DD.CBU.Compute.Api.Contracts.Network20;
using System;
using System.Threading;

namespace MRMPService.MRPService.Modules.MCP
{
    class WorkloadWaitUntilComplete
    {
        public static void Invoke(ComputeApiClient _caas, Guid _moid)
        {
            ServerType deployedServer = _caas.ServerManagement.Server.GetServer(_moid).Result;
            while (deployedServer.state != "NORMAL" && deployedServer.started == false)
            {
                Thread.Sleep(5000);
                deployedServer = _caas.ServerManagement.Server.GetServer(_moid).Result;
            }
        }
    }
}
