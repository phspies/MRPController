using DoubleTakeProxyService.DimensionData;
using DoubleTakeProxyService.DimensionData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleTakeProxyService.DimensionData.API
{
    class ACLObject : Core
    {
        public ACLObject(DimensionData _dimensiondata) : base(_dimensiondata) { }

        public AclRuleList acllist(String network_id)
        {
            orgendpoint(String.Format("/network/{0}/aclrule", network_id));
            AclRuleList acls = get<AclRuleList>(null, true) as AclRuleList;
            return acls;
        }
        //
        public AclRule aclcreate(String network_id, String name, Byte position, String action, String protocol, AclRuleSourceIpRange sourceIpRange, AclRuleDestinationIpRange destinationIpRange, AclRulePortRange portRange, String type)
        {
            AclRule create = new AclRule();
            create.name = name;
            create.position = position;
            create.action = action;
            create.protocol = protocol;
            create.sourceIpRange = sourceIpRange;
            create.destinationIpRange = destinationIpRange;
            create.portRange = portRange;
            create.type = type;
            orgendpoint(String.Format("network/{0}/aclrule", network_id));
            AclRule acl = post<AclRule>(create, false) as AclRule;
            return acl;
        }

        public Status acldelete(String network_id, String acl_id)
        {
            orgendpoint(String.Format("/network/{0}/aclrule/{1}?delete", network_id, acl_id));
            Status acls = get<Status>(null, true) as Status;
            return acls;
        }

    }
}
