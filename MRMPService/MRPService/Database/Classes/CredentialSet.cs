using MRMPService.Utilities;
using System;

namespace MRMPService.LocalDatabase
{
    public class CredentialSet : ICredentialSet, System.IDisposable
    {
        private readonly MRPDatabase _context = new MRPDatabase();
        private IGenericRepository<Credential> _modelRepository;

        public CredentialSet()
        {
        }
        public IGenericRepository<Credential> ModelRepository
        {
            get { return _modelRepository ?? (_modelRepository = new GenericRepository<Credential>(_context)); }
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
