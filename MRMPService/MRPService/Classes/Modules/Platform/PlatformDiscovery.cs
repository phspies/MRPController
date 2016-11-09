using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using MRMPService.PlatformInventory;
using System;

namespace MRMPService.Tasks.DiscoveryPlatform
{
    public class PlatformDiscovery : IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PlatformDiscovery()
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
        public static void PlatformDiscoveryDo(MRPTaskType payload)
        {
            using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                _mrp_api.task().progress(payload, String.Format("Starting platform discovering process"), 5);

                MRPPlatformType _platform = payload.submitpayload.platform;
                MRPCredentialType _platform_credentail = _platform.credential;
                try
                {
                    switch (_platform.platformtype)
                    {
                        case "dimension_data":
                            PlatformDimensionDataMCP2InventoryDo.UpdateMCPPlatform(_platform, true);
                            break;
                        case "hyperv":
                            break;
                        case "vmware":
                            PlatformVMwareInventoryDo.UpdateVMwarePlatform(_platform, true);
                            break;
                        case "rp4vm":
                            PlatformRP4VMInventoryDo.UpdateRP4VMPlatform(_platform, false);
                            break;
                    }
                    _mrp_api.task().progress(payload, String.Format("Successfully refreshed platform resources"), 10);
                    _mrp_api.task().successcomplete(payload);
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Error in inventory process for {0} {1}", (_platform.platformtype + " : " + _platform.moid), ex.ToString()), Logger.Severity.Error);
                    _mrp_api.task().progress(payload, String.Format("Error refreshing platform resources", ex.Message), 10);
                    _mrp_api.task().failcomplete(payload, String.Format("Error refreshing platform resources", ex.ToString()));
                }
            }

        }
    }

}
