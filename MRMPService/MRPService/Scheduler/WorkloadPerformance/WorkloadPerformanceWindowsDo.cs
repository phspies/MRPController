using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MRMPService.Utilities;
using MRMPService.Modules.MRMPPortal.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;

namespace MRMPService.Scheduler.PerformanceCollection
{
    partial class WorkloadPerformance
    {
        public static async Task WorkloadPerformanceWindowsDo(MRPWorkloadType _workload)
        {
            string workload_ip = null;
            if (_workload.workloadtype != "manager")
            {
                workload_ip = _workload.working_ipaddress(true);
            }
            else
            {
                workload_ip = ".";

            }
            List<PerformanceType> _workload_counters = new List<PerformanceType>();
            Logger.log(String.Format("Performance: Start performance collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
            using (new Impersonator((_workload.workloadtype == "manager" ? "." : _workload.get_credential.username), (_workload.workloadtype == "manager" ? null : (String.IsNullOrEmpty(_workload.get_credential.domain) ? "." : _workload.get_credential.domain)), (_workload.workloadtype == "manager" ? null : _workload.get_credential.decrypted_password)))
            {
                //loop each counter in the available counter list
                foreach (string _current_category in MRMPServiceBase._available_counters.Select(x => x.category).Distinct())
                {
                    try
                    {
                        if (!PerformanceCounterCategory.Exists(_current_category, workload_ip))
                        {
                            continue;
                        }
                        PerformanceCounterCategory _current_catergory_object = new PerformanceCounterCategory(_current_category, workload_ip);
                        bool _multi_instance_counter = Convert.ToBoolean(_current_catergory_object.CategoryType);
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
                                if (MRMPServiceBase._available_counters.FirstOrDefault(x => x.category == _current_category).counter != "*" && !MRMPServiceBase._available_counters.Any(x => x.counter == _counter.CounterName))
                                {
                                    continue;
                                }

                                PerfCounterSample _this_workload_counter;
                                bool _found_record = false;
                                CounterSample _s0 = new CounterSample();
                                CounterSample _s1 = new CounterSample();
                                //check instance in the counter object list and create if required
                                using (PerfCounterSampleSet _db_perf = new PerfCounterSampleSet())
                                {
                                    _this_workload_counter = _db_perf.ModelRepository.GetFirstOrDefault(y => y.workload_id == _workload.id && y.category == _current_category && y.counter == _counter.CounterName && y.instance == _current_instance);
                                }
                                if (_this_workload_counter != null)
                                {
                                    _found_record = true;
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
                                        await Task.Delay(new TimeSpan(0, 0, 1));
                                    }
                                }
                                else
                                {
                                    _this_workload_counter = new PerfCounterSample();
                                    _s0 = _counter.NextSample();
                                    await Task.Delay(new TimeSpan(0, 0, 1));
                                }
                                _s1 = _counter.NextSample();
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

                                Performancecounter _perf = new Performancecounter();
                                _perf.workload_id = _workload.id;
                                _perf.timestamp = TimeCalculations.RoundDown(DateTime.FromFileTimeUtc(_s1.TimeStamp100nSec), TimeSpan.FromHours(1)); //_counterobject.timestamp;
                                _perf.category = _this_workload_counter.category;
                                _perf.counter = _this_workload_counter.counter;
                                _perf.instance = _current_instance;
                                _perf.value = Math.Round(_value, 2);
                                _perf.id = Objects.RamdomGuid();
                                using (PerformancecounterSet _perf_db = new PerformancecounterSet())
                                {
                                    _perf_db.ModelRepository.Insert(_perf);
                                }
                                if (_this_workload_counter.category == "Memory" && _this_workload_counter.counter == "Available Bytes")
                                {
                                    long _workload_total_memory = (Convert.ToInt64(_workload.vmemory) * 1024 * 1024 * 1024);
                                    Double _memory_used_bytes = _workload_total_memory - _value;
                                    String _memory_used_counter = "Used Bytes";

                                    _perf = new Performancecounter();
                                    _perf.workload_id = _workload.id;
                                    _perf.timestamp = TimeCalculations.RoundDown(DateTime.UtcNow, TimeSpan.FromHours(1)); //_counterobject.timestamp;
                                    _perf.category = _this_workload_counter.category;
                                    _perf.counter = _memory_used_counter;
                                    _perf.instance = _current_instance;
                                    _perf.value = Math.Round(_memory_used_bytes);
                                    _perf.id = Objects.RamdomGuid();
                                    using (PerformancecounterSet _perf_db = new PerformancecounterSet())
                                    {
                                        _perf_db.ModelRepository.Insert(_perf);
                                    }
                                    Double _percentage_memory_used = ((Convert.ToDouble(_memory_used_bytes) / Convert.ToDouble(_workload_total_memory)) * 100);
                                    String _memory_counter = "% Used";

                                    _perf = new Performancecounter();
                                    _perf.workload_id = _workload.id;
                                    _perf.timestamp = TimeCalculations.RoundDown(DateTime.UtcNow, TimeSpan.FromHours(1)); //_counterobject.timestamp;
                                    _perf.category = _this_workload_counter.category;
                                    _perf.counter = _memory_counter;
                                    _perf.instance = _current_instance;
                                    _perf.value = Math.Round(_percentage_memory_used);
                                    _perf.id = Objects.RamdomGuid();
                                    using (PerformancecounterSet _perf_db = new PerformancecounterSet())
                                    {
                                        _perf_db.ModelRepository.Insert(_perf);
                                    }
                                }
                            }
                        }
                        if (_current_category == "Network Interface")
                        {
                            var _current_timestamp = TimeCalculations.RoundDown(DateTime.UtcNow, TimeSpan.FromHours(1));
                            foreach (var _counter in new string[] { "Bytes Received/sec", "Bytes Sent/sec" })
                            {
                                Performancecounter _perf = new Performancecounter();
                                _perf.workload_id = _workload.id;
                                _perf.timestamp = _current_timestamp;
                                _perf.category = _current_category;
                                _perf.counter = _counter;
                                _perf.instance = "_Total";
                                _perf.id = Objects.RamdomGuid();
                                using (PerformancecounterSet _perf_db = new PerformancecounterSet())
                                {
                                    _perf.value = _perf_db.ModelRepository.Get(x => x.category == "Network Interface" && x.counter == _counter && x.instance != "_Total" && x.timestamp == _current_timestamp).AsEnumerable().Sum(x => x.value);
                                    _perf_db.ModelRepository.Insert(_perf);
                                }
                            }
                        }
                    }
                    catch (UnauthorizedAccessException access_ex)
                    {
                        throw new Exception(access_ex.GetBaseException().Message);
                    }
                    catch (IOException io_ex)
                    {
                        throw new Exception(io_ex.GetBaseException().Message);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.ToLower().Contains("access denied"))
                        {
                            throw new Exception(ex.GetBaseException().Message);
                        }
                        else
                        {
                            Logger.log(String.Format("{0} returned the following error while collecting performance data for counter : {1} : {2}", workload_ip, _current_category, ex.Message), Logger.Severity.Error);
                            continue;
                        }
                    }
                }
                Logger.log(String.Format("Performance: Completed performance collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
            }
        }
    }
}
