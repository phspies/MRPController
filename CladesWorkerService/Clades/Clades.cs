using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CladesWorkerService.Clades.Models;

namespace CladesWorkerService.Clades
{
    class Clades
    {
        private String _apiBase, _username, _password;

        public Clades(String apibase, String username, String password)
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
