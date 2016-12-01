using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Contracts;
using System.Linq;
using System.Collections.Generic;
using MRMPService.MCPCGCollection;

namespace MRMPService.DTPollerCollection
{
    class MCPCGPollerThread
    {
        public void Start()
        {
            while (true)
            {
                DateTime _next_poller_run = DateTime.UtcNow.AddSeconds(MRMPServiceBase.mcp_cg_polling_interval);
                Stopwatch sw = Stopwatch.StartNew();

                Logger.log(String.Format("Staring MCP CG collection process with {0} threads", MRMPServiceBase.mcp_cg_polling_concurrency), Logger.Severity.Info);

                List<MRPManagementobjectType> _mcp_mos;
                using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                {
                    MRManagementobjectFilterType _filter = new MRManagementobjectFilterType() { motype = "MCPConsistencyGroup", entitytype = 1 };
                    _mcp_mos = _mrmp.managementobject().list_filtered(_filter).managementobjects.Where(x => x.target_platform.enabled == true).ToList();
                }
                List<Thread> lstThreads = new List<Thread>();
                var splashStart = new ManualResetEvent(false);

                foreach (MRPManagementobjectType _mcp_mo in _mcp_mos)
                {
                    while (lstThreads.Count(x => x.IsAlive) > MRMPServiceBase.mcp_cg_polling_concurrency)
                    {
                        Task.Delay(TimeSpan.FromSeconds(1));
                    }

                    Thread _inventory_thread = new Thread(async delegate ()
                    {
                        splashStart.Set();
                        try
                        {
                            await MCPCGPoller.PollerDo(_mcp_mo);
                        }
                        catch (Exception ex)
                        {
                            Logger.log(string.Format("Error collecting MCP CG information from {0} with error {1}", _mcp_mo.target_workload.hostname, ex.ToString()), Logger.Severity.Error);
                        }
                    });
                    lstThreads.Add(_inventory_thread);
                    _inventory_thread.Start();
                    splashStart.WaitOne();
                    Logger.log(String.Format("MCP CG Poller Thread Count [active: {0}] [total: {1}] [complete {2}]", lstThreads.Count(x => x.IsAlive), lstThreads.Count(), lstThreads.Count(x => !x.IsAlive)), Logger.Severity.Info);
                }
                while (lstThreads.Any(x => x.IsAlive))
                {

                }



                sw.Stop();

                Logger.log(String.Format("Completed MCP CG collection for {0} jobs in {1} [next run at {2}]",
                    _mcp_mos.Count, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_poller_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_poller_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}

