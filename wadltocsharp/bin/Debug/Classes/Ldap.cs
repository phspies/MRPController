using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Ldap : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public void setLDAPSettings_Method(ldapSettings ldapSettings_object)
{
	endpoint = "/ldap/settings";
	mediatype="*/*";
	put(ldapSettings_object);
}


public LdapServerInfo getLDAPServerInfo_Method()
{
	endpoint = "/ldap/server_info";
	mediatype="application/json";
	return get<LdapServerInfo>();
}


public void clearLDAPSettings_Method()
{
	endpoint = "/ldap/clear_settings";
	mediatype="*/*";
	put();
}


public void testLDAPSettings_Method(ldapSettings ldapSettings_object)
{
	endpoint = "/ldap/settings/test";
	mediatype="*/*";
	post(ldapSettings_object);
}


public void disableLDAPSettings_Method()
{
	endpoint = "/ldap/disable";
	mediatype="*/*";
	put();
}


public void enableLDAPSettings_Method()
{
	endpoint = "/ldap/enable";
	mediatype="*/*";
	put();
}




}
}
