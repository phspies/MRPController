using MRMPService.LocalDatabase;
using MRMPService.API.Types.API;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization.Formatters.Binary;
using MRMPService.Utilities;
using MRMPService.MRMPService.Log;
using System.Collections.Generic;

namespace MRMPService.API.Classes
{
    class WorkloadInventory
    {
        public void WorkloadInventoryDo(MRPWorkloadType _workload)
        {
            MRP_ApiClient _cloud_movey = new MRP_ApiClient();

            //check for credentials
            MRPCredentialType _credential = _workload.credential;
            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Inventory: Error finding credentials for workload {0} {1}", _workload, _workload.hostname));
            }

            string workload_ip = null;
            using (Connection _connection = new Connection())
            {
                workload_ip = _connection.FindConnection(_workload.iplist, true);
            }
            if (workload_ip == null)
            {
                throw new ArgumentException(String.Format("Inventory: Error finding contactable IP for workload {0} {1}", _workload.id, _workload.hostname));
            }

            Logger.log(String.Format("Inventory: Started inventory collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);

            ConnectionOptions options = WMIHelper.ProcessConnectionOptions((String.IsNullOrWhiteSpace(_credential.domain) ? (@".\" + _credential.username) : (_credential.domain + @"\" + _credential.username)), _credential.password);
            ManagementScope connectionScope = WMIHelper.ConnectionScope(workload_ip, options);

            SelectQuery ComputerSystemQuery = new SelectQuery("SELECT Name, NumberOfProcessors, TotalPhysicalMemory FROM Win32_ComputerSystem");
            SelectQuery OperatingSystemQuery = new SelectQuery("SELECT Caption, OSArchitecture FROM Win32_OperatingSystem");
            SelectQuery ProcessorQuery = new SelectQuery("SELECT NumberOfCores, CurrentClockSpeed FROM Win32_Processor");

            //Get operating system type
            foreach (var item in new ManagementObjectSearcher(connectionScope, OperatingSystemQuery).Get())
            {
                try
                {
                    String _caption = item["Caption"].ToString();
                    string _arch = item["OSArchitecture"].ToString();
                    _workload.osedition = OSEditionSimplyfier.Simplyfier(String.Format("{0} {1}", _caption, _arch));
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Error collecting Caption (OS Type) from {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                }
            }


            //Get cpu, core and memory information from server
            foreach (var item in new ManagementObjectSearcher(connectionScope, ComputerSystemQuery).Get())
            {
                try { _workload.hostname = item["Name"].ToString(); }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Error collecting Name (Hostname) from {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                }
                try { _workload.vcpu = int.Parse(item["NumberOfProcessors"].ToString()); }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Error collecting NumberOfProcessors from {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                }
                //convert to GB as WMi reports in Bytes

                try
                {
                    Decimal _long = Int64.Parse(item["TotalPhysicalMemory"].ToString());
                    Decimal _decimal = (_long / 1024 / 1024 / 1024);
                    _workload.vmemory = (int)Math.Round(_decimal, MidpointRounding.AwayFromZero);
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Error collecting TotalPhysicalMemory from {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                }
            }
            foreach (var item in new ManagementObjectSearcher(connectionScope, ProcessorQuery).Get())
            {
                try { _workload.vcore = int.Parse(item["NumberOfCores"].ToString()); }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Error collecting NumberOfCores from {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                }
                try { _workload.vcpu_speed = int.Parse(item["CurrentClockSpeed"].ToString()); } catch (Exception) { }
            }

            //process running workloadprocesses_attributes
            SelectQuery msProcessQuery = new SelectQuery("SELECT * FROM Win32_Process");
            foreach (ManagementObject item in new ManagementObjectSearcher(connectionScope, msProcessQuery).Get())
            {
                MRPWorkloadProcessType _process;
                String _process_name;
                try
                {
                    _process_name = item["Caption"].ToString();
                }
                catch (Exception ex)
                {
                    continue;
                }
                if (_workload.workloadprocesses_attributes == null)
                {
                    _workload.workloadprocesses_attributes = new List<MRPWorkloadProcessType>();
                }
                //if procces already exists in portal, just update it   
                if (_workload.workloadprocesses_attributes.Exists(x => x.caption == _process_name))
                {
                    _process = _workload.workloadprocesses_attributes.FirstOrDefault(x => x.caption == item["Caption"].ToString());
                }
                else
                {
                    _process = new MRPWorkloadProcessType();
                    _workload.workloadprocesses_attributes.Add(_process);
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
            }

            //process installed software
            SelectQuery msSoftwareQuery = new SelectQuery("SELECT * FROM Win32_Product");
            foreach (ManagementObject item in new ManagementObjectSearcher(connectionScope, msSoftwareQuery).Get())
            {
                MRPWorkloadSoftwareType _software;

                //if procces already exists in portal, just update it 
                String _software_name;
                try
                {
                    _software_name = item["Name"].ToString();
                }
                catch (Exception ex)
                {
                    continue;
                }
                if (_workload.workloadsoftwares_attributes == null)
                {
                    _workload.workloadsoftwares_attributes = new List<MRPWorkloadSoftwareType>();
                }
                if (_workload.workloadsoftwares_attributes.Exists(x => x.name == _software_name))
                {
                    _software = _workload.workloadsoftwares_attributes.FirstOrDefault(x => x.name == item["Name"].ToString());
                }
                else
                {
                    _software = new MRPWorkloadSoftwareType();
                    _workload.workloadsoftwares_attributes.Add(_software);
                }
                try { _software.name = item["Name"].ToString(); } catch (Exception) { }
                try { _software.caption = item["Caption"].ToString(); } catch (Exception) { }
                try { _software.description = item["Description"].ToString(); } catch (Exception) { }
                try { _software.installlocation = item["InstallLocation"].ToString(); } catch (Exception) { }
                try { _software.installstate = Int16.Parse(item["InstallState"].ToString()); } catch (Exception) { }
                try { _software.vendor = item["Vendor"].ToString(); } catch (Exception) { }
                try { _software.version = item["Version"].ToString(); } catch (Exception) { }
            }

            //process logical workloadvolumes_attributes

            //set all workloadvolumes_attributes to be destroyed and remove destroy tag as we workloadprocesses_attributes workloadvolumes_attributes
            _workload.workloadvolumes_attributes.ForEach(x => { x._destroy = true; });

            SelectQuery wmiDiskDrives = new SelectQuery("SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject wmiDiskDrive in new ManagementObjectSearcher(connectionScope, wmiDiskDrives).Get())
            {
                foreach (ManagementObject wmiPartitionDrive in wmiDiskDrive.GetRelated("Win32_DiskPartition"))
                {
                    foreach (ManagementObject wmiLogicalDrive in wmiPartitionDrive.GetRelated("Win32_LogicalDisk"))
                    {
                        SelectQuery wmiVolumes = new SelectQuery("SELECT * FROM Win32_Volume where DriveLetter='" + wmiLogicalDrive["DeviceId"] + "'");
                        ManagementObjectSearcher searchVolumes = new ManagementObjectSearcher(connectionScope, wmiVolumes);
                        foreach (ManagementObject wmiVolume in searchVolumes.Get())
                        {
                            MRPWorkloadVolumeType _volume;

                            //if volume already exists in portal, just update it   
                            if (_workload.workloadvolumes_attributes.Exists(x => x.serialnumber == wmiVolume["SerialNumber"].ToString()))
                            {
                                _volume = _workload.workloadvolumes_attributes.FirstOrDefault(x => x.serialnumber == wmiVolume["SerialNumber"].ToString());
                                _volume._destroy = false;
                            }
                            else
                            {
                                _volume = new MRPWorkloadVolumeType();
                                _volume._destroy = false;

                                //Add new volume object to mrpworkload
                                _workload.workloadvolumes_attributes.Add(_volume);
                            }
                            Decimal _freespace = 0;
                            Decimal _size = 0;
                            Decimal _decimal;

                            try { _volume.diskindex = Int16.Parse(wmiDiskDrive["Index"].ToString()); } catch (Exception) { }
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
                        }
                    }
                }
            }


            //process network workloadinterfaces_attributes that has IP address configured
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

                    if (_conn_index == null) //something went wrong and we dont have the index number
                    {
                        _conn_index = 0;
                        _workload.workloadinterfaces_attributes.Add(_interface);

                    }
                    else if (_network_adapters.Count == 1 && _workload.workloadinterfaces_attributes.Count == 1) //if we only have one adapter and the portal also only knows about one, the update the same adapter
                    {
                        _interface = _workload.workloadinterfaces_attributes.First();
                    }
                    else if (_workload.workloadinterfaces_attributes.Exists(x => x.ipaddress == addresses.FirstOrDefault(s => s.Contains('.')))) //try to find the interface by means of the IP address
                    {
                        _interface = _workload.workloadinterfaces_attributes.FirstOrDefault(x => x.ipaddress == addresses.FirstOrDefault(s => s.Contains('.')));
                    }
                    else if (_network_adapters.Count > 1) //if we have more than one adapter in the server, we need to switch to index numbers
                    {
                        //first check if the current workloadinterfaces_attributes uses connection_index information, and get that interface for this loop
                        if (_workload.workloadinterfaces_attributes.Any(x => x.connection_index == _conn_index))
                        {
                            _interface = _workload.workloadinterfaces_attributes.FirstOrDefault(x => x.connection_index == _conn_index);
                        }
                        else
                        {
                            //we give up. this seems like a new interface....
                            _workload.workloadinterfaces_attributes.Add(_interface);
                        }
                    }
                    else
                    {
                        _workload.workloadinterfaces_attributes.Add(_interface);
                    }

                    _interface.ipaddress = addresses.FirstOrDefault(s => s.Contains('.'));
                    _interface.ipv6address = addresses.FirstOrDefault(s => s.Contains(':'));
                    _interface.netmask = netmask.FirstOrDefault(s => s.Contains('.'));
                    _interface.ipv6netmask = netmask.FirstOrDefault(s => s.Contains(':'));
                    _interface.connection_index = (int)_conn_index;
                    try { _interface.connection_id = searchNetInterface["NetConnectionID"].ToString(); } catch (Exception) { }
                    try { _interface.macaddress = searchNetInterface["MACAddress"].ToString(); } catch (Exception) { }
                }
                using (MRP_ApiClient _api = new MRP_ApiClient())
                {
                    _api.workload().InventoryUpdateStatus(_workload, "Success", true);
                }

                //set more aatributes
                _workload.ostype = "windows";


                _cloud_movey.workload().updateworkload(_workload);

                Logger.log(String.Format("Inventory: Completed inventory collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);

            }
        }

        private static string TranslateMemoryUsage(string workingSet)
        {
            int calc = Convert.ToInt32(workingSet);
            calc = calc / 1024;
            return calc.ToString();
        }

        private String GetPartName(String inp)
        {
            String Dependent = "", ret = "";
            ManagementObjectSearcher LogicalDisk = new ManagementObjectSearcher("Select * from Win32_LogicalDiskToPartition");
            foreach (ManagementObject drive in LogicalDisk.Get())
            {
                if (drive["Antecedent"].ToString().Contains(inp))
                {
                    Dependent = drive["Dependent"].ToString();
                    ret = Dependent.Substring(Dependent.Length - 3, 2);
                    break;
                }

            }
            return ret;

        }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}