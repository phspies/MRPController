﻿using MRPService.API.Types.API;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MRPService.LocalDatabase;
using MRPService.MRPService.Log;
using MRPService.Utilities;

namespace MRPService.MRPService.Classes.Background_Classes
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
            API.MRP_ApiClient _cloud_movey = new API.MRP_ApiClient();
            while (true)
            {
                try
                {
                    Logger.log("Staring data upload process", Logger.Severity.Info);

                    List<NetworkFlow> _db_flows;
                    using (MRPDatabase db = new MRPDatabase())
                    {
                        _db_flows = db.NetworkFlows.ToList();
                    }

                    Stopwatch sw = Stopwatch.StartNew();
                    int _new_networkflows, _new_performancecounters, _new_netstat;
                    _new_networkflows = _new_performancecounters = _new_netstat = 0;

                    List<Workload> _workloads;
                    using (MRPDatabase db = new MRPDatabase())
                    {
                        _workloads = db.Workloads.ToList();
                    }

                    //process netflows information
                    foreach (NetworkFlow _db_flow in _db_flows)
                    {
                        MRPNetworkFlowCRUDType _mrp_crud = new MRPNetworkFlowCRUDType();
                        Objects.Copy(_db_flow, _mrp_crud);

                        Workload _source_workload = _workloads.FirstOrDefault(x => x.iplist.Split(',').Contains(_db_flow.source_address));
                        if (_source_workload != null)
                        {
                            _mrp_crud.source_workload_id = _source_workload.id;
                        }
                        Workload _target_workload = _workloads.FirstOrDefault(x => x.iplist.Split(',').Contains(_db_flow.target_address));
                        if (_target_workload != null)
                        {
                            _mrp_crud.target_workload_id = _target_workload.id;
                        }
                        
                        _cloud_movey.netflow().createnetworkflow(_mrp_crud);

                        //remove from local database
                        using (MRPDatabase db = new MRPDatabase())
                        {
                            var _remove = db.NetworkFlows.Find(_db_flow.id);
                            db.NetworkFlows.Remove(_remove);
                            db.SaveChanges();
                        }
                        _new_networkflows += 1;
                    }


                    List<Performance> _local_performance;
                    using (MRPDatabase db = new MRPDatabase())
                    {
                        _local_performance = db.Performance.ToList();
                    }

                    //first ensure we have a list of the portal performance categories and add what is missing
                    MRPPerformanceCategoryListType _categories = _cloud_movey.performancecategory().list();
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
                            _cloud_movey.performancecategory().create(new MRPPerformanceCategoryCRUDType() { category_name = _cat.category, counter_name = _cat.counter, workload_id = _cat.workload_id, instances = _mutiple_instances });
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
                        MRPPerformanceCounterCRUDType _performancecrud = new MRPPerformanceCounterCRUDType();
                        Objects.Copy(_performance, _performancecrud);

                        //inject performance unique ID into crud object
                        _performancecrud.performancecategory_id = _categories.performancecategories.Find(x => x.category_name == _performance.category_name && x.counter_name == _performance.counter_name && x.workload_id == _performance.workload_id).id;

                        //add record to portal
                        _cloud_movey.performancecounter().create(_performancecrud);

                        //remove from local database
                        using (MRPDatabase db = new MRPDatabase())
                        {
                            var _remove = db.Performance.Find(_performance.id);
                            db.Performance.Remove(_remove);
                            db.SaveChanges();
                        }
                        _new_performancecounters += 1;
                    }
                    //process netstat 
                    using (NetstatSet _db_netstat = new NetstatSet())
                    {
                        foreach (Netstat _db_netstat_record in _db_netstat.ModelRepository.Get())
                        {
                            MRPNetworkStatCRUDType _netstatcrud = new MRPNetworkStatCRUDType();
                            Objects.Copy(_db_netstat_record, _netstatcrud);

                            //add record to portal
                            _cloud_movey.netstat().create(_netstatcrud);

                            //remove from local database
                            _db_netstat.ModelRepository.Delete(_db_netstat_record.id);
                            _new_netstat += 1;
                        }
                    }
                    sw.Stop();

                    Logger.log(
                        String.Format("Completed data upload process.{0} netflows.{1} performancecounters. {2} netstats. = Total Elapsed Time: {3}",
                        _new_networkflows, _new_performancecounters, _new_netstat, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)
                        ), Logger.Severity.Info);
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Error in data upload task: {0}", ex.ToString()), Logger.Severity.Error);
                }
                Thread.Sleep(new TimeSpan(1, 0, 0));
            }
        }
    }
}
