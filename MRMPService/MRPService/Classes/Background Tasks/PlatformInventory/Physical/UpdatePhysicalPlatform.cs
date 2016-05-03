using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using MRMPService.API.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DD.CBU.Compute.Api.Client;
using System.Net;
using DD.CBU.Compute.Api.Contracts.Requests.Server20;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Requests;
using DD.CBU.Compute.Api.Contracts.Requests.Infrastructure;
using DD.CBU.Compute.Api.Contracts.General;
using MRMPService.Utilities;
using MRMPService.API;

namespace MRMPService.PlatformInventory
{
    class PlatformPhysicalInventoryDo
    {

        public static void UpdatePhysicalPlatform(String _platform_id)
        {
            MRP_ApiClient _mrp_api_endpoint = new MRP_ApiClient();
            Platform _platform;
            using (PlatformSet _platform_db = new PlatformSet())
            {
                _platform = _platform_db.ModelRepository.GetById(_platform_id);
            }
            if (_platform == null)
            {
                
            }
            try
            {
                Logger.log(String.Format("Started inventory process for {0}", _platform.description), Logger.Severity.Info);
                Stopwatch sw = Stopwatch.StartNew();

                //define object lists
                List<MRPWorkloadType> _mrp_workloads = _mrp_api_endpoint.workload().listworkloads().workloads.Where(x => x.platform_id == _platform_id).ToList();

                //process workloads
                using (WorkloadSet _workload_dbset = new WorkloadSet())
                {
                    List<Workload> _workloads = _workload_dbset.ModelRepository.Get(x => x.platform_id == _platform.id);
                    using (PlatformSet _platform_dbset = new PlatformSet())
                    {
                        Platform _db_platform = _platform_dbset.ModelRepository.GetById(_platform.id);
                        _db_platform.vlan_count = 0;
                        _db_platform.workload_count = _workloads.Count;
                        _db_platform.networkdomain_count = 0;
                        _db_platform.platform_version = "na";

                        _db_platform.lastupdated = DateTime.UtcNow;
                        _db_platform.human_vendor = (new Vendors()).VendorList.First(x => x.ID == _platform.vendor).Vendor;
                        _db_platform.moid = "na";
                        _platform_dbset.Save();
                    }

                    foreach (Workload _workload in _workloads)
                    {
                        PlatformInventoryWorkloadDo.UpdatePhysicalWorkload(_workload.id, _workload.platform_id);
                    }
                }

                sw.Stop();
                Logger.log(String.Format("Completed inventory process for {0} = Total Execute Time: {1}", _platform.description, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)), Logger.Severity.Info);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error in inventory process for {0} {1}", _platform.description, ex.ToString()), Logger.Severity.Error);
            }
        }

    }
}
