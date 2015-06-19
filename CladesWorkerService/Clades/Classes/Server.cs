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
            string error;

            try
            {
                string username = payload.payload.windows.username;
                string password = payload.payload.windows.password;
                string hostname = payload.payload.windows.hostname;
                string domain = payload.payload.windows.domain;
                ConnectionOptions connection = new ConnectionOptions();
                connection.Username = username;
                connection.Password = password;
                connection.Authority = "ntlmdomain:" + domain;

                ManagementScope scope = new ManagementScope("\\\\" + hostname + "\\root\\CIMV2", connection);

                scope.Connect();

                ManagementPath ospath = new ManagementPath("Win32_OperatingSystem");
                var osdevs = new ManagementClass(scope, ospath, null);
                ManagementObjectCollection osmoc = osdevs.GetInstances();
                JObject osinventory = new JObject();
                foreach (ManagementObject mo in osmoc)
                {
                    foreach (PropertyData prop in mo.Properties)
                    {
                        osinventory.Add(new JProperty(prop.Name, prop.Value));
                    }
                }

                ManagementPath hwpath = new ManagementPath("Win32_ComputerSystem");
                var hwdevs = new ManagementClass(scope, hwpath, null);
                ManagementObjectCollection hwmoc = hwdevs.GetInstances();
                JObject hwinventory = new JObject();
                foreach (ManagementObject mo in osmoc)
                {
                    foreach (PropertyData prop in mo.Properties)
                    {
                        hwinventory.Add(new JProperty(prop.Name, prop.Value));
                    }
                }

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
                    //indexhwinventory.Add(new JProperty("IPAddress", (string[])mo["IPAddress"]));
                    //indexhwinventory.Add(new JProperty("IPSubnet", (string[])mo["IPSubnet"]));
                    //indexhwinventory.Add(new JProperty("DefaultIPGateway", (string[])mo["DefaultIPGateway"]));
                    netinventory.Add(new JProperty("network" + index.ToString(), indexhwinventory));
                    index += 1;
                }

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
                                JObject inventory = new JObject(
                                    new JProperty("operatingsystem", osinventory),
                                    new JProperty("hardware", hwinventory),
                                    new JProperty("network", netinventory),
                                    new JProperty("storage",
                                        new JObject(
                                            new JProperty("logicaldisk"+indexdisk.ToString(), logicalinventory)
                                        )
                                    )
                                );
                                TaskUpdateObject task = new TaskUpdateObject() { 
                                    id = Global.agentId, 
                                    hostname = Environment.MachineName, 
                                    task_id = payload.id, 
                                    attributes = new TaskUpdateAttriubutes() {
                                        status = 0,
                                        returnpayload = inventory.ToString(Formatting.None) 
                                    } 
                                };
                                clades.task().update(task);
                            }
                        }
                    }
                }
            }
            catch (ManagementException err)
            {
                error = err.ToString();
                TaskUpdateObject task = new TaskUpdateObject() { id = Global.agentId, hostname = Environment.MachineName, task_id = payload.id, attributes = new TaskUpdateAttriubutes() { status = 2, returnpayload = error } };
                object returnval = clades.task().update(task);
            }
            catch (System.UnauthorizedAccessException unauthorizedErr)
            {
                error = "Connection error (user name or password might be incorrect): " + unauthorizedErr.Message;
                TaskUpdateObject task = new TaskUpdateObject() { id = Global.agentId, hostname = Environment.MachineName, task_id = payload.id, attributes = new TaskUpdateAttriubutes() { status = 2, returnpayload = error } };
                object returnval = clades.task().update(task);
            }

        }
    }
}
