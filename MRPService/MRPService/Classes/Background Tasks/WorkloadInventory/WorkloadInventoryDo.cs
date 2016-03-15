using MRPService.LocalDatabase;
using MRPService.API.Types.API;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization.Formatters.Binary;
using MRPService.Utilities;

namespace MRPService.API.Classes
{
    class WorkloadInventory
    {
        MRP_ApiClient _cloud_movey = new MRP_ApiClient();

        public static void WorkloadInventoryDo(String workload_id)
        {
            WorkloadSet dbworkload = new WorkloadSet();
            CredentialSet dbcredential = new CredentialSet();
            MRP_ApiClient _cloud_movey = new MRP_ApiClient();

            MRPWorkloadType mrpworkload = _cloud_movey.workload().getworkload(workload_id);
            if (mrpworkload == null)
            {
                throw new System.ArgumentException(String.Format("Error finding workload in MRP Portal {0}", workload_id));
            }

            //Check if workload exists
            Workload _workload = dbworkload.ModelRepository.GetById(mrpworkload.id);
            if (_workload == null)
            {
                throw new ArgumentException("Error finding workload in controller database");
            }

            //check for credentials
            Credential _credential = dbcredential.ModelRepository.GetById(mrpworkload.credential_id);
            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Error finding credentials for workload {0} {1}", workload_id, _workload.hostname));
            }

            string workload_ip = Connection.find_working_ip(_workload, true);
            if (workload_ip == null)
            {
                throw new ArgumentException(String.Format("Error finding contactable IP for workload {0} {1}", _workload.id, _workload.hostname));
            }

            ConnectionOptions options = WMIHelper.ProcessConnectionOptions((String.IsNullOrWhiteSpace(_credential.domain) ? "." : (_credential.domain + @"\" + _credential.username)), _credential.password);
            ManagementScope connectionScope = WMIHelper.ConnectionScope(workload_ip, options);

            SelectQuery computerProcessQuery = new SelectQuery("SELECT NumberOfProcessors, TotalPhysicalMemory FROM Win32_ComputerSystem");
            SelectQuery ProcessorProcessQuery = new SelectQuery("SELECT NumberOfCores, CurrentClockSpeed FROM Win32_Processor");

            //Get cpu, core and memory information from server
            foreach (var item in new ManagementObjectSearcher(connectionScope, computerProcessQuery).Get())
            {
                try { _workload.vcpu = int.Parse(item["NumberOfProcessors"].ToString()); } catch (Exception) { }
                //convert to GB as WMi reports in Bytes
                try { _workload.vmemory = (int.Parse(item["TotalPhysicalMemory"].ToString()) / 1024 / 1024 / 1024); } catch (Exception) { }

            }
            foreach (var item in new ManagementObjectSearcher(connectionScope, ProcessorProcessQuery).Get())
            {
                try { _workload.vcore = int.Parse(item["NumberOfCores"].ToString()); } catch (Exception) { }
                try { _workload.vcpu_speed = int.Parse(item["CurrentClockSpeed"].ToString()); } catch (Exception) { }
            }
            //save workload to database
            dbworkload.Save();

            //update mrpworkload object
            mrpworkload.vcore = (int)_workload.vcore;
            mrpworkload.vcpu = (int)_workload.vcpu;
            mrpworkload.vcpu_speed = (int)_workload.vcpu_speed;
            mrpworkload.vmemory = (int)_workload.vmemory;


            //process running processes
            SelectQuery msProcessQuery = new SelectQuery("SELECT * FROM Win32_Process");
            foreach (ManagementObject item in new ManagementObjectSearcher(connectionScope, msProcessQuery).Get())
            {
                MRPWorkloadProcessType _process;

                //if procces already exists in portal, just update it   
                if (mrpworkload.processes.Exists(x => x.caption == item["Caption"].ToString()))
                {
                    _process = mrpworkload.processes.FirstOrDefault(x => x.caption == item["Caption"].ToString());
                }
                else
                {
                    _process = new MRPWorkloadProcessType();
                    mrpworkload.processes.Add(_process);
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
                if (mrpworkload.softwares.Exists(x => x.name == item["Name"].ToString()))
                {
                    _software = mrpworkload.softwares.FirstOrDefault(x => x.name == item["Name"].ToString());
                }
                else
                {
                    _software = new MRPWorkloadSoftwareType();
                    mrpworkload.softwares.Add(_software);
                }
                try { _software.name = item["Name"].ToString(); } catch (Exception) { }
                try { _software.caption = item["Caption"].ToString(); } catch (Exception) { }
                try { _software.description = item["Description"].ToString(); } catch (Exception) { }
                try { _software.installlocation = item["InstallLocation"].ToString(); } catch (Exception) { }
                try { _software.installstate = Int16.Parse(item["InstallState"].ToString()); } catch (Exception) { }
                try { _software.vendor = item["Vendor"].ToString(); } catch (Exception) { }
                try { _software.version = item["Version"].ToString(); } catch (Exception) { }
            }

            //process logical volumes

            //set all volumes to be destroyed and remove destroy tag as we processes volumes
            mrpworkload.volumes.ForEach(x => { x._destroy = true; x.platformstoragetier = null; });
            mrpworkload.disks.ForEach(x => { x._destroy = true; });

            SelectQuery wmiDiskDrives = new SelectQuery("SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject wmiDiskDrive in new ManagementObjectSearcher(connectionScope, wmiDiskDrives).Get())
            {

                MRPWorkloadDiskType _disk;

                //if volume already exists in portal, just update it   
                if (mrpworkload.disks.Exists(x => x.diskindex == Int16.Parse(wmiDiskDrive["Index"].ToString())))
                {
                    _disk = mrpworkload.disks.FirstOrDefault(x => x.diskindex == Int16.Parse(wmiDiskDrive["Index"].ToString()));
                    _disk._destroy = false;
                }
                else
                {
                    _disk = new MRPWorkloadDiskType();
                    mrpworkload.disks.Add(_disk);
                }

                try { _disk.disksize = Int64.Parse(wmiDiskDrive["Size"].ToString()); } catch (Exception) { }
                try { _disk.deviceid = wmiDiskDrive["DeviceID"].ToString(); } catch (Exception) { }
                if (_disk.disksize != 0)
                {
                    _disk.disksize = (_disk.disksize / 1024 / 1024 / 1024);
                }

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
                            if (mrpworkload.volumes.Exists(x => x.serialnumber == wmiVolume["SerialNumber"].ToString()))
                            {
                                _volume = mrpworkload.volumes.FirstOrDefault(x => x.serialnumber == wmiVolume["SerialNumber"].ToString());
                                _volume._destroy = false;
                            }
                            else
                            {
                                _volume = new MRPWorkloadVolumeType();
                                _volume._destroy = false;

                                //Add new volume object to mrpworkload
                                mrpworkload.volumes.Add(_volume);
                            }
                            _volume.diskindex = _disk.diskindex;
                            try { _volume.diskindex = Int16.Parse(wmiDiskDrive["Index"].ToString()); } catch (Exception) { }
                            try { _volume.driveletter = wmiVolume["DriveLetter"].ToString(); } catch (Exception) { }
                            try { _volume.serialnumber = wmiVolume["SerialNumber"].ToString(); } catch (Exception) { }
                            try { _volume.blocksize = Int16.Parse(wmiVolume["BlockSize"].ToString()); } catch (Exception) { }
                            try { _volume.volumename = wmiVolume["Label"].ToString(); } catch (Exception) { }
                            try { _volume.deviceid = wmiVolume["DeviceID"].ToString(); } catch (Exception) { }
                            try { _volume.volumefreespace = Int64.Parse(wmiVolume["FreeSpace"].ToString()); } catch (Exception) { }
                            try { _volume.volumesize = Int64.Parse(wmiVolume["Capacity"].ToString()); } catch (Exception) { }
                            try { _volume.provisioned = true; } catch (Exception) { }
                            if (_volume.volumefreespace > 0)
                            {
                                _volume.volumefreespace = (_volume.volumefreespace / 1024 / 1024 / 1024);
                            }
                            if (_volume.volumesize > 0)
                            {
                                _volume.volumesize = (_volume.volumesize / 1024 / 1024 / 1024);
                            }
                        }
                    }
                }
            }


            //process network interfaces that has IP address configured
            SelectQuery wmiNetInterfaces = new SelectQuery("select * from Win32_NetworkAdapterConfiguration where IPEnabled = 'True'");
            foreach (ManagementObject searchNetInterfaceConfig in new ManagementObjectSearcher(connectionScope, wmiNetInterfaces).Get())
            {
                foreach (ManagementObject searchNetInterface in searchNetInterfaceConfig.GetRelated("Win32_NetworkAdapter"))
                {
                    MRPWorkloadInterfaceType _interface;

                    String[] addresses = (String[])searchNetInterfaceConfig["IPAddress"];
                    String[] netmask = (String[])searchNetInterfaceConfig["IPSubnet"];

                    //if interface already exists in portal, just update it   
                    if (mrpworkload.interfaces.Exists(x => x.ipaddress == addresses.FirstOrDefault(s => s.Contains('.'))))
                    {
                        _interface = mrpworkload.interfaces.FirstOrDefault(x => x.ipaddress == addresses.FirstOrDefault(s => s.Contains('.')));
                    }
                    else
                    {
                        _interface = new MRPWorkloadInterfaceType();
                        
                        //add new interface for mrpworkload
                        mrpworkload.interfaces.Add(_interface);
                    }
                    _interface.ipaddress = addresses.FirstOrDefault(s => s.Contains('.'));
                    _interface.ipv6address = addresses.FirstOrDefault(s => s.Contains(':'));
                    _interface.netmask = netmask.FirstOrDefault(s => s.Contains('.'));
                    _interface.ipv6netmask = netmask.FirstOrDefault(s => s.Contains(':'));
                    try { _interface.connection_index = Int16.Parse(searchNetInterfaceConfig["Index"].ToString()); } catch (Exception) { }
                    try { _interface.connection_id = searchNetInterface["NetConnectionID"].ToString(); } catch (Exception) { }
                    try { _interface.macaddress = searchNetInterface["MACAddress"].ToString(); } catch (Exception) { }

                }

            }
            //Update workload in the portal
            MRPWorkloadCRUDType _update_workload = new MRPWorkloadCRUDType();
            _update_workload.id = _workload.id;
            _update_workload.os_collection_status = true;
            _update_workload.os_collection_message = "Success";

            _update_workload.workloaddisks_attributes = mrpworkload.disks;
            _update_workload.workloadvolumes_attributes = mrpworkload.volumes;
            _update_workload.workloadinterfaces_attributes = mrpworkload.interfaces;
            _update_workload.workloadprocesses_attributes = mrpworkload.processes;
            _update_workload.workloadsoftwares_attributes = mrpworkload.softwares;

            _cloud_movey.workload().updateworkload(_update_workload);
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