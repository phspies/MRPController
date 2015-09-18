using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Types.API
{
    public class MoveyFailovergroupsCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MoveyFailovergroupCRUDType failovergroup { get; set; }
    }
    public class MoveyFailovergroupCRUDType
    {
        public string id { get; set; }
        public string parent_id { get; set; }
        public int? position { get; set; }
        public string group { get; set; }
        public string grouptype { get; set; }
        public bool enabled { get; set; }
    }
    public class MoveyFailovergroupListType
    {
        public List<MoveyFailovergroupType> failovergroups { get; set; }
    }

    public class MoveyFailovergroupType
    {
        public string id { get; set; }
        public string parent_id { get; set; }
        public int position { get; set; }
        public string group { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string organization_id { get; set; }
        public int phase { get; set; }
        public string grouptype { get; set; }
        public bool enabled { get; set; }
        public string recoverypolicy_id { get; set; }
        public object failoversnapshot_id { get; set; }
        public object source_repository_id { get; set; }
        public string target_platform_id { get; set; }
        public object target_repository_id { get; set; }
        public string target_server_suffix { get; set; }
        public bool dt_delete_current_job { get; set; }
    }

}
