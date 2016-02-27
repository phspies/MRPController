using MRPService.Utilities;
using System;

namespace MRPService.LocalDatabase
{
    public class PlatformSet : IPlatformSet, System.IDisposable
    {
        private readonly MRPDatabase _context;
        private IGenericRepository<Platform> _modelRepository;

        public PlatformSet()
        {
            MRPDatabase _context = new MRPDatabase();
        }
        public IGenericRepository<Platform> ModelRepository
        {
            get { return _modelRepository ?? (_modelRepository = new GenericRepository<Platform>(_context)); }
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
