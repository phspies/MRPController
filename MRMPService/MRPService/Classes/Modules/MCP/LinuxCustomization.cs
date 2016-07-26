using DD.CBU.Compute.Api.Contracts.Network20;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using MRMPService.Utilities;
using Renci.SshNet;


namespace MRMPService.Tasks.MCP
{
    partial class MCP_Platform
    {
        static public void LinuxCustomization(MRPTaskType payload, ServerType _newvm)
        {
            MRPTaskSubmitpayloadType _payload = payload.submitpayload;
            MRPPlatformType _platform = _payload.platform;
            MRPWorkloadType _target_workload = _payload.target;
            MRPCredentialType _credential = _target_workload.credential;
            MRPCredentialType _platform_credentail = _platform.credential;

            string workload_ip = null;
            using (Connection _connection = new Connection())
            {
                workload_ip = _connection.FindConnection(String.Join(",", _newvm.networkInfo.primaryNic.ipv6, _newvm.networkInfo.primaryNic.privateIpv4), true);
            }
            if (workload_ip == null)
            {
                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                {
                    _mrp_api.task().failcomplete(payload, String.Format("Error contacting workwork {0} after 3 tries", _newvm.name));
                    throw new ArgumentException(String.Format("Error contacting workwork {0} after 3 tries", _newvm.name));
                }
            }

            ConnectionInfo ConnNfo = new ConnectionInfo(workload_ip, 22, _credential.username, new AuthenticationMethod[] { new PasswordAuthenticationMethod(_credential.username, _credential.encrypted_password) });
            using (var sshclient = new SshClient(ConnNfo))
            {
                sshclient.Connect();

                //get 
                using (var cmd = sshclient.CreateCommand(Path.Combine("~/", "lsblk -l")))
                {
                    cmd.Execute();
                    if (cmd.ExitStatus != 0)
                    {
                        sshclient.Disconnect();
                        throw new ArgumentException(String.Format("Error while running unix setup script: %1", cmd.Result));
                    }

                }
                sshclient.Disconnect();
            }

            foreach (MRPWorkloadVolumeType _volume in _target_workload.workloadvolumes_attributes)
            {
                
            }

            using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                _mrp_api.task().progress(payload, String.Format("Volume setup process exit code: {0}", _exitcode), 81);
            }
        }
    }
}
