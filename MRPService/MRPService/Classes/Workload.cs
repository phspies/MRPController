using MRPService.CloudMRP.Classes.Static_Classes;
using MRPService.Portal.Classes.Static_Classes;
using MRPService.Portal.Types.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.Portal.Controllers
{
    class CloudMRPWorkload
    {
        public static void workload_getinformation(MRPTaskType payload)
        {
            MRPTaskPayloadType _payload = payload.submitpayload;
            CloudMRPPortal CloudMRP = new CloudMRPPortal();
            try
            {
                string username = _payload.windows.username;
                string password = _payload.windows.password;
                string hostname = _payload.windows.ipaddress;
                string domain = _payload.windows.domain;

                ConnectionOptions connection = new ConnectionOptions();
                connection.Username = username;
                connection.Password = password;
                connection.Authority = "ntlmdomain:" + domain;
                String ipaddresslist = _payload.windows.ipaddress;
                ManagementScope scope = new ManagementScope();
                Exception error = new Exception();
                var workingip = Connection.find_working_ip(ipaddresslist, true);
                if (workingip != null)
                {
                    try
                    {
                        CloudMRP.task().progress(payload, "WMI Connect - trying " + workingip, 10);
                        scope = new ManagementScope("\\\\" + workingip.Trim() + "\\root\\CIMV2", connection);
                        scope.Connect();
                    }
                    catch (Exception e)
                    {
                        error = e;
                        CloudMRP.task().failcomplete(payload, "WMI Connect - " + workingip + " failed: " + e.Message);
                        return;
                    }
                } else
                {
                    CloudMRP.task().failcomplete(payload, "WMI Connect: No contatable IP found");
                    return;
                }

                JObject inventory = new JObject();
                int counter = 10;
                string[] wmielements = new string[] {"OperatingSystem", "Processor", "BIOS", "PhysicalMemory", "ComputerSystem"};
                foreach (string wmi in wmielements)
                {
                    counter += 10;
                    CloudMRP.task().progress(payload, "WMI " + wmi, counter);
                    ManagementPath path = new ManagementPath("Win32_" + wmi);
                    var devs = new ManagementClass(scope, path, null);
                    inventory.Add(wmi.ToLower(),sanitizemoc(wmi, devs.GetInstances()));
                }
 
                CloudMRP.task().progress(payload, "WMI Networking", counter+10);
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
                ManagementObjectSearcher moSearch = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection moCollection = moSearch.Get();
                JObject netinventory = new JObject();

                int index = 0;
                // Every record in this collection is a network interface
                foreach (ManagementObject mo in moCollection)
                {
                    JObject indexhwinventory = new JObject();
                    indexhwinventory.Add(new JProperty("network", index.ToString()));
                    foreach (PropertyData prop in mo.Properties)
                    {
                        indexhwinventory.Add(new JProperty(prop.Name, prop.Value));
                    }
                    netinventory.Add(new JProperty("network" + index.ToString(), indexhwinventory));
                    index += 1;
                }

                CloudMRP.task().progress(payload, "WMI Storage", counter+20);
                JObject storage = new JObject();
                ManagementPath diskpath = new ManagementPath("Win32_Diskdrive");
                int indexdisk = 0;
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
                                indexdisk += 1;
                            }
                        }
                    }
                }
                //Check to see if DT is installed
                CloudMRP.task().progress(payload, "WMI Software", counter + 30);

                int softwareindex = 0;

                JObject softwareinventory = new JObject();
                ObjectQuery softwarequery = new ObjectQuery("Select * from Win32_Product"); //where DisplayName like '%DT%'
                ManagementObjectSearcher softwaremoSearch = new ManagementObjectSearcher(scope, softwarequery);
                foreach (ManagementObject mo in softwaremoSearch.Get())
                {
                    JObject pkginventory = new JObject();
                    foreach (PropertyData prop in mo.Properties)
                    {
                        pkginventory.Add(new JProperty(prop.Name, prop.Value));
                    }
                    softwareinventory.Add(new JProperty(mo["Name"].ToString(), pkginventory));
                    softwareindex += 1;
                }
                inventory.Add(new JProperty("network", netinventory));
                inventory.Add(new JProperty("storage", storage));
                inventory.Add(new JProperty("software", softwareinventory));

                CloudMRP.task().successcomplete(payload, inventory.ToString(Formatting.None));

            }
            catch (ManagementException err)
            {
                CloudMRP.task().failcomplete(payload, err.ToString());
            }
            catch (System.UnauthorizedAccessException unauthorizedErr)
            {
                CloudMRP.task().failcomplete(payload, "Connection error (user name or password might be incorrect): " + unauthorizedErr.Message);
            }
            catch (Exception error)
            {
                CloudMRP.task().failcomplete(payload, error.Message);
            }
        }
        static JObject sanitizemoc(string wmi, ManagementObjectCollection moc)
        {
            JObject inventory = new JObject();
            int objects = 0;
            JObject storage = new JObject();

            foreach (ManagementObject mo in moc)
            {
                JObject interiminventory = new JObject();
                interiminventory.Add(new JProperty(wmi, objects.ToString()));
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
