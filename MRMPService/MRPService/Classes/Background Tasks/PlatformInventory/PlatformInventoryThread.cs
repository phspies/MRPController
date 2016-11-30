using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using MRMPService.MRMPAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MRMPService.MRMPAPI;
using System.Threading.Tasks;


namespace MRMPService.PlatformInventory
{
    partial class PlatformInventoryThread
    {

        public void Start()
        {
            while (true)
            {
                DateTime _next_inventory_run = DateTime.UtcNow.AddMinutes(Global.platform_inventory_interval);
                Logger.log(String.Format("Staring platform inventory process with {0} threads", Global.platform_inventory_concurrency), Logger.Severity.Info);
                MRMP_ApiClient _cloud_movey = new MRMP_ApiClient();

                Stopwatch sw = Stopwatch.StartNew();
                int _new_platforms, _updated_platforms;
                _new_platforms = _updated_platforms = 0;

                try
                {
                    //process platform independant items
                    List<MRPPlatformType> _mrp_platforms = _cloud_movey.platform().list_paged_filtered(new MRPPlatformFilterPagedType() { deleted = false, enabled = true, page = 1, page_size = 200 }).platforms;
                    //Process Platforms in paralel
                    Parallel.ForEach(_mrp_platforms, new ParallelOptions { MaxDegreeOfParallelism = Global.platform_inventory_concurrency }, async (platform) =>
                          {
                              try
                              {
                                  await PlatformDoInventory.PlatformInventoryDo(platform);
                              }
                              catch (Exception ex)
                              {
                                  Logger.log(String.Format("Error collecting inventory information from platform {0} with error {1}", platform.platform, ex.ToString()), Logger.Severity.Error);
                              }
                          });

                    sw.Stop();

                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Error in mirror task: {0}", ex.ToString()), Logger.Severity.Error);
                }
                Logger.log(String.Format("Completed platform inventory for {0} platforms in {1} [next run at {2}]", (_updated_platforms + _new_platforms),
                    TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_inventory_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_inventory_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}
