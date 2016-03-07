using MRPService.MRPService.Log;
using MRPService.LocalDatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using static MRPService.Utilities.SyncronizedList;
using System.Threading.Tasks;

namespace MRPService.PerformanceCollection
{
    class WorkloadPerformanceThread
    {
        //create syncronized lists to work in the threaded environment
        private static SyncronisedList<PerfCounter> _countertree = new SyncronisedList<PerfCounter>(new List<PerfCounter>());
        private static SyncronisedList<CollectionCounter> _counters = new SyncronisedList<CollectionCounter>(new List<CollectionCounter>());

        public void Start()
        {

            //Fill counters that needs to be collected from workloads
            _counters.Add(new CollectionCounter() { category = "Processor", counter = "% Idle Time" });
            _counters.Add(new CollectionCounter() { category = "Processor", counter = "% User Time" });
            _counters.Add(new CollectionCounter() { category = "Processor", counter = "% Processor Time" });
            _counters.Add(new CollectionCounter() { category = "Processor", counter = "Interrupts/sec" });


            _counters.Add(new CollectionCounter() { category = "Memory", counter = "Available Bytes" });
            _counters.Add(new CollectionCounter() { category = "Memory", counter = "Page Faults/sec" });
            _counters.Add(new CollectionCounter() { category = "Memory", counter = "Page Reads/sec" });
            _counters.Add(new CollectionCounter() { category = "Memory", counter = "Page Writes/sec" });


            _counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "% Free Space" });
            _counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "% Disk Time" });
            _counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "% Write Disk Time" });
            _counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "% Read Disk Time" });
            _counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Reads/sec" });
            _counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Writes/sec" });
            _counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Read Bytes/sec" });
            _counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Write Bytes/sec" });
            _counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Bytes/sec" });
            _counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Split IO/sec" });
            _counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Current Disk Queue Length" });

            _counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "% Disk Time" });
            _counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "% Write Disk Time" });
            _counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "% Read Disk Time" });
            _counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Reads/sec" });
            _counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Writes/sec" });
            _counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Read Bytes/sec" });
            _counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Write Bytes/sec" });
            _counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Bytes/sec" });
            _counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Split IO/sec" });
            _counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Current Disk Queue Length" });

            _counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Bytes Received/sec" });
            _counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Bytes Sent/sec" });
            _counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Current Bandwidth" });
            _counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Output Queue Length" });
            _counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Packets Recieved/sec" });
            _counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Packets Sent/sec" });


            _counters.Add(new CollectionCounter() { category = "Double-Take Connection", counter = "*" });
            _counters.Add(new CollectionCounter() { category = "Double-Take Kernel", counter = "*" });
            _counters.Add(new CollectionCounter() { category = "Double-Take Source", counter = "*" });
            _counters.Add(new CollectionCounter() { category = "Double-Take Target", counter = "*" });

            while (true)
            {
                DateTime _next_performance_run = DateTime.Now.AddHours(1);
                Stopwatch sw = Stopwatch.StartNew();
                int _processed_workloads = 0;

                Logger.log(String.Format("Staring performance collection process with {0} threads", Global.performance_inventory_concurrency), Logger.Severity.Info);

                using (WorkloadSet workload_set = new WorkloadSet())
                {
                    List<Workload> workloads = workload_set.ModelRepository.Get(x => x.enabled == true && x.iplist != null);
                    _processed_workloads = workloads.Count();
                    Parallel.ForEach(workloads,
                        new ParallelOptions { MaxDegreeOfParallelism = Global.os_inventory_concurrency },
                        (workload) =>
                        {
                            try
                            {
                                WorkloadPerformance.WorkloadPerformanceDo(_countertree, _counters, workload.id);
                                workload_set.PeformanceUpdateStatus(workload.id, "Success", true);
                            }
                            catch (Exception ex)
                            {
                                Logger.log(String.Format("Error collecting performance information from {0} with error {1}", workload.hostname, ex.Message), Logger.Severity.Error);
                                workload_set.PeformanceUpdateStatus(workload.id, ex.Message, false);
                            }
                        });
                }
                sw.Stop();

                Logger.log(String.Format("Completed performance collection for {0} workloads in {1}",
                    _processed_workloads, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)), Logger.Severity.Info);

                //Wait for next run
                while (_next_performance_run > DateTime.Now)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}

