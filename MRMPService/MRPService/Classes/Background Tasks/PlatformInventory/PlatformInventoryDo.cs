using MRMPService.API.Types.API;
using MRMPService.MRMPService.Log;
using System;

namespace MRMPService.PlatformInventory
{
    public class PlatformDoInventory : IDisposable
    {
        public void PlatformInventoryDo(MRPPlatformType _platform, bool full = true)
        {
            try
            {
                switch (_platform.platformtype)
                {
                    case "dimension_data":
                        PlatformDimensionDataMCP2InventoryDo.UpdateMCPPlatform(_platform, full);
                        break;
                    case "vmware":
                        PlatformVMwareInventoryDo.UpdateVMwarePlatform(_platform, full);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error in inventory process for {0} {1}", (_platform.platformtype + " : " + _platform.moid), ex.ToString()), Logger.Severity.Error);
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
