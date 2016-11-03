using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.NetstatCollection
{
    public class ProcessInfo
    {
        public int pid { get; set; }
        public string command { get; set; }
        public string name { get; set; }
    }
}
