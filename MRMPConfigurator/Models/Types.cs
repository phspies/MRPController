using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPConfigurator.Models
{
    class Types
    {
        public Types()
        {
            TypeList = new List<typedetail>();
            TypeList.Add(new typedetail() { ID = "0", Type = "Source" });
            TypeList.Add(new typedetail() { ID = "1", Type = "Target" });

        }
        public List<typedetail> TypeList { get; set; }
    }

    class typedetail
    {
        public string ID { get; set; }
        public string Type { get; set; }
    }
}
