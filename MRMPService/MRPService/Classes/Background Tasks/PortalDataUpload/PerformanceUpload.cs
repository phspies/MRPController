using MRMPService.MRMPAPI.Types.API;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI;
using System.Threading;

namespace MRMPService.MRMPService.Classes.Background_Classes
{
    class countercategory
    {
        public string category_name { get; set; }
        public string counter_name { get; set; }
        public string workload_id { get; set; }
    }
    class PerformanceUpload
    {
        public void Start()
        {
            try
            {
                Logger.log("Starting Performance Upload Thread", Logger.Severity.Debug);
                Stopwatch _sw = Stopwatch.StartNew();
                using (MRMPAPI.MRMP_ApiClient _cloud_movey = new MRMPAPI.MRMP_ApiClient())
                {
                    List<MRPWorkloadType> _mrp_workloads;
                    using (MRMP_ApiClient _api = new MRMP_ApiClient())
                    {
                        _mrp_workloads = _api.workload().listworkloads().workloads;
                    }
                    if (_mrp_workloads == null)
                    {
                        throw new System.ArgumentException(String.Format("Performance Upload: Error connecting retrieving workloads"));
                    }

                    while (true)
                    {
                        int _increment_record_count = 0;
                        IEnumerable<Performance> _increment_records;
                        IEnumerable<String> _workload_grouped;
                        using (MRPDatabase _db = new MRPDatabase())
                        {
                            _increment_records = _db.Performance.Take(500).AsEnumerable();
                            _increment_record_count = _increment_records.Count();
                            _workload_grouped = _increment_records.Select(x => x.workload_id).Distinct().ToList();

                            if (_increment_record_count > 0)
                            {
                                //check if workload exists for performancecounter and remove if required
                                foreach (var _workload in _workload_grouped)
                                {
                                    if (!_mrp_workloads.Exists(x => x.id == _workload))
                                    {
                                        _db.Performance.RemoveRange(_db.Performance.Where(x => x.workload_id == _workload));
                                        _db.SaveChanges();
                                    }
                                }

                                //first ensure we have a list of the portal performance categories and add what is missing
                                MRPPerformanceCategoryListType _categories = _cloud_movey.performancecategory().list();
                                var _local_counterscategories = _increment_records.GroupBy(x => new { x.category_name, x.counter_name, x.workload_id }).Select(group => new countercategory() { category_name = group.Key.category_name, counter_name = group.Key.counter_name, workload_id = group.Key.workload_id }).ToList();


                                bool counters_changed = false;
                                foreach (countercategory _cat in _local_counterscategories)
                                {
                                    if (!_categories.performancecategories.Exists(x => x.category_name == _cat.category_name && x.counter_name == _cat.counter_name && x.workload_id == _cat.workload_id))
                                    {
                                        counters_changed = true;
                                        _cloud_movey.performancecategory().create(new MRPPerformanceCategoryCRUDType() { category_name = _cat.category_name, counter_name = _cat.counter_name, workload_id = _cat.workload_id, instances = true });
                                    }
                                }
                                //get new categories if we add one... 
                                if (counters_changed)
                                {
                                    _categories = _cloud_movey.performancecategory().list();
                                }
                                if (_categories.performancecategories.Count > 0)
                                {
                                    //process performancecounters
                                    List<MRPPerformanceCounterCRUDType> _performancecounters_list = new List<MRPPerformanceCounterCRUDType>();
                                    foreach (Performance _performance in _increment_records)
                                    {
                                        MRPPerformanceCounterCRUDType _performancecrud = new MRPPerformanceCounterCRUDType();

                                        _performancecrud.instance = _performance.instance;
                                        _performancecrud.timestamp = _performance.timestamp;
                                        _performancecrud.value = _performance.value;
                                        _performancecrud.workload_id = _performance.workload_id;


                                        MRPPerformanceCategoryType _category = _categories.performancecategories.FirstOrDefault(x => x.category_name == _performance.category_name && x.counter_name == _performance.counter_name && x.workload_id == _performance.workload_id);
                                        _performancecrud.performancecategory_id = _category.id;
                                        _performancecounters_list.Add(_performancecrud);

                                        if (_performancecounters_list.Count > Global.portal_upload_performanceounter_page_size + 100)
                                        {
                                            _cloud_movey.performancecounter().create(_performancecounters_list);
                                            _performancecounters_list.Clear();
                                        }
                                    }
                                    //upload last remaining records
                                    if (_performancecounters_list.Count > 0)
                                    {
                                        _cloud_movey.performancecounter().create(_performancecounters_list);
                                    }
                                }

                                //remove all processed records from from local database
                                Stopwatch _sw_delete = Stopwatch.StartNew();
                                _db.Performance.RemoveRange(_increment_records);
                                _db.SaveChanges();
                                _sw_delete.Stop();
                                Logger.log(String.Format("Took {0} to delete {1} performance records", TimeSpan.FromMilliseconds(_sw_delete.Elapsed.TotalMilliseconds), _increment_records.Count()), Logger.Severity.Debug);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                _sw.Stop();
                Logger.log(String.Format("Completed Performance Upload Thread in {0}", TimeSpan.FromMilliseconds(_sw.Elapsed.TotalMilliseconds)), Logger.Severity.Debug);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error uploading performance information to portal {0}", ex.ToString()), Logger.Severity.Error);
            }
        }
    }
}
