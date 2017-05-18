﻿using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;

using MRMPService.Scheduler.PlatformInventory.DimensionData;
using MRMPService.Scheduler.PlatformInventory.RP4VM;
using MRMPService.Scheduler.PlatformInventory.VMWare;
using System;

namespace MRMPService.TaskExecutioner.Platform
{
    public class PlatformDiscovery
    {
        public static void PlatformDiscoveryDo(MRPTaskType payload)
        {

            MRMPServiceBase._mrmp_api.task().progress(payload, String.Format("Starting platform discovering process"), 5);

            MRPPlatformType _platform = payload.taskdetail.target_platform;
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
                MRMPServiceBase._mrmp_api.task().progress(payload, String.Format("Successfully refreshed platform resources"), 10);
                MRMPServiceBase._mrmp_api.task().successcomplete(payload);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error in inventory process for {0} {1}", (_platform.platformtype + " : " + _platform.moid), ex.ToString()), Logger.Severity.Error);
                MRMPServiceBase._mrmp_api.task().failcomplete(payload, String.Format("Error refreshing platform resources: {0}", ex.GetBaseException().Message));
            }
        }
    }

}
