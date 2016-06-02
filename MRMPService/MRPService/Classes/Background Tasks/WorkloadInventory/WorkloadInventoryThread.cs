using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using MRMPService.MRMPAPI.Types.API;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MRMPService.MRMPAPI.Classes
{
    class WorkloadInventoryThread
    {
        MRMP_ApiClient _cloud_movey = new MRMP_ApiClient();
        public void Start()
        {

            while (true)
            {
                DateTime _next_inventory_run = DateTime.UtcNow.AddMinutes(Global.os_inventory_interval);
                Stopwatch sw = Stopwatch.StartNew();

                Logger.log(String.Format("Staring operating system inventory process with {0} threads", Global.os_inventory_concurrency), Logger.Severity.Info);

                List<MRPWorkloadType> workloads;
                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                {
                    workloads = _api.workload().listworkloads().workloads;
                }

                Parallel.ForEach(workloads,
                    new ParallelOptions { MaxDegreeOfParallelism = Global.os_inventory_concurrency },
                    (workload) =>
                    {
                        try
                        {
                            switch (workload.ostype.ToUpper())
                            {
                                case "WINDOWS":
                                    (new WorkloadInventory()).WorkloadInventoryWindowsDo(workload);
                                    break;
                                case "UNIX":
                                    (new WorkloadInventory()).WorkloadInventoryUnixDo(workload);
                                    break;
                            }

                            using (MRMP_ApiClient _api = new MRMP_ApiClient())
                            {
                                _api.workload().InventoryUpdateStatus(workload, "Success", true);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Error collecting inventory information from {0} with error {1}", workload.hostname, ex.ToString()), Logger.Severity.Error);
                            using (MRMP_ApiClient _api = new MRMP_ApiClient())
                            {
                                _api.workload().InventoryUpdateStatus(workload, ex.Message, false);
                            }
                        }
                    });

                sw.Stop();

                Logger.log(String.Format("Completed operating system inventory for {0} workloads in {1} [next run at {2}]",
                    workloads.Count, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_inventory_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_inventory_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }

    }
}