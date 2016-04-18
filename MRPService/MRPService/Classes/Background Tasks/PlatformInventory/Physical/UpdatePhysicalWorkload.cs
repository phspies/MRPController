using MRPService.LocalDatabase;
using MRPService.API.Types.API;
using System.Collections.Generic;
using System.Linq;
using MRPService.API;

namespace MRPService.PlatformInventory
{
    partial class PlatformInventoryWorkloadDo
    {
        public static void UpdatePhysicalWorkload(string _workload_id, string _platform_id)
        {
            MRP_ApiClient _cloud_movey = new MRP_ApiClient();

            //Retrieve portal objects
            List<MRPWorkloadType> _mrp_workloads = _cloud_movey.workload().listworkloads().workloads.Where(x => x.platform_id == _platform_id).ToList();
            MRPWorkloadCRUDType _mrp_workload = new MRPWorkloadCRUDType();

            using (WorkloadSet _workload_db = new WorkloadSet())
            {
                Workload _workload = _workload_db.ModelRepository.GetById(_workload_id);
                _mrp_workload.id = _workload_id;
                _mrp_workload.credential_id = _workload.credential_id;
                _mrp_workload.enabled = _workload.enabled;
            }
            //Update if the portal has this workload and create if it's new to the portal....
            if (_mrp_workloads.Exists(x => x.id == _workload_id))
            {
                _cloud_movey.workload().updateworkload(_mrp_workload);
            }
            else
            {
                _cloud_movey.workload().createworkload(_mrp_workload);
            }

        }

    }
}
