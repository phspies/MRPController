using MRMPService.LocalDatabase;
using MRMPService.MRMPAPI.Types.API;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization.Formatters.Binary;
using MRMPService.Utilities;
using MRMPService.MRMPService.Log;
using System.Collections.Generic;

namespace MRMPService.MRMPAPI.Classes
{
    partial class WorkloadInventory
    {
        public void WorkloadInventoryUnixDo(MRPWorkloadType _workload)
        {
            MRPWorkloadType _updated_workload = new MRPWorkloadType() { id = _workload.id };

            //check for credentials
            MRPCredentialType _credential = _workload.credential;
            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Error finding credentials"));
            }

            string workload_ip = null;
            using (Connection _connection = new Connection())
            {
                workload_ip = _connection.FindConnection(_workload.iplist, true);
            }
            if (workload_ip == null)
            {
                throw new ArgumentException(String.Format("Error finding contactable IP"));
            }

            Logger.log(String.Format("Inventory: Started inventory collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);



            _updated_workload.ostype = "unix";
            _updated_workload.provisioned = true;

            using (MRMP_ApiClient _api = new MRMP_ApiClient())
            {
                _api.workload().InventoryUpdateStatus(_updated_workload, "Success", true);
                _api.workload().updateworkload(_updated_workload);
            }

            Logger.log(String.Format("Inventory: Completed inventory collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}