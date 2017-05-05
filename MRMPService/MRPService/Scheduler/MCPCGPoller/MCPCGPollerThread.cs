using MRMPService.MRMPService.Log;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MRMPService.Modules.MRMPPortal.Contracts;
using System.Linq;
using System.Collections.Generic;
using MRMPService.Scheduler.MCPCGCollection;

namespace MRMPService.Scheduler.DTPollerCollection
{
    class MCPCGPollerThread
    {
        public async void Start()
        {
            while (true)
            {
                try
                {
                    DateTime _next_poller_run = DateTime.UtcNow.AddSeconds(MRMPServiceBase.mcp_cg_polling_interval);
                    Stopwatch sw = Stopwatch.StartNew();

                    Logger.log(String.Format("Staring MCP CG collection process with {0} threads", MRMPServiceBase.mcp_cg_polling_concurrency), Logger.Severity.Info);

                    List<MRPManagementobjectType> _mcp_mos;

                    MRManagementobjectFilterType _filter = new MRManagementobjectFilterType() { motype = "MCPConsistencyGroup", entitytype = 1 };
                    _mcp_mos = (MRMPServiceBase._mrmp_api.managementobject().list_filtered(_filter)).managementobjects.Where(x => x.target_platform.enabled == true && x.internal_state != "deleted").ToList();


                    Parallel.ForEach(_mcp_mos, new ParallelOptions() { MaxDegreeOfParallelism = MRMPServiceBase.mcp_cg_polling_concurrency }, _mcp_mo =>
                    {
                        try
                        {
                            MCPCGPoller.PollerDo(_mcp_mo);
                        }
                        catch (Exception ex)
                        {
                            Logger.log(string.Format("Error collecting MCP CG information from {0} with error {1}", _mcp_mo.target_workload.hostname, ex.ToString()), Logger.Severity.Error);
                        }
                    });

                    sw.Stop();

                    Logger.log(String.Format("Completed MCP CG collection for {0} jobs in {1} [next run at {2}]",
                        _mcp_mos.Count, TimeSpan.FromMilliseconds(sw.Elapsed.TotalSeconds), _next_poller_run), Logger.Severity.Info);

                    //Wait for next run
                    while (_next_poller_run > DateTime.UtcNow)
                    {
                        Thread.Sleep(new TimeSpan(0, 0, 5));
                    }
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("MCPCGPoller: {0}", ex.ToString()), Logger.Severity.Fatal);
                }
            }
        }
    }
}

