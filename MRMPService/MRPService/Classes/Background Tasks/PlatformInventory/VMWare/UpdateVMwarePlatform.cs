using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using MRMPService.API.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MRMPService.Utilities;
using VMware.Vim;
using MRMPService.VMWare;
using System.Collections.Specialized;
using MRMPService.API;

namespace MRMPService.PlatformInventory
{
    class PlatformVMwareInventoryDo
    {
        static public void UpdateVMwarePlatform(String _platform_id, bool full = true)
        {
            MRP_ApiClient _cloud_movey = new MRP_ApiClient();

            Platform _platform;
            using (PlatformSet _platform_db = new PlatformSet())
            {
                _platform = _platform_db.ModelRepository.GetById(_platform_id);
            }

            Logger.log(String.Format("Started inventory process for {0} : {1}", _platform.human_vendor, _platform.datacenter), Logger.Severity.Info);
            Stopwatch sw = Stopwatch.StartNew();

            //define object lists
            Credential _credential;
            using (MRPDatabase db = new MRPDatabase())
            {
                List<Credential> _workercredentials = db.Credentials.ToList();
                _credential = _workercredentials.FirstOrDefault(x => x.id == _platform.credential_id);

            }
            String username = String.Concat((String.IsNullOrEmpty(_credential.domain) ? "" : (_credential.domain + @"\")), _credential.username);
            VimApiClient _vim = new VimApiClient(_platform.url, username, _credential.password);


            //update localdb platform information
            Datacenter dc = _vim.datacenter().GetDataCenter(_platform.moid);
            List<DistributedVirtualSwitch> networkdomain_list = _vim.networks().GetDVSwitches(dc);
            NameValueCollection filter = new NameValueCollection();
            List<VirtualMachine> _vmware_workload_list = _vim.workload().GetWorkloads(dc, filter).Where(x => x.Config.GuestId.Contains("win") && x.Runtime.PowerState == VirtualMachinePowerState.poweredOn).ToList();
            List<Network> _vmeare_vlan_list = _vim.networks().GetPortGroups(dc).ToList();

            int workloads, networkdomains, vlans;
            string workloads_md5, networkdomains_md5, vlans_md5;
            workloads = networkdomains = vlans = 0;

            workloads = _vmware_workload_list.Count();
            workloads_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(_vmware_workload_list));
            vlans = _vmeare_vlan_list.Count();
            vlans_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(_vmeare_vlan_list));
            networkdomains = networkdomain_list.Count();
            networkdomains_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(networkdomain_list));

            using (MRPDatabase db = new MRPDatabase())
            {
                Platform __platform = db.Platforms.Find(_platform.id);
                __platform.vlan_count = vlans;
                __platform.workload_count = workloads;
                __platform.networkdomain_count = networkdomains;
                __platform.platform_version = "na";

                __platform.lastupdated = DateTime.UtcNow;
                __platform.human_vendor = (new Vendors()).VendorList.First(x => x.ID == _platform.vendor).Vendor;
                __platform.moid = dc.MoRef.Value;
                db.SaveChanges();
            }

            List<MRPWorkloadType> _mrp_workloads = _cloud_movey.workload().listworkloads().workloads.Where(x => x.platform_id == _platform_id).ToList();
            List<MRPPlatformdomainType> _mrp_domains = _cloud_movey.platformdomain().listplatformdomains().platformdomains.Where(x => x.platform_id == _platform_id).ToList();
            List<MRPPlatformnetworkType> _mrp_networks = _cloud_movey.platformnetwork().listplatformnetworks().platformnetworks.Where(x => _mrp_domains.Exists(y => y.id == x.platformdomain_id)).ToList();

            //Process standard port groups aka "VM Networks"
            MRPPlatformdomainCRUDType _std_platformdomain = new MRPPlatformdomainCRUDType();
            _std_platformdomain.platformnetworks_attributes = new List<MRPPlatformnetworkCRUDType>();
            _std_platformdomain.moid = "std_pg";
            _std_platformdomain.domain = "Network";
            _std_platformdomain.platform_id = _platform.id;

            foreach (Network _vmware_network in _vim.networks().GetStandardPgs(dc))
            {
                MRPPlatformnetworkCRUDType _platformnetwork = new MRPPlatformnetworkCRUDType();
                _platformnetwork.moid = _vmware_network.MoRef.Value;
                _platformnetwork.network = _vmware_network.Name;
                _platformnetwork.networkdomain_moid = "std_pg";
                _platformnetwork.provisioned = true;
                if (_mrp_networks.Exists(x => x.moid == _vmware_network.MoRef.Value))
                {
                    _platformnetwork.id = _mrp_networks.FirstOrDefault(x => x.moid == _vmware_network.MoRef.Value).id;
                    //_updated_platformnetworks += 1;
                }
                else
                {
                    //_new_platformnetworks += 1;
                }
                _std_platformdomain.platformnetworks_attributes.Add(_platformnetwork);
            }
            if (_mrp_domains.Exists(x => x.moid == "std_pg"))
            {
                _std_platformdomain.id = _mrp_domains.FirstOrDefault(x => x.moid == "std_pg").id;
                _cloud_movey.platformdomain().updateplatformdomain(_std_platformdomain);
                //_updated_platformnetworks += 1;
            }
            else
            {
                _cloud_movey.platformdomain().createplatformdomain(_std_platformdomain);
                //_new_platformnetworks += 1;
            }

            //Process distributes switches
            foreach (DistributedVirtualSwitch _vmware_domain in _vim.networks().GetDVSwitches(dc))
            {
                MRPPlatformdomainCRUDType _platformdomain = new MRPPlatformdomainCRUDType();
                _platformdomain.platformnetworks_attributes = new List<MRPPlatformnetworkCRUDType>();
                _platformdomain.moid = _vmware_domain.MoRef.Value;
                _platformdomain.domain = _vmware_domain.Name;
                _platformdomain.platform_id = _platform.id;

                foreach (DistributedVirtualPortgroup _vmware_network in _vim.networks().GetDVPortGroups(_vmware_domain))
                {
                    MRPPlatformnetworkCRUDType _platformnetwork = new MRPPlatformnetworkCRUDType();
                    _platformnetwork.moid = _vmware_network.MoRef.Value;
                    _platformnetwork.network = _vmware_network.Name;
                    _platformnetwork.platformdomain_id = _platformdomain.id;
                    _platformnetwork.networkdomain_moid = _vmware_domain.MoRef.Value;
                    _platformnetwork.provisioned = true;
                    if (_mrp_networks.Exists(x => x.moid == _vmware_network.MoRef.Value))
                    {
                        _platformnetwork.id = _mrp_networks.FirstOrDefault(x => x.moid == _vmware_network.MoRef.Value).id;
                        //_updated_platformnetworks += 1;
                    }
                    else
                    {
                        //_new_platformnetworks += 1;
                    }
                    _platformdomain.platformnetworks_attributes.Add(_platformnetwork);
                }
                if (_mrp_domains.Exists(x => x.moid == _vmware_domain.MoRef.Value))
                {
                    _platformdomain.id = _mrp_domains.FirstOrDefault(x => x.moid == _vmware_domain.MoRef.Value).id;
                    _cloud_movey.platformdomain().updateplatformdomain(_platformdomain);
                    //_updated_platformnetworks += 1;
                }
                else
                {
                    _cloud_movey.platformdomain().createplatformdomain(_platformdomain);
                    //_new_platformnetworks += 1;
                }
            }

            //process deleted platform workloads
            using (MRPDatabase db = new MRPDatabase())
            {
                foreach (var _workload in db.Workloads.Where(x => x.platform_id == _platform.id))
                {
                    if (!_vmware_workload_list.Any(x => x.MoRef.Value == _workload.moid))
                    {
                        db.Workloads.Remove(db.Workloads.Find(_workload.id));
                        db.SaveChanges();
                        //_removed_workloads += 1;
                    }
                }
            }
            if (full)
            {
                foreach (VirtualMachine _vmware_workload in _vmware_workload_list)
                {
                    PlatformInventoryWorkloadDo.UpdateVMWareWorkload(_vmware_workload.MoRef.Value, _platform.id);
                }
            }
            sw.Stop();


        }
    }
}
