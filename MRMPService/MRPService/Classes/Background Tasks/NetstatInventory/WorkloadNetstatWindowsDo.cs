using MRMPService.LocalDatabase;
using MRMPService.MRMPAPI.Types.API;
using System;
using System.IO;
using System.Linq;
using System.Management;
using MRMPService.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MRMPService.MRMPService.Log;

namespace MRMPService.MRMPAPI.Classes
{
    partial class WorkloadNetstat
    {

        public static void WorkloadNetstatWindowsDo(MRPWorkloadType workload)
        {
            MRPWorkloadType _workload = workload;
            if (_workload == null)
            {
                throw new ArgumentException("Inventory: Error finding workload in manager database");
            }
            MRPCredentialType _credential = _workload.credential;
            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Error finding credentials for workload {0} {1}", _workload.id, _workload.hostname));
            }

            string workload_ip = null;
            using (Connection _connection = new Connection())
            {
                workload_ip = _connection.FindConnection(_workload.iplist, true);
            }
            if (workload_ip == null)
            {
                throw new ArgumentException(String.Format("Error contacting workload for workload {0} {1}", _workload.id, _workload.hostname));
            }
            Logger.log(String.Format("Netstat: Started netstat collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);


            using (new Impersonator(_credential.username, (String.IsNullOrWhiteSpace(_credential.domain) ? "." : _credential.domain), _credential.encrypted_password))
            {
                ConnectionOptions options = WMIHelper.ProcessConnectionOptions(((String.IsNullOrWhiteSpace(_credential.domain) ? "." : _credential.domain) + "\\" + _credential.username), _credential.encrypted_password);
                ManagementScope connectionScope = WMIHelper.ConnectionScope(workload_ip, options);

                string remoteFile = @"\\" + workload_ip + @"\c$\netstat.out";
                string netstatCmd = @"cmd.exe /c netstat -a -n -o > C:\netstat.out";
                Dictionary<string, string> netstatCmdParams = new Dictionary<string, string>();
                netstatCmdParams["CommandLine"] = netstatCmd;

                ManagementPath wmiObjectPath = new ManagementPath("Win32_Process");
                ObjectGetOptions ogo = new ObjectGetOptions();
                ManagementBaseObject returnValue;

                using (ManagementClass mc = new ManagementClass(connectionScope, wmiObjectPath, ogo))
                {
                    ManagementBaseObject inparams = mc.GetMethodParameters("Create");

                    if (netstatCmdParams != null)
                    {
                        foreach (var p in netstatCmdParams)
                        {
                            inparams[p.Key] = p.Value;
                        }
                    }

                    returnValue = mc.InvokeMethod("Create", inparams, null);

                }
                int processId = 0;
                if (returnValue != null)
                {
                    processId = Convert.ToInt32(returnValue.Properties["ProcessId"].Value);
                }

                if (processId == 0)
                {
                    throw new ArgumentException(String.Format("Error running netstat on {0} {1}", _workload.id, _workload.hostname));
                }
                //Wait for the process to complete
                Process process = new Process();

                while (true)
                {
                    try
                    {
                        process = Process.GetProcessById(processId, workload_ip);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }

                List<ProcessInfo> _processes = new List<ProcessInfo>();
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, new ObjectQuery("SELECT * FROM Win32_Process"));

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    try
                    {
                        string name = queryObj["Name"].ToString();
                        string command = "";
                        if (queryObj["CommandLine"] != null)
                        {
                            command = queryObj["CommandLine"].ToString();
                        }

                        int pid = Int16.Parse(queryObj["ProcessId"].ToString());
                        _processes.Add(new ProcessInfo() { name = name, command = command, pid = pid });
                    }
                    catch (Exception ex) { }
                }


                //get remote netstat file
                List<String> netstatList;
                var logFile = File.ReadAllLines(remoteFile);
                netstatList = new List<String>(logFile);

                foreach (string row in netstatList)
                {
                    string[] tokens = row.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length > 4 && (tokens[0].Equals("UDP") || tokens[0].Equals("TCP")))
                    {
                        if (tokens[3] == "ESTABLISHED" || tokens[3] == "CLOSE_WAIT")
                        {
                            int _pid = tokens[1] == "UDP" ? Int16.Parse(tokens[3]) : Int16.Parse(tokens[4]);
                            if (_processes.FirstOrDefault(x => x.pid == _pid) != null && tokens[2].Split(':')[0] != "0.0.0.0" && tokens[2].Split(':')[0].ToString() != "127.0.0.1")
                            {
                                using (NetstatSet _netstat_db = new NetstatSet())
                                {
                                    try
                                    {
                                        _netstat_db.ModelRepository.Insert(new Netstat()
                                        {
                                            id = Objects.RamdomGuid(),
                                            workload_id = workload.id,
                                            proto = tokens[0],
                                            pid = _pid,
                                            process = _processes.FirstOrDefault(x => x.pid == _pid).name,
                                            source_ip = IPSplit.Parse(tokens[1]).Address.ToString(),
                                            source_port = IPSplit.Parse(tokens[1]).Port,
                                            target_ip = IPSplit.Parse(tokens[2]).Address.ToString(),
                                            target_port = IPSplit.Parse(tokens[2]).Port,
                                            state = tokens[3]
                                        });
                                    }
                                    catch (Exception ex)
                                    { }
                                }
                            }
                        }
                    }
                }
                Logger.log(String.Format("Inventory: Completed netstat collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);

            }
        }
    }
}