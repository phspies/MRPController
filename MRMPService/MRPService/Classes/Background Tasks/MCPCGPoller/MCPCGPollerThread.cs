﻿using MRMPService.MRMPService.Log;
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
        public async void Start()
        {
            while (true)
            {
                DateTime _next_poller_run = DateTime.UtcNow.AddSeconds(MRMPServiceBase.mcp_cg_polling_interval);
                Stopwatch sw = Stopwatch.StartNew();

                Logger.log(String.Format("Staring MCP CG collection process with {0} threads", MRMPServiceBase.mcp_cg_polling_concurrency), Logger.Severity.Info);

                List<MRPManagementobjectType> _mcp_mos;

                MRManagementobjectFilterType _filter = new MRManagementobjectFilterType() { motype = "MCPConsistencyGroup", entitytype = 1 };
                _mcp_mos = (await MRMPServiceBase._mrmp_api.managementobject().list_filtered(_filter)).managementobjects.Where(x => x.target_platform.enabled == true).ToList();


                Parallel.ForEach(_mcp_mos, new ParallelOptions() { MaxDegreeOfParallelism = MRMPServiceBase.mcp_cg_polling_concurrency }, async _mcp_mo =>
                {
                    try
                    {
                        await MCPCGPoller.PollerDo(_mcp_mo);
                    }
                    catch (Exception ex)
                    {
                        Logger.log(string.Format("Error collecting MCP CG information from {0} with error {1}", _mcp_mo.target_workload.hostname, ex.ToString()), Logger.Severity.Error);
                    }
                });

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

