using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DoubleTakeRestProxy
{
    public static class Global
    {

        /// <summary>
        /// Static value protected by access routine.
        /// </summary>
        static bool _debug;

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

    public class UriModel
    {
        public UriBuilder sourceUri { get; set; }
        public UriBuilder sourceShortUri { get; set; }
        public UriBuilder targetUri { get; set; }
        public UriBuilder targetShortUri { get; set; }

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