using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Group_link_policy_templates : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public ConsistencyGroupLinkPolicyTemplate getConsistencyGroupLinkPolicyTemplates_Method(restString restString_object)
{
	endpoint = "/group_link_policy_templates/by_name";
	mediatype="application/json";
	return post<ConsistencyGroupLinkPolicyTemplate>(restString_object);
}


public ConsistencyGroupLinkPolicyTemplateSet getConsistencyGroupLinkPolicyTemplates_Method()
{
	endpoint = "/group_link_policy_templates";
	mediatype="application/json";
	return get<ConsistencyGroupLinkPolicyTemplateSet>();
}



}
}
