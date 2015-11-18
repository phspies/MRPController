using CloudMoveyWorkerService.Portal.Sqlite.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace CloudMoveyWorkerService.Portal.Classes.Static_Classes.Background_Classes
{
    class PerformanceWorker
    {
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_LOGON_SERVICE = 3;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

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
        public static List<PerfCounter> _countertree = new List<PerfCounter>();
        private static List<String> _categories = new List<string>();
        private static String machinename = null;
        private static CloudMoveyEntities dbcontext = new CloudMoveyEntities();
        private static CloudMoveyPortal _cloud_movey = new CloudMoveyPortal();
        static void Start()
        {

            _categories.Add("Processor");
            _categories.Add("Processor Information");
            _categories.Add("Memory");
            _categories.Add("LogicalDisk");
            _categories.Add("PhysicalDisk");
            _categories.Add("Network Interface");
            _categories.Add("Server");
            _categories.Add("Double-Take Connection");
            _categories.Add("Double-Take Kernel");
            _categories.Add("Double-Take Source");
            _categories.Add("Double-Take Target");

            

            while (true)
            {
                foreach (var _workload in dbcontext.Workloads.Where(x => x.enabled == true))
                {
                    //var workingip = Connection.find_working_ip(ipaddresslist);
                    //if (workingip != null)



                        machinename = _workload.hostname;
                    Credential _credential = dbcontext.Credentials.FirstOrDefault(x => x.id == _workload.credential_id);
                    IntPtr userHandle = new IntPtr(0);
                    LogonUser(_credential.username, (_credential.domain == null ? "." : _credential.domain), _credential.password, LOGON32_LOGON_SERVICE, LOGON32_PROVIDER_DEFAULT, ref userHandle);
                    WindowsIdentity identity = new WindowsIdentity(userHandle);
                    WindowsImpersonationContext context = identity.Impersonate();
                    foreach (string pcc in _categories)
                    {
                        try
                        {
                            PerformanceCounterCategory _pc = new PerformanceCounterCategory(pcc, machinename);
                            //_countertree.Add(new _PerfCounter() { category = pcc, counter = "" });
                            if (_pc.CategoryType == PerformanceCounterCategoryType.SingleInstance)
                            {
                                List<InstanceCounters> _instances = new List<InstanceCounters>();
                                _instances.Add(new InstanceCounters() { instance = "" });
                                foreach (var _counter in _pc.GetCounters())
                                {
                                    if (!_countertree.Exists(x => x.category == pcc && x.counter == _counter.CounterName))
                                    {
                                        _countertree.Add(new PerfCounter() { category = pcc, counter = _counter.CounterName, instances = _instances });
                                    }
                                    PerformanceCounter _pcounter = new PerformanceCounter(_counter.CategoryName, _counter.CounterName, string.Empty, machinename);
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
                                    Console.WriteLine(String.Format("{0}   {1}:{2}:{3} = {4}", _counterobject.timestamp, _pcounter.CategoryName, _pcounter.CounterName, _counterobject.instance, _counterobject.value));
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
                                        PerformanceCounter _pcounter = new PerformanceCounter(_counter.CategoryName, _counter.CounterName, _counter.InstanceName, machinename);

                                        InstanceCounters _counterobject = _countertree.Where(
                                            x => x.category == _counter.CategoryName &&
                                            x.counter == _counter.CounterName)
                                            .Select(x => x.instances.SingleOrDefault(y => y.instance == _counter.InstanceName))
                                            .FirstOrDefault();
                                        _counterobject.s1 = _pcounter.NextSample();
                                        if (_counterobject.s0.CounterType != _counterobject.s1.CounterType)
                                        {
                                            _counterobject.s0 = _counter.NextSample();
                                        }
                                        _counterobject.value = CounterSampleCalculator.ComputeCounterValue(_counterobject.s0, _counterobject.s1);
                                        _counterobject.s0 = _counterobject.s1;
                                        Console.WriteLine(String.Format("{0}   {1}:{2}:{3} = {4}", _counterobject.timestamp, _pcounter.CategoryName, _pcounter.CounterName, _counterobject.instance, _counterobject.value));

                                    }
                                }


                            }
                        }
                        catch (System.InvalidOperationException error)
                        {
                            Console.WriteLine(error.Message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    context.Undo();
                }
                Thread.Sleep(new TimeSpan(0, 5, 0));
            }
        }
    }
}

