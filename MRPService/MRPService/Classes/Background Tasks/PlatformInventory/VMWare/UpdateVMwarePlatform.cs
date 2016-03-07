using MRPService.MRPService.Log;
using MRPService.LocalDatabase;
using MRPService.API.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MRPService.Utilities;
using VMware.Vim;
using MRPService.VMWare;
using System.Collections.Specialized;
using MRPService.API;

namespace MRPService.PlatformInventory
{
    class PlatformVMwareInventoryDo
    {
        static public void UpdateVMwarePlatform(String _platform_id, bool full = true)
        {
            ApiClient _cloud_movey = new ApiClient();

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
            List<Network> networkdomain_list = _vim.networks().GetPortGroups(dc);
            NameValueCollection filter = new NameValueCollection();
            List<VirtualMachine> workload_list = _vim.workload().GetWorkloads(dc, filter).Where(x => x.Config.GuestId.Contains("win") && x.Runtime.PowerState == VirtualMachinePowerState.poweredOn).ToList();
            List<Network> vlan_list = _vim.networks().GetPortGroups(dc).ToList();

            int workloads, networkdomains, vlans;
            string workloads_md5, networkdomains_md5, vlans_md5;
            workloads = networkdomains = vlans = 0;

            workloads = workload_list.Count();
            workloads_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(workload_list));
            vlans = vlan_list.Count();
            vlans_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(vlan_list));
            networkdomains = networkdomain_list.Count();
            networkdomains_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(networkdomain_list));

            using (MRPDatabase db = new MRPDatabase())
            {
                Platform __platform = db.Platforms.Find(_platform.id);
                __platform.vlan_count = vlans;
                __platform.workload_count = workloads;
                __platform.networkdomain_count = networkdomains;
                __platform.platform_version = "na";

                __platform.lastupdated = DateTime.Now;
                __platform.human_vendor = (new Vendors()).VendorList.First(x => x.ID == _platform.vendor).Vendor;
                __platform.moid = dc.MoRef.Value;
                db.SaveChanges();
            }

            MRPWorkloadListType _currentplatformworkloads = _cloud_movey.workload().listworkloads();
            MRPPlatformnetworkListType _currentplatformnetworks = _cloud_movey.platformnetwork().listplatformnetworks();
            MRPPlatformdomainListType _currentplatformdomains = _cloud_movey.platformdomain().listplatformdomains();


            //Console.WriteLine(String.Format("DC: {0} {1}", dc.Name, dc.MoRef.Value));
            //Console.WriteLine("\n\tDatastores -----------------\n");
            //foreach (Datastore ds in _vim.datastore().DatastoreList(dc))
            //{
            //    Console.WriteLine(String.Format("\t\tDS: {0} {1}", ds.Name, ds.MoRef.Value));
            //}
            //Console.WriteLine("\n\tVirtual Machines -----------------\n");
            //foreach (VirtualMachine vm in _vim.workload().GetWorkloads(dc))
            //{
            //    Console.WriteLine(String.Format("\t\t{0} {1}", vm.Name, vm.MoRef.Value));
            //    vm.Config.GuestId
            //        }
            //Console.WriteLine("\n\tNetworks -----------------\n");
            //foreach (Network net in _vim.networks().GetPortGroups(dc))
            //{
            //    Console.WriteLine(String.Format("\t\t{0} {1}", net.Name, net.MoRef.Value));
            //}

        }
    }
}
