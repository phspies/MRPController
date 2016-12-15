using MRMPService.Utilities;
using System;

namespace MRMPService.LocalDatabase
{
    public class PerformancecounterSet : IPerformancecounterSet, System.IDisposable
    {
        private readonly MRPDatabase _context = new MRPDatabase();
        private IGenericRepository<Performancecounter> _modelRepository;

        public PerformancecounterSet()
        {
        }
        public IGenericRepository<Performancecounter> ModelRepository
        {
            get { return _modelRepository ?? (_modelRepository = new GenericRepository<Performancecounter>(_context)); }
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
