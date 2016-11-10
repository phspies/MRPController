using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MRMPService.Utilities;
using static MRMPService.Utilities.SyncronizedList;
using MRMPService.MRMPAPI.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MRMPService.PerformanceCollection
{
    partial class WorkloadPerformance
    {
        public static void WorkloadPerformanceWindowsDo(SyncronisedList<CollectionCounter> _available_counters, MRPWorkloadType _workload)
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

            List<PerformanceType> _workload_counters = new List<PerformanceType>();

            Logger.log(String.Format("PerformanceType: Start PerformanceType collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);

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
                        Logger.log(String.Format("{0} returned the following error while collecting PerformanceType data for counter : {1} : {2}", workload_ip, _current_category, ex.Message), Logger.Severity.Error);
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

                            PerfCounterSample _this_workload_counter = new PerfCounterSample();
                            bool _found_record = false;
                            CounterSample _s0 = new CounterSample();
                            CounterSample _s1 = new CounterSample();
                            //check instance in the counter object list and create if required
                            using (PerfCounterSampleSet _db_perf = new PerfCounterSampleSet())
                            {
                                if (_db_perf.ModelRepository.Exists(y => y.workload_id == _workload.id && y.category == _current_category && y.counter == _counter.CounterName && y.instance == _current_instance))
                                {
                                    _found_record = true;
                                    _this_workload_counter = _db_perf.ModelRepository.GetFirstOrDefault(y => y.workload_id == _workload.id && y.category == _current_category && y.counter == _counter.CounterName && y.instance == _current_instance);
                                    try
                                    {
                                        JToken _sample_token = JObject.Parse(_this_workload_counter.sample);
                                        _s0 = new CounterSample(
                                            (long)_sample_token.SelectToken("RawValue"),
                                            (long)_sample_token.SelectToken("BaseValue"),
                                            (long)_sample_token.SelectToken("CounterFrequency"),
                                            (long)_sample_token.SelectToken("SystemFrequency"),
                                            (long)_sample_token.SelectToken("TimeStamp"),
                                            (long)_sample_token.SelectToken("TimeStamp100nSec"),
                                            (PerformanceCounterType)(int)_sample_token.SelectToken("CounterType"),
                                            (long)_sample_token.SelectToken("CounterTimeStamp")
                                            );
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.log(String.Format("Error deserialzing performance record: {0}", ex.GetBaseException().Message), Logger.Severity.Error);
                                        _s0 = _counter.NextSample();
                                    }
                                }
                                else
                                {
                                    _s0 = _counter.NextSample();
                                }
                            }

                            //get current current counter from server
                            _s1 = _counter.NextSample();

                            //check counter type between old and new counter and reset counter objects if they have a missmatch
                            if (_s0.CounterType != _s1.CounterType)
                            {
                                _s0 = _counter.NextSample();
                                _s1 = _counter.NextSample();
                            }

                            double _value = CounterSampleCalculator.ComputeCounterValue(_s0, _s1);


                            _this_workload_counter.sample = JsonConvert.SerializeObject(_s1);
                            using (PerfCounterSampleSet _db_perf = new PerfCounterSampleSet())
                            {
                                if (_found_record)
                                {
                                    _db_perf.Save();
                                }
                                else
                                {
                                    _this_workload_counter.workload_id = _workload.id;
                                    _this_workload_counter.category = _counter.CategoryName;
                                    _this_workload_counter.counter = _counter.CounterName;
                                    _this_workload_counter.instance = _current_instance;
                                    _db_perf.ModelRepository.Insert(_this_workload_counter);
                                }
                            }

                            PerformanceType _perf = new PerformanceType();
                            _perf.workload_id = _workload.id;
                            _perf.timestamp = TimeCalculations.RoundDown(DateTime.FromFileTimeUtc(_s1.TimeStamp100nSec), TimeSpan.FromHours(1)); //_counterobject.timestamp;
                            _perf.category_name = _this_workload_counter.category;
                            _perf.counter_name = _this_workload_counter.counter;
                            _perf.instance = _current_instance;
                            _perf.value = Math.Round(_value,2);
                            _perf.id = Objects.RamdomGuid();
                            _workload_counters.Add(_perf);


                            //process custom counters


                            if (_this_workload_counter.category == "Memory" && _this_workload_counter.counter == "Available Bytes")
                            {
                                long _workload_total_memory = (Convert.ToInt64(_workload.vmemory) * 1024 * 1024 * 1024);

                                //memory: Used Bytes
                                Double _memory_used_bytes = _workload_total_memory - _value;
                                String _memory_used_counter_name = "Used Bytes";

                                _perf = new PerformanceType();
                                _perf.workload_id = _workload.id;
                                _perf.timestamp = TimeCalculations.RoundDown(DateTime.UtcNow, TimeSpan.FromHours(1)); //_counterobject.timestamp;
                                _perf.category_name = _this_workload_counter.category;
                                _perf.counter_name = _memory_used_counter_name;
                                _perf.instance = _current_instance;
                                _perf.value = Math.Round(_memory_used_bytes);
                                _perf.id = Objects.RamdomGuid();
                                _workload_counters.Add(_perf);

                                //memory: % used
                                Double _percentage_memory_used = ((Convert.ToDouble(_memory_used_bytes) / Convert.ToDouble(_workload_total_memory)) * 100);
                                String _memory_counter_name = "% Used";

                                _perf = new PerformanceType();
                                _perf.workload_id = _workload.id;
                                _perf.timestamp = TimeCalculations.RoundDown(DateTime.UtcNow, TimeSpan.FromHours(1)); //_counterobject.timestamp;
                                _perf.category_name = _this_workload_counter.category;
                                _perf.counter_name = _memory_counter_name;
                                _perf.instance = _current_instance;
                                _perf.value = Math.Round(_percentage_memory_used);
                                _perf.id = Objects.RamdomGuid();
                                _workload_counters.Add(_perf);
                            }

                        }

                    }
                }
            }
            WorkloadPerformanceUpload.Upload(_workload_counters, _workload);

            Logger.log(String.Format("PerformanceType: Completed PerformanceType collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}
