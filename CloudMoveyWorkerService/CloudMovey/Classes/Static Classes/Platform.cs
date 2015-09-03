using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.CaaS.Models;
using CloudMoveyWorkerService.CloudMovey.Types;
using Newtonsoft.Json;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;

namespace CloudMoveyWorkerService.CloudMovey.Controllers
{
    class Platform
    {
        public static void mcp_provisionvm(MoveyTaskType payload)
        {
            MoveyTaskPayloadType _payload = payload.submitpayload;
            CloudMovey CloudMovey = new CloudMovey();
            DimensionData CaaS = new DimensionData((string)_payload.platform.mcpendpoint.url, (string)_payload.platform.username, (string)_payload.platform.password, null);
            try
            {
                MoveyWorkloadType _target = _payload.mcp.target;

                String _ostype = String.Format("{0} {1}", _target.ostype, _target.osedition);
                String mcp_template_name = mcp_getimage_name(_ostype);
                List<Option> _options = new List<Option>();
                _options.Add(new Option() { option = "operatingSystemId", value = mcp_template_name });
                _options.Add(new Option() { option = "operatingSystemFamily", value = "WINDOWS" });
                _options.Add(new Option() { option = "location", value = _payload.platform.moid });
                _options.Add(new Option() { option = "state", value = "NORMAL" });

                ImagesWithDiskSpeedImage _platformimage = CaaS.serverimage().platformserverimages(_options).image.Find(x => x.softwareLabel.Count == 0);
                if (_platformimage == null)
                {
                    CloudMovey.task().failcomplete(payload, "Template image not found: " + JsonConvert.SerializeObject(_options));
                    return;
                }
                DeployServer _vm = new DeployServer();

                List<DeployServerDisk> _disks = new List<DeployServerDisk>();
               
                foreach (MoveyWorkloadVolumeType volume in _target.volumes)
                {
                    if (_platformimage.disk.Exists(x => x.scsiId == volume.diskindex))
                    {
                        _disks.Add(new DeployServerDisk() { scsiId = (byte)volume.diskindex, speed = volume.platformstoragetier.shortname });
                    }
                }

                string moid = _payload.platform.moid;
                List<Option> _dcoptions = new List<Option>();
                _dcoptions.Add(new Option() { option = "id", value = moid });
                DatacenterType dc = (CaaS.datacenter().datacenters(_dcoptions) as DatacenterListType).datacenter[0];
                switch (dc.type)
                {
                    case "MCP 1.0":
                        _vm.name = _target.hostname;
                        _vm.description = String.Format("Server Created by CloudMovey [{0}]", DateTime.Now);
                        _vm.networkId = _target.interfaces[0].platformnetwork.moid;
                        _vm.imageId = _platformimage.id;
                        _vm.start = false;
                        _vm.disk = _disks;
                        _vm.administratorPassword = (string)_target.password;
                        break;
                    case "MCP 2.0":
                        break;
                }

                Status _status = CaaS.serverimage().serverimagedeploy(_vm);
                if (_status.resultCode == "REASON_0")
                {
                    String _vm_id = _status.additionalInformation.value;
                    CloudMovey.task().progress(payload, String.Format("{0} provisioning started in {1}({2})", _vm.name, dc.displayName, dc.id), 20);
                    List<Option> _vmoptions = new List<Option>();
                    _vmoptions.Add(new Option() { option = "id", value = _vm_id });
                    ServersWithBackupServer _newvm = CaaS.server().platformservers(_vmoptions).server[0];
                    while (_newvm.state != "NORMAL" && _newvm.isStarted == false)
                    {
                        if (_newvm.status != null)
                        {
                            if (_newvm.status.step != null)
                            {
                                CloudMovey.task().progress(payload, String.Format("Provisioning step: {0}", _newvm.status.step.name), 30 + _newvm.status.step.number);
                            }
                        }
                        _newvm = CaaS.server().platformservers(_vmoptions).server[0];
                        Thread.Sleep(5000);
                    }

                    //Update CPU and Memory for server
                    CaaS.serverimage().servermodify(server_id: _vm_id, cpuCount: _target.cpu, memory: _target.memory);
                    _newvm = CaaS.server().platformservers(_vmoptions).server[0];
                    CloudMovey.task().progress(payload, String.Format("Updating CPU and Memory: {0} : {1}", _target.cpu, _target.memory), 60);
                    while (_newvm.state != "NORMAL" && _newvm.isStarted == false)
                    {
                    _newvm = CaaS.server().platformservers(_vmoptions).server[0];
                        Thread.Sleep(5000);
                    }

                    //Expand C: drive and Add additional disks if required
                    int count = 0;
                    foreach (MoveyWorkloadVolumeType _volume in _target.volumes)
                    {
                        if (_newvm.disk.ToList().Exists(x => x.scsiId == _volume.diskindex))
                        {
                            if (_newvm.disk.ToList().Find(x => x.scsiId == _volume.diskindex).sizeGb < _volume.disksize)
                            {
                                CloudMovey.task().progress(payload, String.Format("Extending storage: {0} : {1}GB", _volume.diskindex, _volume.disksize), 60 + count);
                                CaaS.serverimage().serverdiskexpand(_vm_id, _volume.diskindex, (byte)_volume.disksize);
                            }
                        }
                        else
                        {
                            CloudMovey.task().progress(payload, String.Format("Adding storage: {0} : {1}GB on {2}", _volume.diskindex, _volume.disksize, _volume.platformstoragetier.storagetier), 60 + count);
                            CaaS.serverimage().serveraddstorage(_vm_id, _volume.disksize, _volume.platformstoragetier.shortname);
                        }
                        _newvm = CaaS.server().platformservers(_vmoptions).server[0];
                        while (_newvm.state != "NORMAL" && _newvm.isStarted == false)
                        {
                            _newvm = CaaS.server().platformservers(_vmoptions).server[0];
                            Thread.Sleep(5000);
                        }
                        count += 1;
                    }

                    //Start Server
                    CloudMovey.task().progress(payload, String.Format("Power on server"), 60 + count + 1);
                    CaaS.serverimage().serverstart(_vm_id);
                    _newvm = CaaS.server().platformservers(_vmoptions).server[0];
                    while (_newvm.state != "NORMAL" && _newvm.isStarted == false)
                    {
                        _newvm = CaaS.server().platformservers(_vmoptions).server[0];
                        Thread.Sleep(5000);
                    }

                    CloudMovey.task().progress(payload, String.Format("Server powered on"), 60 + count + 2);
                    _newvm = CaaS.server().platformservers(_vmoptions).server[0];

                    //create diskpart file for virtual machine and copy it to the remote server
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
                    foreach (ServersWithBackupServerDisk _disk in _newvm.disk.ToList().FindAll(x => x.scsiId != 0).OrderBy(x => x.scsiId))
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
                            string serverPath = Path.Combine(_newvm.privateIp, remoteInstallFiles);
                            serverPath = @"\\" + serverPath + @"\diskpart.txt";
                            File.WriteAllLines(serverPath, _diskpart_struct.ConvertAll(Convert.ToString));
                        }
                        catch (Exception e)
                        {
                            CloudMovey.task().failcomplete(payload, String.Format("Error creating disk layout file on server: {0}", e.Message)); 
                        }
                    }

                    //Run Diskpart Command on Server
                    //Create connection object to remote machine
                    CloudMovey.task().progress(payload, String.Format("Volume setup process on {0}", _newvm.name), 80);

                    ConnectionOptions connOptions = new ConnectionOptions() { EnablePrivileges = true, Username = "Administrator", Password = _target.password };
                    connOptions.Impersonation = ImpersonationLevel.Impersonate;
                    connOptions.Authentication = AuthenticationLevel.Default;
                    ManagementScope scope = new ManagementScope(@"\\" + _newvm.privateIp + @"\root\CIMV2", connOptions);
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
                        CloudMovey.task().failcomplete(payload, String.Format("Failed diskpart process on {0} ({1})", _newvm.name, _exitcode));
                        return;
                    }
                    else
                    {
                        CloudMovey.task().progress(payload, String.Format("Volume setup process exit code: {0}", _exitcode), 81);
                    }

                    CloudMovey.task().successcomplete(payload, JsonConvert.SerializeObject(_newvm));
                }
                else
                {
                    CloudMovey.task().failcomplete(payload, String.Format("Failed to create target virtual machine: {0}",_status.resultDetail));
                }
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(payload, e.ToString());
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
            CloudMovey CloudMovey = new CloudMovey();
            Tasks tasks = new Tasks(CloudMovey);
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.username, (string)payload.payload.password, null);
            try
            {
                CloudMovey.task().progress(payload, "MCP Data Gathering", 50);
                CloudMovey.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.datacenter().datacenters()));
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(payload, e.ToString());
            }
        }
        public static void mcp_gettemplates(dynamic payload)
        {
            CloudMovey CloudMovey = new CloudMovey();
            Tasks tasks = new Tasks(CloudMovey);
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.username, (string)payload.payload.password, null);
            try
            {
                CloudMovey.task().progress(payload, "MCP Data Gathering", 50);
                List<Option> filter = new List<Option>();
                filter.Add(new Option() { option = "location", value = payload.payload.locationId });
                filter.Add(new Option() { option = "operatingSystemFamily", value = "WINDOWS" });
                filter.Add(new Option() { option = "state", value = "NORMAL" });
                CloudMovey.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.datacenter().templates(filter)));
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(payload, e.ToString());
            }
        }
        public static void mcp_retrieveservers(dynamic payload)
        {
            CloudMovey CloudMovey = new CloudMovey();
            Tasks tasks = new Tasks(CloudMovey);
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.platform.username, (string)payload.payload.platform.password, null);
            try
            {
                CloudMovey.task().progress(payload, "MCP Data Gathering", 50);
                string moid = payload.payload.platform.moid;
                List<Option> options = new List<Option>();
                options.Add(new Option() { option = "id", value = moid });
                DatacenterType dc = (CaaS.datacenter().datacenters(options) as DatacenterListType).datacenter[0];
                switch (dc.type)
                {
                    case "MCP 1.0":
                        List<Option> filter = new List<Option>();
                        filter.Add(new Option() { option = "location", value = payload.payload.platform.moid });
                        CloudMovey.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.server().platformservers(filter)));
                        break;
                    case "MCP 2.0":
                        List<Option> _options = new List<Option>() { new Option() { option = "datacenterId", value = moid } };
                        CloudMovey.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.mcp2servers().listservers(_options)));
                        break;
                }
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(payload, e.ToString());
            }
        }
        public static void mcp_retrievenetworks(dynamic payload)
        {
            CloudMovey CloudMovey = new CloudMovey();
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.platform.username, (string)payload.payload.platform.password, null);
            try
            {
                CloudMovey.task().progress(payload, "MCP Data Gathering", 50);
                string moid = payload.payload.platform.moid;
                List<Option> options = new List<Option>();
                options.Add(new Option() { option = "id", value = moid });
                DatacenterType dc = (CaaS.datacenter().datacenters(options) as DatacenterListType).datacenter[0];
                switch(dc.type)
                {
                    case "MCP 1.0":
                        CloudMovey.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.network().networklist(moid)));
                        break;
                    case "MCP 2.0":
                        List<Option> _options = new List<Option>() { new Option() {option = "datacenterId", value = moid}};
                        CloudMovey.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.mcp2vlans().listvlan(_options)));
                        break;
                }
                
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(payload, e.ToString());
            }
        }
    }
}
