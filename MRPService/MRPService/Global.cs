using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Web;

namespace MRPService
{
    public static class Global
    {

        /// <summary>
        /// Static value protected by access routine.
        /// </summary>
        /// 

        public static object mcp_exec = new object();

        static bool _debug;
        static String _agent_id;
        static String _api_base;
        static String _worker_version = "0.0.1";

        public static int os_inventory_interval=1440;
        public static int os_inventory_concurrency=10;

        public static int platform_inventory_interval=1440;
        public static int platform_inventory_concurrency=10;

        public static int performance_inventory_concurrency=10;

        public static int portal_upload_interval=60;

        public static int scheduler_interval=1;

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
        public static String version_number
        {
            get
            {
                return _worker_version;
            }
            set
            {
                _worker_version = value;
            }
        }
        public static String api_base
        {
            get
            {
                return _api_base;
            }
            set
            {
                _api_base = value;
            }
        }
        public static String agent_id
        {
            get
            {
                return _agent_id;
            }
            set
            {
                _agent_id = value;
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

        public class TryLock : IDisposable
        {
            private object locked;

            public bool HasLock { get; private set; }

            public TryLock(object obj)
            {
                if (Monitor.TryEnter(obj))
                {
                    HasLock = true;
                    locked = obj;
                }
            }

            public void Dispose()
            {
                if (HasLock)
                {
                    Monitor.Exit(locked);
                    locked = null;
                    HasLock = false;
                }
            }
        }
    }


    [DataContract]
    public class JobManagerRequest
    {
        [DataMember]
        public CredentialsInfo credentials;

        [DataMember]
        public string jobtype;


        [DataMember]
        public Guid jobid;

        [DataMember]
        public String workloadname;

        [DataMember]
        public Guid imageid;
    }

    [DataContract]
    public class CredentialsInfo
    {
        [DataMember]
        public String sourceIPAddress;

        [DataMember]
        public String sourceUserDomain;

        [DataMember]
        public String sourceUserAccount;

        [DataMember]
        public String sourceUserPassword;

        [DataMember]
        public String targetIPAddress;

        [DataMember]
        public String targetUserDomain;

        [DataMember]
        public String targetUserAccount;

        [DataMember]
        public String targetUserPassword;
    }


    //subsystem
    //objecttype
    //command
    //inputObject
    //outputObject
    public class InstructionAction
    {
        private String subsystemField;

        public String subsystem
        {
            get { return subsystemField; }
            set { subsystemField = value; }
        }
    }

}