using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Settings : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public ManagementSettings getManagementSettings_Method()
{
	endpoint = "/settings/management_settings";
	mediatype="application/json";
	return get<ManagementSettings>();
}


public FullRecoverPointSettingsContext getFullRecoverPointSettingsContext_Method()
{
	endpoint = "/settings/context";
	mediatype="application/json";
	return get<FullRecoverPointSettingsContext>();
}


public ConsistencyGroupCopyPolicy getDefaultGroupCopyPolicy_Method()
{
	endpoint = "/settings/defaults/group_copy_policy";
	mediatype="application/json";
	return get<ConsistencyGroupCopyPolicy>();
}


public ConsistencyGroupLinkPolicy getDefaultRemoteGroupLinkPolicy_Method()
{
	endpoint = "/settings/defaults/group_link_policy/remote";
	mediatype="application/json";
	return get<ConsistencyGroupLinkPolicy>();
}


public UserEventLogsFilter getDefaultUserEventLogsFilters_Method()
{
	endpoint = "/settings/defaults/user_event_logs_filters";
	mediatype="application/json";
	return get<UserEventLogsFilter>();
}


public ConsistencyGroupPolicy getDefaultGroupPolicy_Method()
{
	endpoint = "/settings/defaults/group_policy";
	mediatype="application/json";
	return get<ConsistencyGroupPolicy>();
}


public ConsistencyGroupLinkPolicy getDefaultLocalGroupLinkPolicy_Method()
{
	endpoint = "/settings/defaults/group_link_policy/local";
	mediatype="application/json";
	return get<ConsistencyGroupLinkPolicy>();
}


public RestStringSet getDefaultUsersNames_Method()
{
	endpoint = "/settings/defaults/users_names";
	mediatype="application/json";
	return get<RestStringSet>();
}


public RestStringSet getDefaultRolesNames_Method()
{
	endpoint = "/settings/defaults/roles_names";
	mediatype="application/json";
	return get<RestStringSet>();
}


public SystemGlobalPolicy getDefaultSystemGlobalPolicy_Method()
{
	endpoint = "/settings/defaults/system_global_policy";
	mediatype="application/json";
	return get<SystemGlobalPolicy>();
}



public void clearSettings_Method()
{
	endpoint = "/settings";
	mediatype="*/*";
	delete();
}



}
}
