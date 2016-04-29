﻿using MRMPService.API.Types.API;
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
                DateTime _next_upload_run = DateTime.Now.AddMinutes(Global.portal_upload_interval);

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
                    List<MRPNetworkFlowCRUDType> _networkflow_list = new List<MRPNetworkFlowCRUDType>();
                    foreach (NetworkFlow _db_flow in _db_flows)
                    {
                        MRPNetworkFlowCRUDType _mrp_crud = new MRPNetworkFlowCRUDType();
                        Objects.Copy(_db_flow, _mrp_crud);

                        //add record to list
                        _networkflow_list.Add(_mrp_crud);

                        //process batch
                        if (_networkflow_list.Count() > Global.portal_upload_netflow_page_size)
                        {
                            _cloud_movey.netflow().createnetworkflow(_networkflow_list);
                            _networkflow_list.Clear();
                        }

                        //remove from local database
                        using (MRPDatabase db = new MRPDatabase())
                        {
                            var _remove = db.NetworkFlows.Find(_db_flow.id);
                            db.NetworkFlows.Remove(_remove);
                            db.SaveChanges();
                        }
                        _new_networkflows += 1;
                    }
                    //process any remaining records
                    if (_networkflow_list.Count() > 0)
                    {
                        _cloud_movey.netflow().createnetworkflow(_networkflow_list);
                    }

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
                            _new_performancecounters += 1;
                        }
                        //upload last remaining records
                        if (_performancecounters_list.Count > 0)
                        {
                            _cloud_movey.performancecounter().create(_performancecounters_list);
                        }
                    }

                    //process netstat 
                    using (NetstatSet _db_netstat = new NetstatSet())
                    {
                        List<MRPNetworkStatCRUDType> _netstat_list = new List<MRPNetworkStatCRUDType>();

                        foreach (Netstat _db_netstat_record in _db_netstat.ModelRepository.Get())
                        {
                            MRPNetworkStatCRUDType _netstatcrud = new MRPNetworkStatCRUDType();
                            Objects.Copy(_db_netstat_record, _netstatcrud);

                            //add record to list
                            _netstat_list.Add(_netstatcrud);

                            //process batch
                            if (_netstat_list.Count > Global.portal_upload_netstat_page_size)
                            {
                                //add record to portal
                                _cloud_movey.netstat().create_bulk(_netstat_list);

                                //reset list 
                                _netstat_list.Clear();
                            }

                            //remove from local database
                            _db_netstat.ModelRepository.Delete(_db_netstat_record.id);
                            _new_netstat += 1;
                        }
                        //process last remaining netstat entries
                        if (_netstat_list.Count > 0)
                        {
                            _cloud_movey.netstat().create_bulk(_netstat_list);
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
                //Wait for next run
                while (_next_upload_run > DateTime.Now)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}
