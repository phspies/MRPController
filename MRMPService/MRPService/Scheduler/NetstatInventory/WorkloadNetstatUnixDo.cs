using MRMPService.LocalDatabase;
using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Linq;
using MRMPService.Utilities;
using System.Collections.Generic;
using MRMPService.MRMPService.Log;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Threading.Tasks;

namespace MRMPService.Scheduler.NetstatCollection
{
    partial class WorkloadNetstat
    {
        static string _password;
        static private void HandleKeyEvent(object sender, AuthenticationPromptEventArgs e)
        {
            foreach (AuthenticationPrompt prompt in e.Prompts)
            {
                if (prompt.Request.IndexOf("Password:", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    prompt.Response = _password;
                }
            }
        }
        public static void WorkloadNetstatUnixDo(MRMPWorkloadBaseType _workload)
        {

            List<NetworkFlowType> _workload_netstats = new List<NetworkFlowType>();
            _password = _workload.GetCredentials().decrypted_password;
            string workload_ip = _workload.GetContactibleIP(true);
            Logger.log(String.Format("Netstat: Started netstat collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);


            KeyboardInteractiveAuthenticationMethod _keyboard_authentication = new KeyboardInteractiveAuthenticationMethod(_workload.GetCredentials().username);
            _keyboard_authentication.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);
            PasswordAuthenticationMethod _password_authentication = new PasswordAuthenticationMethod(_workload.GetCredentials().username, _password);
            ConnectionInfo ConnNfo = new ConnectionInfo(workload_ip, 22, _workload.GetCredentials().username, new AuthenticationMethod[] { _keyboard_authentication, _password_authentication });


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

                                    try
                                    {
                                        using (NetworkFlowSet _ctx_netflow = new NetworkFlowSet())
                                        {
                                            _ctx_netflow.ModelRepository.Insert(new NetworkFlow()
                                            {
                                                id = Objects.RamdomGuid(),
                                                source_address = IPSplit.Parse(tokens[4]).Address.ToString(),
                                                source_port = IPSplit.Parse(tokens[4]).Port,
                                                target_address = IPSplit.Parse(tokens[3]).Address.ToString(),
                                                target_port = IPSplit.Parse(tokens[3]).Port,
                                                protocol = tokens[0].Equals("tcp") ? 6 : 17,
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
                }
                sshclient.Disconnect();
            }
            Logger.log(String.Format("Inventory: Completed netstat collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}