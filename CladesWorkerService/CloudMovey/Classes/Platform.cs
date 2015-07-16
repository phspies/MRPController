using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.CaaS.Models;
using CloudMoveyWorkerService.CloudMovey.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CloudMoveyWorkerService.CloudMovey.Controllers
{
    class Platform
    {
        public static void mcp_provisionvm(dynamic payload)
        {
            CloudMoveyWorkerService.CloudMovey.CloudMovey CloudMovey = new CloudMoveyWorkerService.CloudMovey.CloudMovey(Global.apiBase, null, null);
            DimensionData CaaS = new DimensionData((string)payload.payload.platform.mcpendpoint.url, (string)payload.payload.platform.username, (string)payload.payload.platform.password, null);
            try
            {
                String _ostype = String.Format("{0} {1}", payload.payload.mcp.target.ostype, payload.payload.mcp.target.osedition);
                String mcp_template_name = mcp_getimage_name(_ostype); 
                List<Option> _options = new List<Option>();
                _options.Add(new Option() { option = "operatingSystemId", value = mcp_template_name });
                _options.Add(new Option() { option = "operatingSystemFamily", value = "WINDOWS" });
                _options.Add(new Option() { option = "location", value = payload.payload.platform.moid });
                _options.Add(new Option() { option = "state", value = "NORMAL" });

                ImagesWithDiskSpeed _platformimages = CaaS.serverimage().platformserverimages(_options);
                if (_platformimages.image.Count() == 0)
                {
                    CloudMovey.task().failcomplete(payload, "Image not found: " + JsonConvert.SerializeObject(_options));
                }
                DeployServer _vm = new DeployServer();

                List<DeployServerDisk> _disks = new List<DeployServerDisk>();
                foreach (var volume in payload.payload.mcp.target.volumes)
                {
                    _disks.Add(new DeployServerDisk() { scsiId = volume.diskindex, speed =  volume.platformstoragetier.shortname});
                }

                string moid = payload.payload.platform.moid;
                List<Option> _dcoptions = new List<Option>();
                _dcoptions.Add(new Option() { option = "id", value = moid });
                DatacenterType dc = CaaS.datacenter().datacenters(_dcoptions).datacenter[0];
                switch (dc.type)
                {
                    case "MCP 1.0":
                        _vm.name = payload.payload.mcp.target.hostname;
                        _vm.description = String.Format("server created by CloudMovey at {0}", DateTime.Now);
                        _vm.networkId = payload.payload.mcp.target.interfaces[0].platformnetwork.moid;
                        _vm.imageId = _platformimages.image.First().id;
                        _vm.start = false;
                        _vm.disk = _disks;
                        _vm.administratorPassword = payload.payload.mcp.target.password;
                        break;
                    case "MCP 2.0":
                        break;
                }

                Status _status = CaaS.serverimage().serverimagedeploy(_vm);
                if (_status.resultCode == "REASON_0")
                {
                    String _vm_id = _status.additionalInformation.value;
                    CloudMovey.task().progress(payload, String.Format("{0}, ID: {1}, created on {1}",_vm.name, _vm_id, dc.displayName), 30);
                    List<Option> _vmoptions = new List<Option>();
                    _vmoptions.Add(new Option() { option = "id", value = _vm_id });
                    ServersWithBackup _newvm = CaaS.server().platformservers(_vmoptions);
                    while (_newvm.server[0].state != "NORMAL")
                    {
                        if (_newvm.server[0].status != null)
                        {
                            CloudMovey.task().progress(payload, String.Format("Waiting for VM to be provisioned: {0}", _newvm.server[0].status.action, 40));
                        }
                        _newvm = CaaS.server().platformservers(_vmoptions);
                        Thread.Sleep(5000);
                    }
                    CloudMovey.task().progress(payload, String.Format("Server Successfully provisioned: {0}", JsonConvert.SerializeObject(_newvm.server[0])), 50);

                    //wait for server to power down after deployment
                    _newvm = CaaS.server().platformservers(_vmoptions);
                    while (_newvm.server[0].state != "NORMAL" && _newvm.server[0].isStarted == false)
                    {
                        _newvm = CaaS.server().platformservers(_vmoptions);
                    }

                    //Update CPU and Memory for server
                    CaaS.serverimage().servermodify(_vm_id, null, null, payload.payload.mcp.target.cpu, payload.payload.mcp.target.memory, null);
                    _newvm = CaaS.server().platformservers(_vmoptions);
                    CloudMovey.task().progress(payload, String.Format("Updating CPU and Memory: {0} : {1}", payload.payload.mcp.target.cpu, payload.payload.mcp.target.memory), 60);
                    while (_newvm.server[0].state != "NORMAL")
                    {
                        _newvm = CaaS.server().platformservers(_vmoptions);
                        Thread.Sleep(5000);
                    }

                    //Expand C: drive and Add additional disks if required
                    int count = 0;
                    foreach (var _volume in payload.payload.mcp.target.volumes.Children())
                    {
                        if (_newvm.server[0].disk.ToList().Exists(x => x.scsiId == (int)_volume.diskindex && x.sizeGb < (int)_volume.disksize))
                        {
                            CaaS.serverimage().serverdiskexpand(_vm_id,(int)_volume.diskindex,(byte)_volume.disksize);
                        }
                        else
                        {
                            CaaS.serverimage().serveraddstorage(_vm_id, (int)_volume.disksize, _volume.platformstoragetier.shortname);
                        }
                        _newvm = CaaS.server().platformservers(_vmoptions);
                        while (_newvm.server[0].state != "NORMAL")
                        {
                            CloudMovey.task().progress(payload, String.Format("Updating storage: {0} : {1}GB", (int)_volume.diskindex, (int)_volume.disksize), 60 + count);
                            _newvm = CaaS.server().platformservers(_vmoptions);
                            Thread.Sleep(5000);
                        }
                        count += 1;
                    }

                    //Start Server
                    CloudMovey.task().progress(payload, String.Format("Power on server"), 60 + count + 1);
                    CaaS.serverimage().serverstart(_vm_id);
                    _newvm = CaaS.server().platformservers(_vmoptions);
                    while (_newvm.server[0].state != "NORMAL")
                    {
                        _newvm = CaaS.server().platformservers(_vmoptions);
                        Thread.Sleep(5000);
                    }

                    CloudMovey.task().progress(payload, String.Format("Server powered on"), 60 + count + 2);
                    //create diskpart file for virtual machine and copy it to the remote server
                    MemoryStream  memoryStream = new MemoryStream();
                    using (TextWriter tw = new StreamWriter(memoryStream))
                    {
                        tw.WriteLine("select volume 0");
                        tw.WriteLine("extend");
                        foreach (ServersWithBackupServerDisk _disk in _newvm.server[0].disk.ToList().FindAll(x => x.scsiId != 0).OrderBy(x => x.scsiId))
                        {
                            JObject _volumes = payload.payload.mcp.target.volumes;
                            JObject _vmdisk = _volumes.Children<JObject>().FirstOrDefault(o => o["diskindex"] != null && o["diskindex"].ToString() == _disk.scsiId.ToString());
                            tw.WriteLine("select disk {0}", _disk.id);
                            tw.WriteLine("clean");
                            tw.WriteLine("create partition primary");
                            tw.WriteLine("select partition 1");
                            tw.WriteLine("format fs=ntfs quick");
                            tw.WriteLine("assign letter={0}", (String)_vmdisk.GetValue("driveletter")[0]);
                            tw.WriteLine("active");
                        }
                    }
                    using (Impersonation.LogonUser(".", "Administrator", payload.payload.mcp.target.password, LogonType.Batch))
                    {

                        try
                        {
                            string remoteInstallFiles = @"C:\";
                            remoteInstallFiles = remoteInstallFiles.Replace(':', '$');
                            string serverPath = Path.Combine(_newvm.server[0].privateIp, remoteInstallFiles);
                            serverPath = @"\\" + serverPath + @"\diskpart.txt";
                            FileStream fileStream = new FileStream(serverPath, FileMode.CreateNew);
                            memoryStream.WriteTo(fileStream);
                        }
                        catch (Exception e)
                        {
                            CloudMovey.task().failcomplete(payload, String.Format("Error creating diskpart file on server: {0}", e.Message)); 
                        }
                    }

                    //Run Diskpart Command on Server
                    //Create connection object to remote machine
                    ConnectionOptions connOptions = new ConnectionOptions() { EnablePrivileges = true, Username = "Administrator", Password = payload.payload.mcp.target.password };
                    connOptions.Impersonation = ImpersonationLevel.Impersonate;
                    connOptions.Authentication = AuthenticationLevel.Default;
                    ManagementScope scope = new ManagementScope(@"\\" + _newvm.server[0].privateIp + @"\root\CIMV2", connOptions);
                    scope.Connect();

                    string installCmd = @"diskpart /s diskpart.txt";
                    Dictionary<string, string> installCmdParams = new Dictionary<string, string>();
                    installCmdParams["CommandLine"] = installCmd;
                    installCmdParams["CurrentDirectory"] = @"C:\Windows";

                    Dictionary<string, object> returnValues = new Dictionary<string, object>();
                    ManagementPath wmiObjectPath = new ManagementPath("Win32_Process");
                    ObjectGetOptions ogo = new ObjectGetOptions();
                    ManagementBaseObject returnValue;
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

                    int processId = 0;
                    if (returnValues != null)
                    {
                        processId = Convert.ToInt32(returnValue.Properties["ProcessId"].Value);
                    }
                    CloudMovey.task().progress(payload, String.Format("Started diskpart process on {0} ({1})", _newvm.server[0].name, processId), 80);
                }
                CloudMovey.task().successcomplete(payload, String.Format("Successfully deployed and configured {0}", payload.payload.mcp.target.hostname));
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
            CloudMoveyWorkerService.CloudMovey.CloudMovey CloudMovey = new CloudMoveyWorkerService.CloudMovey.CloudMovey(Global.apiBase, null, null);
            TasksObject tasks = new TasksObject(CloudMovey);
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
            CloudMoveyWorkerService.CloudMovey.CloudMovey CloudMovey = new CloudMoveyWorkerService.CloudMovey.CloudMovey(Global.apiBase, null, null);
            TasksObject tasks = new TasksObject(CloudMovey);
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
            CloudMoveyWorkerService.CloudMovey.CloudMovey CloudMovey = new CloudMoveyWorkerService.CloudMovey.CloudMovey(Global.apiBase, null, null);
            TasksObject tasks = new TasksObject(CloudMovey);
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.platform.username, (string)payload.payload.platform.password, null);
            try
            {
                CloudMovey.task().progress(payload, "MCP Data Gathering", 50);
                string moid = payload.payload.platform.moid;
                List<Option> options = new List<Option>();
                options.Add(new Option() { option = "id", value = moid });
                DatacenterType dc = CaaS.datacenter().datacenters(options).datacenter[0];
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
            CloudMoveyWorkerService.CloudMovey.CloudMovey CloudMovey = new CloudMoveyWorkerService.CloudMovey.CloudMovey(Global.apiBase, null, null);
            TasksObject tasks = new TasksObject(CloudMovey);
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.platform.username, (string)payload.payload.platform.password, null);
            try
            {
                CloudMovey.task().progress(payload, "MCP Data Gathering", 50);
                string moid = payload.payload.platform.moid;
                List<Option> options = new List<Option>();
                options.Add(new Option() { option = "id", value = moid });
                DatacenterType dc = CaaS.datacenter().datacenters(options).datacenter[0];
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
