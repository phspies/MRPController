using MRMPService.Utilities;
using System;

namespace MRMPService.LocalDatabase
{
    public class NetstatSet : INetstatSet, System.IDisposable
    {
        private readonly MRPDatabase _context = new MRPDatabase();
        private IGenericRepository<Netstat> _modelRepository;

        public NetstatSet()
        {
        }
        public IGenericRepository<Netstat> ModelRepository
        {
            get { return _modelRepository ?? (_modelRepository = new GenericRepository<Netstat>(_context)); }
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
