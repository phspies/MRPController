using MRPService.LocalDatabase;
using System.Linq;

namespace MRPService.DoubleTake
{
    public class MRP_DoubleTake
    {
        public Workload _source_workload, _target_workload;
       
        public MRP_DoubleTake(string source_workload_id, string target_workload_id)
        {
            MRPDatabase db = new MRPDatabase();

            if (target_workload_id == null)
            {
                throw new System.ArgumentException("Target workload ID cannot be null");
            }
            _target_workload = db.Workloads.FirstOrDefault(x => x.id == target_workload_id);


            //source could be empty in certian instances
            if (source_workload_id != null)
            {
                _source_workload = db.Workloads.FirstOrDefault(x => x.id == source_workload_id);
            }
        }

        public Core Common()
        {
            return new Core(this);
        }
        public ManagementService ManagementService()
        {
            return new ManagementService(this);
        }

    }
}
