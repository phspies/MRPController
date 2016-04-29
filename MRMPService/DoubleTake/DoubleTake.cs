using DoubleTake.Web.Client;
using System;

namespace MRMPService.DoubleTake
{
    class Doubletake : IDisposable
    {
        public string _source_workload_id, _target_workload_id;
        public Doubletake(string source_workload_id, string target_workload_id)
        {
            _source_workload_id = source_workload_id;
            _target_workload_id = target_workload_id;
        }

        public Job job()
        {
            return new Job(this);
        }
        public Workload workload()
        {
            return new Workload(this);
        }
        public Management management()
        {
            return new Management(this);
        }
        public void Dispose()
        {
            System.GC.SuppressFinalize(this);
        }
    }
}
