using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static MRMPService.Utilities.SyncronizedList;
using System.Threading.Tasks;
using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPAPI;

namespace MRMPService.PerformanceCollection
{
    class WorkloadPerformanceThread
    {
        //create syncronized lists to work in the threaded environment
        private static SyncronisedList<WorkloadCounters> _workload_counters = new SyncronisedList<WorkloadCounters>(new List<WorkloadCounters>());
        private static SyncronisedList<CollectionCounter> _available_counters = new SyncronisedList<CollectionCounter>(new List<CollectionCounter>());

        public void Start()
        {

            //Fill counters that needs to be collected from workloads
            _available_counters.Add(new CollectionCounter() { category = "Processor", counter = "% Idle Time" });
            _available_counters.Add(new CollectionCounter() { category = "Processor", counter = "% User Time" });
            _available_counters.Add(new CollectionCounter() { category = "Processor", counter = "% Processor Time" });
            _available_counters.Add(new CollectionCounter() { category = "Processor", counter = "Interrupts/sec" });

            _available_counters.Add(new CollectionCounter() { category = "Memory", counter = "Available Bytes" });
            _available_counters.Add(new CollectionCounter() { category = "Memory", counter = "Page Faults/sec" });
            _available_counters.Add(new CollectionCounter() { category = "Memory", counter = "Page Reads/sec" });
            _available_counters.Add(new CollectionCounter() { category = "Memory", counter = "Page Writes/sec" });

            _available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "% Free Space" });
            _available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "% Disk Time" });
            _available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "% Write Disk Time" });
            _available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "% Read Disk Time" });
            _available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Reads/sec" });
            _available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Writes/sec" });
            _available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Read Bytes/sec" });
            _available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Write Bytes/sec" });
            _available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Bytes/sec" });
            _available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Split IO/sec" });
            _available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Current Disk Queue Length" });

            _available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "% Disk Time" });
            _available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "% Write Disk Time" });
            _available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "% Read Disk Time" });
            _available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Reads/sec" });
            _available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Writes/sec" });
            _available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Read Bytes/sec" });
            _available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Write Bytes/sec" });
            _available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Bytes/sec" });
            _available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Split IO/sec" });
            _available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Current Disk Queue Length" });

            _available_counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Bytes Received/sec" });
            _available_counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Bytes Sent/sec" });
            _available_counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Current Bandwidth" });
            _available_counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Output Queue Length" });
            _available_counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Packets Recieved/sec" });
            _available_counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Packets Sent/sec" });


            _available_counters.Add(new CollectionCounter() { category = "Double-Take Connection", counter = "*" });
            _available_counters.Add(new CollectionCounter() { category = "Double-Take Kernel", counter = "*" });
            _available_counters.Add(new CollectionCounter() { category = "Double-Take Source", counter = "*" });
            _available_counters.Add(new CollectionCounter() { category = "Double-Take Target", counter = "*" });

            while (true)
            {
                DateTime _next_performance_run = DateTime.UtcNow.AddHours(1);
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                int _processed_workloads = 0;

                Logger.log(String.Format("Staring performance collection process with {0} threads", Global.os_performance_concurrency), Logger.Severity.Info);

                List<MRPWorkloadType> workloads;

                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                {
                    workloads = _api.workload().listworkloads().workloads.Where(x => x.perf_collection_enabled == true).ToList();
                }

                _processed_workloads = workloads.Count();


                List<Thread> lstThreads = new List<Thread>();
                foreach (var workload in workloads)
                {
                    while (lstThreads.Count(x => x.IsAlive) > Global.os_performance_concurrency - 1)
                    {
                        Thread.Sleep(1000);
                    }
                    var splashStart = new ManualResetEvent(false);
                    Thread _inventory_thread = new Thread(delegate ()
                    {
                        splashStart.Set();
                        try
                        {
                            switch (workload.ostype.ToUpper())
                            {
                                case "WINDOWS":
                                    WorkloadPerformance.WorkloadPerformanceWindowsDo(_workload_counters, _available_counters, workload);
                                    break;
                                case "UNIX":
                                    WorkloadPerformance.WorkloadPerformanceUnixDo(workload);
                                    break;
                            }
                            using (MRMP_ApiClient _api = new MRMP_ApiClient())
                            {
                                _api.workload().PeformanceUpdateStatus(workload, "Success", true);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Error collecting performance information from {0} with error {1}", workload.hostname, ex.ToString()), Logger.Severity.Error);
                            using (MRMP_ApiClient _api = new MRMP_ApiClient())
                            {
                                _api.workload().PeformanceUpdateStatus(workload, ex.Message, false);
                            }
                        }
                    });
                    lstThreads.Add(_inventory_thread);
                    _inventory_thread.Start();
                    splashStart.WaitOne();

                    Logger.log(String.Format("Workload Performance Thread Count [active: {0}] [total: {1}] [complete {2}]", lstThreads.Count(x => x.IsAlive), lstThreads.Count(), lstThreads.Count(x => !x.IsAlive)), Logger.Severity.Info);
                }
                while (lstThreads.Any(x => x.IsAlive))
                {

                }

                sw.Stop();

                Logger.log(String.Format("Completed performance collection for {0} workloads in {1} [next run at {2}]",
                    _processed_workloads, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_performance_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_performance_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}

