using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.Portal.Types.API
{
    public class MRPTaskWindowsType
    {
        public string username { get; set; }
        public string password { get; set; }
        public string hostname { get; set; }
        public string ipaddress { get; set; }
        public string domain { get; set; }
    }
}
