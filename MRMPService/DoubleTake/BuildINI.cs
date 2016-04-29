using MRMPService.MRMPService.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.DoubleTake
{
    class BuildINI
    {
        public static List<String> BuildINIFile(MRPTaskDeploymentpolicyType _deployment_policy, dt_server_type _source_workload)
        {
            List<String> _setup_file = new List<string>();
            _setup_file.Add("[Config]");
            _setup_file.Add("DTSETUPTYPE=DTSO");
            switch (_source_workload)
            {
                case dt_server_type.source:
                    _setup_file.Add("DTACTIVATIONCODE=" + (String.IsNullOrEmpty(_deployment_policy.source_activation_code) ? "123456789012345678901234" : _deployment_policy.source_activation_code));
                    break;
                case dt_server_type.target:
                    _setup_file.Add("DTACTIVATIONCODE=" + (String.IsNullOrEmpty(_deployment_policy.target_activation_code) ? "123456789012345678901234" : _deployment_policy.target_activation_code));
                    break;
            }
            _setup_file.Add("DOUBLETAKEFOLDER=" + '"' + _deployment_policy.dt_installpath + '"');
            _setup_file.Add("QMEMORYBUFFERMAX=" + _deployment_policy.dt_max_memory);
            _setup_file.Add("DISKQUEUEFOLDER=" + '"' + _deployment_policy.dt_queue_folder + '"');
            switch (_deployment_policy.dt_queue_scheme)
            {
                case "no_queue":
                    _setup_file.Add("DISKQUEUEMAXSIZE=0");
                    break;
                case "unlimited_queue":
                    _setup_file.Add("DISKQUEUEMAXSIZE=UNLIMITED");
                    break;
                case "limit_queue":
                    _setup_file.Add("DISKQUEUEMAXSIZE=" + _deployment_policy.dt_queue_limit_disk_size.ToString());
                    break;
            }
            _setup_file.Add("DISKFREESPACEMIN=" + _deployment_policy.dt_queue_min_disk_free_size);
            _setup_file.Add("DTSERVICESTARTUP=1");
            _setup_file.Add("PORT=6320");
            _setup_file.Add("WINFW_CONFIG_OPTION=NOT_INUSE_ONLY");
            _setup_file.Add("LICENSE_ACTIVATION_OPTION=1");

            return _setup_file;

        }
    }
}
