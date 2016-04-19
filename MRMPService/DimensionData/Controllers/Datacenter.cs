using MRPService.CaaS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.CaaS1
{
    class DatacenterObject : Core
    {
        public DatacenterObject(DimensionData _dimensiondata) : base(_dimensiondata) { }

        public DatacenterListType datacenters(List<Option> _options=null)
        {
            orgendpoint("/infrastructure/datacenter");
            urloptions = _options;
            return get<DatacenterListType>(null, true) as DatacenterListType;
        }
    }
}
