using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cloudManage.Models
{
    class NetworkDataModel
    {
        public String id { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public String location { get; set; }
        public String privateNet { get; set; }
        public String multicast { get; set; }
        public String resourcePath { get; set; }
    }
}
