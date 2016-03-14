using MRPService.LocalDatabase;
using MRPService.API.Types.API;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization.Formatters.Binary;
using MRPService.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MRPService.API.Classes
{
    class WorkloadNetstat
    {
        ApiClient _cloud_movey = new ApiClient();

        public static void WorkloadNetstatDo(String workload_id)
        {
            WorkloadSet dbworkload = new WorkloadSet();
            CredentialSet dbcredential = new CredentialSet();
            ApiClient _cloud_movey = new ApiClient();

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

            using (new Impersonator(_credential.username, (String.IsNullOrWhiteSpace(_credential.domain) ? "." : _credential.domain), _credential.password))
            {
                ConnectionOptions options = WMIHelper.ProcessConnectionOptions();

                options.Username = (String.IsNullOrWhiteSpace(_credential.domain) ? "." : _credential.domain) + "\\" + _credential.username;
                options.Password = _credential.password;

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
                    throw new ArgumentException(String.Format("Error running netstat on {0} {1}", workload_id, _workload.hostname));
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
                        _processes.Add(new ProcessInfo() { name = queryObj["Name"].ToString(), command = queryObj["CommandLine"].ToString(), pid = Int16.Parse(queryObj["ProcessId"].ToString()) });
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
                                    _netstat_db.ModelRepository.Insert(new Netstat()
                                    {
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
                            }
                        }
                    }
                }
            }
        }
    }
}