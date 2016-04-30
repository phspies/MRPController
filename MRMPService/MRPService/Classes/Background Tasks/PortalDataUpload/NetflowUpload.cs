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
            API.MRP_ApiClient _cloud_movey = new API.MRP_ApiClient();
            List<NetworkFlow> _db_flows;
            using (MRPDatabase db = new MRPDatabase())
            {
                _db_flows = db.NetworkFlows.ToList();
            }

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
            }
            //process any remaining records
            if (_networkflow_list.Count() > 0)
            {
                _cloud_movey.netflow().createnetworkflow(_networkflow_list);
            }

        }
    }
}
