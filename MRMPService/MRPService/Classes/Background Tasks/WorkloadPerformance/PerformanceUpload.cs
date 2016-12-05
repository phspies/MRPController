using MRMPService.MRMPAPI.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using MRMPService.MRMPService.Log;
using System.Threading.Tasks;
using MRMPService.MRMPAPI;

namespace MRMPService.PerformanceCollection
{
    class CounterCategory
    {
        public string category_name { get; set; }
        public string counter_name { get; set; }
        public string workload_id { get; set; }
    }
    class WorkloadPerformanceUpload
    {
        static public async Task Upload(List<PerformanceType> _performancecounters, MRPWorkloadType _workload)
        {
            try
            {
                Logger.log(String.Format("Starting Performance Upload for {0}", _workload.hostname), Logger.Severity.Debug);
                Stopwatch _sw = Stopwatch.StartNew();
                MRPPerformanceCategoryListType _categories = new MRPPerformanceCategoryListType();

                _categories = await MRMPServiceBase._mrmp_api.performancecategory().list(new MRPPerformanceCategoryFilterType() { workload_id = _workload.id });
                //first ensure we have a list of the portal performance categories and add what is missing
                var _local_counterscategories = _performancecounters.GroupBy(x => new { x.category_name, x.counter_name, x.workload_id }).Select(group => new CounterCategory() { category_name = group.Key.category_name, counter_name = group.Key.counter_name, workload_id = group.Key.workload_id }).ToList();


                bool counters_changed = false;

                foreach (CounterCategory _cat in _local_counterscategories)
                {
                    if (!_categories.performancecategories.Exists(x => x.category_name == _cat.category_name && x.counter_name == _cat.counter_name && x.workload_id == _cat.workload_id))
                    {
                        counters_changed = true;
                        await MRMPServiceBase._mrmp_api.performancecategory().create(new MRPPerformanceCategoryCRUDType() { category_name = _cat.category_name, counter_name = _cat.counter_name, workload_id = _cat.workload_id, instances = true });
                    }
                }
                //get new categories if we add one... 
                if (counters_changed)
                {
                    _categories = await MRMPServiceBase._mrmp_api.performancecategory().list(new MRPPerformanceCategoryFilterType() { workload_id = _workload.id });
                }

                if (_categories.performancecategories.Count > 0)
                {
                    //process performancecounters
                    List<MRPPerformanceCounterCRUDType> _performancecounters_list = new List<MRPPerformanceCounterCRUDType>();
                    foreach (PerformanceType _performance in _performancecounters)
                    {
                        MRPPerformanceCounterCRUDType _performancecrud = new MRPPerformanceCounterCRUDType();

                        _performancecrud.instance = _performance.instance;
                        _performancecrud.timestamp = _performance.timestamp;
                        _performancecrud.value = _performance.value;
                        _performancecrud.workload_id = _performance.workload_id;

                        MRPPerformanceCategoryType _category = _categories.performancecategories.FirstOrDefault(x => x.category_name == _performance.category_name && x.counter_name == _performance.counter_name && x.workload_id == _performance.workload_id);
                        _performancecrud.performancecategory_id = _category.id;
                        _performancecounters_list.Add(_performancecrud);

                        if (_performancecounters_list.Count > MRMPServiceBase.portal_upload_performanceounter_page_size)
                        {

                            await MRMPServiceBase._mrmp_api.performancecounter().create(_performancecounters_list);
                            _performancecounters_list.Clear();
                        }
                    }
                    //upload last remaining records
                    if (_performancecounters_list.Count > 0)
                    {
                        await MRMPServiceBase._mrmp_api.performancecounter().create(_performancecounters_list);
                    }
                }

                _sw.Stop();
                Logger.log(String.Format("Completed Performance Upload for {0} in {1}", _workload.hostname, TimeSpan.FromMilliseconds(_sw.Elapsed.TotalMilliseconds)), Logger.Severity.Debug);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error uploading performance information to portal {0}", ex.ToString()), Logger.Severity.Error);
            }
        }
    }
}
