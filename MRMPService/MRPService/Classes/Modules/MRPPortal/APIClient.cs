using MRMPService.MRMPAPI.Types.API;
using System;

namespace MRMPService.MRMPAPI
{
    class MRMP_ApiClient : IDisposable
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
        public MRPPlatformDomain platformdomain()
        {
            return new MRPPlatformDomain(this);
        }
        public MRPPlatformNetwork platformnetwork()
        {
            return new MRPPlatformNetwork(this);
        }
        public MRPManagementobject managementobject()
        {
            return new MRPManagementobject(this);
        }
        public MRPManagementobjectStat managementobjectstat()
        {
            return new MRPManagementobjectStat(this);
        }
        public MRPManagementobjectSnapshots jobimage()
        {
            return new MRPManagementobjectSnapshots(this);
        }
        public MRPProtectiongrouptree stacktree()
        {
            return new MRPProtectiongrouptree(this);
        }


        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
    }
}
