using MRMPService.Utilities;
using System;

namespace MRMPService.LocalDatabase
{
    public class PerfCounterSampleSet : IPerfCounterSampleSet, System.IDisposable
    {
        private readonly MRPDatabase _context = new MRPDatabase();
        private IGenericRepository<PerfCounterSample> _modelRepository;

        public PerfCounterSampleSet()
        {
        }
        public IGenericRepository<PerfCounterSample> ModelRepository
        {
            get { return _modelRepository ?? (_modelRepository = new GenericRepository<PerfCounterSample>(_context)); }
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
