using MRPService.API;
using MRPService.API.Types.API;
using MRPService.Utilities;
using System;

namespace MRPService.LocalDatabase
{
    public class WorkloadSet : IWorkloadSet, System.IDisposable
    {
        private readonly MRPDatabase _context = new MRPDatabase();
        private IGenericRepository<Workload> _modelRepository;

        public WorkloadSet()
        {
        }
        public IGenericRepository<Workload> ModelRepository
        {
            get { return _modelRepository ?? (_modelRepository = new GenericRepository<Workload>(_context)); }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void InventoryUpdateStatus(string workload_id, string message, bool status)
        {
            var workload = this.ModelRepository.GetById(workload_id);
            if (status)
            {
                workload.dt_collection_status = true;
                workload.dt_contact_error_count = 0;
            }
            else
            {
                workload.dt_collection_status = false;
                workload.dt_contact_error_count = workload.dt_contact_error_count == null ? 1 : workload.dt_contact_error_count++;
            }
            workload.dt_collection_message = message;
            workload.dt_last_contact = DateTime.Now;
            this.Save();
        }
        public void PeformanceUpdateStatus(string workload_id, string message, bool status)
        {
            var workload = this.ModelRepository.GetById(workload_id);
            if (status)
            {
                workload.perf_collection_status = true;
                workload.perf_contact_error_count = 0;
            }
            else
            {
                workload.perf_collection_status = false;
                workload.perf_contact_error_count = workload.perf_contact_error_count == null ? 1 : workload.perf_contact_error_count++;
            }
            workload.perf_collection_message = message;
            workload.perf_last_contact = DateTime.Now;
            this.Save();
        }
        public void DoubleTakeUpdateStatus(string workload_id, string message, bool status)
        {
            var workload = this.ModelRepository.GetById(workload_id);
            if (status)
            {
                workload.dt_collection_status = true;
                workload.dt_contact_error_count = 0;
            }
            else
            {
                workload.dt_collection_status = false;
                workload.dt_contact_error_count = workload.dt_contact_error_count == null ? 1 : workload.dt_contact_error_count++;
            }
            workload.dt_collection_message = message;
            workload.dt_last_contact = DateTime.Now;
            this.Save();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
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
