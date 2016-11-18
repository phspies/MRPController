using Microsoft.Win32;
using MRMPService.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;
using static MRMPService.Utilities.SyncronizedList;

namespace MRMPService
{
    public static class Global
    {

        /// <summary>
        /// Static value protected by access routine.
        /// </summary>
        /// 

        public static object mcp_exec = new object();

        static bool _debug;
        static String _manager_id;

        static System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        static FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        static string _manager_version = fvi.FileVersion;

        public static SyncronisedList<CollectionCounter> _available_counters = new SyncronisedList<CollectionCounter>(new List<CollectionCounter>());


        public static int os_inventory_interval = (int)MRPRegistry.RegAccess("os_inventory_interval",1440, RegistryValueKind.DWord); //minutes
        public static int os_inventory_concurrency = (int)MRPRegistry.RegAccess("os_inventory_concurrency", 10, RegistryValueKind.DWord);

        public static int dt_job_polling_interval = (int)MRPRegistry.RegAccess("dt_job_polling_interval", 900, RegistryValueKind.DWord); //minutes
        public static int dt_job_polling_concurrency = (int)MRPRegistry.RegAccess("dt_job_polling_concurrency", 10, RegistryValueKind.DWord);

        public static int dt_event_polling_interval = (int)MRPRegistry.RegAccess("dt_event_polling_interval", 600, RegistryValueKind.DWord); //minutes
        public static int dt_event_polling_concurrency = (int)MRPRegistry.RegAccess("dt_event_polling_concurrency", 10, RegistryValueKind.DWord);

        public static int rp4vm_event_polling_interval = (int)MRPRegistry.RegAccess("rp4vm_event_polling_interval", 300, RegistryValueKind.DWord); //minutes
        public static int rp4vm_event_polling_concurrency = (int)MRPRegistry.RegAccess("rp4vm_event_polling_concurrency", 2, RegistryValueKind.DWord);

        public static int rp4vm_group_polling_interval = (int)MRPRegistry.RegAccess("rp4vm_group_polling_interval", 300, RegistryValueKind.DWord); //minutes
        public static int rp4vm_group_polling_concurrency = (int)MRPRegistry.RegAccess("rp4vm_group_polling_concurrency", 2, RegistryValueKind.DWord);

        public static int os_netstat_interval = (int)MRPRegistry.RegAccess("os_netstat_interval", 60, RegistryValueKind.DWord);  //minutes
        public static int os_netstat_concurrency = (int)MRPRegistry.RegAccess("os_netstat_concurrency", 20, RegistryValueKind.DWord);

        public static int platform_inventory_interval = (int)MRPRegistry.RegAccess("platform_inventory_interval", 1440, RegistryValueKind.DWord); //minutes
        public static int platform_inventory_concurrency = (int)MRPRegistry.RegAccess("platform_inventory_concurrency", 10, RegistryValueKind.DWord);
        public static int platform_workload_inventory_concurrency = (int)MRPRegistry.RegAccess("platform_workload_inventory_concurrency", 10, RegistryValueKind.DWord);

        public static int os_performance_concurrency = (int)MRPRegistry.RegAccess("os_performance_concurrency", 10, RegistryValueKind.DWord);

        public static int portal_upload_interval = (int)MRPRegistry.RegAccess("portal_upload_interval", 30, RegistryValueKind.DWord);//seconds
        public static int portal_upload_netflow_page_size = (int)MRPRegistry.RegAccess("portal_upload_netflow_page_size", 100, RegistryValueKind.DWord); 
        public static int portal_upload_performanceounter_page_size = (int)MRPRegistry.RegAccess("portal_upload_performanceounter_page_size", 100, RegistryValueKind.DWord);

        public static int scheduler_interval = (int)MRPRegistry.RegAccess("scheduler_interval", 5, RegistryValueKind.DWord); //seconds
        public static int scheduler_concurrency = (int)MRPRegistry.RegAccess("scheduler_concurrency", 30, RegistryValueKind.DWord);

        public static string api_url = (string)MRPRegistry.RegAccess("api_url", null, RegistryValueKind.String);

        static String _organization_id;
        static int _worker_queue_count;
        static EventLog _eventLog;
    
        public static int worker_queue_count
        {
            
            get { return _worker_queue_count; }
            set { _worker_queue_count = value; }
        }

        public static EventLog event_log
        {
            get
            {
                return _eventLog;
            }
            set
            {
                _eventLog = value;
            }
        }
        public static String manager_version
        {
            get
            {
                return _manager_version;
            }
            set
            {
                _manager_version = value;
            }
        }
        public static String api_base
        {
            get
            {
                return api_url == null ? "https://mrmp.dimensiondata.com" : api_url;
            }
        }
        public static String manager_id
        {
            get
            {
                return _manager_id;
            }
            set
            {
                _manager_id = value;
            }
        }

        /// <summary>
        /// Access routine for global variable.
        /// </summary>
        public static bool debug
        {
            get
            {
                return _debug;
            }
            set
            {
                _debug = value;
            }
        }
        public static string organization_id
        {
            get
            {
                return _organization_id;
            }
            set
            {
                _organization_id = value;
            }
        }

        /// <summary>
        /// Global static field.
        /// </summary>
        public static bool GlobalBoolean;
    }
}