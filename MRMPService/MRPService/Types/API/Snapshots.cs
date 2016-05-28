using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{

    public class MRPSnapshotType
    {
        public string id { get; set; }
        public string comment { get; set; }
        public Guid moid { get; set; }
        public SnapshotCreationReason reason { get; set; }
        public TargetStates states { get; set; }
        public DateTime timestamp { get; set; }
    }
}
