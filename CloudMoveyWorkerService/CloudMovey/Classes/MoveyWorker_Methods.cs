using CloudMoveyWorkerService.CloudMovey.Types;
using System;
using System.Net;

namespace CloudMoveyWorkerService.CloudMovey
{
    class MoveyWorker : Core
    {
        public MoveyWorker(CloudMovey _CloudMovey) : base(_CloudMovey)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        }
        public bool confirm_worker(MoveyWorkerType _worker)
        {
            endpoint = ("/api/v1/workers/confirm.json");
            Object returnval = post<MoveyWorkerType>(_worker);
            if (returnval is MoveyError)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool register_worker(MoveyWorkerType _worker)
        {
            endpoint = ("/api/v1/workers/register.json");
            Object returnval = post<MoveyWorkerType>(this);
            if (returnval is MoveyError)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
