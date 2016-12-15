using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Linq;
using System.Management;
using MRMPService.Utilities;
using MRMPService.MRMPService.Log;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Classes
{
    partial class WorkloadInventory
    {
        static public async Task WorkloadInventoryWindowsDo(MRPWorkloadType _workload)
        {

            _workload = await MRMPServiceBase._mrmp_api.workload().get_by_id(_workload.id);
            MRPWorkloadType _updated_workload = new MRPWorkloadType()
            {
                id = _workload.id,
                workloadsoftwares = _workload.workloadsoftwares,
                workloadprocesses = _workload.workloadprocesses,
                workloadvolumes = _workload.workloadvolumes,
                workloadinterfaces = _workload.workloadinterfaces,
                workloaddisks = _workload.workloaddisks
            };
            _updated_workload.workloadvolumes.ForEach(x => x.deleted = true);
            _updated_workload.workloaddisks.ForEach(x => x.deleted = true);
            _updated_workload.workloadinterfaces.ForEach(x => x.deleted = true);
            _updated_workload.workloadprocesses.ForEach(x => x._destroy = true);
            _updated_workload.workloadsoftwares.ForEach(x => x._destroy = true);
            //check for credentials
            MRPCredentialType _credential = _workload.credential;

            string workload_ip = null;
            Logger.log(String.Format("Inventory: Started inventory collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);

            String domainuser = null;
            if (_workload.workloadtype != "manager")
            {
                if (_credential == null)
                {
                    throw new ArgumentException(String.Format("Error finding credentials"));
                }

                if (!String.IsNullOrWhiteSpace(_credential.domain))
                {
                    domainuser = (_credential.domain + @"\" + _credential.username);
                }
                else
                {
                    domainuser = @".\" + _credential.username;
                }
                using (Connection _connection = new Connection())
                {
                    workload_ip = _connection.FindConnection(_workload.iplist, true);
                }
                if (workload_ip == null)
                {
                    throw new ArgumentException(String.Format("Error contacting workload"));
                }
            }
            else
            {
                if (String.IsNullOrEmpty(_workload.iplist))
                {
                    workload_ip = "127.0.0.1";
                }
                else
                {
                    using (Connection _connection = new Connection())
                    {
                        workload_ip = _connection.FindConnection(_workload.iplist, true);
                    }
                }
            }
            ConnectionOptions options = WMIHelper.ProcessConnectionOptions(domainuser, (_workload.workloadtype == "manager" ? null : _credential.encrypted_password));
            ManagementScope connectionScope = WMIHelper.ConnectionScope(workload_ip, options);

            SelectQuery ComputerSystemQuery = new SelectQuery("SELECT Manufacturer, Model, Caption, NumberOfProcessors, TotalPhysicalMemory FROM Win32_ComputerSystem");
            SelectQuery OperatingSystemQuery = new SelectQuery("SELECT Caption FROM Win32_OperatingSystem");
            SelectQuery ProcessorQuery = new SelectQuery("SELECT NumberOfCores, AddressWidth, CurrentClockSpeed FROM Win32_Processor");
            SelectQuery BiosQuery = new SelectQuery("SELECT SerialNumber FROM Win32_BIOS");
            string _arch = null;

            try
            {
                foreach (var item in new ManagementObjectSearcher(connectionScope, ProcessorQuery).Get())
                {
                    try { _updated_workload.vcore = int.Parse(item["NumberOfCores"].ToString()); }
                    catch (Exception ex)
                    {
                        Logger.log(String.Format("Error collecting NumberOfCores from {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                    }
                    try { _updated_workload.vcpu_speed = int.Parse(item["CurrentClockSpeed"].ToString()); } catch (Exception) { }
                    try
                    {
                        _arch = String.Format("{0}bit", item["AddressWidth"].ToString());
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("{0} Error collecting information from Win32_Processor: {1}", _updated_workload.osedition, ex.GetBaseException().Message));
            }
            //Get operating system type
            try
            {
                foreach (var item in new ManagementObjectSearcher(connectionScope, OperatingSystemQuery).Get())
                {
                    try
                    {
                        String _caption = item["Caption"].ToString();
                        _updated_workload.osedition = OSEditionSimplyfier.Simplyfier(String.Format("{0} {1}", _caption, _arch));
                    }
                    catch (Exception ex)
                    {
                        Logger.log(String.Format("Error collecting Caption (OS Type) from {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("{0} Error collecting information from Win32_OperatingSystem: {1}", _updated_workload.osedition, ex.GetBaseException().Message));
            }



            //Get operating system type
            try
            {
                foreach (var item in new ManagementObjectSearcher(connectionScope, BiosQuery).Get())
                {
                    try
                    {
                        _updated_workload.serialnumber = item["SerialNumber"].ToString();
                    }
                    catch (Exception ex)
                    {
                        Logger.log(String.Format("{0} Error collecting Enclosure info from {1} : {2}", _updated_workload.osedition, _workload.hostname, ex.Message), Logger.Severity.Error);
                    }
                }

                //Get cpu, core and memory information from server

                foreach (var item in new ManagementObjectSearcher(connectionScope, ComputerSystemQuery).Get())
                {
                    try
                    {
                        _updated_workload.model = item["Manufacturer"].ToString() + " " + item["Model"].ToString();
                        _updated_workload.hardwaretype = _updated_workload.model.ToLower().Contains("virtual") ? "virtual" : "physical";
                    }

                    catch (Exception ex)
                    {
                        Logger.log(String.Format("Error collecting Model from {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                    }
                    try { _updated_workload.hostname = item["Caption"].ToString(); }
                    catch (Exception ex)
                    {
                        Logger.log(String.Format("Error collecting Name (Hostname) from {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                    }
                    try { _updated_workload.vcpu = int.Parse(item["NumberOfProcessors"].ToString()); }
                    catch (Exception ex)
                    {
                        Logger.log(String.Format("Error collecting NumberOfProcessors from {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                    }
                    //convert to GB as WMi reports in Bytes

                    try
                    {
                        Decimal _long = Int64.Parse(item["TotalPhysicalMemory"].ToString());
                        Decimal _decimal = (_long / 1024 / 1024 / 1024);
                        _updated_workload.vmemory = (int)Math.Round(_decimal, MidpointRounding.AwayFromZero);
                    }
                    catch (Exception ex)
                    {
                        Logger.log(String.Format("Error collecting TotalPhysicalMemory from {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("{0} Error collecting information from Win32_ComputerSystem: {1}", _updated_workload.osedition, ex.GetBaseException().Message));
            }


            //process running workloadprocesses_attributes
            try
            {
                SelectQuery msProcessQuery = new SelectQuery("SELECT * FROM Win32_Process");
                foreach (ManagementObject item in new ManagementObjectSearcher(connectionScope, msProcessQuery).Get())
                {
                    String _process_name;
                    try
                    {
                        _process_name = item["Caption"].ToString();
                        MRPWorkloadProcessType _process = new MRPWorkloadProcessType();
                        //if procces already exists in portal, just update it   
                        if (_workload.workloadprocesses != null)
                        {
                            if (_updated_workload.workloadprocesses.Exists(x => x.caption == _process_name))
                            {
                                _process = _updated_workload.workloadprocesses.FirstOrDefault(x => x.caption == _process_name);
                            }
                            else
                            {
                                _updated_workload.workloadprocesses.Add(_process);
                            }
                        }

                        try { _process.caption = item["Caption"].ToString(); } catch (Exception) { }
                        try { _process.commandline = item["CommandLine"].ToString(); } catch (Exception) { }
                        try { _process.name = item["Name"].ToString(); } catch (Exception) { }
                        try { _process.processid = Int16.Parse(item["ProcessId"].ToString()); } catch (Exception) { }
                        try { _process.writeoperationcount = Int64.Parse(item["WriteOperationCount"].ToString()); } catch (Exception) { }
                        try { _process.writetransfercount = Int64.Parse(item["WriteTransferCount"].ToString()); } catch (Exception) { }
                        try { _process.readoperationcount = Int64.Parse(item["ReadOperationCount"].ToString()); } catch (Exception) { }
                        try { _process.readtransfercount = Int64.Parse(item["ReadTransferCount"].ToString()); } catch (Exception) { }
                        try { _process.threadcount = Int32.Parse(item["ThreadCount"].ToString()); } catch (Exception) { }
                        try { _process.virtualsize = Int64.Parse(item["VirtualSize"].ToString()); } catch (Exception) { }
                        _process._destroy = false;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("{0} Error collecting information from Win32_Process: {1}", _updated_workload.osedition, ex.GetBaseException().Message));
            }

            //process installed software
            try
            {
                SelectQuery msSoftwareQuery = new SelectQuery("SELECT Name,Caption,Description,InstallLocation,InstallState,Vendor,Version  FROM Win32_Product");
                foreach (ManagementObject item in new ManagementObjectSearcher(connectionScope, msSoftwareQuery).Get())
                {
                    MRPWorkloadSoftwareType _software = new MRPWorkloadSoftwareType(); ;
                    try
                    {
                        String _software_name = item["Name"].ToString();
                        if (_updated_workload.workloadsoftwares.Exists(x => x.name == _software_name))
                        {
                            _software = _updated_workload.workloadsoftwares.FirstOrDefault(x => x.name == _software_name);
                        }
                        else
                        {
                            _updated_workload.workloadsoftwares.Add(_software);
                        }
                        _software.name = _software_name;
                        try { _software.caption = item["Caption"].ToString(); } catch (Exception) { }
                        try { _software.description = item["Description"].ToString(); } catch (Exception) { }
                        try { _software.installlocation = item["InstallLocation"].ToString(); } catch (Exception) { }
                        try { _software.installstate = Int16.Parse(item["InstallState"].ToString()); } catch (Exception) { }
                        try { _software.vendor = item["Vendor"].ToString(); } catch (Exception) { }
                        try { _software.version = item["Version"].ToString(); } catch (Exception) { }
                        _software._destroy = false;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("{0} Error collecting information from Win32_Product: {1}", _updated_workload.osedition, ex.GetBaseException().Message));
            }
            //process logical workloadvolumes_attributes
            try
            {
                SelectQuery wmiDiskDrives = new SelectQuery("SELECT * FROM Win32_DiskDrive where InterfaceType != 'USB'");
                foreach (ManagementObject wmiDiskDrive in new ManagementObjectSearcher(connectionScope, wmiDiskDrives).Get())
                {
                    Decimal _freespace = 0;
                    Decimal _size = 0;
                    Decimal _decimal;
                    MRPWorkloadDiskType _disk = new MRPWorkloadDiskType();
                    int? _disk_index = null;
                    try
                    {
                        _disk_index = Int16.Parse(wmiDiskDrive["Index"].ToString());
                    }
                    catch (Exception ex)
                    {
                        Logger.log(String.Format("Error determining disk index {0} : {1}", _workload.hostname, ex.GetBaseException().Message), Logger.Severity.Error);
                    }
                    if (_disk_index != null)
                    {
                        if (_updated_workload.workloaddisks.Exists(x => x.diskindex == _disk_index))
                        {
                            _disk = _updated_workload.workloaddisks.FirstOrDefault(x => x.diskindex == _disk_index);
                        }
                        else
                        {
                            _updated_workload.workloaddisks.Add(_disk);
                        }

                        try
                        {
                            _size = Int64.Parse(wmiDiskDrive["Size"].ToString());

                            _decimal = (_size / 1024 / 1024 / 1024);
                            _disk.disksize = (int)Math.Round(_decimal, MidpointRounding.AwayFromZero);
                            _disk.diskindex = _disk_index;
                            _disk.deleted = false;
                            _disk.provisioned = true;
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Error determining disk size {0} [{1}] : {2}", _workload.hostname, _disk_index, ex.GetBaseException().Message), Logger.Severity.Error);
                        }
                    }

                    foreach (ManagementObject wmiPartitionDrive in wmiDiskDrive.GetRelated("Win32_DiskPartition"))
                    {
                        foreach (ManagementObject wmiLogicalDrive in wmiPartitionDrive.GetRelated("Win32_LogicalDisk"))
                        {
                            SelectQuery wmiVolumes = new SelectQuery("SELECT * FROM Win32_Volume where DriveLetter='" + wmiLogicalDrive["DeviceId"] + "'");
                            ManagementObjectSearcher searchVolumes = new ManagementObjectSearcher(connectionScope, wmiVolumes);
                            foreach (ManagementObject wmiVolume in searchVolumes.Get())
                            {
                                MRPWorkloadVolumeType _volume = new MRPWorkloadVolumeType();


                                if (_updated_workload.workloadvolumes.Exists(x => x.serialnumber == wmiVolume["SerialNumber"].ToString()))
                                {
                                    _volume = _updated_workload.workloadvolumes.FirstOrDefault(x => x.serialnumber == wmiVolume["SerialNumber"].ToString());
                                }
                                else
                                {
                                    _updated_workload.workloadvolumes.Add(_volume);
                                }

                                _freespace = 0;
                                _size = 0;

                                try { _volume.filesystem_type = wmiVolume["FileSystem"].ToString(); } catch (Exception) { }
                                _volume.diskindex = (int)_disk_index;
                                try { _volume.driveletter = wmiVolume["DriveLetter"].ToString().Substring(0, 1); } catch (Exception) { }
                                try { _volume.serialnumber = wmiVolume["SerialNumber"].ToString(); } catch (Exception) { }
                                try { _volume.blocksize = Int16.Parse(wmiVolume["BlockSize"].ToString()); } catch (Exception) { }
                                try { _volume.volumename = wmiVolume["Label"].ToString(); } catch (Exception) { }
                                try { _volume.deviceid = wmiVolume["DeviceID"].ToString(); } catch (Exception) { }
                                try { _freespace = Int64.Parse(wmiVolume["FreeSpace"].ToString()); } catch (Exception) { }
                                try { _size = Int64.Parse(wmiVolume["Capacity"].ToString()); } catch (Exception) { }
                                try { _volume.provisioned = true; } catch (Exception) { }
                                if (_freespace > 0)
                                {
                                    _decimal = (_freespace / 1024 / 1024 / 1024);
                                    _volume.volumefreespace = (int)Math.Round(_decimal, MidpointRounding.AwayFromZero);
                                }
                                if (_size > 0)
                                {
                                    _decimal = (_size / 1024 / 1024 / 1024);
                                    _volume.volumesize = (int)Math.Round(_decimal, MidpointRounding.AwayFromZero);
                                }
                                _volume.deleted = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("{0} Error collecting information from Win32_DiskDrive: {1}", _updated_workload.osedition, ex.GetBaseException().Message));
            }

            //process network workloadinterfaces_attributes that has IP address configured
            try
            {
                SelectQuery wmiNetInterfaces = new SelectQuery("select * from Win32_NetworkAdapterConfiguration where IPEnabled = 'True'");
                ManagementObjectCollection _network_adapters = new ManagementObjectSearcher(connectionScope, wmiNetInterfaces).Get();

                foreach (ManagementObject searchNetInterfaceConfig in _network_adapters)
                {
                    foreach (ManagementObject searchNetInterface in searchNetInterfaceConfig.GetRelated("Win32_NetworkAdapter"))
                    {
                        int? _conn_index = null;
                        try { _conn_index = Int16.Parse(searchNetInterfaceConfig["Index"].ToString()); } catch (Exception) { }
                        MRPWorkloadInterfaceType _interface = new MRPWorkloadInterfaceType();

                        String[] addresses = (String[])searchNetInterfaceConfig["IPAddress"];
                        String[] netmask = (String[])searchNetInterfaceConfig["IPSubnet"];
                        if (_workload.workloadinterfaces != null)
                        {
                            if (_updated_workload.workloadinterfaces.Exists(x => x.ipaddress == addresses.FirstOrDefault(s => s.Contains('.')))) //try to find the interface by means of the IP address
                            {
                                _interface = _updated_workload.workloadinterfaces.FirstOrDefault(x => x.ipaddress == addresses.FirstOrDefault(s => s.Contains('.')));
                            }
                            else if (_network_adapters.Count == 1 && _updated_workload.workloadinterfaces.Count == 1) //if we only have one adapter and the portal also only knows about one, the update the same adapter
                            {
                                _interface = _updated_workload.workloadinterfaces.First();
                            }
                            else if (_network_adapters.Count > 1) //if we have more than one adapter in the server, we need to switch to index numbers
                            {
                                //first check if the current workloadinterfaces_attributes uses connection_index information, and get that interface for this loop
                                if (_updated_workload.workloadinterfaces.Any(x => x.connection_index == _conn_index))
                                {
                                    _interface = _updated_workload.workloadinterfaces.FirstOrDefault(x => x.connection_index == _conn_index);
                                }
                                else
                                {
                                    _updated_workload.workloadinterfaces.Add(_interface);
                                }
                            }
                            else
                            {
                                _updated_workload.workloadinterfaces.Add(_interface);
                            }
                        }

                        _interface.ipaddress = addresses.FirstOrDefault(s => s.Contains('.'));
                        _interface.ipv6address = addresses.FirstOrDefault(s => s.Contains(':'));
                        _interface.netmask = netmask.FirstOrDefault(s => s.Contains('.'));
                        _interface.ipv6netmask = netmask.FirstOrDefault(s => s.Contains(':'));
                        _interface.connection_index = (int)_conn_index;
                        try { _interface.caption = searchNetInterface["Caption"].ToString(); } catch (Exception) { }
                        try { _interface.connection_id = searchNetInterface["NetConnectionID"].ToString(); } catch (Exception) { }
                        try { _interface.macaddress = searchNetInterface["MACAddress"].ToString(); } catch (Exception) { }
                        _interface.deleted = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("{0} Error collecting information from Win32_NetworkAdapterConfiguration: {1}", _updated_workload.osedition, ex.GetBaseException().Message));
            }

            _updated_workload.ostype = "windows";
            _updated_workload.provisioned = true;

            await MRMPServiceBase._mrmp_api.workload().updateworkload(_updated_workload);
            await MRMPServiceBase._mrmp_api.workload().InventoryUpdateStatus(_updated_workload, "Success", true);

            Logger.log(String.Format("Inventory: Completed inventory collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}