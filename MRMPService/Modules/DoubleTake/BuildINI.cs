
using MRMPService.Exceptions;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.Utilities;
using System;
using System.Collections.Generic;

namespace MRMPService.MRMPDoubleTake
{
    class BuildINI
    {
        public static List<String> BuildINIFile(MRPDeploymentpolicyType _deployment_policy, dt_server_type _source_workload)
        {
			ExceptionFactory.CheckArgumentIsNullOrEmpty(_deployment_policy.dt_queue_scheme);
			ExceptionFactory.CheckArgumentIsNullOrEmpty(_source_workload.ToString());

			List<String> _setup_file = new List<string>();
            _setup_file.Add("[Config]");
            _setup_file.Add("DTSETUPTYPE=DTSO");
			string dtActivationCode = "123456789012345678901234";
			switch (_source_workload)
            {
                case dt_server_type.source:
					if (!string.IsNullOrWhiteSpace(_deployment_policy.source_activation_code))
						dtActivationCode = _deployment_policy.source_activation_code;
					break;
                case dt_server_type.target:
					if (!string.IsNullOrWhiteSpace(_deployment_policy.target_activation_code))
						dtActivationCode = _deployment_policy.target_activation_code;
                    break;
				default:
					throw ExceptionFactory.MRPDeploymentServerTypeNotSupported(_source_workload.ToString());
            }
			_setup_file.Add($"DTACTIVATIONCODE={dtActivationCode.EscapeData()}");
			_setup_file.Add("DOUBLETAKEFOLDER=" + '"' + _deployment_policy.dt_installpath + '"');
            _setup_file.Add("QMEMORYBUFFERMAX=" + _deployment_policy.dt_max_memory);
            _setup_file.Add("DISKQUEUEFOLDER=" + '"' + _deployment_policy.dt_windows_queue_folder + '"');
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
				default:
					throw ExceptionFactory.MRPDeploymentPolicyTypeQueueSchemeNotSupported(_deployment_policy.dt_queue_scheme);
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
