using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using MRMPService.MRMPAPI.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MRMPService.Utilities;
using VMware.Vim;
using MRMPService.VMWare;
using System.Collections.Specialized;
using MRMPService.MRMPAPI;

namespace MRMPService.PlatformInventory
{
    class PlatformVMwareInventoryDo
    {
        static public void UpdateVMwarePlatform(MRPPlatformType _platform, bool full = true)
        {
            MRMP_ApiClient _cloud_movey = new MRMP_ApiClient();

            Logger.log(String.Format("Started inventory process for {0} : {1}", _platform.platformtype, _platform.moid), Logger.Severity.Info);
            Stopwatch sw = Stopwatch.StartNew();

            //define object lists
            MRPCredentialType _vmware_credential = _platform.credential;

            String username = String.Concat((String.IsNullOrEmpty(_vmware_credential.domain) ? "" : (_vmware_credential.domain + @"\")), _vmware_credential.username);
            VimApiClient _vim = new VimApiClient(_platform.url, username, _vmware_credential.encrypted_password);


            //update localdb platform information
            Datacenter dc = _vim.datacenter().GetDataCenter(_platform.moid);
            List<DistributedVirtualSwitch> networkdomain_list = _vim.networks().GetDVSwitches(dc);
            NameValueCollection filter = new NameValueCollection();
            List<VirtualMachine> _vmware_workload_list = _vim.workload().GetWorkloads(dc, filter).Where(x => x.Runtime.PowerState == VirtualMachinePowerState.poweredOn).ToList();
            List<Network> _vmware_vlan_list = _vim.networks().GetPortGroups(dc).ToList();

            int workloads;
            workloads = 0;

            List<MRPWorkloadType> _mrp_workloads = _cloud_movey.workload().list_by_platform_all(_platform).workloads.ToList();
            List<MRPPlatformdomainType> _mrp_domains = _cloud_movey.platformdomain().list_by_platform(_platform).platformdomains.ToList();
            List<MRPPlatformnetworkType> _mrp_networks = _cloud_movey.platformnetwork().list_by_platform(_platform).platformnetworks.ToList();

            //Process standard port groups aka "VM Networks"
            MRPPlatformdomainType _std_platformdomain = new MRPPlatformdomainType();
            _std_platformdomain.platformnetworks_attributes = new List<MRPPlatformnetworkType>();
            _std_platformdomain.moid = "std_pg";
            _std_platformdomain.domain = "Network";
            _std_platformdomain.platform_id = _platform.id;

            foreach (Network _vmware_network in _vim.networks().GetStandardPgs(dc))
            {
                MRPPlatformnetworkType _platformnetwork = new MRPPlatformnetworkType();

                if (_mrp_networks.Exists(x => x.moid == _vmware_network.MoRef.Value))
                {
                    _platformnetwork = _mrp_networks.FirstOrDefault(x => x.moid == _vmware_network.MoRef.Value);
                }

                _platformnetwork.moid = _vmware_network.MoRef.Value;
                _platformnetwork.network = _vmware_network.Name;
                _platformnetwork.networkdomain_moid = "std_pg";
                _platformnetwork.provisioned = true;

                _std_platformdomain.platformnetworks_attributes.Add(_platformnetwork);

            }
            if (_mrp_domains.Exists(x => x.moid == "std_pg"))
            {
                _std_platformdomain.id = _mrp_domains.FirstOrDefault(x => x.moid == "std_pg").id;
                _cloud_movey.platformdomain().update(_std_platformdomain);
                //_updated_platformnetworks += 1;
            }
            else
            {
                _cloud_movey.platformdomain().create(_std_platformdomain);
                //_new_platformnetworks += 1;
            }

            //Process distributes switches
            foreach (DistributedVirtualSwitch _vmware_domain in _vim.networks().GetDVSwitches(dc))
            {
                MRPPlatformdomainType _platformdomain = new MRPPlatformdomainType();
                _platformdomain.platformnetworks_attributes = new List<MRPPlatformnetworkType>();
                _platformdomain.moid = _vmware_domain.MoRef.Value;
                _platformdomain.domain = _vmware_domain.Name;
                _platformdomain.platform_id = _platform.id;

                foreach (DistributedVirtualPortgroup _vmware_network in _vim.networks().GetDVPortGroups(_vmware_domain))
                {
                    MRPPlatformnetworkType _platformnetwork = new MRPPlatformnetworkType();
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
                    _cloud_movey.platformdomain().update(_platformdomain);
                    //_updated_platformnetworks += 1;
                }
                else
                {
                    _cloud_movey.platformdomain().create(_platformdomain);
                    //_new_platformnetworks += 1;
                }
            }

            if (full)
            {
                //refresh domains and networks from portal
                _mrp_domains = _cloud_movey.platformdomain().list_by_platform(_platform).platformdomains.ToList();
                _mrp_networks = _cloud_movey.platformnetwork().list_by_platform(_platform).platformnetworks.ToList();

                foreach (VirtualMachine _vmware_workload in _vmware_workload_list)
                {
                    PlatformInventoryWorkloadDo.UpdateVMWareWorkload(_vmware_workload.MoRef.Value, _platform, _mrp_workloads, _mrp_domains, _mrp_networks);
                }
            }
            sw.Stop();


        }
    }
}
