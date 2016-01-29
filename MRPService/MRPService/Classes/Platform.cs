using MRPService.CaaS;
using MRPService.Portal;
using MRPService.Portal.Types.API;
using Newtonsoft.Json;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;

namespace MRPService.CloudMRP.Controllers
{
    class CloudMRPPlatform
    {
        public static void mcp_provisionvm(MRPTaskType payload)
        {
            MRPTaskPayloadType _payload = payload.submitpayload;
            CloudMRPPortal CloudMRP = new CloudMRPPortal();
            DimensionData CaaS = new DimensionData((string)_payload.platform.mcpendpoint.url, (string)_payload.platform.username, (string)_payload.platform.password, null);
            try
            {
                MRPWorkloadType _target = _payload.mcp.target;

                String _ostype = String.Format("{0} {1}", _target.ostype, _target.osedition);
                String mcp_template_name = mcp_getimage_name(_ostype);
                List<Option> _options = new List<Option>();
                _options.Add(new Option() { option = "operatingSystemId", value = mcp_template_name });
                _options.Add(new Option() { option = "operatingSystemFamily", value = "WINDOWS" });
                _options.Add(new Option() { option = "location", value = _payload.platform.moid });
                _options.Add(new Option() { option = "state", value = "NORMAL" });

                ServerType _platformimage = new ServerType();
                if (_platformimage == null)
                {
                    CloudMRP.task().failcomplete(payload, "Template image not found: " + JsonConvert.SerializeObject(_options));
                    return;
                }
                DeployServerType _vm = new DeployServerType();

                List<DeployServerTypeDisk> _disks = new List<DeployServerTypeDisk>();
               
                foreach (MRPWorkloadDiskType volume in _target.disks)
                {
                    if (_platformimage.disk.Exists(x => x.scsiId == volume.diskindex))
                    {
                        _disks.Add(new DeployServerTypeDisk() { scsiId = (byte)volume.diskindex, speed = volume.platformstoragetier.shortname });
                    }
                }

                string moid = _payload.platform.moid;
                List<Option> _dcoptions = new List<Option>();
                _dcoptions.Add(new Option() { option = "id", value = moid });
                DatacenterType dc = (CaaS.datacenter().datacenters(_dcoptions) as DatacenterListType).datacenter[0];

                _vm.name = _target.hostname;
                _vm.description = String.Format("Workload Created by CloudMRP [{0}]", DateTime.Now);
                _vm.networkInfo.primaryNic = new VlanIdOrPrivateIpType() { ItemElementName = ItemChoiceType1.privateIpv4, Item = _target.interfaces[0].platformnetwork.moid };
                _vm.imageId = _platformimage.id;
                _vm.start = false;
                _vm.disk = _disks;
                _vm.administratorPassword = (string)_target.password;

                ResponseType _status = CaaS.workload().deploy(_vm);
                if (_status.responseCode == "REASON_0")
                {
                    String _vm_id = _status.info[0].value;
                    CloudMRP.task().progress(payload, String.Format("{0} provisioning started in {1}({2})", _vm.name, dc.displayName, dc.id), 20);
                    List<Option> _vmoptions = new List<Option>();
                    ServerType _newvm = CaaS.workload().get(_vm_id);
                    while (_newvm.state != "NORMAL" && _newvm.started == false)
                    {
                        if (_newvm.progress != null)
                        {
                            if (_newvm.progress.step != null)
                            {
                                CloudMRP.task().progress(payload, String.Format("Provisioning step: {0}", _newvm.progress.step.name), 30 + _newvm.progress.step.number);
                            }
                        }
                        _newvm = CaaS.workload().get(_vm_id);
                        Thread.Sleep(5000);
                    }

                    //Update CPU and Memory for workload
                    ReconfigureServerType _reconfiure = new ReconfigureServerType();
                    CaaS.workload().reconfigure(new ReconfigureServerType() { id = _vm_id, cpuCount = (uint)_target.vcpu, cpuCountSpecified = true, memoryGb = (uint)_target.vmemory, memoryGbSpecified = true });
                    _newvm = CaaS.workload().get(_vm_id);
                    CloudMRP.task().progress(payload, String.Format("Updating CPU and Memory: {0} : {1}", _target.vcpu, _target.vmemory), 60);
                    while (_newvm.state != "NORMAL" && _newvm.started == false)
                    {
                        _newvm = CaaS.workload().get(_vm_id);
                        Thread.Sleep(5000);
                    }

                    //Expand C: drive and Add additional disks if required
                    int count = 0;
                    foreach (MRPWorkloadDiskType _volume in _target.disks)
                    {
                        if (_newvm.disk.ToList().Exists(x => x.scsiId == _volume.diskindex))
                        {
                            if (_newvm.disk.ToList().Find(x => x.scsiId == _volume.diskindex).sizeGb < _volume.disksize)
                            {
                                CloudMRP.task().progress(payload, String.Format("Extending storage: {0} : {1}GB", _volume.diskindex, _volume.disksize), 60 + count);
                                //CaaS.workload().workloaddiskexpand(_vm_id, _volume.diskindex, (byte)_volume.disksize);
                            }
                        }
                        else
                        {
                            CloudMRP.task().progress(payload, String.Format("Adding storage: {0} : {1}GB on {2}", _volume.diskindex, _volume.disksize, _volume.platformstoragetier.storagetier), 60 + count);
                            //CaaS.workloadimage().workloadaddstorage(_vm_id, _volume.disksize, _volume.platformstoragetier.shortname);
                        }
                        _newvm = CaaS.workload().get(_vm_id);
                        while (_newvm.state != "NORMAL" && _newvm.started == false)
                        {
                            _newvm = CaaS.workload().get(_vm_id);
                            Thread.Sleep(5000);
                        }
                        count += 1;
                    }

                    //Start Workload
                    CloudMRP.task().progress(payload, String.Format("Power on workload"), 60 + count + 1);
                    CaaS.workload().start(new StartServerType() { id = _vm_id });
                    _newvm = CaaS.workload().get(_vm_id);
                    while (_newvm.state != "NORMAL" && _newvm.started == false)
                    {
                        _newvm = CaaS.workload().get(_vm_id);
                        Thread.Sleep(5000);
                    }

                    CloudMRP.task().progress(payload, String.Format("Workload powered on"), 60 + count + 2);
                    _newvm = CaaS.workload().get(_vm_id);

                    //create diskpart file for virtual machine and copy it to the remote workload
                    List<String> _driveletters = new List<String>();
                    List<String> _systemdriveletters = new List<String>();
                    _systemdriveletters.AddRange("DEFGHIJKLMNOPQRSTUVWXYZ".Select(d => d.ToString()));
                    List <String> _diskpart_struct = new List<string>();
                    _diskpart_struct.Add("select volume c");
                    _diskpart_struct.Add("extend noerr");
                    _diskpart_struct.Add("");
                    List<String> _availabledriveletters = _systemdriveletters.Except(_driveletters).ToList<String>();
                    _diskpart_struct.Add("select volume d");
                    _diskpart_struct.Add(String.Format("assign letter={0} noerr", _availabledriveletters.Last()));
                    _diskpart_struct.Add("");
                    foreach (ServerTypeDisk _disk in _newvm.disk.ToList().FindAll(x => x.scsiId != 0).OrderBy(x => x.scsiId))
                    {
                        string _driveletter = _target.volumes.Find(x => x.diskindex == _disk.scsiId).driveletter.Substring(0, 1);
                        _driveletters.Add(_driveletter.ToString());
                        _diskpart_struct.Add(String.Format("select disk {0}", _disk.scsiId));
                        _diskpart_struct.Add("ATTRIBUTES DISK CLEAR READONLY");
                        _diskpart_struct.Add("ONLINE DISK");
                        _diskpart_struct.Add("clean");
                        _diskpart_struct.Add("create partition primary");
                        _diskpart_struct.Add("select partition 1");
                        _diskpart_struct.Add("format fs=ntfs quick");
                        _diskpart_struct.Add(String.Format("assign letter={0} noerr", _driveletter));
                        _diskpart_struct.Add("active");
                        _diskpart_struct.Add("");
                    }

                    using (Impersonation.LogonUser(".", "Administrator", _target.password, LogonType.Batch))
                    {
                        try
                        {
                            string remoteInstallFiles = @"C:\";
                            remoteInstallFiles = remoteInstallFiles.Replace(':', '$');
                            string workloadPath = Path.Combine(_newvm.networkInfo.primaryNic.privateIpv4, remoteInstallFiles);
                            workloadPath = @"\\" + workloadPath + @"\diskpart.txt";
                            File.WriteAllLines(workloadPath, _diskpart_struct.ConvertAll(Convert.ToString));
                        }
                        catch (Exception e)
                        {
                            CloudMRP.task().failcomplete(payload, String.Format("Error creating disk layout file on workload: {0}", e.Message)); 
                        }
                    }

                    //Run Diskpart Command on Workload
                    //Create connection object to remote machine
                    CloudMRP.task().progress(payload, String.Format("Volume setup process on {0}", _newvm.name), 80);

                    ConnectionOptions connOptions = new ConnectionOptions() { EnablePrivileges = true, Username = "Administrator", Password = _target.password };
                    connOptions.Impersonation = ImpersonationLevel.Impersonate;
                    connOptions.Authentication = AuthenticationLevel.Default;
                    ManagementScope scope = new ManagementScope(@"\\" + _newvm.networkInfo.primaryNic.privateIpv4 + @"\root\CIMV2", connOptions);
                    scope.Connect();

                    string diskpartCmd = @"diskpart /s C:\diskpart.txt";
                    Dictionary<string, string> installCmdParams = new Dictionary<string, string>();
                    installCmdParams["CommandLine"] = diskpartCmd;
                    installCmdParams["CurrentDirectory"] = @"C:\Windows";

                    Dictionary<string, object> returnValues = new Dictionary<string, object>();
                    ManagementPath wmiObjectPath = new ManagementPath("Win32_Process");
                    ObjectGetOptions ogo = new ObjectGetOptions();
                    ManagementBaseObject returnValue;
                    int processId = 0;
                    using (ManagementClass mc = new ManagementClass(scope, wmiObjectPath, ogo))
                    {
                        ManagementBaseObject inparams = mc.GetMethodParameters("Create");
                        if (installCmdParams != null)
                        {
                            foreach (var p in installCmdParams)
                            {
                                inparams[p.Key] = p.Value;
                            }
                        }
                        returnValue = mc.InvokeMethod("Create", inparams, null);
                    }

                    if (returnValues != null)
                    {
                        processId = Convert.ToInt32(returnValue.Properties["ProcessId"].Value);
                    }
                    int _exitcode = Convert.ToInt32(returnValue.Properties["ReturnValue"].Value);
                    if (_exitcode != 0)
                    {
                        CloudMRP.task().failcomplete(payload, String.Format("Failed diskpart process on {0} ({1})", _newvm.name, _exitcode));
                        return;
                    }
                    else
                    {
                        CloudMRP.task().progress(payload, String.Format("Volume setup process exit code: {0}", _exitcode), 81);
                    }

                    CloudMRP.task().successcomplete(payload, JsonConvert.SerializeObject(_newvm));
                }
                else
                {
                    CloudMRP.task().failcomplete(payload, String.Format("Failed to create target virtual machine: {0}",_status.error));
                }
            }
            catch (Exception e)
            {
                CloudMRP.task().failcomplete(payload, e.ToString());
            }
        }
        public static string mcp_getimage_name(String _ostype)
        {
            String _template = "WIN";
            bool X64 = false;
            if (_ostype.Contains("2012"))
            {
                _template = _template + "2012";
                if (_ostype.Contains("R2"))
                {
                    _template = _template + "R2";
                    X64 = true;
                }
            }
            else if (_ostype.Contains("2008"))
            {
                _template = _template + "2008";
                if (_ostype.Contains("R2"))
                {
                    _template = _template + "R2";
                    X64 = true;
                }
            }
            if (_ostype.Contains("Standard"))
            {
                _template = _template + "S";
                X64 = true;

            }
            else if (_ostype.Contains("Enterprise"))
            {
                _template = _template + "E";
                X64 = true;

            }
            else if (_ostype.Contains("Datacenter"))
            {
                _template = _template + "DC";
                X64 = true;

            }
            switch (X64)
            {
                case true:
                    _template = _template + "64";
                    break;
                case false:
                    _template = _template + "32";
                    break;
            }
            return _template;
        }
        public static void mcp_getdatacenters(dynamic payload)
        {
            CloudMRPPortal CloudMRP = new CloudMRPPortal();
            MRPTask tasks = new MRPTask(CloudMRP);
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.username, (string)payload.payload.password, null);
            try
            {
                CloudMRP.task().progress(payload, "MCP Data Gathering", 50);
                CloudMRP.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.datacenter().datacenters()));
            }
            catch (Exception e)
            {
                CloudMRP.task().failcomplete(payload, e.ToString());
            }
        }
        public static void mcp_gettemplates(dynamic payload)
        {
            CloudMRPPortal CloudMRP = new CloudMRPPortal();
            MRPTask tasks = new MRPTask(CloudMRP);
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.username, (string)payload.payload.password, null);
            try
            {
                CloudMRP.task().progress(payload, "MCP Data Gathering", 50);
                List<Option> filter = new List<Option>();
                filter.Add(new Option() { option = "location", value = payload.payload.locationId });
                filter.Add(new Option() { option = "operatingSystemFamily", value = "WINDOWS" });
                filter.Add(new Option() { option = "state", value = "NORMAL" });
                CloudMRP.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.templates().customertemplates(filter)));
            }
            catch (Exception e)
            {
                CloudMRP.task().failcomplete(payload, e.ToString());
            }
        }
        public static void mcp_retrieveworkloads(dynamic payload)
        {
            CloudMRPPortal CloudMRP = new CloudMRPPortal();
            MRPTask tasks = new MRPTask(CloudMRP);
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.platform.username, (string)payload.payload.platform.password, null);
            try
            {
                CloudMRP.task().progress(payload, "MCP Data Gathering", 50);
                string moid = payload.payload.platform.moid;
                List<Option> options = new List<Option>();
                options.Add(new Option() { option = "id", value = moid });
                DatacenterType dc = (CaaS.datacenter().datacenters(options) as DatacenterListType).datacenter[0];
                List<Option> _options = new List<Option>() { new Option() { option = "datacenterId", value = moid } };
                CloudMRP.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.workloads().list(_options)));
            }
            catch (Exception e)
            {
                CloudMRP.task().failcomplete(payload, e.ToString());
            }
        }
        public static void mcp_retrievenetworks(dynamic payload)
        {
            CloudMRPPortal CloudMRP = new CloudMRPPortal();
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.platform.username, (string)payload.payload.platform.password, null);
            try
            {
                CloudMRP.task().progress(payload, "MCP Data Gathering", 50);
                string moid = payload.payload.platform.moid;
                List<Option> options = new List<Option>();
                options.Add(new Option() { option = "id", value = moid });
                DatacenterType dc = (CaaS.datacenter().datacenters(options) as DatacenterListType).datacenter[0];
                List<Option> _options = new List<Option>() { new Option() {option = "datacenterId", value = moid}};
                CloudMRP.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.vlans().list(_options)));
                
            }
            catch (Exception e)
            {
                CloudMRP.task().failcomplete(payload, e.ToString());
            }
        }
    }
}
