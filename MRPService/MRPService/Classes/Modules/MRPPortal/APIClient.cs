using System;

namespace MRPService.API
{
    class MRP_ApiClient
    {
        public MRPTask task()
        {
            return new MRPTask(this);
        }
        public MRPPlatformtemplate platformtemplate()
        {
            return new MRPPlatformtemplate(this);
        }
        public PortalCredential credential()
        {
            return new PortalCredential(this);
        }
        public MRPWorker worker()
        {
            return new MRPWorker(this);
        }
        public MRPWorkload workload()
        {
            return new MRPWorkload(this);
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
        public MRPPlatformDomain platformdomain()
        {
            return new MRPPlatformDomain(this);
        }
        public MRPPlatformNetwork platformnetwork()
        {
            return new MRPPlatformNetwork(this);
        }
        public MRPJob job()
        {
            return new MRPJob(this);
        }
    }
}
