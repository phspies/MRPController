using MRMPService.LocalDatabase;
using MRMPService.API.Types.API;
using System.Collections.Generic;
using System.Linq;
using MRMPService.API;
using MRMPService.Utilities;

namespace MRMPService.PlatformInventory
{
    partial class PlatformInventoryWorkloadDo
    {
        public static void UpdatePhysicalWorkload(string _workload_id, string _platform_id)
        {
            MRP_ApiClient _mrmp_portal = new MRP_ApiClient();

            //Retrieve portal objects
            List<MRPWorkloadType> _mrp_workloads = _mrmp_portal.workload().listworkloads().workloads.Where(x => x.platform_id == _platform_id).ToList();
            MRPWorkloadCRUDType _mrp_workload = new MRPWorkloadCRUDType();

            using (WorkloadSet _workload_db = new WorkloadSet())
            {
                Workload _workload = _workload_db.ModelRepository.GetById(_workload_id);
                Objects.Copy(_workload, _mrp_workload);

                //mark workload and provisioned
                _mrp_workload.provisioned = true;
            }
            //Update if the portal has this workload and create if it's new to the portal....
            if (_mrp_workloads.Exists(x => x.id == _workload_id))
            {
                _mrmp_portal.workload().updateworkload(_mrp_workload);
            }
            else
            {
                _mrmp_portal.workload().createworkload(_mrp_workload);
            }

        }

    }
}
