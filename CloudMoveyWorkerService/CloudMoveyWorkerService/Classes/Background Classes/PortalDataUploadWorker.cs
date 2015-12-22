using CloudMoveyWorkerService.Database;
using CloudMoveyWorkerService.Portal;
using CloudMoveyWorkerService.Portal.Types.API;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Utils;

namespace CloudMoveyWorkerService.CloudMoveyWorkerService.Classes.Background_Classes
{
    class PortalDataUploadWorker
    {
        class countercategory
        {
            public string category { get; set; }
            public string counter { get; set; }
        }
        public void Start()
        {
            CloudMoveyPortal _cloud_movey = new CloudMoveyPortal();
            while (true)
            {
                try
                {
                    Global.event_log.WriteEntry("Staring data upload process");

                    Stopwatch sw = Stopwatch.StartNew();
                    int _new_networkflows, _new_performancecounters;
                    _new_networkflows = _new_performancecounters = 0;
                    foreach (NetworkFlow _flow in LocalData.get_as_list<NetworkFlow>())
                    {
                        MoveyNetworkFlowCRUDType _flowcrud = new MoveyNetworkFlowCRUDType();
                        Objects.MapObjects(_flow, _flowcrud);
                        _cloud_movey.netflow().createnetworkflow(_flowcrud);

                        //remove from local database
                        LocalData.delete_record<NetworkFlow>(_flow.id);

                        _new_networkflows += 1;
                    }

                    List<Performance> _local_performance = LocalData.get_as_list<Performance>().ToList();
                    //first ensure we have a list of the portal performance categories and add what is missing
                    MoveyPerformanceCategoryListType _categories = _cloud_movey.performancecategory().list();
                    var _local_counterscategories = _local_performance.GroupBy(x => new { x.category_name, x.counter_name })
                        .Select(y => new countercategory()
                        {
                            category = y.Key.category_name,
                            counter = y.Key.counter_name,
                        }
                    );

                    bool counters_changed = false;
                    foreach (MoveyPerformanceCategoryType _cat in _categories.performancecategories)
                    {
                        //if category does not exist in the portal, add it
                        if (!_local_counterscategories.ToList().Exists(x => x.category == _cat.category_name && x.counter == _cat.counter_name))
                        {
                            counters_changed = true;
                            _cloud_movey.performancecategory().create(new MoveyPerformanceCategoryCRUDType() { category_name = _cat.category_name, counter_name = _cat.counter_name });
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
                        _performancecrud.performancecategory_id = _categories.performancecategories.Find(x => x.category_name == _performance.category_name && x.counter_name == _performance.counter_name).id;

                        //add record to portal
                        _cloud_movey.performancecounter().create(_performancecrud);

                        //remove from local database
                        LocalData.delete_record<Performance>(_performance.id);

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
