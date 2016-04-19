using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.API.Types.API
{
    public class MRPWorkloadProcessType
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public string caption { get; set; }
        public string commandline { get; set; }
        public int processid { get; set; }
        public Int64 virtualsize { get; set; }
        public string name { get; set; }
        public int threadcount { get; set; }
        public Int64 writeoperationcount { get; set; }
        public Int64 writetransfercount { get; set; }
        public Int64 readoperationcount { get; set; }
        public Int64 readtransfercount { get; set; }
    }
}
