using MRPService.MRPService.Log;
using MRPService.LocalDatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using MRPService.Utilities;
using static MRPService.Utilities.SyncronizedList;
using System.Runtime.InteropServices;

namespace MRPService.PerformanceCollection
{
    class WorkloadPerformance
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


        public static void WorkloadPerformanceDo(SyncronisedList<PerfCounter> _countertree, SyncronisedList<CollectionCounter> _counters, String workload_id)
        {
            WorkloadSet dbworkload = new WorkloadSet();

            Workload mrpworkload = dbworkload.ModelRepository.GetById(workload_id);
            if (mrpworkload == null)
            {
                throw new System.ArgumentException(String.Format("Error finding workload in local database {0}", workload_id));
            }

            //check for credentials
            CredentialSet dbcredential = new CredentialSet();
            Credential _credential = dbcredential.ModelRepository.GetById(mrpworkload.credential_id);
            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Error finding credentials for workload {0} {1}", mrpworkload.id, mrpworkload.hostname));
            }

            //check for working IP
            string workload_ip = Connection.find_working_ip(mrpworkload, true);
            if (workload_ip == null)
            {
                throw new ArgumentException(String.Format("Error finding contactable IP for workload {0} {1}", mrpworkload.id, mrpworkload.hostname));
            }

            //Impersonate credentials before collection of information
            using (new Impersonator(_credential.username, (String.IsNullOrEmpty(_credential.domain) ? "." : _credential.domain), _credential.password))
            {
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
                        Logger.log(String.Format("{0} returned the following error while collecting performance data for counter : {1} : {2}", workload_ip, pcc, ex.Message), Logger.Severity.Error);
                        continue;
                    }


                    PerformanceCounterCategory _pc = new PerformanceCounterCategory(pcc, workload_ip);
                    if (_pc.CategoryType == PerformanceCounterCategoryType.SingleInstance)
                    {
                        List<InstanceCounters> _instances = new List<InstanceCounters>();
                        _instances.Add(new InstanceCounters() { instance = "" });
                        foreach (var _counter in _pc.GetCounters())
                        {
                            if (_counters.FirstOrDefault(x => x.category == pcc).counter != "*")
                            {
                                if (!_counters.Any(x => x.counter == _counter.CounterName))
                                {
                                    continue;
                                }
                            }

                            if (!_countertree.Any(x => x.category == pcc && x.counter == _counter.CounterName))
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

                            Performance _perf = new Performance();
                            _perf.workload_id = mrpworkload.id;
                            _perf.timestamp = RoundDown(DateTime.UtcNow, TimeSpan.FromHours(1)); //_counterobject.timestamp;
                            _perf.category_name = _pcounter.CategoryName;
                            _perf.counter_name = _pcounter.CounterName;
                            _perf.instance = _counterobject.instance;
                            _perf.value = _counterobject.value;
                            _perf.id = Objects.RamdomGuid();

                            using (PerformanceSet performance_db_set = new PerformanceSet())
                            {
                                performance_db_set.ModelRepository.Insert(_perf);
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
                                if (_counters.FirstOrDefault(x => x.category == pcc).counter != "*")
                                {
                                    if (!_counters.Any(x => x.counter == _counter.CounterName))
                                    {
                                        continue;
                                    }
                                }

                                if (!_countertree.Any(x => x.category == pcc && x.counter == _counter.CounterName))
                                {
                                    _countertree.Add(new PerfCounter() { category = pcc, counter = _counter.CounterName, instances = _instances });
                                }
                                else
                                {
                                    if (!_countertree.FirstOrDefault(x => x.category == pcc && x.counter == _counter.CounterName).instances.Exists(x => x.instance == _instance.instance))
                                    {
                                        _countertree.FirstOrDefault(x => x.category == pcc && x.counter == _counter.CounterName).instances.Add(_instance);
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
                                _perf.workload_id = mrpworkload.id;
                                _perf.timestamp = RoundDown(DateTime.UtcNow, TimeSpan.FromHours(1)); //_counterobject.timestamp;
                                _perf.category_name = _pcounter.CategoryName;
                                _perf.counter_name = _pcounter.CounterName;
                                _perf.instance = _counterobject.instance;
                                _perf.value = _counterobject.value;
                                _perf.id = Objects.RamdomGuid();
                                using (PerformanceSet performance_db_set = new PerformanceSet())
                                {
                                    performance_db_set.ModelRepository.Insert(_perf);
                                }
                            }
                        }
                    }
                }
            }
        }

        static DateTime RoundDown(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks / d.Ticks) * d.Ticks);
        }
    }
}

