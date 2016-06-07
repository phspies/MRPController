
using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VM
{

    public class GroupCopyPolicyTemplates : Core
    {

        public GroupCopyPolicyTemplates(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public ConsistencyGroupCopyPolicyTemplate getConsistencyGroupCopyPolicyTemplates_Method(restString restString_object)
        {
            endpoint = "/group_copy_policy_templates/by_name";
            mediatype = "application/json";
            return post<ConsistencyGroupCopyPolicyTemplate>(restString_object);
        }

        public consistencyGroupCopyPolicyTemplateSet getConsistencyGroupCopyPolicyTemplates_Method()
        {
            endpoint = "/group_copy_policy_templates";
            mediatype = "application/json";
            return get<consistencyGroupCopyPolicyTemplateSet>();
        }
    }
}
