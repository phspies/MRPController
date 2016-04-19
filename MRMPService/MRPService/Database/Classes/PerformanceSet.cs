using MRMPService.Utilities;
using System;

namespace MRMPService.LocalDatabase
{
    public class PerformanceSet : IPerformanceSet, System.IDisposable
    {
        private readonly MRPDatabase _context = new MRPDatabase();
        private IGenericRepository<Performance> _modelRepository;

        public PerformanceSet()
        {
        }
        public IGenericRepository<Performance> ModelRepository
        {
            get { return _modelRepository ?? (_modelRepository = new GenericRepository<Performance>(_context)); }
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
