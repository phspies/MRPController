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
        private String _apiBase, _username, _password;

        public CloudMovey()
        {
            _apiBase = Global.apiBase;
        }

        public MoveyTask task()
        {
            return new MoveyTask(this);
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
        public String ApiBase
        {
            get { return _apiBase; }
        }
        public String Username
        {
            get { return _username; }
        }
        public String Password
        {
            get { return _password; }
        }

    }
}
