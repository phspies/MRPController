using MRMPService.MRMPService.Log;
using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VMware.Vim;
using MRMPService.VMWare;
using System.Collections.Specialized;
using MRMPService.MRMPAPI;
using System.Threading.Tasks;
using System.Threading;

namespace MRMPService.Scheduler.PlatformInventory.VMWare
{
    class PlatformVMwareInventoryDo
    {
        static public void UpdateVMwarePlatform(MRPPlatformType _platform, bool full = true)
        {
            Logger.log(String.Format("Started inventory process for {0} : {1}", _platform.platformtype, _platform.platform), Logger.Severity.Info);
            Stopwatch sw = Stopwatch.StartNew();
            _platform = MRMPServiceBase._mrmp_api.platform().get_by_id(_platform.id);
            MRPCredentialType _vmware_credential = _platform.credential;

            String username = String.Concat(_vmware_credential.username, (String.IsNullOrEmpty(_vmware_credential.domain) ? "" : (@"@" + _vmware_credential.domain)));
            VimApiClient _vim = new VimApiClient(_platform.vmware_url, username, _vmware_credential.decrypted_password);


            if (_platform.platformdatacenter == null)
            {
                throw new Exception("No datacenter selected");
            }

            Datacenter dc = _vim.datacenter().GetDataCenter(_platform.platformdatacenter.moid);

            List<DistributedVirtualSwitch> networkdomain_list = _vim.networks().GetDVSwitches(dc);
            if (networkdomain_list.Count() == 0)
            {
                Logger.log(String.Format("No network domains found in {0}", _platform.platform), Logger.Severity.Warn);
            }
            List<Datastore> datastore_list = _vim.datastore().DatastoreList(dc);
            if (datastore_list.Count() == 0)
            {
                Logger.log(String.Format("No datastores found in {0}", _platform.platform), Logger.Severity.Warn);
            }
            List<ComputeResource> cluster_list = _vim.datacenter().ClusterList(dc);
            if (cluster_list.Count() == 0)
            {
                Logger.log(String.Format("No clusters found in {0}", _platform.platform), Logger.Severity.Warn);
            }
            List<Network> _vmware_vlan_list = _vim.networks().GetPortGroups(dc).ToList();
            if (_vmware_vlan_list.Count() == 0)
            {
                Logger.log(String.Format("No vlans found in {0}", _platform.platform), Logger.Severity.Warn);
            }

            NameValueCollection filter = new NameValueCollection();
            List<VirtualMachine> _vmware_workload_list = _vim.workload().GetWorkloads(dc, filter).Where(x => x.Runtime.PowerState == VirtualMachinePowerState.poweredOn).ToList();
            List<MRPPlatformdomainType> _mrp_domains = _platform.platformdomains;

            MRPPlatformType _update_platform = new MRPPlatformType()
            {
                id = _platform.id,
                vcenter_uuid = _vim.vcenter().GetvCenterAbout().InstanceUuid
            };

            _update_platform.platformdatacenters = new List<MRPPlatformdatacenterType>();
            MRPPlatformdatacenterType _datacenter = new MRPPlatformdatacenterType();
            _datacenter.platformclusters = new List<MRPPlatformclusterType>();
            if (_platform.platformdatacenters.Exists(x => x.moid == dc.MoRef.Value))
            {
                _datacenter.id = _platform.platformdatacenters.FirstOrDefault(x => x.moid == dc.MoRef.Value).id;
            }
            _datacenter.moid = dc.MoRef.Value;

            _update_platform.platformdatacenters.Add(_datacenter);

            foreach (ComputeResource _cluster in cluster_list)
            {
                MRPPlatformclusterType _mrp_cluster = new MRPPlatformclusterType();
                if (_platform.platformdatacenters.FirstOrDefault(x => x.moid == dc.MoRef.Value).platformclusters.Exists(x => x.moid == _cluster.MoRef.Value))
                {
                    _mrp_cluster.id = _platform.platformdatacenters.FirstOrDefault(x => x.moid == dc.MoRef.Value).platformclusters.FirstOrDefault(x => x.moid == _cluster.MoRef.Value).id;
                }
                _mrp_cluster.moid = _cluster.MoRef.Value;
                _mrp_cluster.cluster = _cluster.Name;
                _mrp_cluster.networkcount = _cluster.Network.Count();
                _mrp_cluster.hostcount = _cluster.Summary.NumHosts;
                _mrp_cluster.totalcpu = _cluster.Summary.TotalCpu;
                _mrp_cluster.totalmemory = _cluster.Summary.TotalMemory;
                _mrp_cluster.resourcepool_moid = _cluster.ResourcePool.Value;

                _datacenter.platformclusters.Add(_mrp_cluster);
            }

            _update_platform.platformdatastores = new List<MRPPlatformdatastoreType>();
            foreach (Datastore _datastore in datastore_list.Where(x => x != null))
            {
                MRPPlatformdatastoreType _platform_datastore = new MRPPlatformdatastoreType();
                if (_platform.platformdatastores.Exists(x => x.moid == _datastore.MoRef.Value))
                {
                    _platform_datastore.id = _platform.platformdatastores.FirstOrDefault(x => x.moid == _datastore.MoRef.Value).id;
                }
                _platform_datastore.datastore = _datastore.Name;
                _platform_datastore.moid = _datastore.MoRef.Value;
                _platform_datastore.totalcapacity = _datastore.Summary.Capacity;
                _platform_datastore.freecapacity = _datastore.Summary.FreeSpace;

                _update_platform.platformdatastores.Add(_platform_datastore);
            }

            _update_platform.platformdomains = new List<MRPPlatformdomainType>();

            //Process standard port groups aka "VM Networks"
            MRPPlatformdomainType _std_platformdomain = new MRPPlatformdomainType();
            if (_platform.platformdomains.Exists(x => x.moid == "std_pg"))
            {
                _std_platformdomain.id = _platform.platformdomains.FirstOrDefault(x => x.moid == "std_pg").id;
            }
            _std_platformdomain.platformnetworks = new List<MRPPlatformnetworkType>();
            _std_platformdomain.moid = "std_pg";
            _std_platformdomain.domain = "Network";
            _std_platformdomain.domaintype = "VM";
            _std_platformdomain.platform_id = _platform.id;

            _update_platform.platformdomains.Add(_std_platformdomain);

            foreach (Network _vmware_network in _vim.networks().GetStandardPgs(dc).Where(x => x != null))
            {
                MRPPlatformnetworkType _platformnetwork = new MRPPlatformnetworkType();
                if (_mrp_domains.Exists(x => x.moid == "std_pg"))
                {
                    if (_mrp_domains.FirstOrDefault(x => x.moid == "std_pg").platformnetworks.Exists(y => y.moid == _vmware_network.MoRef.Value))
                    {
                        _platformnetwork.id = _mrp_domains.FirstOrDefault(x => x.moid == "std_pg").platformnetworks.FirstOrDefault(y => y.moid == _vmware_network.MoRef.Value).id;
                    }
                }

                _platformnetwork.moid = _vmware_network.MoRef.Value;
                _platformnetwork.network = _vmware_network.Name;
                _platformnetwork.networkdomain_moid = "std_pg";

                _platformnetwork.provisioned = true;

                _std_platformdomain.platformnetworks.Add(_platformnetwork);
            }

            //Process distributes switches
            foreach (DistributedVirtualSwitch _vmware_domain in _vim.networks().GetDVSwitches(dc).Where(x => x != null))
            {
                MRPPlatformdomainType _platformdomain = new MRPPlatformdomainType();
                _platformdomain.platformnetworks = new List<MRPPlatformnetworkType>();
                if (_mrp_domains.Exists(x => x.moid == _vmware_domain.MoRef.Value))
                {
                    _platformdomain.id = _mrp_domains.FirstOrDefault(x => x.moid == _vmware_domain.MoRef.Value).id;
                }

                _platformdomain.moid = _vmware_domain.MoRef.Value;
                _platformdomain.domain = _vmware_domain.Name;
                _platformdomain.domaintype = "DVS";
                _platformdomain.platform_id = _platform.id;

                _update_platform.platformdomains.Add(_platformdomain);

                foreach (DistributedVirtualPortgroup _vmware_network in _vim.networks().GetDVPortGroups(_vmware_domain).Where(x => x != null))
                {
                    if (!_vmware_network.Tag.Any(x => x.Key.Contains("UPLINK")))
                    {
                        MRPPlatformnetworkType _platformnetwork = new MRPPlatformnetworkType();
                        if (_mrp_domains.Exists(x => x.moid == _vmware_domain.MoRef.Value))
                        {
                            if (_mrp_domains.FirstOrDefault(x => x.moid == _vmware_domain.MoRef.Value).platformnetworks.Any(x => x.moid == _vmware_network.MoRef.Value))
                            {
                                _platformnetwork.id = _mrp_domains.FirstOrDefault(x => x.moid == _vmware_domain.MoRef.Value).platformnetworks.FirstOrDefault(x => x.moid == _vmware_network.MoRef.Value).id;
                            }
                        }
                        _platformnetwork.moid = _vmware_network.MoRef.Value;
                        _platformnetwork.network = _vmware_network.Name;
                        _platformnetwork.platformdomain_id = _platformdomain.id;
                        _platformnetwork.networkdomain_moid = _vmware_domain.MoRef.Value;
                        _platformnetwork.provisioned = true;

                        _platformdomain.platformnetworks.Add(_platformnetwork);
                    }
                }
            }
            MRMPServiceBase._mrmp_api.platform().update(_update_platform);

            MRPPlatformType _refreshed_platfrom = new MRPPlatformType();

            _refreshed_platfrom = MRMPServiceBase._mrmp_api.platform().get_by_id(_platform.id);
            List<MRMPWorkloadBaseType> _mrp_workloads = new List<MRMPWorkloadBaseType>();

            MRPWorkloadListType _paged_workload = MRMPServiceBase._mrmp_api.workload().list_paged_filtered_brief(new MRPWorkloadFilterPagedType() { platform_id = _platform.id, page = 1 });
            while (_paged_workload.pagination.page_size > 0)
            {
                _mrp_workloads.AddRange(_paged_workload.workloads);
                if (_paged_workload.pagination.next_page > 0)
                {
                    _paged_workload = MRMPServiceBase._mrmp_api.workload().list_paged_filtered_brief(new MRPWorkloadFilterPagedType() { platform_id = _platform.id, page = _paged_workload.pagination.next_page });
                }
                else
                {
                    break;
                }
            }

            //update deleted workloads
            _update_platform = new MRPPlatformType() { id = _platform.id, workloads = new List<MRMPWorkloadBaseType>() };
            foreach (var _workload in _mrp_workloads.Where(x => x.workloadtype != "manager"))
            {
                MRMPWorkloadBaseType _mrp_workload = new MRMPWorkloadBaseType() { id = _workload.id };
                if (!_vmware_workload_list.Any(x => x.MoRef.Value == _workload.moid))
                {
                    _mrp_workload.deleted = true;
                    _mrp_workload.enabled = false;
                }
                else
                {
                    _mrp_workload.deleted = false;
                }
                _update_platform.workloads.Add(_mrp_workload);
            }

            MRMPServiceBase._mrmp_api.platform().update(_update_platform);

            if (full)
            {
                _refreshed_platfrom = MRMPServiceBase._mrmp_api.platform().get_by_id(_platform.id);
                int _multiplyer = (_vmware_workload_list.Count() > 75) ? (_vmware_workload_list.Count()) / 75 : 1;
                int _concurrency = (MRMPServiceBase.platform_workload_inventory_concurrency * _multiplyer);

                List<Thread> lstThreads = new List<Thread>();
                Logger.log(String.Format("VMWare: Starting inventory collection process with {0} threads", _concurrency), Logger.Severity.Debug);

                foreach (var _vmware_workload in _vmware_workload_list)
                {
                    while (lstThreads.Count(x => x.IsAlive) >= _concurrency)
                    {
                        Thread.Sleep(500);
                    }
                    Thread _inventory_thread = new Thread(() =>
                    {
                        try
                        {
                            PlatformInventoryWorkloadDo.UpdateVMWareWorkload(_vmware_workload.MoRef.Value, _refreshed_platfrom, _mrp_workloads);
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Error collecting inventory information from VMware workload {0} with error {1}", _vmware_workload.Config.GuestFullName, ex.GetBaseException().Message), Logger.Severity.Error);
                        }
                    });
                    _inventory_thread.Name = _vmware_workload.MoRef.Value;
                    lstThreads.Add(_inventory_thread);
                    _inventory_thread.Start();
                    Logger.log(String.Format("VMWare Workload Inventory Thread Count [active: {0}] [total: {1}] [complete {2}]", lstThreads.Count(x => x.IsAlive), lstThreads.Count(), lstThreads.Count(x => !x.IsAlive)), Logger.Severity.Info);
                }
            }
            Logger.log(String.Format("Completed inventory process for {0} : {1}", _platform.platformtype, _platform.platform), Logger.Severity.Info);

        }
    }
}
