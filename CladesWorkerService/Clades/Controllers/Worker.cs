using CladesWorkerService.Clades.Models;
using CladesWorkerService.Clades.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CladesWorkerService.Clades.API
{
    class WorkerObject : Core
    {
        public WorkerObject(Clades _clades) : base(_clades) { }
        public Worker worker;
        public Error status;
        public bool get_worker()
        {
            endpoint = (String.Format("/clades/worker/get_worker.json?id={0}", Global.agentId));
            Object returnval = get<Worker>(null, true);
            if (returnval is Error)
            {
                status = (Error)returnval;
                EventLog.WriteEntry("Clades", JsonConvert.SerializeObject(status));
                return false;
            } else
            {
                worker = (Worker)returnval;
                EventLog.WriteEntry("Clades", JsonConvert.SerializeObject(worker));
                return true;
            }
        }
        public bool register_worker(Worker newworker)
        {
            endpoint = ("/clades/worker/create_worker.json");
            Object returnval = post<Worker>(newworker, false);
            if (returnval is Error)
            {
                status = (Error)returnval;
                EventLog.WriteEntry("Clades", JsonConvert.SerializeObject(status));
                return false;
            }
            else
            {
                worker = (Worker)returnval;
                EventLog.WriteEntry("Clades", JsonConvert.SerializeObject(worker));
                return true;
            }
        }
    }
}


