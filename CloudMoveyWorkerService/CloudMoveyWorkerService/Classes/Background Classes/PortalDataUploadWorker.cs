using CloudMoveyWorkerService.Portal;
using CloudMoveyWorkerService.Portal.Types.API;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Utils;
using CloudMoveyWorkerService.LocalDatabase;

namespace CloudMoveyWorkerService.CloudMoveyWorkerService.Classes.Background_Classes
{
    class PortalDataUploadWorker
    {
        class countercategory
        {
            public string category { get; set; }
            public string counter { get; set; }
            public string workload_id { get; set; }
        }
        public void Start()
        {
            CloudMoveyPortal _cloud_movey = new CloudMoveyPortal();
            while (true)
            {
                try
                {
                    Global.event_log.WriteEntry("Staring data upload process");

                    List<NetworkFlow> _db_flows;
                    using (LocalDB db = new LocalDB())
                    {
                        _db_flows = db.NetworkFlows.ToList();
                    }

                    Stopwatch sw = Stopwatch.StartNew();
                    int _new_networkflows, _new_performancecounters;
                    _new_networkflows = _new_performancecounters = 0;
                    foreach (NetworkFlow _flow in _db_flows)
                    {
                        MoveyNetworkFlowCRUDType _flowcrud = new MoveyNetworkFlowCRUDType();
                        Objects.MapObjects(_flow, _flowcrud);
                        _cloud_movey.netflow().createnetworkflow(_flowcrud);

                        //remove from local database
                        using (LocalDB db = new LocalDB())
                        {
                            var _remove = db.NetworkFlows.Find(_flow.id);
                            db.NetworkFlows.Remove(_remove);
                            db.SaveChanges();
                        }
                        _new_networkflows += 1;
                    }


                    List<Performance> _local_performance;
                    using (LocalDB db = new LocalDB())
                    {
                        _local_performance = db.Performance.ToList();
                    }

                    //first ensure we have a list of the portal performance categories and add what is missing
                    MoveyPerformanceCategoryListType _categories = _cloud_movey.performancecategory().list();
                    var _local_counterscategories = _local_performance.GroupBy(x => new { x.category_name, x.counter_name, x.workload_id }).Select(group => new countercategory() {  category = group.Key.category_name, counter = group.Key.counter_name, workload_id = group.Key.workload_id }).ToList();
                    

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
                            _cloud_movey.performancecategory().create(new MoveyPerformanceCategoryCRUDType() { category_name = _cat.category, counter_name = _cat.counter, workload_id = _cat.workload_id, instances = _mutiple_instances });
                        }
                    }
                    //get new categories if we add one... 
                    if (counters_changed)
                    {
                        _categories = _cloud_movey.performancecategory().list();
                    }



                    //process performancecounters
                    foreach (Performance _performance in _local_performance)
                    {
                        MoveyPerformanceCounterCRUDType _performancecrud = new MoveyPerformanceCounterCRUDType();
                        Objects.MapObjects(_performance, _performancecrud);

                        //inject performance unique ID into crud object
                        _performancecrud.performancecategory_id = _categories.performancecategories.Find(x => x.category_name == _performance.category_name && x.counter_name == _performance.counter_name && x.workload_id == _performance.workload_id).id;

                        //add record to portal
                        _cloud_movey.performancecounter().create(_performancecrud);

                        //remove from local database
                        using (LocalDB db = new LocalDB())
                        {
                            var _remove = db.Performance.Find(_performance.id);
                            db.Performance.Remove(_remove);
                            db.SaveChanges();
                        }
                        _new_performancecounters += 1;
                    }


                    sw.Stop();

                    Global.event_log.WriteEntry(
                        String.Format("Completed data upload process.{2}{0} netflows.{2}{1} performancecounters.{2}{2} Total Execute Time: {3}",
                        _new_networkflows, _new_performancecounters,
                        Environment.NewLine, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)
                        ));
                }
                catch (Exception ex)
                {
                    Global.event_log.WriteEntry(String.Format("Error in data upload task: {0}", ex.ToString()), EventLogEntryType.Error);
                }
                Thread.Sleep(new TimeSpan(1, 0, 0));
            }
        }
    }
}
