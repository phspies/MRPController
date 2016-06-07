using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Snmp : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public void setSNMPSettings_Method(snmpSettings snmpSettings_object)
{
	endpoint = "/snmp/settings";
	mediatype="*/*";
	put(snmpSettings_object);
}


public void disableSNMPSettings_Method()
{
	endpoint = "/snmp/disable";
	mediatype="*/*";
	put();
}


public void enableSNMPSettings_Method()
{
	endpoint = "/snmp/enable";
	mediatype="*/*";
	put();
}




}
}
