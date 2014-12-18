using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cloudManage.Models
{
    class TemplateDataModel
    {
        public String id { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public String operatingSystem { get; set; }
        public int cpuCount { get; set; }
        public int memory { get; set; }
        public String resourcePath { get; set; }
        public String location { get; set; }
        public int osStorage { get; set; }
        public int additionalLocalStorage { get; set; }
        public DateTime created { get; set; }

    }
}
