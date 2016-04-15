using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.API.Types.API
{
    public class MRPNetworkStatsCRUDType
    {
        public string controller_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPNetworkStatCRUDType networkstat { get; set; }
    }
    public class MRPNetworkStatCRUDType
    {
        public string workload_id { get; set; }
        public string proto { get; set; }
        public string source_ip { get; set; }
        public int source_port { get; set; }
        public string target_ip { get; set; }
        public int target_port { get; set; }
        public string state { get; set; }
        public int pid { get; set; }
        public string process { get; set; }

    }

}
