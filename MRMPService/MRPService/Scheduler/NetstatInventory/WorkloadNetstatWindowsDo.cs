using MRMPService.LocalDatabase;
using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.IO;
using System.Linq;
using System.Management;
using MRMPService.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MRMPService.MRMPService.Log;
using System.Threading.Tasks;

namespace MRMPService.Scheduler.NetstatCollection
{
    partial class WorkloadNetstat
    {

        public static void WorkloadNetstatWindowsDo(MRMPWorkloadBaseType _workload)
        {
            string workload_ip = _workload.GetContactibleIP(true);
            Logger.log(String.Format("Netstat: Started netstat collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
            _workload.ExecuteProcessWMI(@"C:\", @"cmd.exe /c netstat -a -n -o > C:\netstat.out");
            List<ProcessInfo> _processes = new List<ProcessInfo>();
            foreach (ManagementObject queryObj in _workload.GetManagementObjectSearcher(new SelectQuery("SELECT * FROM Win32_Process")).Get())
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
                catch (Exception) { }
            }
            foreach (string row in _workload.ReadRemoteTXTWindows(@"\\" + workload_ip + @"\c$\netstat.out"))
            {
                string[] tokens = row.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 4 && (tokens[0].Equals("UDP") || tokens[0].Equals("TCP")))
                {
                    if (tokens[3] == "ESTABLISHED" || tokens[3] == "CLOSE_WAIT")
                    {
                        Int32 _pid = tokens[1] == "UDP" ? Int16.Parse(tokens[3]) : Int16.Parse(tokens[4]);
                        if (_processes.FirstOrDefault(x => x.pid == _pid) != null && tokens[2].Split(':')[0] != "0.0.0.0" && tokens[2].Split(':')[0].ToString() != "127.0.0.1")
                        {
                            try
                            {
                                using (NetworkFlowSet _ctx_netflow = new NetworkFlowSet())
                                {
                                    _ctx_netflow.ModelRepository.Insert(new NetworkFlow()
                                    {
                                        id = Objects.RamdomGuid(),
                                        source_address = IPSplit.Parse(tokens[1]).Address.ToString(),
                                        source_port = IPSplit.Parse(tokens[1]).Port,
                                        target_address = IPSplit.Parse(tokens[2]).Address.ToString(),
                                        target_port = IPSplit.Parse(tokens[2]).Port,
                                        protocol = tokens[0].Equals("TCP") ? 6 : 17,
                                        timestamp = DateTime.UtcNow,
                                    });
                                }
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }
            }
            Logger.log(String.Format("Inventory: Completed netstat collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}