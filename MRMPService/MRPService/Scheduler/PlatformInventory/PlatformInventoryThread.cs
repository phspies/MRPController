using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MRMPService.MRMPAPI;
using System.Threading.Tasks;


namespace MRMPService.Scheduler.PlatformInventory
{
    partial class PlatformInventoryThread
    {

        public void Start()
        {
            while (true)
            {
                try
                {
                    DateTime _next_inventory_run = DateTime.UtcNow.AddMinutes(MRMPServiceBase.platform_inventory_interval);
                    Logger.log(String.Format("Staring platform inventory process with {0} threads", MRMPServiceBase.platform_inventory_concurrency), Logger.Severity.Debug);

                    Stopwatch sw = Stopwatch.StartNew();
                    //process platform independant items
                    List<MRPPlatformType> _mrp_platforms = (MRMPServiceBase._mrmp_api.platform().list(new MRPPlatformFilterPagedType() { deleted = false, enabled = true, page = 1, page_size = 200 })).platforms;
                    //Process Platforms in paralel
                    if (_mrp_platforms.Count > 0)
                    {
                        Parallel.ForEach(_mrp_platforms, new ParallelOptions { MaxDegreeOfParallelism = MRMPServiceBase.platform_inventory_concurrency }, (platform) =>
                              {
                                  try
                                  {
                                      PlatformDoInventory.PlatformInventoryDo(platform);
                                  }
                                  catch (Exception ex)
                                  {
                                      Logger.log(String.Format("Error collecting inventory information from platform {0} with error {1}", platform.platform, ex.ToString()), Logger.Severity.Error);
                                  }
                              });
                    }
                    sw.Stop();
                    Logger.log(String.Format("Completed platform inventory for platforms in {0} [next run at {1}]", TimeSpan.FromMilliseconds(sw.Elapsed.TotalSeconds), _next_inventory_run), Logger.Severity.Info);

                    //Wait for next run
                    while (_next_inventory_run > DateTime.UtcNow)
                    {
                        Thread.Sleep(new TimeSpan(0, 0, 5));
                    }
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("PlatformInventory: {0}", ex.ToString()), Logger.Severity.Fatal);
                }
            }
        }
    }
}
