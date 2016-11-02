using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Log;
using System;

namespace MRMPService.PlatformInventory
{
    public class PlatformDoInventory : IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PlatformDoInventory()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
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
                    case "rp4vm":
                        PlatformRP4VMInventoryDo.UpdateRP4VMPlatform(_platform);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error in inventory process for {0} {1}", (_platform.platformtype + " : " + _platform.moid), ex.ToString()), Logger.Severity.Error);
            }

        }

    }
}
