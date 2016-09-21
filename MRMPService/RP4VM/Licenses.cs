using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VMTypes
{
    public class Licenses : Core
    {
        public Licenses(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public clusterLicenseReportSet getLicenseReport_Method()
        {
            endpoint = "/licenses/report";
            mediatype = "application/json";
            return get<clusterLicenseReportSet>();
        }

        public void removeLicense_Method(restString restString_object)
        {
            endpoint = "/licenses";
            mediatype = "*/*";
            delete(restString_object);
        }
    }
}
