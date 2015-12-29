using System;

namespace CloudMoveyWorkerService.Portal
{
    class CloudMoveyPortal
    {
        private String _apiBase;

        public CloudMoveyPortal()
        {
            _apiBase = Global.api_base;
        }

        public MoveyTask task()
        {
            return new MoveyTask(this);
        }
        public MoveyPlatformtemplate platformtemplate()
        {
            return new MoveyPlatformtemplate(this);
        }
        public PortalCredential credential()
        {
            return new PortalCredential(this);
        }
        public MoveyWorker worker()
        {
            return new MoveyWorker(this);
        }
        public MoveyWorkload workload()
        {
            return new MoveyWorkload(this);
        }
        public MoveyNetworkflow netflow()
        {
            return new MoveyNetworkflow(this);
        }
        public MoveyPerformanceCounter performancecounter()
        {
            return new MoveyPerformanceCounter(this);
        }
        public MoveyPerformanceCategory performancecategory()
        {
            return new MoveyPerformanceCategory(this);
        }
        public PortalPlatform platform()
        {
            return new PortalPlatform(this);
        }
        public MoveyPlatformdomain platformdomain()
        {
            return new MoveyPlatformdomain(this);
        }
        public MoveyPlatformnetwork platformnetwork()
        {
            return new MoveyPlatformnetwork(this);
        }
        public String ApiBase
        {
            get { return _apiBase; }
        }
    }
}
