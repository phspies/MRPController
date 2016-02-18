using System;

namespace MRPService.Portal
{
    class CloudMRPPortal
    {
        private String _apiBase;

        public CloudMRPPortal()
        {
            _apiBase = Global.api_base;
        }

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
        public MRPPerformanceCategory performancecategory()
        {
            return new MRPPerformanceCategory(this);
        }
        public PortalPlatform platform()
        {
            return new PortalPlatform(this);
        }
        public MRPPlatformdomain platformdomain()
        {
            return new MRPPlatformdomain(this);
        }
        public MRPPlatformnetwork platformnetwork()
        {
            return new MRPPlatformnetwork(this);
        }
        public MRPJob job()
        {
            return new MRPJob(this);
        }
        public String ApiBase
        {
            get { return _apiBase; }
        }
    }
}
