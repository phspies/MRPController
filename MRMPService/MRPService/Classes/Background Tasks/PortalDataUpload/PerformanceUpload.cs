using MRMPService.API.Types.API;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;
using MRMPService.Utilities;

namespace MRMPService.MRMPService.Classes.Background_Classes
{
    class countercategory
    {
        public string category { get; set; }
        public string counter { get; set; }
        public string workload_id { get; set; }
    }
    class PerformanceUpload
    {
        public void Start()
        {
            API.MRP_ApiClient _cloud_movey = new API.MRP_ApiClient();

            List<Performance> _local_performance;
            using (MRPDatabase db = new MRPDatabase())
            {
                _local_performance = db.Performance.ToList();
            }

            //check if workload exists for performancecounter and remove if required
            using (WorkloadSet _db_workload = new WorkloadSet())
            {
                var _workload_grouped = _local_performance.Select(x => x.workload_id).ToList().Distinct();
                foreach (var _workload in _workload_grouped)
                {
                    if (_db_workload.ModelRepository.GetById(_workload) == null)
                    {
                        MRPDatabase _db = new MRPDatabase();
                        _db.Performance.RemoveRange(_db.Performance.Where(x => x.workload_id == _workload));
                        _db.SaveChanges();
                    }
                }
            }
            //get an updated list form database
            using (MRPDatabase db = new MRPDatabase())
            {
                _local_performance = db.Performance.ToList();
            }


            //first ensure we have a list of the portal performance categories and add what is missing
            MRPPerformanceCategoryListType _categories = _cloud_movey.performancecategory().list();
            var _local_counterscategories = _local_performance.GroupBy(x => new { x.category_name, x.counter_name, x.workload_id }).Select(group => new countercategory() { category = group.Key.category_name, counter = group.Key.counter_name, workload_id = group.Key.workload_id }).ToList();


            bool counters_changed = false;
            foreach (countercategory _cat in _local_counterscategories)
            {
                bool _mutiple_instances = false;
                if (!_local_performance.Exists(x => x.category_name == _cat.category && x.counter_name == _cat.counter && x.instance == ""))
                {
                    _mutiple_instances = true;
                }
                if (!_categories.performancecategories.Exists(x => x.category_name == _cat.category && x.counter_name == _cat.counter && x.workload_id == _cat.workload_id))
                {
                    counters_changed = true;
                    _cloud_movey.performancecategory().create(new MRPPerformanceCategoryCRUDType() { category_name = _cat.category, counter_name = _cat.counter, workload_id = _cat.workload_id, instances = _mutiple_instances });
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

                foreach (Performance _performance in _local_performance)
                {

                    MRPPerformanceCounterCRUDType _performancecrud = new MRPPerformanceCounterCRUDType();

                    Objects.Copy(_performance, _performancecrud);

                    //inject performance unique ID into crud object
                    _performancecrud.performancecategory_id = _categories.performancecategories.Find(x => x.category_name == _performance.category_name && x.counter_name == _performance.counter_name && x.workload_id == _performance.workload_id).id;

                    //add record to list
                    _performancecounters_list.Add(_performancecrud);

                    //process batch
                    if (_performancecounters_list.Count > Global.portal_upload_performanceounter_page_size)
                    {
                        _cloud_movey.performancecounter().create(_performancecounters_list);
                        _performancecounters_list.Clear();
                    }

                    //remove from local database
                    using (MRPDatabase db = new MRPDatabase())
                    {
                        var _remove = db.Performance.Find(_performance.id);
                        db.Performance.Remove(_remove);
                        db.SaveChanges();
                    }
                }
                //upload last remaining records
                if (_performancecounters_list.Count > 0)
                {
                    _cloud_movey.performancecounter().create(_performancecounters_list);
                }
            }
        }
    }
}
