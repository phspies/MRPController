using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CladesWorkerService
{
    public static class Global
    {

        /// <summary>
        /// Static value protected by access routine.
        /// </summary>
        static bool _debug;
        static String _agentId;
        static String _apiBase;
        static String _verionNumber;
        static EventLog _eventLog;
        public static EventLog eventLog
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
        public static String verionNumber
        {
            get
            {
                return _verionNumber;
            }
            set
            {
                _verionNumber = value;
            }
        }
        public static String apiBase
        {
            get
            {
                return _apiBase;
            }
            set
            {
                _apiBase = value;
            }
        }
        public static String agentId
        {
            get
            {
                return _agentId;
            }
            set
            {
                _agentId = value;
            }
        }

        /// <summary>
        /// Access routine for global variable.
        /// </summary>
        public static bool Debug
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

        /// <summary>
        /// Global static field.
        /// </summary>
        public static bool GlobalBoolean;
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
        public String servername;

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