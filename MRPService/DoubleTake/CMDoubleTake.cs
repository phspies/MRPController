using MRPService.LocalDatabase;
using System.Linq;

namespace MRPService.MRPDoubleTake
{
    public class MRPDoubleTake
    {
        public Workload _source_workload, _target_workload;

        public MRPDoubleTake(string source_workload_id, string target_workload_id)
        {
            LocalDB db = new LocalDB();

            if (_target_workload != null)
            {
                _target_workload = db.Workloads.FirstOrDefault(x => x.id == target_workload_id);
            }

            //source could be empty in certian instances
            if (_source_workload != null)
            {
                _source_workload = db.Workloads.FirstOrDefault(x => x.id == source_workload_id);
            }
        }

        public MRPDoubleTake_DisasterRecovery disasterrecovery() {
            return new MRPDoubleTake_DisasterRecovery(this);
        }

        public MRPDoubleTake_HighAvailability highavailability()
        {
            return new MRPDoubleTake_HighAvailability(this);
        }
        public MRPDoubleTake_Core common()
        {
            return new MRPDoubleTake_Core(this);
        }

    }
}
