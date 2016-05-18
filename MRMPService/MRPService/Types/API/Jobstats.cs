using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{

    public class MRPJobstatListType
    {
        public List<MRPJobstatType> jobstats { get; set; }
    }
    public class MRPJobstatsCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPJobstatType jobstat { get; set; }
    }
    public class MRPJobstatType
    {
        public string job_id { get; set; }
        public string replication_status { get; set; }
        public Int64 replication_queue { get; set; }
        public Int64 replication_disk_queue { get; set; }
        public Int64 replication_bytes_sent { get; set; }
        public Int64 replication_bytes_sent_compressed { get; set; }


        public string mirror_status { get; set; }
        public Int64 mirror_skipped { get; set; }
        public Int64 mirror_remaining { get; set; }
        public int mirror_percent_complete { get; set; }
        public DateTime connected_since { get; set; }

        public string stransmit_mode { get; set; }

        public DateTime recovery_point_objective { get; set; }


    }
    public class MRPJobstatIDGETType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public string worker_hostname { get; set; }
        public string jobstat_id { get; set; }
    }
    public class MRPJobstatDTIDGETType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public string worker_hostname { get; set; }
        public string dt_jobstat_id { get; set; }
    }
}
