using System;

namespace MRMPService.PlatformInventory
{
    public class PlatformDoInventory : IDisposable
    {
        public void PlatformInventoryDo(string platform_id, int _platform_vendor_id, bool full = true)
        {
            switch (_platform_vendor_id)
            {
                case 0:
                    PlatformDimensionDataMCP2InventoryDo.UpdateMCPPlatform(platform_id, full);
                    break;
                case 1:
                    PlatformVMwareInventoryDo.UpdateVMwarePlatform(platform_id, full);
                    break;
            }
        }
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
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
