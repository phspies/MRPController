using CloudMoveyWorkerService.CaaS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CaaS
{
    class SoftwareObject : Core
    {
        public SoftwareObject(DimensionData _dimensiondata) : base(_dimensiondata) { }
        public SoftwareLabels softwarelabels()
        {
            orgendpoint("/softwarelabel");
            SoftwareLabels softwarelabels = get<SoftwareLabels>(null, true) as SoftwareLabels;
            return softwarelabels;
        }
    }
}
