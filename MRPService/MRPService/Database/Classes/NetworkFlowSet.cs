using MRPService.Utilities;
using System;

namespace MRPService.LocalDatabase
{
    public class NetworkFlowSet : INetworkFlowSet, System.IDisposable
    {
        private readonly MRPDatabase _context = new MRPDatabase();
        private IGenericRepository<NetworkFlow> _modelRepository;

        public NetworkFlowSet()
        {
        }
        public IGenericRepository<NetworkFlow> ModelRepository
        {
            get { return _modelRepository ?? (_modelRepository = new GenericRepository<NetworkFlow>(_context)); }
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
