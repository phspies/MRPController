

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

    public class RP4VMSystem : Core
    {

        public RP4VMSystem(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public restString getServerCharsetName_Method()
        {
            endpoint = "/system/server_charset_name";
            mediatype = "application/json";
            return get<restString>();
        }


        public void addSSHKey_Method(SSHKey sshKey_object)
        {
            endpoint = "/system/ssh_keys";
            mediatype = "*/*";
            post(sshKey_object);
        }


        public TransactionID getBalanceLoadRecommendationWithGroupsToExclude_Method(consistencyGroupUIDSet consistencyGroupUIDSet_object)
        {
            endpoint = "/system/balance_load_recommendation";
            mediatype = "application/json";
            return post<TransactionID>(consistencyGroupUIDSet_object);
        }


        public SystemStatistics getSystemStatistics_Method()
        {
            endpoint = "/system/statistics";
            mediatype = "application/json";
            return get<SystemStatistics>();
        }


        public void setSystemAlertsSettings_Method(setSystemAlertsSettingsParams setSystemAlertsSettingsParams_object)
        {
            endpoint = "/system/system_alerts/settings";
            mediatype = "*/*";
            put(setSystemAlertsSettingsParams_object);
        }


        public SystemReportSettings getSystemReportSettings_Method()
        {
            endpoint = "/system/system_report";
            mediatype = "application/json";
            return get<SystemReportSettings>();
        }


        public void setSystemReportSettings_Method(setSystemReportSettingsParams setSystemReportSettingsParams_object)
        {
            endpoint = "/system/system_report/settings";
            mediatype = "*/*";
            put(setSystemReportSettingsParams_object);
        }


        public TransactionID detectBottlenecks_Method(DetectBottlenecksFilter detectBottlenecksFilter_object)
        {
            endpoint = "/system/detect_bottlenecks";
            mediatype = "application/json";
            return post<TransactionID>(detectBottlenecksFilter_object);
        }


        public ClusterUID getLocalCluster_Method()
        {
            endpoint = "/system/local_cluster";
            mediatype = "application/json";
            return get<ClusterUID>();
        }


        public RpaUID getCurrentRPA_Method()
        {
            endpoint = "/system/current_rpa";
            mediatype = "application/json";
            return get<RpaUID>();
        }


        public RecoverPointVersion getRecoverPointVersion_Method()
        {
            endpoint = "/system/version";
            mediatype = "application/json";
            return get<RecoverPointVersion>();
        }


        public FullRecoverPointContext getFullRecoverPointContext_Method()
        {
            endpoint = "/system/context";
            mediatype = "application/json";
            return get<FullRecoverPointContext>();
        }


        public ConnectionInfo getConnectionInfo_Method()
        {
            endpoint = "/system/connection_info";
            mediatype = "application/json";
            return get<ConnectionInfo>();
        }


        public void disableFirstTimeWizard_Method()
        {
            endpoint = "/system/disable_first_time_wizard";
            mediatype = "*/*";
            put();
        }


        public void runCallHomeEvent_Method()
        {
            endpoint = "/system/call_home/run";
            mediatype = "*/*";
            put();
        }


        public CallHomeEvents generateCallHomeEvents_Method(restString restString_object)
        {
            endpoint = "/system/call_home_events";
            mediatype = "application/json";
            return post<CallHomeEvents>(restString_object);
        }


        public void testSyRConnectivity_Method(restString restString_object)
        {
            endpoint = "/system/test_syr_connectivity";
            mediatype = "*/*";
            put(restString_object);
        }


        public void setSystemSecurityLevel_Method(restSystemSecurityLevel restSystemSecurityLevel_object)
        {
            endpoint = "/system/security_level";
            mediatype = "*/*";
            put(restSystemSecurityLevel_object);
        }


        public void sendLoginEvent_Method(restBoolean restBoolean_object)
        {
            endpoint = "/system/login_events";
            mediatype = "*/*";
            post(restBoolean_object);
        }


        public MonitoredParametersStatus getMonitoredParametersStatus_Method()
        {
            endpoint = "/system/monitored_parameters/status";
            mediatype = "application/json";
            return get<MonitoredParametersStatus>();
        }


        public SystemVersionState getSystemVersionState_Method()
        {
            endpoint = "/system/version/state";
            mediatype = "application/json";
            return get<SystemVersionState>();
        }


        public void setSyslogSettings_Method(SyslogSettings syslogSettings_object)
        {
            endpoint = "/system/syslog/settings";
            mediatype = "*/*";
            put(syslogSettings_object);
        }


        public SystemSettings getSystemSettings_Method()
        {
            endpoint = "/system/settings";
            mediatype = "application/json";
            return get<SystemSettings>();
        }


        public void setCallHomeEvents_Method(CallHomeEvents callHomeEvents_object)
        {
            endpoint = "/system/call_home";
            mediatype = "*/*";
            post(callHomeEvents_object);
        }


        public SystemStatusContext getSystemStatusContext_Method()
        {
            endpoint = "/system/status/context";
            mediatype = "application/json";
            return get<SystemStatusContext>();
        }


        public SystemStatus getSystemStatus_Method()
        {
            endpoint = "/system/status";
            mediatype = "application/json";
            return get<SystemStatus>();
        }


        public SystemInternalConfigParams getSystemInternalConfigParams_Method()
        {
            endpoint = "/system/internal_config_params";
            mediatype = "application/json";
            return get<SystemInternalConfigParams>();
        }


        public restString getSystemInternalConfigParamsConflicts_Method()
        {
            endpoint = "/system/internal_config_params_conflicts";
            mediatype = "application/json";
            return get<restString>();
        }


        public void applyBalanceLoadRecommendation_Method(BalanceLoadRecommendation balanceLoadRecommendation_object)
        {
            endpoint = "/system/balance_load_recommendation/apply";
            mediatype = "*/*";
            put(balanceLoadRecommendation_object);
        }


        public clusterVirtualInfrastructuresStateSet getVirtualInfrastructuresStateFromAllCluster_Method()
        {
            endpoint = "/system/virtual_infrastructures/state";
            mediatype = "application/json";
            return get<clusterVirtualInfrastructuresStateSet>();
        }


        public restInteger getRecommendedRpaNumber_Method()
        {
            endpoint = "/system/recommended_rpa";
            mediatype = "application/json";
            return get<restInteger>();
        }


        public void disableSystemAlertsSettings_Method()
        {
            endpoint = "/system/system_alerts/disable";
            mediatype = "*/*";
            put();
        }


        public void enableSystemAlertsSettings_Method()
        {
            endpoint = "/system/system_alerts/enable";
            mediatype = "*/*";
            put();
        }


        public void disableSystemReportSettings_Method()
        {
            endpoint = "/system/system_report/disable";
            mediatype = "*/*";
            put();
        }


        public void enableSystemReportSettings_Method()
        {
            endpoint = "/system/system_report/enable";
            mediatype = "*/*";
            put();
        }


        public void setSystemMiscSettings_Method(SystemMiscellaneousSettings systemMiscellaneousSettings_object)
        {
            endpoint = "/system/misc_settings";
            mediatype = "*/*";
            put(systemMiscellaneousSettings_object);
        }


        public void disableSyslogSettings_Method()
        {
            endpoint = "/system/syslog/disable";
            mediatype = "*/*";
            put();
        }


        public void enableSyslogSettings_Method()
        {
            endpoint = "/system/syslog/enable";
            mediatype = "*/*";
            put();
        }


        public void addEventsLogsFilter_Method(SystemEventLogsFilter systemEventLogsFilter_object)
        {
            endpoint = "/system/event_logs_filters";
            mediatype = "*/*";
            post(systemEventLogsFilter_object);
        }


        public SystemEventLogsFilter getEventsLogsFilter_Method(long filterId)
        {
            endpoint = "/system/event_logs_filters/{filterId}";
            endpoint.Replace("{filterId}", filterId.ToString());
            mediatype = "application/json";
            return get<SystemEventLogsFilter>();
        }


        public void renameEventLogsFilter_Method(long filterId, string newName = null)
        {
            endpoint = "/system/event_logs_filters/{filterId}/rename";
            endpoint.Replace("{filterId}", filterId.ToString());
            if (newName != null) { url_params.Add(new KeyValuePair<string, string>("newName", newName.ToString())); }

            mediatype = "*/*";
            put();
        }


        public void setUserProperties_Method(UserDefinedProperties userDefinedProperties_object)
        {
            endpoint = "/system/user_properties";
            mediatype = "*/*";
            put(userDefinedProperties_object);
        }


        public void suppressCallHomeEvents_Method(restLong restLong_object)
        {
            endpoint = "/system/call_home/suppress";
            mediatype = "*/*";
            put(restLong_object);
        }


        public restLong calculateVolumeSize_Method(calculateVolumeSizeRestParams calculateVolumeSizeRestParams_object)
        {
            endpoint = "/system/volume_size";
            mediatype = "application/json";
            return post<restLong>(calculateVolumeSizeRestParams_object);
        }


        public void setSuppressAutoRegistrationWarning_Method(restBoolean restBoolean_object)
        {
            endpoint = "/system/suppress_auto_registration_warning";
            mediatype = "*/*";
            put(restBoolean_object);
        }


        public void registerContactEmail_Method(restString restString_object)
        {
            endpoint = "/system/contact_email";
            mediatype = "*/*";
            put(restString_object);
        }


        public void setVMsNetworkPolicies_Method(vmNetworkPoliciesSet vmNetworkPoliciesSet_object)
        {
            endpoint = "/system/virtual_infrastructures/virtual_machines/vms_network_policies";
            mediatype = "*/*";
            put(vmNetworkPoliciesSet_object);
        }


        public restBoolean getIsTrialVersion_Method()
        {
            endpoint = "/system/trial_version";
            mediatype = "application/json";
            return get<restBoolean>();
        }
    }
}
