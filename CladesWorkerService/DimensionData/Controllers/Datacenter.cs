using DoubleTakeProxyService.DimensionData.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleTakeProxyService.DimensionData.API
{
    class DatacenterObject : Core
    {
        public DatacenterObject(DimensionData _dimensiondata) : base(_dimensiondata) { }
        public DatacentersWithMaintenanceStatus datacenters()
        {
            orgendpoint("/datacenterWithMaintenanceStatus?");
            DatacentersWithMaintenanceStatus datacenters = get<DatacentersWithMaintenanceStatus>(null, true) as DatacentersWithMaintenanceStatus;
            return datacenters;
        }

    }
}
