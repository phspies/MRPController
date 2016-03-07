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
