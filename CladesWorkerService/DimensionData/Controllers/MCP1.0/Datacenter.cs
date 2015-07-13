using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.CaaS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CaaS1
{
    class DatacenterObject : Core
    {
        public DatacenterObject(DimensionData _dimensiondata) : base(_dimensiondata) { }
        public DatacenterListType datacenters(List<Option> _options=null)
        {

            orgendpoint2("/infrastructure/datacenter");
            urloptions = _options;
            return get<DatacenterListType>(null, true) as DatacenterListType;
        }
        public ImagesWithDiskSpeed templates(List<Option> options = null)
        {
            simpleendpoint("/base/imageWithDiskSpeed");
            urloptions = options;
            ImagesWithDiskSpeed softwarelabels = get<ImagesWithDiskSpeed>(null, true) as ImagesWithDiskSpeed;
            return softwarelabels;
        }
    }
}
