using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.LocalDatabase
{
    class Workloads_Update
    {
        static public void InventoryUpdateStatus(string workload_id, string message, bool status)
        {
            using (WorkloadSet _workload_db = new WorkloadSet())
            {
                var workload = _workload_db.ModelRepository.GetById(workload_id);
                if (status)
                {
                    workload.dt_collection_status = true;
                    workload.dt_contact_error_count = 0;
                }
                else
                {
                    workload.dt_collection_status = false;
                    workload.dt_contact_error_count = workload.dt_contact_error_count == null ? 1 : workload.dt_contact_error_count++;
                }
                workload.dt_collection_message = message;
                workload.dt_last_contact = DateTime.Now;
                _workload_db.Save();
            }
        }

        static public void PeformanceUpdateStatus(string workload_id, string message, bool status)
        {
            using (WorkloadSet _workload_db = new WorkloadSet())
            {
                var workload = _workload_db.ModelRepository.GetById(workload_id);
                if (status)
                {
                    workload.perf_collection_status = true;
                    workload.perf_contact_error_count = 0;
                }
                else
                {
                    workload.perf_collection_status = false;
                    workload.perf_contact_error_count = workload.perf_contact_error_count == null ? 1 : workload.perf_contact_error_count++;
                }
                workload.perf_collection_message = message;
                workload.perf_last_contact = DateTime.Now;
                _workload_db.Save();
            }
        }
        static public void DoubleTakeUpdateStatus(string workload_id, string message, bool status)
        {
            using (WorkloadSet _workload_db = new WorkloadSet())
            {
                var workload = _workload_db.ModelRepository.GetById(workload_id);
                if (status)
                {
                    workload.dt_collection_status = true;
                    workload.dt_contact_error_count = 0;
                }
                else
                {
                    workload.dt_collection_status = false;
                    workload.dt_contact_error_count = workload.dt_contact_error_count == null ? 1 : workload.dt_contact_error_count++;
                }
                workload.dt_collection_message = message;
                workload.dt_last_contact = DateTime.Now;
                _workload_db.Save();
            }
        }
    }
}
