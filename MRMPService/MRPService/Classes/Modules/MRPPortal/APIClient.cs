using MRMPService.MRMPAPI.Contracts;
using System;

namespace MRMPService.MRMPAPI
{
    public class MRMP_ApiClient
    {
        public MRPOrganization organization()
        {
            return new MRPOrganization(this);
        }
        public MRPTask task()
        {
            return new MRPTask(this);
        }
        public MRPManager manager()
        {
            return new MRPManager(this);
        }
        public MRPWorkload workload()
        {
            return new MRPWorkload(this);
        }
        public MRPEvent @event()
        {
            return new MRPEvent(this);
        }
        public MRPNetworkflow netflow()
        {
            return new MRPNetworkflow(this);
        }
        public MRPPerformanceCounter performancecounter()
        {
            return new MRPPerformanceCounter(this);
        }
        public MRPNetworkstat netstat()
        {
            return new MRPNetworkstat(this);
        }
        public MRPPerformanceCategory performancecategory()
        {
            return new MRPPerformanceCategory(this);
        }
        public PortalPlatform platform()
        {
            return new PortalPlatform(this);
        }
        public PortalPlatformDatacenter platformdatacenter()
        {
            return new PortalPlatformDatacenter(this);
        }
 
        public MRPManagementobject managementobject()
        {
            return new MRPManagementobject(this);
        }

    }
}
