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

        public void UpdateStatus(string workload_id, string message, int status)
        {
            var workload = this.ModelRepository.GetById(workload_id);
            workload.last_contact_message = message;
            workload.last_contact_attempt = DateTime.Now;
            workload.last_contact_status = status;
            this.Save();

            //update portal with error
            ApiClient _cloud_movey = new ApiClient();
            MRPWorkloadCRUDType _update_workload = new MRPWorkloadCRUDType();
            _update_workload.id = workload_id;
            _update_workload.provisioned = true;
            _update_workload.os_collection_status = false;
            _update_workload.os_collection_message = message;
            _cloud_movey.workload().updateworkload(_update_workload);



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
