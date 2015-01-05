using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleTakeProxyService.DimensionData.Models
{
    class Account
    {
        public String userName { get; set; }
        public String fullName { get; set; }
        public String lastName { get; set; }
        public String emailAddress { get; set; }
        public String orgId { get; set; }
        public List<Role> roles { get; set; }
    }
    class Role {
        public String name { get; set; }
    }
}
