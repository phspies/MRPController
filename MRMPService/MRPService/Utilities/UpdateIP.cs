using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Management;

namespace MRMPService.Utilities
{
    public class UpdateIP
    {
        static public void setInterfaceAddresses(MRPWorkloadType _source_workload, MRPWorkloadType _target_workload)
        {
            string workload_ip;
            MRPCredentialType _credential = _source_workload.credential;
            String domainuser = null;

            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Error finding credentials"));
            }

            if (!String.IsNullOrWhiteSpace(_credential.domain))
            {
                domainuser = (_credential.domain + @"\" + _credential.username);
            }
            else
            {
                domainuser = @".\" + _credential.username;
            }
            using (Connection _connection = new Connection())
            {
                workload_ip = _connection.FindConnection(_source_workload.iplist, true);
            }
            if (workload_ip == null)
            {
                throw new ArgumentException(String.Format("Error contacting workload"));
            }

            ConnectionOptions connOptions = new ConnectionOptions();
            connOptions.Impersonation = ImpersonationLevel.Impersonate;
            connOptions.Authentication = AuthenticationLevel.Default;
            connOptions.EnablePrivileges = true;
            connOptions.Username = (_source_workload.credential.domain == null ? "." : _source_workload.credential.domain) + @"\" + _source_workload.credential.username;
            connOptions.Password = _source_workload.credential.encrypted_password;
            ManagementScope scope = new ManagementScope(@"\\" + workload_ip + @"\root\CIMV2", connOptions);
            ManagementPath p = new ManagementPath("Win32_NetworkAdapterConfiguration");
            ManagementClass objMC = new ManagementClass(scope, p, null);
            ManagementObjectCollection objMOC = objMC.GetInstances();

            bool _found_interface = false;
            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    if (objMO["Caption"].ToString() == _source_workload.workloadinterfaces[0].caption)
                    {
                        _found_interface = true;
                        try
                        {
                            ManagementBaseObject objNewIP = objMO.GetMethodParameters("EnableStatic");
                            ManagementBaseObject objSetIP = null;
                            ManagementBaseObject objNewGate = objMO.GetMethodParameters("SetGateways");
                            ManagementBaseObject objNewDNS = objMO.GetMethodParameters("SetDNSServerSearchOrder");

                            //Set Values
                            objNewDNS["DNSServerSearchOrder"] = String.Join(",", _target_workload.primary_dns, _target_workload.secondary_dns);
                            objNewGate["DefaultIPGateway"] = new string[] {  _target_workload.workloadinterfaces[0].platformnetwork.ipv4gateway, _target_workload.workloadinterfaces[0].platformnetwork.ipv6gateway };
                            objNewGate["GatewayCostMetric"] = new int[] { 1 };
                            objNewIP["IPAddress"] = new string[] {  _target_workload.workloadinterfaces[0].ipaddress, _target_workload.workloadinterfaces[0].ipv6address };
                            objNewIP["SubnetMask"] = new string[] { _target_workload.workloadinterfaces[0].netmask, _target_workload.workloadinterfaces[0].ipv6netmask };

                            objSetIP = objMO.InvokeMethod("EnableStatic", objNewIP, null);
                            objSetIP = objMO.InvokeMethod("SetGateways", objNewGate, null);
                            objSetIP = objMO.InvokeMethod("SetDNSServerSearchOrder", objNewDNS, null);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(String.Format("Error updating workload network information: {0}", ex.GetBaseException().Message));
                        }
                    }
                }
            }
            if (!_found_interface)
            {
                throw new Exception(String.Format("Error finding {0} on {1}", _source_workload.workloadinterfaces[0].ipaddress, _source_workload.hostname));
            }
        }
    }
}
