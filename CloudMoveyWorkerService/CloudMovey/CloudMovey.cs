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

        public CloudMovey(String apibase, String username, String password)
        {
            _apiBase = apibase;
            _username = username;
            _password = password;
        }

        public TasksObject task()
        {
            return new TasksObject(this);
        }

        public Worker worker()
        {
            return new Worker(this);
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
