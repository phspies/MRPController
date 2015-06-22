using CladesWorkerService.Clades.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace CladesWorkerService.Clades.Controllers
{
    class Server
    {
        public static void server_getinformation(dynamic payload)
        {
            CladesWorkerService.Clades.Clades clades = new CladesWorkerService.Clades.Clades(Global.apiBase, null, null);
            TasksObject tasks = new TasksObject(clades);
            try
            {
                clades.task().progress(payload, "WMI Connect", 10);
                string username = payload.payload.windows.username;
                string password = payload.payload.windows.password;
                string hostname = payload.payload.windows.ipaddress;
                string domain = payload.payload.windows.domain;
                ConnectionOptions connection = new ConnectionOptions();
                connection.Username = username;
                connection.Password = password;
                connection.Authority = "ntlmdomain:" + domain;
                ManagementScope scope = new ManagementScope("\\\\" + hostname + "\\root\\CIMV2", connection);
                scope.Connect();

                JObject inventory = new JObject();
                int counter = 10;
                string[] wmielements = new string[] {"OperatingSystem", "Processor", "BIOS", "PhysicalMemory", "ComputerSystem"};
                foreach (string wmi in wmielements)
                {
                    counter += 10;
                    clades.task().progress(payload, "WMI " + wmi, counter);
                    ManagementPath path = new ManagementPath("Win32_" + wmi);
                    var devs = new ManagementClass(scope, path, null);
                    inventory.Add(wmi.ToLower(),sanitizemoc(wmi, devs.GetInstances()));
                }
                //clades.task().progress(payload, "WMI Operating System", 20);
                //ManagementPath ospath = new ManagementPath("Win32_OperatingSystem");
                //var osdevs = new ManagementClass(scope, ospath, null);
                //JObject osinventory = sanitizemoc(osdevs.GetInstances());


                //clades.task().progress(payload, "WMI CPU", 30);
                //ManagementPath bicpupath = new ManagementPath("Win32_Processor");
                //var cpudevs = new ManagementClass(scope, bicpupath, null);
                //JObject cpuinventory = sanitizemoc(cpudevs.GetInstances());

                //clades.task().progress(payload, "WMI Bios", 30);
                //ManagementPath biospath = new ManagementPath("Win32_BIOS");
                //var biosdevs = new ManagementClass(scope, biospath, null);
                //JObject biosinventory = sanitizemoc(biosdevs.GetInstances());

                //clades.task().progress(payload, "WMI Memory", 30);
                //ManagementPath mempath = new ManagementPath("Win32_PhysicalMemory");
                //var memdevs = new ManagementClass(scope, mempath, null);
                //ManagementObjectCollection memmoc = memdevs.GetInstances();
                //JObject meminventory = new JObject();
                //foreach (ManagementObject mo in memmoc)
                //{
                //    foreach (PropertyData prop in mo.Properties)
                //    {
                //        meminventory = sanitize(meminventory, prop);
                //    }
                //}

                //clades.task().progress(payload, "WMI Hardware", 30);
                //ManagementPath hwpath = new ManagementPath("Win32_ComputerSystem");
                //var hwdevs = new ManagementClass(scope, hwpath, null);
                //ManagementObjectCollection hwmoc = hwdevs.GetInstances();
                //JObject hwinventory = new JObject();
                //foreach (ManagementObject mo in hwmoc)
                //{
                //    foreach (PropertyData prop in mo.Properties)
                //    {
                //        hwinventory = sanitize(hwinventory, prop);
                //    }
                //}

                clades.task().progress(payload, "WMI Networking", counter+10);
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
                ManagementObjectSearcher moSearch = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection moCollection = moSearch.Get();
                JObject netinventory = new JObject();

                int index = 0;
                // Every record in this collection is a network interface
                foreach (ManagementObject mo in moCollection)
                {
                    JObject indexhwinventory = new JObject();

                    foreach (PropertyData prop in mo.Properties)
                    {
                        indexhwinventory.Add(new JProperty(prop.Name, prop.Value));
                    }
                    netinventory.Add(new JProperty("network" + index.ToString(), indexhwinventory));
                    index += 1;
                }

                clades.task().progress(payload, "WMI Storage", counter+20);
                JObject storage = new JObject();
                ManagementPath diskpath = new ManagementPath("Win32_Diskdrive");
                using (var devs = new ManagementClass(scope, diskpath, null))
                {
                    ManagementObjectCollection moc = devs.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        JObject diskinventory = new JObject();
                        foreach (PropertyData prop in mo.Properties)
                        {
                            diskinventory.Add(new JProperty(prop.Name, prop.Value));
                        }
                        foreach (ManagementObject b in mo.GetRelated("Win32_DiskPartition"))
                        {
                            JObject partinventory = new JObject();
                            foreach (PropertyData prop in b.Properties)
                            {
                                partinventory.Add(new JProperty(prop.Name, prop.Value));
                            }
                            int indexdisk = 0;
                            foreach (ManagementBaseObject c in b.GetRelated("Win32_LogicalDisk"))
                            {
                                JObject logicalinventory = new JObject();
                                foreach (PropertyData prop in c.Properties)
                                {
                                    logicalinventory.Add(new JProperty(prop.Name, prop.Value));
                                }
                                partinventory.Add(new JProperty("PhysicalDisk", diskinventory));
                                logicalinventory.Add(new JProperty("DiskPartition", partinventory));
                                storage.Add(new JProperty("logicaldisk" + indexdisk.ToString(), logicalinventory));
                            }
                        }
                    }
                }
                inventory.Add(new JProperty("network", netinventory));
                inventory.Add(new JProperty("storage", storage));
                clades.task().successcomplete(payload, inventory.ToString(Formatting.None));

            }
            catch (ManagementException err)
            {
                clades.task().failcomplete(payload, err.ToString());
            }
            catch (System.UnauthorizedAccessException unauthorizedErr)
            {
                clades.task().failcomplete(payload, "Connection error (user name or password might be incorrect): " + unauthorizedErr.Message);
            }
            //catch (Exception error)
            //{
            //    clades.task().failcomplete(payload, error.Message);
            //}
        }
        static JObject sanitizemoc(string wmi, ManagementObjectCollection moc)
        {
            JObject inventory = new JObject();
            int objects = 0;
            JObject storage = new JObject();

            foreach (ManagementObject mo in moc)
            {
                JObject interiminventory = new JObject();
                foreach (PropertyData prop in mo.Properties)
                {
                    interiminventory.Add(new JProperty(prop.Name, prop.Value));
                }
                inventory.Add(new JProperty(wmi + objects.ToString(), interiminventory));
                objects += 1;
            }
            return inventory;
        }
    }
}
