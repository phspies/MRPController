using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using MRMPService.PlatformInventory;
using System;

namespace MRMPService.Tasks.DiscoveryPlatform
{
    public class PlatformDiscovery
    {
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
