using CloudMoveyWorkerService.CaaS1;
using CloudMoveyWorkerService.CaaS2;
using CloudMoveyWorkerService.Portal.Sqlite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CMDoubleTake
{
    class CMDoubleTake
    {
        public Workload _source_workload, _target_workload;
        private static CloudMoveyEntities dbcontext = new CloudMoveyEntities();

        public CMDoubleTake(string source_workload_id, string target_workload_id)
        {
            //confirm systems exists
            _target_workload = dbcontext.Workloads.FirstOrDefault(x => x.id == target_workload_id);
            _source_workload = dbcontext.Workloads.FirstOrDefault(x => x.id == source_workload_id);

            //target can never be empty
            if (_target_workload == null)
            {
                //report error to console
            }

            //source could be empty in certian instances
            if (_source_workload == null)
            {

            }
        }

        public CMDoubleTake_DisasterRecovery disasterrecovery() {
            return new CMDoubleTake_DisasterRecovery(this);
        }

        public CMDoubleTake_HighAvailability highavailability()
        {
            return new CMDoubleTake_HighAvailability(this);
        }

    }
}
