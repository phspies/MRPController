using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Portal.Types.API
{
    public class MoveyPerformanceCategoriesCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MoveyPerformanceCategoryCRUDType performancecategory { get; set; }
    }
    public class MoveyPerformanceCategoryCRUDType
    {
        public string category_name { get; set; }
        public string counter_name { get; set; }
        public string workload_id { get; set; }
        public bool instances { get; set; }
    }
    public class MoveyPerformanceCategoryListType
    {
        public List<MoveyPerformanceCategoryType> performancecategories { get; set; }
    }
    public class MoveyPerformanceCategoryType
    {
        public string id { get; set; }
        public string category_name { get; set; }
        public string counter_name { get; set; }
        public string workload_id { get; set; }
        public bool instances { get; set; }

    }
}
