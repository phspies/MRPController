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

    class NetstatUpload
    {
        public void Start()
        {
            API.MRP_ApiClient _cloud_movey = new API.MRP_ApiClient();

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
                }
                //process last remaining netstat entries
                if (_netstat_list.Count > 0)
                {
                    _cloud_movey.netstat().create_bulk(_netstat_list);
                }
            }
        }
    }
}
