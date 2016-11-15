using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Log;
using System;

namespace MRMPService.PlatformInventory
{
    public class PlatformDoInventory
    {
        static public void PlatformInventoryDo(MRPPlatformType _platform, bool full = true)
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
