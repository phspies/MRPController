using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MRMPService.MRMPAPI;
using MRMPService.RP4VMAPI;
using MRMPService.RP4VMTypes;

namespace MRMPService.PlatformInventory
{
    class PlatformRP4VMInventoryDo : IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PlatformRP4VMInventoryDo()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        public static void UpdateRP4VMPlatform(MRPPlatformType _platform, bool full = true)
        {
            using (MRMP_ApiClient _mrp_api_endpoint = new MRMP_ApiClient())
            {
                Logger.log(String.Format("Started inventory process for {0} : {1}", _platform.platformtype, _platform.moid), Logger.Severity.Info);
                Stopwatch sw = Stopwatch.StartNew();
                MRPPlatformdatacenterListType _mrmp_datacenters = null;
                MRPPlatformType _parent_platform = _mrp_api_endpoint.platform().get_by_id(_platform.parent_platform_id);


                RP4VM_ApiClient _rp4vm;
                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                {
                    SystemSettings _rp4vm_settings = null;
                    try
                    {
                        _mrmp_datacenters = _mrp_api.platformdatacenter().list(_platform);
                        String username = String.Concat((String.IsNullOrEmpty(_platform.credential.domain) ? "" : (_platform.credential.domain + @"\")), _platform.credential.username);
                        _rp4vm = new RP4VM_ApiClient(_platform.rp4vm_url, username, _platform.credential.encrypted_password);
                        _rp4vm_settings = _rp4vm.system().getSystemSettings_Method();
                    }
                    catch (Exception ex)
                    {
                        Logger.log(String.Format("Error inventory platform {0} : {1}", _platform.platform, ex.GetBaseException().Message), Logger.Severity.Error);
                    }
                    if (_rp4vm_settings != null)
                    {
                        MRPPlatformType _update_platform = new MRPPlatformType() { id = _parent_platform.id };
                        foreach (ArraySettings _array_settings in _rp4vm_settings.clustersSettings.SelectMany(a => a.ampsSettings).SelectMany(x => x.managedArrays).Where(x => x.serialNumber == _parent_platform.vcenter_uuid))
                        {
                            foreach (MRPPlatformdatastoreType _datastore in _parent_platform.platformdatastores_attributes)
                            {
                                if (_array_settings.resourcePools.Exists(x => x.resourcePoolUID.storageResourcePoolId == _datastore.moid))
                                {
                                    var _resource_pool = _array_settings.resourcePools.FirstOrDefault(x => x.resourcePoolUID.storageResourcePoolId == _datastore.moid);
                                    if (_update_platform.platformdatastores_attributes == null)
                                    {
                                        _update_platform.platformdatastores_attributes = new List<MRPPlatformdatastoreType>();
                                    }
                                    _update_platform.platformdatastores_attributes.Add(new MRPPlatformdatastoreType()
                                    {
                                        id = _datastore.id,
                                        rp4vm_arrayid = _array_settings.arrayUID.id,
                                        rp4vm_clusterid = _array_settings.arrayUID.clusterUID.id,
                                        rp4vm_resourcepoolid = _resource_pool.resourcePoolUID.uuid
                                    });
                                }
                            }
                        }

                        _mrp_api.platform().update(_update_platform);

                    }
                    else
                    {
                    }

                    sw.Stop();

                    Logger.log(
                        String.Format("Completed inventory process for {1} = Total Execute Time: {0}", TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), (_platform.platformtype + " : " + _platform.moid)
                        ), Logger.Severity.Info);
                }
            }

        }
    }
}