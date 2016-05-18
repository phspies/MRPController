using System;
using System.Collections.Generic;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRMPEventTypes
    {
        public static int DT = 0;
        public static int Manager = 1;
        public static int API = 2;
        public static int Workload = 3;
        public static int Job = 4;
    }
    public class MRMPEventSeverityTypes
    {
        public static int Error = 3;
        public static int Information = 1;
        public static int Warning = 2;
        public static int Success = 0;
    }
    public class MRMPEventType
    {
        public string object_id { get; set; }
        public DateTime timestamp { get; set; }
        public string message { get; set; }
        public int event_id { get; set; }
        public string severity { get; set; }
        public string response { get; set; }
        public int source_subsystem { get; set; }
    }
    public class MRPEventsCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRMPEventType @event { get; set; }
    }
    public class MRPInternalEventType
    {
        public int id { set; get; }
        public string name { set; get; }
        public string severity { get; set; }
        public string response { get; set; }
    }
    public class MRPInternalEvents
    {
        public List<MRPInternalEventType> internal_events
        {
            get
            {
                List<MRPInternalEventType> _events = new List<MRPInternalEventType>();
                _events.Add(new MRPInternalEventType() { id = 1, name = "dt_evalation_expired", severity = "Error", response = " Contact your vendor to purchase either a single or site license." });
                _events.Add(new MRPInternalEventType() { id = 2, name = "dt_evalation_expire_soon", severity = "Information", response = " Contact your vendor before the evaluation period expires to purchase either a single or site license." });
                _events.Add(new MRPInternalEventType() { id = 3, name = "dt_evalation_activated", severity = "Information", response = " Contact your vendor before the evaluation period expires to purchase either a single or site license." });
                _events.Add(new MRPInternalEventType() { id = 4, name = "dt_duplicate_license", severity = "Warning", response = "If you have an evaluation license or a site license, no action is necessary. If you have a single license, you must purchase either another single license or a site license." });
                _events.Add(new MRPInternalEventType() { id = 5, name = "dt_os_limitation_license", severity = "Error", response = " Verify your license key has been entered correctly." });
                _events.Add(new MRPInternalEventType() { id = 6, name = "dt_evaluation_expire_today", severity = "Warning", response = " Contact your vendor to purchase either a single or site license." });
                _events.Add(new MRPInternalEventType() { id = 7, name = "dt_license_key_invalid", severity = "Error", response = "If you are in the process of installing Double-Take, verify that you are using a 24 character alpha-numeric key. If Double-Take is already installed, confirm that the key entered is correct. If the key appears to be correct, contact technical support." });
                _events.Add(new MRPInternalEventType() { id = 8, name = "dt_license_key_valid", severity = "Information", response = " No action required." });
                _events.Add(new MRPInternalEventType() { id = 100, name = "dt_crital_error", severity = "Error", response = " Contact technical support with the details from this message." });
                _events.Add(new MRPInternalEventType() { id = 101, name = "dt_service_aborted", severity = "Error", response = " Restart the Double-Take service. Contact technical support if this event occurs repeatedly." });
                _events.Add(new MRPInternalEventType() { id = 200, name = "dt_exchange_failover_commit", severity = "Information", response = "See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 201, name = "dt_exchange_failover_test", severity = "Information", response = "See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 202, name = "dt_exchange_failback_commit", severity = "Information", response = "See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 203, name = "dt_exchange_failbacl_test", severity = "Information", response = "See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 204, name = "dt_exchange_setup_started", severity = "Information", response = "See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 205, name = "dt_exchange_logfile_error", severity = "Error", response = "See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 210, name = "dt_exchange_completed_move", severity = "Success", response = "See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 211, name = "dt_exchange_completed_move_warning", severity = "Warning", response = "See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 212, name = "dt_exchange_failover_complete", severity = "Success", response = "See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 213, name = "dt_exchange_failover_complete_warning", severity = "Warning", response = "See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 214, name = "dt_exchange_setup_complete", severity = "Success", response = "See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 220, name = "dt_exchange_start_failed", severity = "Error", response = "Restart failover. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 221, name = "dt_exchange_start_failed_commandline", severity = "Error", response = "See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 222, name = "dt_exchange_start_failed_license", severity = "Error", response = "Verify your license key has been entered correctly and contact technical support." });
                _events.Add(new MRPInternalEventType() { id = 223, name = "dt_exchange_start_failed_ad", severity = "Error", response = "Restart failover. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 224, name = "dt_exchange_failover_ad", severity = "Error", response = "Verify the Exchange server names and the account has sufficient privileges to update Active Directory." });
                _events.Add(new MRPInternalEventType() { id = 1000, name = "dt_exchange_exception", severity = "Error", response = "Run the installation and select Repair. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 1001, name = "dt_dtcounter_dll_not_initialized", severity = "Error", response = "Run the installation and select Repair. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 1002, name = "dt_dtcounter_dll_shared_memory", severity = "Error", response = "Run the installation and select Repair. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 1003, name = "dt_dtcounter_dll_performance_registry", severity = "Error", response = "Run the installation and select Repair. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 1004, name = "dt_dtcounter_dll_performance_first_registry", severity = "Error", response = "Run the installation and select Repair. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 1005, name = "dt_dtcounter_dll_firsthelp", severity = "Error", response = "Run the installation and select Repair. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 1006, name = "dt_dtcounter_dll_create_handler", severity = "Error", response = "Run the installation and select Repair. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 4000, name = "dt_kernel_started", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4001, name = "dt_target_started", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4002, name = "dt_source_started", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4003, name = "dt_source_stopped", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4004, name = "dt_target_stopped", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4005, name = "dt_kernel_stopped", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4007, name = "dt_auto_disconnect_error", severity = "Warning", response = "The connection is auto-disconnecting because the disk-based queue on the source has been filled, the service has encountered an unknown file ID, the target server has restarted, or an error has occurred during disk queuing on the source or target (for example, Double-Take cannot read from or write to the transaction log file)." });
                _events.Add(new MRPInternalEventType() { id = 4008, name = "dt_auto_diconnected_success", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4009, name = "dt_auto_reconnect", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4010, name = "dt_auto_reconnect_success", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4011, name = "dt_auto_reconnect_failed", severity = "Error", response = "Manually reestablish the job to target connection." });
                _events.Add(new MRPInternalEventType() { id = 4014, name = "dt_service_transmission_start", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4015, name = "dt_service_transmission_stopped", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4016, name = "dt_service_established", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4017, name = "dt_service_diconnect", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4018, name = "dt_failover_disabled", severity = "Warning", response = "Perform a restoration." });
                _events.Add(new MRPInternalEventType() { id = 4019, name = "dt_service_mirror_started", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4020, name = "dt_service_mirror_paused", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4021, name = "dt_service_mirror_resumed", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4022, name = "dt_service_mirror_stopped", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4023, name = "dt_service_mirror_completed", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4024, name = "dt_service_replication_started", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4025, name = "dt_service_replication_stopped", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4026, name = "dt_target_paused_user", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4027, name = "dt_target_resumed_user", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4028, name = "dt_registration_dns_class_failed", severity = "Warning", response = "Verify that the Active Directory server is running and that the Double-Take service has permission to update Active Directory." });
                _events.Add(new MRPInternalEventType() { id = 4029, name = "dt_registration_dns_instance_failed", severity = "Warning", response = "Verify that the Active Directory server is running and that the Double-Take service has permission to update Active Directory." });
                _events.Add(new MRPInternalEventType() { id = 4030, name = "dt_rsresource_unknown_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource.dll. Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4031, name = "dt_rsresource_open_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource.dll, Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4032, name = "dt_rsresource_version_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource.dll.Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4033, name = "dt_rsresource_invalid_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource.dll. Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4034, name = "dt_service_name_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource.dll. Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4035, name = "dt_product_name_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource.dll. Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4036, name = "dt_vendor_name_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource. Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4037, name = "dt_vendor_url_name_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource. Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4038, name = "dt_license_key_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource. Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4039, name = "dt_rsresource_read_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource. Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4040, name = "dt_license_hardware_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource. Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4041, name = "dt_license_os_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource. Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4042, name = "dt_license_appkit_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource. Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4043, name = "dt_license_cpulimit_error", severity = "Error", response = "Reinstall the software, using the installation Repair option, to install a new copy of the RSResource. Contact technical support if this error persists. " });
                _events.Add(new MRPInternalEventType() { id = 4044, name = "dt_unkown_replication_error", severity = "Error", response = "Contact technical support if this error persists." });
                _events.Add(new MRPInternalEventType() { id = 4045, name = "dt_value_warning", severity = "Warning", response = "Verify that the Double-Take port value you are trying to use is within the valid range. If it is not, it will automatically be reset to the default value. " });
                _events.Add(new MRPInternalEventType() { id = 4046, name = "dt_network_port_conflict", severity = "Error", response = "Verify that the Double-Take ports are not conflicting with ports used by other applications." });
                _events.Add(new MRPInternalEventType() { id = 4047, name = "dt_zlib_disabled", severity = "Information", response = "The compression levels available depend on your operating system. You can reinstall the software, using the installation Repair option, to install a new copy of the DynaZip. dll, or contact technical support if this error persists." });
                _events.Add(new MRPInternalEventType() { id = 4048, name = "dt_service_delete_ophans_start", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4049, name = "dt_service_delete_ophans_paused", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4050, name = "dt_service_delete_ophans_resume", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4051, name = "dt_service_delete_ophans_stopped", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4052, name = "dt_service_delete_ophans_completed", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4053, name = "dt_service_restore_start", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4054, name = "dt_service_restore_paused", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4055, name = "dt_service_restore_resumed", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4056, name = "dt_service_restore_stopped", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4057, name = "dt_service_restore_paused", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4058, name = "dt_service_verification_started", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4059, name = "dt_service_verification_paused", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4060, name = "dt_service_verification_resumed", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4061, name = "dt_service_verification_stopped", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4062, name = "dt_service_verification_completed", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4063, name = "dt_bandwith_change", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4064, name = "dt_bandwith_change_timebased", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4065, name = "dt_target_connection_changed", severity = "Warning", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4066, name = "dt_license_virtual_limit_error", severity = "Error", response = "The license key you are using is for the Virtual SystemsTM edition. This code will not work on non-virtual server environments." });
                _events.Add(new MRPInternalEventType() { id = 4067, name = "dt_no_replication_error", severity = "Error", response = "Check other messages for errors with the Double-Take drivers, and correct as required. If there are no driver messages, verify that your drives are connected to the source. If this error persists, contact technical support." });
                _events.Add(new MRPInternalEventType() { id = 4068, name = "dt_failed_volume_Error", severity = "Error", response = "Reboot the source server. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 4069, name = "dt_orphansdir_missing", severity = "Warning", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4070, name = "dt_connection_read_error", severity = "Error", response = "Initiate a remirror to guarantee data integrity. Contact technical support if this event occurs repeatedly." });
                _events.Add(new MRPInternalEventType() { id = 4071, name = "dt_connection_checksum_error", severity = "Warning", response = "Initiate a remirror to guarantee data integrity. Contact technical support if this event occurs repeatedly." });
                _events.Add(new MRPInternalEventType() { id = 4072, name = "dt_queuesizealertthreshold_exceeded", severity = "Warning", response = "If the queue reaches capacity, Double-Take will automatically begin the auto-disconnect process. If you see this message repeatedly, you may want to consider a larger queue or upgrading your server hardware to keep up with the amount of data changes in your environment. " });
                _events.Add(new MRPInternalEventType() { id = 4073, name = "dt_replication_set_modified", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4096, name = "dt_registry_unknown_warning", severity = "Warning", response = "Delete the parameter and report this issue to technical support." });
                _events.Add(new MRPInternalEventType() { id = 4097, name = "dt_wmi_initialization_error", severity = "Warning", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4097, name = "dt_filesystem_filter_failed", severity = "Error", response = "Reboot your server and contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 4098, name = "dt_registry_load_failed", severity = "Warning", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4098, name = "dt_control_device_not_created", severity = "Error", response = "Reboot your server and contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 4099, name = "dt_driver_hardlink_support_error", severity = "Warning", response = "Hard links are not supported." });
                _events.Add(new MRPInternalEventType() { id = 4099, name = "dt_driver_filter_register_error", severity = "Error", response = "Reboot your server and contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 4100, name = "dt_driver_missmatch_error", severity = "Error", response = "Reboot your server. Reinstall the software if this event occurs again. Contact technical support if this event occurs after reinstalling the software." });
                _events.Add(new MRPInternalEventType() { id = 4110, name = "dt_target_write_diskfull_error", severity = "Warning", response = "The disk on the target is full. The operation will be retried according to the TGExecutionRetryLimit setting." });
                _events.Add(new MRPInternalEventType() { id = 4111, name = "dt_target_write_sharing_error", severity = "Warning", response = "A sharing violation error is prohibiting Double-Take from writing on the target. The operation will be retried according to the TGExecutionRetryLimit setting." });
                _events.Add(new MRPInternalEventType() { id = 4112, name = "dt_target_write_accessdenied_error", severity = "Warning", response = "An access denied error is prohibiting Double-Take from writing on the target. The operation will be retried according to the TGExecutionRetryLimit setting." });
                _events.Add(new MRPInternalEventType() { id = 4113, name = "dt_target_write_unknown_error", severity = "Warning", response = "An unknown error is prohibiting Double-Take from writing on the target. The operation will be retried according to the TGExecutionRetryLimit setting." });
                _events.Add(new MRPInternalEventType() { id = 4120, name = "dt_target_write_success_retry", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4150, name = "dt_target_write_success_retry", severity = "Error", response = "The operation has been retried according to the TGExecutionRetryLimit setting but was not able to be written to the target and the operation was discarded. Correct the problem and remirror the files." });
                _events.Add(new MRPInternalEventType() { id = 4155, name = "dt_filesystem_timeout_error", severity = "Warning", response = "Correct the file system error and then remirror or perform a verification with remirror to synchronize the changes." });
                _events.Add(new MRPInternalEventType() { id = 4200, name = "dt_inband_submitted", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4201, name = "dt_inband_discarded", severity = "Warning", response = "A task may be discarded in the following scenarios= all connections to a target are manually disconnected, replication is stopped for all connections to a target, or an auto-disconnect occurs. If one of these scenarios did not cause the task to be discarded, contact technical support." });
                _events.Add(new MRPInternalEventType() { id = 4202, name = "dt_inband_script_running", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4203, name = "dt_inband_script_completed", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4204, name = "dt_inband_script_error", severity = "Error", response = "Review the task and its associated script(s) for syntax errors." });
                _events.Add(new MRPInternalEventType() { id = 4205, name = "dt_inband_script_timeout", severity = "Warning", response = "The timeout specified for the script to complete has expired. Normal processing will continue. You may need to manually terminate the script if it will never complete" });
                _events.Add(new MRPInternalEventType() { id = 4206, name = "dt_inband_script_disabled", severity = "Warning", response = "The timeout period was set to zero (0). Double-Take will not wait for the script to complete before continuing, No action is required." });
                _events.Add(new MRPInternalEventType() { id = 4207, name = "dt_inband_script_disabled_server", severity = "Warning", response = "Enable task command processing." });
                _events.Add(new MRPInternalEventType() { id = 4300, name = "dt_connection_out_of_order", severity = "Error", response = "You may need to stop and restart your job." });
                _events.Add(new MRPInternalEventType() { id = 4301, name = "dt_block_target_path_error", severity = "Error", response = "If you need to block your target paths, contact technical support." });
                _events.Add(new MRPInternalEventType() { id = 4302, name = "dt_target_blocked", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4303, name = "dt_blocking_failed", severity = "Warning", response = "If you need to block your target paths, contact technical support." });
                _events.Add(new MRPInternalEventType() { id = 4304, name = "dt_target_unblocked", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4305, name = "dt_unblocking_failed", severity = "Warning", response = "If you need to unblock your target paths, contact technical support." });
                _events.Add(new MRPInternalEventType() { id = 4306, name = "dt_connection_already_blocked", severity = "Warning", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4307, name = "dt_connection_already_unblocked", severity = "Warning", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4308, name = "dt_registry_corrupted", severity = "Error", response = "If you need to block your target paths, contact technical support." });
                _events.Add(new MRPInternalEventType() { id = 4400, name = "dt_snapshot_create_error", severity = "Error", response = "The snapshot could not be created. This may be due to a lack of disk space or memory or another reason. The error code is the Microsoft VSS error. Check your VSS documentation or contact technical support." });
                _events.Add(new MRPInternalEventType() { id = 4401, name = "dt_snapshot_auto_delete_error", severity = "Error", response = "The automatic snapshot could not be deleted. This may be due to a lack of memory, the file does not exist, or another reason. The error code is the Microsoft Volume Shadow Copy error. Check your Volume Shadow Copy documentation or contact technical support." });
                _events.Add(new MRPInternalEventType() { id = 4402, name = "dt_snapshot_delete_error", severity = "Error", response = "The snapshot could not be deleted. This may be due to a lack of memory, the file does not exist, or another reason. The error code is the Microsoft Volume Shadow Copy error. Check your Volume Shadow Copy documentation or contact technical support." });
                _events.Add(new MRPInternalEventType() { id = 4403, name = "dt_snapshot_auto_create_error", severity = "Error", response = "No action required. A snapshot will automatically be created when the target data reaches a good state." });
                _events.Add(new MRPInternalEventType() { id = 4404, name = "dt_snapshot_auto_create", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4405, name = "dt_snapshot_auto_removed", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4406, name = "dt_snapshot_auto_enabled", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4407, name = "dt_snapshot_auto_disabled", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 4408, name = "dt_souce_ophans_move_error", severity = "Warning", response = "Orphan files could not be moved. For example, the location could be out of disk space. Check the Double-Take log for more information." });
                _events.Add(new MRPInternalEventType() { id = 4409, name = "dt_souce_ophans_delete_error", severity = "Warning", response = "Orphan files could not be deleted. Check the Double-Take log for more information." });
                _events.Add(new MRPInternalEventType() { id = 4410, name = "dt_registry_hive_dump_error", severity = "Error", response = "Contact technical support." });
                _events.Add(new MRPInternalEventType() { id = 4411, name = "dt_firewall_warning", severity = "Warning", response = "The firewall port needs to be unblocked or restrictions against Double-Take removed so that Double-Take data can be transmitted." });
                _events.Add(new MRPInternalEventType() { id = 5105, name = "dt_script_attempt", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 5106, name = "dt_script_success", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 5107, name = "dt_script_error", severity = "Error", response = "Verify that the script identified exists with the proper permissions." });
                _events.Add(new MRPInternalEventType() { id = 6000, name = "dt_general_information", severity = "Information", response = "This is a placeholder message for many other messages. See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 6001, name = "dt_general_warning", severity = "Warning", response = "This is a placeholder message for many other messages. See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 6002, name = "dt_general_error", severity = "Error", response = "This is a placeholder message for many other messages. See the specific log message for additional details." });
                _events.Add(new MRPInternalEventType() { id = 6003, name = "dt_job_create_success", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6004, name = "dt_job_started", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6005, name = "dt_job_stopped", severity = "Information", response = "No action required. If desired, you can restart your job." });
                _events.Add(new MRPInternalEventType() { id = 6006, name = "dt_job_deleted", severity = "Information", response = "No action required. If desired, you can re-create your job." });
                _events.Add(new MRPInternalEventType() { id = 6007, name = "dt_operation_failed", severity = "Error", response = "No action required. If desired, you can re-create your job." });
                _events.Add(new MRPInternalEventType() { id = 6008, name = "dt_operation_success", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6009, name = "dt_log_error", severity = "Error", response = "There is a problem with logging. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 6010, name = "dt_failover_condition_met", severity = "Warning", response = "Check your source machine and initiate failover, if user intervention for failover is configured. If you bring your source machine back online without initiating failover, the failover condition met state will be canceled. " });
                _events.Add(new MRPInternalEventType() { id = 6011, name = "dt_source_ping_fail", severity = "Error", response = "Check your source machine and initiate failover, if user intervention for failover is configured. If you bring your source machine back online without initiating failover, the source machine should start responding to the ping. " });
                _events.Add(new MRPInternalEventType() { id = 6012, name = "dt_target_failed_reboot", severity = "Error", response = "Reboot the target server to complete full server failover." });
                _events.Add(new MRPInternalEventType() { id = 6050, name = "dt_firewall_restriction", severity = "Error", response = "Verify the specified firewall port is open for Double-Take traffic." });
                _events.Add(new MRPInternalEventType()
                {
                    id = 6051,
                    name = "dt_firewall_blocked",
                    severity = "Error",
                    response = "Verify the specified firewall port is open for Double - Take traffic."
                });
                _events.Add(new MRPInternalEventType()
                {
                    id = 6052,
                    name = "dt_ipaddress_removed",
                    severity = "Informational",
                    response = "No action required."
                });
                _events.Add(new MRPInternalEventType() { id = 6053, name = "dt_source_netbios_removed", severity = "Informational", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6054, name = "dt_target_shares_removed", severity = "Informational", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6055, name = "dt_source_adspn_removed", severity = "Informational", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6056, name = "dt_target_ip_configured", severity = "Informational", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6057, name = "dt_target_netbios_added", severity = "Informational", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6058, name = "dt_target_shares_added", severity = "Informational", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6059, name = "dt_target_adspn_added", severity = "Informational", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6100, name = "dt_replica_provision_started", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6101, name = "dt_replica_provision_success", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6102, name = "dt_replica_provision_failed", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 6110, name = "dt_replica_failover_started", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6111, name = "dt_replica_failover_completed", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6112, name = "dt_replica_failover_error", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 6120, name = "dt_replica_failover_undo", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 6121, name = "dt_replica_provision_resume", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType() { id = 8000, name = "mrmp_manager_configured", severity = "Information", response = "No action required." });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8001,
                    name = "mrmp_manager_connection_warning",
                    severity = "Warning",
                    response = "Check network connections and manager for availability"
                });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8002,
                    name = "mrmp_manager_connection_error",
                    severity = "Error",
                    response = "Check network connections and manager for availability"
                });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8003,
                    name = "mrmp_workload_volume_freespace",
                    severity = "Warning",
                    response = "Increase free storage space on workload"
                });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8004,
                    name = "mrmp_workload_connection_os_error",
                    severity = "Warning",
                    response = "Ensure netwok connectivity and check credentials configured for workload"
                });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8005,
                    name = "mrmp_workload_connection_perf_error",
                    severity = "Warning",
                    response = "Ensure netwok connectivity and check credentials configured for workload"
                });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8006,
                    name = "mrmp_workload_connection_dt_error",
                    severity = "Warning",
                    response = "Ensure netwok connectivity and check credentials configured for workload"
                });

                _events.Add(new MRPInternalEventType()
                {
                    id = 8007,
                    name = "mrmp_workload_connection_netstat_error",
                    severity = "Warning",
                    response = "Ensure netwok connectivity and check credentials configured for workload"
                });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8004,
                    name = "mrmp_workload_connection_os_restored",
                    severity = "Information",
                    response = "Ensure netwok connectivity and check credentials configured for workload"
                });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8005,
                    name = "mrmp_workload_connection_perf_restored",
                    severity = "Information",
                    response = "Ensure netwok connectivity and check credentials configured for workload"
                });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8006,
                    name = "mrmp_workload_connection_dt_restored",
                    severity = "Information",
                    response = "Ensure netwok connectivity and check credentials configured for workload"
                });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8007,
                    name = "mrmp_workload_connection_netstat_restored",
                    severity = "Information",
                    response = "Ensure netwok connectivity and check credentials configured for workload"
                });
                _events.Add(new MRPInternalEventType() { id = 8008, name = "mrmp_workload_dt_event_collection", severity = "Error", response = "Ensure network connectivity between the manager and the workload in question. Ensure credentials configured for the workload is correct." });


                _events.Add(new MRPInternalEventType() { id = 8009, name = "mrmp_manager_db_compact_error", severity = "Error", response = "Contact technical support if this event occurs." });
                _events.Add(new MRPInternalEventType() { id = 8010, name = "mrmp_manager_wcf_error", severity = "Error", response = "Contact technical support if this event occurs." });
                _events.Add(new MRPInternalEventType() { id = 8011, name = "mrmp_manager_db_size", severity = "Warning", response = "Contact technical support if this event occurs." });
                _events.Add(new MRPInternalEventType() { id = 8012, name = "mrmp_workload_volume_added", severity = "Warning", response = "Update service stack information to match the source workload." });
                _events.Add(new MRPInternalEventType() { id = 8012, name = "mrmp_api_error", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 8013, name = "mrmp_manager_startup", severity = "Information", response = "No action required" });
                _events.Add(new MRPInternalEventType() { id = 8014, name = "mrmp_manager_startup_error", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation.Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 8015, name = "mrmp_manager_performance_thread_error", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 8016, name = "mrmp_manager_inventory_thread_error", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 8017, name = "mrmp_manager_netstat_thread_error", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 8018, name = "mrmp_manager_dt_thread_error", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 8019, name = "mrmp_manager_scheduler_thread_error", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8100,
                    name = "mrmp_workload_provision_started",
                    severity = "Information",
                    response = "No action required"
                });
                _events.Add(new MRPInternalEventType() { id = 8101, name = "mrmp_workload_provision_failed", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8102,
                    name = "mrmp_workload_dt_installation_started",
                    severity = "Information",
                    response = "No action required"
                });
                _events.Add(new MRPInternalEventType() { id = 8103, name = "mrmp_workload_dt_installation_failed", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8104,
                    name = "mrmp_dt_job_create_started",
                    severity = "Information",
                    response = "No action required"
                });
                _events.Add(new MRPInternalEventType() { id = 8105, name = "mrmp_dt_job_create_failed", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType()
                {
                    id = 8106,
                    name = "mrmp_dt_job_failover_started",
                    severity = "Information",
                    response = "No action required"
                });
                _events.Add(new MRPInternalEventType() { id = 8107, name = "mrmp_dt_job_create_failed", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation. Contact technical support if this event occurs again." });
                _events.Add(new MRPInternalEventType() { id = 8108, name = "mrmp_dt_target_timeout", severity = "Error", response = "Review the additional error information to identify the problem. Correct the problem and retry the operation. Contact technical support if this event occurs again." });
                return _events;
            }
        }
    }
}
