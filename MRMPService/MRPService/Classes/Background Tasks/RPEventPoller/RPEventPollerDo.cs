using MRMPService.MRMPService.Log;
using System;
using MRMPService.MRMPAPI.Contracts;
using MRMPService.RP4VMTypes;
using MRMPService.RP4VMAPI;
using System.Threading.Tasks;
using MRMPService.MRMPAPI;

namespace MRMPService.RPEventPollerCollection
{
    class RPEventPollerDo
    {
        public static async Task PollerDo(MRPManagementobjectType _mrp_managementobject)
        {
            //refresh managementobject from portal

            _mrp_managementobject = await MRMPServiceBase._mrmp_api.managementobject().getmanagementobject_id(_mrp_managementobject.id);

            //check for credentials
            MRPPlatformType _target_platform = _mrp_managementobject.target_platform;
            MRPCredentialType _platform_credential = _target_platform.credential;
            if (_platform_credential == null)
            {
                throw new ArgumentException(String.Format("RP4VM Group: Error finding credentials for appliance {0}", _target_platform.rp4vm_url));
            }

            //check for working IP

            Logger.log(String.Format("RP4VM Group: Start RP4vM Event collection for {0}", _target_platform.rp4vm_url), Logger.Severity.Info);

            //first try to connect to the target server to make sure we can connect
            eventsPage _group_events = new eventsPage();

            //first try to connect to the target server to make sure we can connect
            try
            {
                String username = String.Concat((String.IsNullOrEmpty(_platform_credential.domain) ? "" : (_platform_credential.domain + @"\")), _platform_credential.username);

                using (RP4VM_ApiClient _rp4vm = new RP4VM_ApiClient(_target_platform.rp4vm_url, username, _platform_credential.encrypted_password))
                {
                    ManagementSettings _info = _rp4vm.settings().getManagementSettings_Method();

                    //now we try to get the snapshot information we have. IF we can't, then we know the group has been deleted...
                    //_group_stats = _rp4vm.events().get(Int64.Parse(_mrp_managementobject.moid));
                    _group_events = _rp4vm.events().getEventLogsByFilter_Method(
                        new UserEventLogsFilter()
                        {
                            topics = new System.Collections.Generic.List<eventLogTopic>() {
                                eventLogTopic.CONSISTENCY_GROUP
                            }

                        });

                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("RP4VM Group: Error collecting information from {0} : {1}", _target_platform.rp4vm_url, ex.ToString()), Logger.Severity.Info);
            }
        }
    }
}

