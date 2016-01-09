using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.CloudMovey.Classes.Static_Classes;
using CloudMoveyWorkerService.LocalDatabase;
using CloudMoveyWorkerService.Portal.Types.API;
using CloudMoveyWorkerService.WCF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace CloudMoveyWorkerService.Portal.Classes
{
    class OSInventoryWorker
    {
        CloudMoveyPortal _cloud_movey = new CloudMoveyPortal();
        public void Start()
        {
            CloudMoveyPortal _cloud_movey = new CloudMoveyPortal();
            LocalDB db = new LocalDB();

            while (true)
            {
                Stopwatch sw = Stopwatch.StartNew();
                int _new_credentials, _new_platforms, _new_platformnetworks, _new_workloads, _updated_credentials, _updated_platforms, _updated_platformnetworks, _updated_workloads;
                _new_credentials = _new_platforms = _new_platformnetworks = _new_workloads = _updated_credentials = _updated_platformnetworks = _updated_platforms = _updated_workloads = 0;



                    Global.event_log.WriteEntry("Staring operating system inventory process");

                    MoveyWorkloadListType _currentplatformworkloads = _cloud_movey.workload().listworkloads();
                    foreach (MoveyWorkloadType _workload in _currentplatformworkloads.workloads)
                    {
                        try
                        {
                            Workload __workload = db.Workloads.FirstOrDefault(x => x.id == _workload.id);
                            string workload_ip = Connection.find_working_ip(__workload, true);
                            Credential _credential = db.Credentials.FirstOrDefault(x => x.id == _workload.credential_id);

                            ConnectionOptions options = ProcessConnectionOptions();

                            options.Username = (String.IsNullOrWhiteSpace(_credential.domain) ? "." : _credential.domain) + "\\" + _credential.username;
                            options.Password = _credential.password;

                            ManagementScope connectionScope = ConnectionScope(workload_ip, options);

                            //process running processes
                            SelectQuery msProcessQuery = new SelectQuery("SELECT * FROM Win32_Process");
                            ManagementObjectSearcher searchProcessProcedure = new ManagementObjectSearcher(connectionScope, msProcessQuery);


                            foreach (ManagementObject item in searchProcessProcedure.Get())
                            {
                                MoveyWorkloadProcessType _process;

                                //if procces already exists in portal, just update it   
                                if (_workload.processes.Exists(x => x.caption == item["Caption"].ToString()))
                                {
                                    _process = _workload.processes.FirstOrDefault(x => x.caption == item["Caption"].ToString());
                                }
                                else
                                {
                                    _process = new MoveyWorkloadProcessType();
                                    _workload.processes.Add(_process);
                                }

                                try { _process.caption = item["Caption"].ToString(); } catch (Exception) { }
                                try { _process.commandline = item["CommandLine"].ToString(); } catch (Exception) { }
                                try { _process.name = item["Name"].ToString(); } catch (Exception) { }
                                try { _process.processid = Int16.Parse(item["ProcessId"].ToString()); } catch (Exception) { }
                                try { _process.writeoperationcount = Int64.Parse(item["WriteOperationCount"].ToString()); } catch (Exception) { }
                                try { _process.writetransfercount = Int64.Parse(item["WriteTransferCount"].ToString()); } catch (Exception) { }
                                try { _process.readoperationcount = Int64.Parse(item["ReadOperationCount"].ToString()); } catch (Exception) { }
                                try { _process.readtransfercount = Int64.Parse(item["ReadTransferCount"].ToString()); } catch (Exception) { }
                                try { _process.threadcount = Int16.Parse(item["ThreadCount"].ToString()); } catch (Exception) { }
                                try { _process.virtualsize = Int16.Parse(item["ThreadCount"].ToString()); } catch (Exception) { }
                            }

                            //process installed software
                            SelectQuery msSoftwareQuery = new SelectQuery("SELECT * FROM Win32_Product");
                            ManagementObjectSearcher searchSoftwareProcedure = new ManagementObjectSearcher(connectionScope, msSoftwareQuery);


                            foreach (ManagementObject item in searchSoftwareProcedure.Get())
                            {
                                MoveyWorkloadSoftwareType _software;

                                //if procces already exists in portal, just update it   
                                if (_workload.softwares.Exists(x => x.name == item["Name"].ToString()))
                                {
                                    _software = _workload.softwares.FirstOrDefault(x => x.name == item["Name"].ToString());
                                }
                                else
                                {
                                    _software = new MoveyWorkloadSoftwareType();
                                    _workload.softwares.Add(_software);
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
                            _workload.volumes.ForEach(x => x._destroy = 1);
                            _workload.disks.ForEach(x => x._destroy = 1);

                            SelectQuery wmiDiskDrives = new SelectQuery("SELECT * FROM Win32_DiskDrive");
                            ManagementObjectSearcher searchDiskProcedure = new ManagementObjectSearcher(connectionScope, wmiDiskDrives);

                            foreach (ManagementObject wmiDiskDrive in searchDiskProcedure.Get())
                            {

                                MoveyWorkloadDiskType _disk;

                                //if volume already exists in portal, just update it   
                                if (_workload.disks.Exists(x => x.diskindex == Int16.Parse(wmiDiskDrive["Index"].ToString())))
                                {
                                    _disk = _workload.disks.FirstOrDefault(x => x.diskindex == Int16.Parse(wmiDiskDrive["Index"].ToString()));
                                    _disk._destroy = 0;
                                }
                                else
                                {
                                    _disk = new MoveyWorkloadDiskType();
                                    _workload.disks.Add(_disk);
                                }

                                try { _disk.disksize = Int16.Parse(wmiDiskDrive["Size"].ToString()); } catch (Exception) { }
                                try { _disk.deviceid = wmiDiskDrive["DeviceID"].ToString(); } catch (Exception) { }


                                foreach (ManagementObject wmiPartitionDrive in wmiDiskDrive.GetRelated("Win32_DiskPartition"))
                                {

                                    foreach (ManagementObject wmiLogicalDrive in wmiPartitionDrive.GetRelated("Win32_LogicalDisk"))
                                    {
                                        SelectQuery wmiVolumes = new SelectQuery("SELECT * FROM Win32_Volume where DriveLetter='" + wmiLogicalDrive["DeviceId"] + "'");
                                        ManagementObjectSearcher searchVolumes = new ManagementObjectSearcher(connectionScope, wmiVolumes);
                                        foreach (ManagementObject wmiVolume in searchVolumes.Get())
                                        {


                                            MoveyWorkloadVolumeType _volume;

                                            //if volume already exists in portal, just update it   
                                            if (_workload.volumes.Exists(x => x.serialnumber == wmiVolume["SerialNumber"].ToString()))
                                            {
                                                _volume = _workload.volumes.FirstOrDefault(x => x.serialnumber == wmiVolume["SerialNumber"].ToString());
                                                _volume._destroy = 0;
                                            }
                                            else
                                            {
                                                _volume = new MoveyWorkloadVolumeType();
                                                _workload.volumes.Add(_volume);
                                            }
                                            try { _volume.diskindex = Int16.Parse(wmiDiskDrive["Index"].ToString()); } catch (Exception) { }
                                            try { _volume.driveletter = wmiVolume["DriveLetter"].ToString(); } catch (Exception) { }
                                            try { _volume.serialnumber = wmiVolume["SerialNumber"].ToString(); } catch (Exception) { }
                                            try { _volume.blocksize = Int16.Parse(wmiVolume["BlockSize"].ToString()); } catch (Exception) { }
                                            try { _volume.volumename = wmiVolume["Label"].ToString(); } catch (Exception) { }
                                            try { _volume.deviceid = wmiVolume["DeviceID"].ToString(); } catch (Exception) { }
                                            try { _volume.volumefreespace = Int64.Parse(wmiVolume["FreeSpace"].ToString()); } catch (Exception) { }
                                            try { _volume.volumesize = Int64.Parse(wmiVolume["Capacity"].ToString()); } catch (Exception) { }
                                            try { _volume.provisioned = true; } catch (Exception) { }
                                        }
                                    }
                                }
                            }


                            //process network interfaces
                            SelectQuery wmiNetInterfaces = new SelectQuery("select * from Win32_NetworkAdapterConfiguration where IPEnabled = 'True'");
                            ManagementObjectSearcher searchNetInterfacesConfig = new ManagementObjectSearcher(connectionScope, wmiNetInterfaces);
                            foreach (ManagementObject searchNetInterfaceConfig in searchNetInterfacesConfig.Get())
                            {
                                foreach (ManagementObject searchNetInterface in searchNetInterfaceConfig.GetRelated("Win32_NetworkAdapter"))
                                {
                                    MoveyWorkloadInterfaceType _interface;

                                    String[] addresses = (String[])searchNetInterfaceConfig["IPAddress"];
                                    String[] netmask = (String[])searchNetInterfaceConfig["IPSubnet"];

                                    //if interface already exists in portal, just update it   
                                    if (_workload.interfaces.Exists(x => x.ipaddress == addresses.FirstOrDefault(s => s.Contains('.'))))
                                    {
                                        _interface = _workload.interfaces.FirstOrDefault(x => x.ipaddress == addresses.FirstOrDefault(s => s.Contains('.')));
                                    }
                                    else
                                    {
                                        _interface = new MoveyWorkloadInterfaceType();
                                        _workload.interfaces.Add(_interface);
                                    }
                                    _interface.ipaddress = addresses.FirstOrDefault(s => s.Contains('.'));
                                    _interface.ipv6address = addresses.FirstOrDefault(s => s.Contains(':'));
                                    _interface.netmask = netmask.FirstOrDefault(s => s.Contains('.'));
                                    _interface.ipv6netmask = netmask.FirstOrDefault(s => s.Contains(':'));
                                    try { _interface.connection_index = Int16.Parse(searchNetInterfaceConfig["Index"].ToString()); } catch (Exception) { }
                                    try { _interface.connection_id = searchNetInterface["NetConnectionID"].ToString(); } catch (Exception) { }
                                }

                            }
                            //Update workload in the portal
                            MoveyWorkloadCRUDType _update_workload = new MoveyWorkloadCRUDType();
                            _update_workload.id = _workload.id;
                            _update_workload.workloaddisks_attributes = _workload.disks;
                            _update_workload.workloadvolumes_attributes = _workload.volumes;
                            _update_workload.workloadinterfaces_attributes = _workload.interfaces;
                            _update_workload.workloadprocesses_attributes = _workload.processes;
                            _update_workload.workloadsoftwares_attributes = _workload.softwares;

                            _cloud_movey.workload().updateworkload(_update_workload);

                    }
                    catch (Exception ex)
                    {
                        Global.event_log.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                    }
                }



                sw.Stop();

                Global.event_log.WriteEntry(
                    String.Format("Completed data mirroring process.{6}{0} new credentials.{6}{1} new platforms.{6}{7} new platform networks.{6}{2} new workloads.{6}{3} updated credentials.{6}{4} updated platforms.{6}{8} updated platform networks.{6}{5} updated workloads.{6}{6}Total Execute Time: {9}",
                    _new_credentials, _new_platforms, _new_workloads, _updated_credentials, _updated_platforms, _updated_workloads,
                    Environment.NewLine, _new_platformnetworks, _updated_platformnetworks, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)
                    ));

                Thread.Sleep(new TimeSpan(24, 0, 0));

            }

        }
    
        private static string TranslateMemoryUsage(string workingSet)
        {
            int calc = Convert.ToInt32(workingSet);
            calc = calc / 1024;
            return calc.ToString();
        }
        private static ConnectionOptions ProcessConnectionOptions()
        {
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Default;
            options.EnablePrivileges = true;
            return options;
        }

        private static ManagementScope ConnectionScope(string machineName, ConnectionOptions options)
        {
            ManagementScope connectScope = new ManagementScope();
            connectScope.Path = new ManagementPath(@"\\" + machineName + @"\root\CIMV2");
            connectScope.Options = options;

            try
            {
                connectScope.Connect();
            }
            catch (ManagementException e)
            {
                Console.WriteLine("An Error Occurred: " + e.Message.ToString());
            }
            return connectScope;
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