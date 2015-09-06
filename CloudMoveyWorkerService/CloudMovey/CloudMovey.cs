using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudMoveyWorkerService.CloudMovey.Models;

namespace CloudMoveyWorkerService.CloudMovey
{
    class CloudMovey
    {
        private String _apiBase;

        public CloudMovey()
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
        public MoveyCredential credential()
        {
            return new MoveyCredential(this);
        }
        public MoveyWorker worker()
        {
            return new MoveyWorker(this);
        }
        public MoveyWorkload workload()
        {
            return new MoveyWorkload(this);
        }
        public MoveyPlatform platform()
        {
            return new MoveyPlatform(this);
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
