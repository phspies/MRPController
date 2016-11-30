using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Log;
using System;
using System.Threading.Tasks;

namespace MRMPService.PlatformInventory
{
    public class PlatformDoInventory
    {
        static public async Task PlatformInventoryDo(MRPPlatformType _platform, bool full = true)
        {
            switch (_platform.platformtype)
            {
                case "dimension_data":
                    await PlatformDimensionDataMCP2InventoryDo.UpdateMCPPlatform(_platform, full);
                    break;
                case "vmware":
                    await PlatformVMwareInventoryDo.UpdateVMwarePlatform(_platform, full);
                    break;
                case "rp4vm":
                    await PlatformRP4VMInventoryDo.UpdateRP4VMPlatform(_platform);
                    break;
            }
        }
    }
}
