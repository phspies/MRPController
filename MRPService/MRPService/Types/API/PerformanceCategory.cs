using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.API.Types.API
{
    public class MRPPerformanceCategoriesCRUDType
    {
        public string controller_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPPerformanceCategoryCRUDType performancecategory { get; set; }
    }
    public class MRPPerformanceCategoryCRUDType
    {
        public string category_name { get; set; }
        public string counter_name { get; set; }
        public string workload_id { get; set; }
        public bool instances { get; set; }
    }
    public class MRPPerformanceCategoryListType
    {
        public List<MRPPerformanceCategoryType> performancecategories { get; set; }
    }
    public class MRPPerformanceCategoryType
    {
        public string id { get; set; }
        public string category_name { get; set; }
        public string counter_name { get; set; }
        public string workload_id { get; set; }
        public bool instances { get; set; }

    }
}
