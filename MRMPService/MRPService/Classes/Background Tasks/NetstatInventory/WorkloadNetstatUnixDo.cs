using MRMPService.LocalDatabase;
using MRMPService.MRMPAPI.Types.API;
using System;
using System.IO;
using System.Linq;
using System.Management;
using MRMPService.Utilities;
using System.Collections.Generic;
using MRMPService.MRMPService.Log;
using Renci.SshNet;

namespace MRMPService.MRMPAPI.Classes
{
    partial class WorkloadNetstat
    {

        public static void WorkloadNetstatUnixDo(MRPWorkloadType workload)
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


            ConnectionInfo ConnNfo = new ConnectionInfo(workload_ip, 22, _credential.username,
                 new AuthenticationMethod[] { new PasswordAuthenticationMethod(_credential.username, _credential.encrypted_password) }
             );
            List<ProcessInfo> _processes = new List<ProcessInfo>();
            List<String> netstatList = new List<string>();

            using (var sshclient = new SshClient(ConnNfo))
            {
                sshclient.Connect();
                using (var cmd = sshclient.CreateCommand("ps -e --no-headers"))
                {
                    cmd.Execute();
                    if (cmd.ExitStatus != 0)
                    {
                        sshclient.Disconnect();
                        throw new ArgumentException(String.Format("Error while running process list command: {0}", cmd.Result));
                    }
                    foreach (string _line in cmd.Result.GetLines(true))
                    {
                        string[] tokens = _line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        _processes.Add(new ProcessInfo() { pid = Int16.Parse(tokens[0]), name = tokens[3] });
                    }
                }
                using (var cmd = sshclient.CreateCommand("netstat -atnp"))
                {
                    cmd.Execute();
                    if (cmd.ExitStatus != 0)
                    {
                        sshclient.Disconnect();
                        throw new ArgumentException(String.Format("Error while running netstat command: {0}", cmd.Result));
                    }
                    foreach (string _line in cmd.Result.GetLines(true))
                    {
                        string[] tokens = _line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length == 7 && (tokens[0] == "udp" || tokens[0] == "tcp"))
                        {
                            if (tokens[5] == "ESTABLISHED" || tokens[5] == "CLOSE_WAIT")
                            {
                                int _pid = Int16.Parse(tokens[6].Split('/').First());
                                if (_processes.FirstOrDefault(x => x.pid == _pid) != null && tokens[3].Split(':')[0] != "0.0.0.0" && tokens[4].Split(':')[0].ToString() != "127.0.0.1")
                                {
                                    using (NetstatSet _netstat_db = new NetstatSet())
                                    {
                                        _netstat_db.ModelRepository.Insert(new Netstat()
                                        {
                                            id = Objects.RamdomGuid(),
                                            workload_id = workload.id,
                                            proto = tokens[0],
                                            pid = _pid,
                                            process = _processes.FirstOrDefault(x => x.pid == _pid).name,
                                            source_ip = IPSplit.Parse(tokens[4]).Address.ToString(),
                                            source_port = IPSplit.Parse(tokens[4]).Port,
                                            target_ip = IPSplit.Parse(tokens[3]).Address.ToString(),
                                            target_port = IPSplit.Parse(tokens[3]).Port,
                                            state = tokens[5]
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                sshclient.Disconnect();
            }
            Logger.log(String.Format("Inventory: Completed netstat collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}