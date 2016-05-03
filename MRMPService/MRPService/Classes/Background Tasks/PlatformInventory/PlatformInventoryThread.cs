using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using MRMPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MRMPService.API;
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
                MRP_ApiClient _cloud_movey = new MRP_ApiClient();

                Stopwatch sw = Stopwatch.StartNew();
                int _new_platforms, _updated_platforms;
                _new_platforms = _updated_platforms = 0;

                try
                {
                    //process platform independant items
                    MRPPlatformListType _platformplatforms = _cloud_movey.platform().listplatforms();

                    //process platforms
                    List<Platform> _workerplatforms;
                    using (MRPDatabase db = new MRPDatabase())
                    {
                        _workerplatforms = db.Platforms.ToList();
                    }

                    foreach (var _platform in _workerplatforms)
                    {
                        MRPPlatformCRUDType _crudplatform = new MRPPlatformCRUDType();
                        _crudplatform.id = _platform.id;
                        _crudplatform.manager_id = Global.manager_id;
                        _crudplatform.credential_id = _platform.credential_id;
                        _crudplatform.platform_version = _platform.platform_version;
                        _crudplatform.platformtype = (new Vendors()).VendorList.FirstOrDefault(x => x.ID == _platform.vendor).Vendor.Replace(" ", "_").ToLower();
                        _crudplatform.moid = _platform.moid;
                        _crudplatform.platform = _platform.description;

                        if (_platformplatforms.platforms.Exists(x => x.id == _platform.id))
                        {
                            _cloud_movey.platform().updateplatform(_crudplatform);
                            _updated_platforms += 1;
                        }
                        else
                        {
                            _cloud_movey.platform().createplatform(_crudplatform);
                            _new_platforms += 1;
                        }
                    }

                    //Process Platforms in paralel
                    Parallel.ForEach(_workerplatforms, new ParallelOptions { MaxDegreeOfParallelism = Global.platform_inventory_concurrency }, (platform) =>
                          {
                              try
                              {
                                  using (PlatformDoInventory _inventoryclass = new PlatformDoInventory())
                                  {
                                      _inventoryclass.PlatformInventoryDo(platform.id, platform.vendor);
                                  }
                              }
                              catch (Exception ex)
                              {
                                  Logger.log(String.Format("Error collecting inventory information from platform {0} with error {1}", platform.description, ex.ToString()), Logger.Severity.Error);
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
