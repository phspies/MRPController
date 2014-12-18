using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cloudManage.Models
{
    class DetailReportModel
    {
        public String name { get; set; }
        public String type { get; set; }
        public String location { get; set; }
        public String privateip { get; set; }
        public String status { get; set; }
        public String starttime { get; set; }
        public String endtime { get; set; }
        public String duration { get; set; }
        public String cpu { get; set; }
        public String ram { get; set; }
        public String storage { get; set; }
        public String cpuHours { get; set; }
        public String ramHours { get; set; }
        public String storageHours { get; set; }
        public String networkIn { get; set; }
        public String networkOut { get; set; }
        public String subadminHours { get; set; }
        public String networkHours { get; set; }
        public String publicIpHours { get; set; }
    }
}
