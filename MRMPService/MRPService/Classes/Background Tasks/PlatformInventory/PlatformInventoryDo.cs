using MRMPService.MRMPService.Log;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MRMPService.API;
using MRMPService.LocalDatabase;

namespace MRMPService.PlatformInventory
{
    class Inventory
    {
        static public void PlatformInventoryDo(string platform_id, bool full = true)
        {
            Stopwatch sw = Stopwatch.StartNew();

            Platform _platform;
            using (PlatformSet _platform_db = new PlatformSet())
            {
                _platform = _platform_db.ModelRepository.GetById(platform_id);
            }
            switch (_platform.vendor)
            {
                case 0:
                    PlatformDimensionDataMCP2InventoryDo.UpdateMCPPlatform(platform_id, full);
                    break;
                case 1:
                    PlatformVMwareInventoryDo.UpdateVMwarePlatform(platform_id, full);
                    break;
                case 3:
                    PlatformPhysicalInventoryDo.UpdatePhysicalPlatform(platform_id);
                    break;

            }
            sw.Stop();

            Logger.log(
                String.Format("Completed platform inventory for {0} in {1}",
                _platform.moid, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)
                ), Logger.Severity.Info);

        }
    }
}
