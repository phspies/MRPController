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
    class PerformanceUpload : IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PerformanceUpload()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        public void Start()
        {
            try
            {
                Logger.log("Starting Performance Upload Thread", Logger.Severity.Debug);
                Stopwatch _sw = Stopwatch.StartNew();
                using (MRMP_ApiClient _cloud_movey = new MRMP_ApiClient())
                {
                    MRPPerformanceCategoryListType _categories = _cloud_movey.performancecategory().list();
                    while (true)
                    {
                        IEnumerable<Performance> _increment_records;
                        using (MRPDatabase _db = new MRPDatabase())
                        {
                            _increment_records = _db.Performance.Take(500).AsEnumerable();
                            if (_increment_records.Count() == 0)
                            {
                                break;
                            }
                            //first ensure we have a list of the portal performance categories and add what is missing
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

                                    if (_performancecounters_list.Count > Global.portal_upload_performanceounter_page_size)
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
