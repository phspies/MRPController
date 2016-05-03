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
    class NetflowUpload
    {
        public void Start()
        {
            Logger.log("Starting Netflow Upload Thread", Logger.Severity.Debug);
            Stopwatch _sw = Stopwatch.StartNew();
            using (API.MRP_ApiClient _cloud_movey = new API.MRP_ApiClient())
            {
                using (MRPDatabase db = new MRPDatabase())
                {
                    List<NetworkFlow> _db_flows = db.NetworkFlows.AsEnumerable().ToList();

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
                    }
                    //process any remaining records
                    if (_networkflow_list.Count() > 0)
                    {
                        _cloud_movey.netflow().createnetworkflow(_networkflow_list);
                    }
                    //remove all processed records from from local database
                    Stopwatch _sw_delete = Stopwatch.StartNew();
                    db.NetworkFlows.RemoveRange(_db_flows);
                    db.SaveChanges();
                    _sw_delete.Stop();
                    Logger.log(String.Format("Took {0} to delete {1} performance records", TimeSpan.FromMilliseconds(_sw_delete.Elapsed.TotalMilliseconds), _db_flows.Count()), Logger.Severity.Debug);

                }
            }
            _sw.Stop();
            Logger.log(String.Format("Completed Netflow Upload Thread in {0}", TimeSpan.FromMilliseconds(_sw.Elapsed.TotalMilliseconds)), Logger.Severity.Debug);

        }
    }
}
