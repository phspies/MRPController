﻿using CloudMoveyWorkerService.CloudMovey.Types.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Controllers
{
    class Server
    {
        public static void server_getinformation(MoveyTaskType payload)
        {
            MoveyTaskPayloadType _payload = payload.submitpayload;
            CloudMovey CloudMovey = new CloudMovey();
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
                Exception error = new Exception() ;
                var workingip = find_working_ip(ipaddresslist);
                if (workingip != null)
                {
                    try
                    {
                        CloudMovey.task().progress(payload, "WMI Connect - trying " + workingip, 10);
                        scope = new ManagementScope("\\\\" + workingip.Trim() + "\\root\\CIMV2", connection);
                        scope.Connect();
                    }
                    catch (Exception e)
                    {
                        error = e;
                        CloudMovey.task().progress(payload, "WMI Connect - " + workingip + " failed: " + e.Message, 10);
                    }
                }

                JObject inventory = new JObject();
                int counter = 10;
                string[] wmielements = new string[] {"OperatingSystem", "Processor", "BIOS", "PhysicalMemory", "ComputerSystem"};
                foreach (string wmi in wmielements)
                {
                    counter += 10;
                    CloudMovey.task().progress(payload, "WMI " + wmi, counter);
                    ManagementPath path = new ManagementPath("Win32_" + wmi);
                    var devs = new ManagementClass(scope, path, null);
                    inventory.Add(wmi.ToLower(),sanitizemoc(wmi, devs.GetInstances()));
                }
 
                CloudMovey.task().progress(payload, "WMI Networking", counter+10);
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

                CloudMovey.task().progress(payload, "WMI Storage", counter+20);
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
                CloudMovey.task().progress(payload, "WMI Software", counter + 30);

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

                CloudMovey.task().successcomplete(payload, inventory.ToString(Formatting.None));

            }
            catch (ManagementException err)
            {
                CloudMovey.task().failcomplete(payload, err.ToString());
            }
            catch (System.UnauthorizedAccessException unauthorizedErr)
            {
                CloudMovey.task().failcomplete(payload, "Connection error (user name or password might be incorrect): " + unauthorizedErr.Message);
            }
            catch (Exception error)
            {
                CloudMovey.task().failcomplete(payload, error.Message);
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
        private static string find_working_ip(string iplist)
        {
            foreach (string ip in iplist.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                Ping pinger = new Ping();
                try
                {
                    PingReply reply = pinger.Send(ip);
                    if (reply.Status == IPStatus.Success)
                    {
                        return ip;
                    }
                }
                catch (PingException)
                {
                    // Discard PingExceptions and return false;
                }
            }
            return null;
        }
    }
}
