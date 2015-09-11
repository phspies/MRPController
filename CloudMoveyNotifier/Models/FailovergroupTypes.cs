using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyNotifier.Models
{
    class Failovergroups
    {
        public Failovergroups()
        {
            FailovergroupList = new List<failovergroupdetail>();
            FailovergroupList.Add(new failovergroupdetail() { ID = 0, Failovergroup_type = "Location" });
            FailovergroupList.Add(new failovergroupdetail() { ID = 1, Failovergroup_type = "Function" });
            FailovergroupList.Add(new failovergroupdetail() { ID = 2, Failovergroup_type = "Service" });
            FailovergroupList.Add(new failovergroupdetail() { ID = 3, Failovergroup_type = "Business Unit" });
        }
        public List<failovergroupdetail> FailovergroupList { get; set; }
    }

    class failovergroupdetail
    {
        public int ID { get; set; }
        public string Failovergroup_type { get; set; }
    }
}
