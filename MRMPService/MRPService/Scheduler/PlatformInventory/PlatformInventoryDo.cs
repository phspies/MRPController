using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.Scheduler.PlatformInventory.DimensionData;
using MRMPService.Scheduler.PlatformInventory.RP4VM;
using MRMPService.Scheduler.PlatformInventory.VMWare;
using System.Threading.Tasks;

namespace MRMPService.Scheduler.PlatformInventory
{
    public class PlatformDoInventory
    {
        static public void PlatformInventoryDo(MRPPlatformType _platform, bool full = true)
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
    }
}
