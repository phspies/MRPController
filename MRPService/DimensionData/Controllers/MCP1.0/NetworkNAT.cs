using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.CaaS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CaaS1
{
    class NATObject : Core
    {
        public NATObject(DimensionData _dimensiondata) : base(_dimensiondata) { }
        /// <summary>
        /// NAT rule list
        /// </summary>
        /// <param name="network_id"></param>
        /// <returns></returns>
        public NatRules natlist(String network_id)
        {
            orgendpoint(String.Format("/network/{0}/natrule", network_id));
            NatRules acls = get<NatRules>(null, true) as NatRules;
            return acls;
        }
        /// <summary>
        /// Create NAT Rule
        /// </summary>
        /// <param name="network_id"></param>
        /// <param name="name"></param>
        /// <param name="sourceIP"></param>
        /// <returns></returns>
        public NatRule natcreate(String network_id, String name, String sourceIP)
        {
            NatRule create = new NatRule();
            create.name = name;
            create.sourceIp = sourceIP;
            orgendpoint(String.Format("id}/network/{0}/natrule", network_id));
            NatRule nat = post<NatRule>(create, false) as NatRule;
            return nat;
        }
        /// <summary>
        /// Delete NAT Rule
        /// </summary>
        /// <param name="network_id"></param>
        /// <param name="nat_id"></param>
        /// <returns></returns>
        public Status natdelete(String network_id, String nat_id)
        {
            orgendpoint(String.Format("/network/{0}/natrule/{1}?delete", network_id, nat_id));
            Status acls = get<Status>(null, true) as Status;
            return acls;
        }

    }
}
