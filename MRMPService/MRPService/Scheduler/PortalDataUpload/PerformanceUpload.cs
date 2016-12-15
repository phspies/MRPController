using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MRMPService.MRMPService.Log;
using System.Threading.Tasks;
using MRMPService.LocalDatabase;
using System.Linq;

namespace MRMPService.Scheduler.PortalDataUpload
{
    class WorkloadPerformanceUpload
    {
        public async void Start()
        {
            try
            {
                Logger.log("Starting Performance Upload Thread", Logger.Severity.Debug);
                Stopwatch _sw = Stopwatch.StartNew();
                while (true)
                {
                    List<Performancecounter> _increment_records = new List<Performancecounter>();
                    using (MRPDatabase _db = new MRPDatabase())
                    {
                        _increment_records = _db.Performancecounters.Take(200).ToList();
                        if (_increment_records.Count() == 0)
                        {
                            break;
                        }
                    }
                    List<MRPPerformanceCounterCRUDType> _performancecounters_list = new List<MRPPerformanceCounterCRUDType>();
                    foreach (Performancecounter _db_perf in _increment_records)
                    {
                        MRPPerformanceCounterCRUDType _performancecrud = new MRPPerformanceCounterCRUDType();
                        _performancecrud.instance = _db_perf.instance;
                        _performancecrud.category = _db_perf.category;
                        _performancecrud.counter = _db_perf.counter;
                        _performancecrud.timestamp = _db_perf.timestamp;
                        _performancecrud.value = _db_perf.value;
                        _performancecrud.workload_id = _db_perf.workload_id;
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
                        _performancecounters_list.Clear();
                    }

                    //remove all processed records from from local database
                    Stopwatch _sw_delete = Stopwatch.StartNew();
                    using (PerformancecounterSet _db = new PerformancecounterSet())
                    {
                        foreach (var _record in _increment_records)
                        {
                            _db.ModelRepository.Delete(_record.id);
                        }
                    }
                    _sw_delete.Stop();
                    Logger.log(String.Format("Took {0} to delete {1} performance records", TimeSpan.FromMilliseconds(_sw_delete.Elapsed.TotalMilliseconds), _increment_records.Count()), Logger.Severity.Debug);
                }
                _sw.Stop();
                Logger.log(String.Format("Completed Performance Upload Thread in {0}", TimeSpan.FromMilliseconds(_sw.Elapsed.TotalMilliseconds)), Logger.Severity.Debug);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error Uploading Performance information to portal {0}", ex.ToString()), Logger.Severity.Error);
            }
        }
    }
}
