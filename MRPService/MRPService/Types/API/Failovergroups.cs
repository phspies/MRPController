using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.Portal.Types.API
{
    public class MRPFailovergroupsCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MRPFailovergroupCRUDType failovergroup { get; set; }
    }
    public class MRPFailovergroupCRUDType
    {
        public string id { get; set; }
        public string parent_id { get; set; }
        public int? position { get; set; }
        public string group { get; set; }
        public string grouptype { get; set; }
        public bool enabled { get; set; }
    }
    public class MRPFailovergroupListType
    {
        public List<MRPFailovergroupType> failovergroups { get; set; }
    }

    public class MRPFailovergroupType
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
