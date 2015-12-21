using CloudMoveyWorkerService.Database;
using System.Linq;

namespace CloudMoveyWorkerService.CMDoubleTake
{
    class CMDoubleTake
    {
        public Workload _source_workload, _target_workload;

        public CMDoubleTake(string source_workload_id, string target_workload_id)
        {
            //confirm systems exists
            _target_workload = LocalData.get_as_list<Workload>().FirstOrDefault(x => x.id == target_workload_id);
            _source_workload = LocalData.get_as_list<Workload>().FirstOrDefault(x => x.id == source_workload_id);

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
