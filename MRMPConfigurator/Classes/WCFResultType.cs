using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MRMPConfigurator.Classes
{
    public class WCFResultType
    {
        public object result { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
    }
    namespace Common
    {
        /// <summary>
        /// Generic WCF service exception. 
        /// </summary>

        [DataContract]
        public class UnexpectedServiceFault
        {
            [DataMember]
            public string ErrorMessage { get; set; }
            [DataMember]
            public string StackTrace { get; set; }
            [DataMember]
            public string Target { get; set; }
            [DataMember]
            public string Source { get; set; }
        }
    }
}
