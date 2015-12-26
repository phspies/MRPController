using CloudMoveyWorkerService.CloudMovey.Classes.Static_Classes;
using CloudMoveyWorkerService.LocalDatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using Utils;

namespace CloudMoveyWorkerService.Portal.Classes.Static_Classes.Background_Classes
{
    class PerformanceWorker
    {
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_LOGON_SERVICE = 3;
        public const int LOGON32_PROVIDER_DEFAULT = 0;
        public const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
        public const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern bool LogonUser(
            String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);
        public class InstanceCounters
        {
            public string instance { get; set; }
            public double value { get; set; }
            public CounterSample s0 { get; set; }
            public CounterSample s1 { get; set; }
            public DateTime timestamp { get; set; }
        }
        public class PerfCounter
        {
            public string category { get; set; }
            public string counter { get; set; }
            public List<InstanceCounters> instances { get; set; }
        }
        public class CollectionCounter
        {
            public string category { get; set; }
            public string counter { get; set; }

        }
        public static List<PerfCounter> _countertree = new List<PerfCounter>();
        private static List<String> _categories = new List<string>();
        private static List<CollectionCounter> _counters = new List<CollectionCounter>();
        private static CloudMoveyPortal _cloud_movey = new CloudMoveyPortal();

        public void Start()
        {
            LocalDB db = new LocalDB();


            _categories.Add("");

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
                List<Workload> _workloads = db.Workloads.ToList();
                if (_workloads != null)
                {
                    foreach (var _workload in _workloads.Where(x => x.enabled == true))
                    {
                        string workload_ip = Connection.find_working_ip(_workload);
                        Credential _credential = db.Credentials.FirstOrDefault(x => x.id == _workload.credential_id);
                        IntPtr userHandle = new IntPtr(0);
                        
                        try {
                            LogonUser(_credential.username, (_credential.domain == null ? "." : _credential.domain), _credential.password, LOGON32_LOGON_NEW_CREDENTIALS, LOGON32_PROVIDER_DEFAULT, ref userHandle);
                            WindowsIdentity identity = new WindowsIdentity(userHandle);
                            WindowsImpersonationContext context = identity.Impersonate();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        foreach (string pcc in _counters.Select(x => x.category).Distinct())
                        {
                            try
                            {
                                if (!PerformanceCounterCategory.Exists(pcc, workload_ip))
                                {

                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                Global.event_log.WriteEntry(String.Format("{0} returned the following error while collecting performance data: {1}", workload_ip, ex.ToString()));
                                continue;
                            }



                            try
                            {

                                PerformanceCounterCategory _pc = new PerformanceCounterCategory(pcc, workload_ip);
                                if (_pc.CategoryType == PerformanceCounterCategoryType.SingleInstance)
                                {
                                    List<InstanceCounters> _instances = new List<InstanceCounters>();
                                    _instances.Add(new InstanceCounters() { instance = "" });
                                    foreach (var _counter in _pc.GetCounters())
                                    {
                                        if (_counters.Find(x => x.category == pcc).counter != "*")
                                        {
                                            if (!_counters.Exists(x => x.counter == _counter.CounterName))
                                            {
                                                continue;
                                            }
                                        }

                                        if (!_countertree.Exists(x => x.category == pcc && x.counter == _counter.CounterName))
                                        {
                                            _countertree.Add(new PerfCounter() { category = pcc, counter = _counter.CounterName, instances = _instances });
                                        }
                                        PerformanceCounter _pcounter = new PerformanceCounter(_counter.CategoryName, _counter.CounterName, string.Empty, workload_ip);
                                        InstanceCounters _counterobject = _countertree.Where(
                                            x => x.category == _counter.CategoryName &&
                                            x.counter == _counter.CounterName)
                                            .Select(x => x.instances.SingleOrDefault(y => y.instance == ""))
                                            .FirstOrDefault();
                                        _counterobject.s1 = _pcounter.NextSample();
                                        if (_counterobject.s0.CounterType != _counterobject.s1.CounterType)
                                        {
                                            _counterobject.s0 = _counter.NextSample();
                                        }
                                        _counterobject.value = CounterSampleCalculator.ComputeCounterValue(_counterobject.s0, _counterobject.s1);
                                        _counterobject.s0 = _counterobject.s1;

                                        using (var _db = new LocalDB())
                                        {
                                            Performance _perf = new Performance();
                                            _perf.workload_id = _workload.id;
                                            _perf.timestamp = DateTime.Now; //_counterobject.timestamp;
                                            _perf.category_name = _pcounter.CategoryName;
                                            _perf.counter_name = _pcounter.CounterName;
                                            _perf.instance = _counterobject.instance;
                                            _perf.value = _counterobject.value;
                                            _perf.id = Objects.RamdomGuid();

                                            db.Performance.Add(_perf);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    List<InstanceCounters> _instances = new List<InstanceCounters>();
                                    foreach (string _instance in _pc.GetInstanceNames())
                                    {
                                        if (!_instances.Exists(x => x.instance == _instance))
                                        {
                                            _instances.Add(new InstanceCounters() { instance = _instance });
                                        }
                                    }
                                    foreach (var _instance in _instances)
                                    {
                                        foreach (var _counter in _pc.GetCounters(_instance.instance))
                                        {

                                            //only collect info for specified counters or evenrything within the category
                                            if (_counters.Find(x => x.category == pcc).counter != "*")
                                            {
                                                if (!_counters.Exists(x => x.counter == _counter.CounterName))
                                                {
                                                    continue;
                                                }
                                            }

                                            if (!_countertree.Exists(x => x.category == pcc && x.counter == _counter.CounterName))
                                            {
                                                _countertree.Add(new PerfCounter() { category = pcc, counter = _counter.CounterName, instances = _instances });
                                            }
                                            else
                                            {
                                                if (!_countertree.Find(x => x.category == pcc && x.counter == _counter.CounterName).instances.Exists(x => x.instance == _instance.instance))
                                                {
                                                    _countertree.Find(x => x.category == pcc && x.counter == _counter.CounterName).instances.Add(_instance);
                                                }
                                            }
                                            PerformanceCounter _pcounter = new PerformanceCounter(_counter.CategoryName, _counter.CounterName, _counter.InstanceName, workload_ip);

                                            InstanceCounters _counterobject = _countertree.Where(x => x.category == _counter.CategoryName &&
                                                        x.counter == _counter.CounterName).Select(x => x.instances.SingleOrDefault(y => y.instance == _counter.InstanceName)).FirstOrDefault();

                                            _counterobject.s1 = _pcounter.NextSample();
                                            if (_counterobject.s0.CounterType != _counterobject.s1.CounterType)
                                            {
                                                _counterobject.s0 = _counter.NextSample();
                                            }
                                            _counterobject.value = CounterSampleCalculator.ComputeCounterValue(_counterobject.s0, _counterobject.s1);
                                            _counterobject.s0 = _counterobject.s1;

                                            Performance _perf = new Performance();
                                            _perf.workload_id = _workload.id;
                                            _perf.timestamp = DateTime.Now; //_counterobject.timestamp;
                                            _perf.category_name = _pcounter.CategoryName;
                                            _perf.counter_name = _pcounter.CounterName;
                                            _perf.instance = _counterobject.instance;
                                            _perf.value = _counterobject.value;
                                            _perf.id = Objects.RamdomGuid();
                                            using (var _db = new LocalDB())
                                            {
                                                db.Performance.Add(_perf);
                                                db.SaveChanges();
                                            }


                                        }
                                    }
                                }
                            }
                            catch (System.InvalidOperationException error)
                            {
                                Global.event_log.WriteEntry(error.ToString(), EventLogEntryType.Error);
                            }
                            catch (Exception ex)
                            {
                                Global.event_log.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                            }
                        }
                        
                    }
                    
                }
                Thread.Sleep(new TimeSpan(0, 30, 0));
            }
        }
    }
}

