﻿using MRPService.MRPService.Log;
using MRPService.LocalDatabase;
using MRPService.API.Types.API;
using MRPService.WCF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DD.CBU.Compute.Api.Client;
using System.Net;
using DD.CBU.Compute.Api.Contracts.Requests.Server20;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Requests;
using DD.CBU.Compute.Api.Contracts.Requests.Infrastructure;
using DD.CBU.Compute.Api.Contracts.General;
using MRPService.Utilities;
using VMware.Vim;
using MRPService.VMWare;
using System.Collections.Specialized;

namespace MRPService.API.Classes
{
    partial class PlatformInventoryWorker
    {
        ApiClient _cloud_movey = new ApiClient();

        //Order or sync process
        // 1. update worker information from portal - perf_collection
        // 2. push credentials to portal
        // 3. delete workloads from local DB that is no longer in source platforms
        // 4. push workloads and networks to portal

        public void Start()
        {
            while (true)
            { 

                Stopwatch sw = Stopwatch.StartNew();
                int _new_credentials, _new_platforms, _new_platformnetworks, _new_workloads, _updated_credentials, _updated_platforms, _updated_platformnetworks, _updated_workloads, _removed_workloads;
                _new_credentials = _new_platforms = _new_platformnetworks = _new_workloads = _updated_credentials = _updated_platformnetworks = _updated_platforms = _updated_workloads = _removed_workloads = 0;

                try
                {
   
                    Logger.log("Staring platform inventory process", Logger.Severity.Info);

                    //process platform independant items

                    //process platforms
                    List<Platform> _workerplatforms;
                    using (MRPDatabase db = new MRPDatabase())
                    {
                        _workerplatforms = db.Platforms.ToList();
                    }
                    MRPPlatformListType _platformplatforms = _cloud_movey.platform().listplatforms();
                    foreach (var _platform in _workerplatforms)
                    {
                        MRPPlatformCRUDType _crudplatform = new MRPPlatformCRUDType();
                        _crudplatform.id = _platform.id;
                        _crudplatform.worker_id = Global.agent_id;
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

                    //process dimension data networks
                    foreach (var _platform in _workerplatforms.Where(x => x.vendor == 0 && x.platform_version == "MCP 2.0"))
                    {
                        UpdateMCPPlatform(_platform);
                    }
                    //process dimension data networks
                    foreach (var _platform in _workerplatforms.Where(x => x.vendor == 1))
                    {
                        UpdateVMwarePlatform(_platform);
                    }
                    sw.Stop();

                    Logger.log(
                        String.Format("Completed data mirroring process.{0} new platforms, {1} updated platforms = total elapsed time: {2}",
                        _new_platforms, _updated_platforms,TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)
                        ), Logger.Severity.Info);
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Error in mirror task: {0}", ex.ToString()), Logger.Severity.Error);
                }
                Thread.Sleep(new TimeSpan(1, 0, 0));
            }
        }
 

    }
}
