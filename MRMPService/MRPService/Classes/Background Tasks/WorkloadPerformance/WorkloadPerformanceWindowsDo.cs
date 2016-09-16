using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MRMPService.Utilities;
using static MRMPService.Utilities.SyncronizedList;
using MRMPService.MRMPAPI.Types.API;

namespace MRMPService.PerformanceCollection
{
    partial class WorkloadPerformance
    {
        public static void WorkloadPerformanceWindowsDo(SyncronisedList<WorkloadCounters> _workload_counters, SyncronisedList<CollectionCounter> _available_counters, MRPWorkloadType _workload)
        {
            #region load and check workload information
            //check for credentials
            MRPCredentialType _credential = _workload.credential;


            //check for working IP
            string workload_ip = null;
            if (_workload.workloadtype != "manager")
            {
                if (_credential == null)
                {
                    throw new ArgumentException(String.Format("Error finding credentials"));
                }
                using (Connection _connection = new Connection())
                {
                    workload_ip = _connection.FindConnection(_workload.iplist, true);
                }
                if (workload_ip == null)
                {
                    throw new ArgumentException(String.Format("Error contacting workload"));
                }
            }
            else
            {
                workload_ip = ".";

            }
            #endregion

            Logger.log(String.Format("Performance: Start performance collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);

            //grab perfcounter tree from the list for this server and create one if it does not exists
            WorkloadCounters _this_workload_counters = new WorkloadCounters();
            if (!_workload_counters.Any(x => x.workload_id == _workload.id))
            {
                //create new workload object with counters and instances hives
                WorkloadCounters _new_workload = new WorkloadCounters() { workload_id = _workload.id, counters = new List<PerfCounter>() };
                _workload_counters.Add(_new_workload);
            }
            _this_workload_counters = _workload_counters.FirstOrDefault(x => x.workload_id == _workload.id);

            //Impersonate credentials before collection of information
            using (new Impersonator((_workload.workloadtype == "manager" ? "." : _credential.username), (_workload.workloadtype == "manager" ? null : (String.IsNullOrEmpty(_credential.domain) ? "." : _credential.domain)), (_workload.workloadtype == "manager" ? null : _credential.encrypted_password)))
            {
                //loop each counter in the available counter list
                foreach (string _current_category in _available_counters.Select(x => x.category).Distinct())
                {
                    //test if counter exists and only process those we can collect
                    try
                    {
                        if (!PerformanceCounterCategory.Exists(_current_category, workload_ip))
                        {
                            continue;
                        }
                    }
                    catch (UnauthorizedAccessException access_ex)
                    {
                        throw new ArgumentException(access_ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Logger.log(String.Format("{0} returned the following error while collecting performance data for counter : {1} : {2}", workload_ip, _current_category, ex.Message), Logger.Severity.Error);
                        continue;
                    }

                    //the counter was found and we now trying to collect the Category object from the server
                    PerformanceCounterCategory _current_catergory_object = new PerformanceCounterCategory(_current_category, workload_ip);

                    //create single instance flag
                    bool _multi_instance_counter = Convert.ToBoolean(_current_catergory_object.CategoryType);

                    //Build temporary instance list
                    List<String> _instances = new List<string>();
                    if (_multi_instance_counter)
                    {
                        foreach (string _instance in _current_catergory_object.GetInstanceNames())
                        {
                            if (!_instances.Any(x => x == _instance))
                            {
                                _instances.Add(_instance);
                            }
                        }
                    }
                    else
                    {
                        if (!_instances.Any(x => x == "_Total"))
                        {
                            _instances.Add("_Total");
                        }
                    }

                    //loop instances
                    foreach (string _current_instance in _instances)
                    {
                        //get all counters for this instance, if single only get the single instance counters
                        List<PerformanceCounter> _counters = _multi_instance_counter ? _current_catergory_object.GetCounters(_current_instance).ToList() : _current_catergory_object.GetCounters().ToList();
                        foreach (var _counter in _counters)
                        {
                            //only collect info for specified counters or evenrything within the category
                            if (_available_counters.FirstOrDefault(x => x.category == _current_category).counter != "*" && !_available_counters.Any(x => x.counter == _counter.CounterName))
                            {
                                continue;
                            }

                            //check counter and category in the workload counter object list and create if required
                            if (!_this_workload_counters.counters.Exists(y => y.category == _current_category && y.counter == _counter.CounterName))
                            {
                                _this_workload_counters.counters.Add(new PerfCounter() { category = _current_category, counter = _counter.CounterName, instances = new List<InstanceCounters>() });
                            }

                            //check instance in the counter object list and create if required
                            if (!_this_workload_counters.counters.FirstOrDefault(y => y.category == _current_category && y.counter == _counter.CounterName).instances.Exists(x => x.instance == _current_instance))
                            {
                                _this_workload_counters.counters.FirstOrDefault(y => y.category == _current_category && y.counter == _counter.CounterName).instances.Add(new InstanceCounters() { instance = _current_instance });
                            }

                            //Get Counter object from server
                            string _collection_instance = _multi_instance_counter ? _current_instance : string.Empty;
                            PerformanceCounter _current_performance_counter = new PerformanceCounter(_counter.CategoryName, _counter.CounterName, _collection_instance, workload_ip);

                            //Find counter for this category, counters and workload 
                            InstanceCounters _counterobject = _this_workload_counters.counters.Where(x =>
                                x.category == _counter.CategoryName &&
                                x.counter == _counter.CounterName)
                                .Select(x => x.instances.SingleOrDefault(y => y.instance == _current_instance))
                                .FirstOrDefault();

                            //get current current counter from server
                            _counterobject.s1 = _current_performance_counter.NextSample();

                            //check counter type between old and new counter and reset counter objects if they have a missmatch
                            if (_counterobject.s0.CounterType != _counterobject.s1.CounterType)
                            {
                                _counterobject.s0 = _counter.NextSample();
                                _counterobject.s1 = _counter.NextSample();
                            }

                            _counterobject.value = CounterSampleCalculator.ComputeCounterValue(_counterobject.s0, _counterobject.s1);
                            _counterobject.s0 = _counterobject.s1;

                            Performance _perf = new Performance();
                            _perf.workload_id = _workload.id;
                            _perf.timestamp = TimeCalculations.RoundDown(DateTime.UtcNow, TimeSpan.FromHours(1)); //_counterobject.timestamp;
                            _perf.category_name = _current_performance_counter.CategoryName;
                            _perf.counter_name = _current_performance_counter.CounterName;
                            _perf.instance = _current_instance;
                            _perf.value = _counterobject.value;
                            _perf.id = Objects.RamdomGuid();
                            using (PerformanceSet performance_db_set = new PerformanceSet())
                            {
                                performance_db_set.ModelRepository.Insert(_perf);
                            }

                            //process custom counters


                            if (_current_performance_counter.CategoryName == "Memory" && _current_performance_counter.CounterName == "Available Bytes")
                            {
                                long _workload_total_memory = (Convert.ToInt64(_workload.vmemory) * 1024 * 1024 * 1024);

                                //memory: Used Bytes
                                Double _memory_used_bytes = _workload_total_memory - _counterobject.value;
                                String _memory_used_counter_name = "Used Bytes";

                                _perf = new Performance();
                                _perf.workload_id = _workload.id;
                                _perf.timestamp = TimeCalculations.RoundDown(DateTime.UtcNow, TimeSpan.FromHours(1)); //_counterobject.timestamp;
                                _perf.category_name = _current_performance_counter.CategoryName;
                                _perf.counter_name = _memory_used_counter_name;
                                _perf.instance = _current_instance;
                                _perf.value = _memory_used_bytes;
                                _perf.id = Objects.RamdomGuid();
                                using (PerformanceSet performance_db_set = new PerformanceSet())
                                {
                                    performance_db_set.ModelRepository.Insert(_perf);
                                }
                                //memory: % used
                                Double _percentage_memory_used = ((Convert.ToDouble(_memory_used_bytes) / Convert.ToDouble(_workload_total_memory)) * 100);
                                String _memory_counter_name = "% Used";

                                _perf = new Performance();
                                _perf.workload_id = _workload.id;
                                _perf.timestamp = TimeCalculations.RoundDown(DateTime.UtcNow, TimeSpan.FromHours(1)); //_counterobject.timestamp;
                                _perf.category_name = _current_performance_counter.CategoryName;
                                _perf.counter_name = _memory_counter_name;
                                _perf.instance = _current_instance;
                                _perf.value = _percentage_memory_used;
                                _perf.id = Objects.RamdomGuid();
                                using (PerformanceSet performance_db_set = new PerformanceSet())
                                {
                                    performance_db_set.ModelRepository.Insert(_perf);
                                }
                            }
                        }
                    }
                }
                Logger.log(String.Format("Performance: Completed performance collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
            }
        }
    }
}
