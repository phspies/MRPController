using MRMPService.LocalDatabase;
using MRMPService.MRMPAPI.Contracts;
using System;
using System.Linq;
using MRMPService.Utilities;
using System.Collections.Generic;
using MRMPService.MRMPService.Log;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Threading.Tasks;

namespace MRMPService.NetstatCollection
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
        public static async Task WorkloadNetstatUnixDo(MRPWorkloadType _workload)
        {
            await Task.Run(async () =>
            {
                List<NetworkFlowType> _workload_netstats = new List<NetworkFlowType>();
                if (_workload == null)
                {
                    throw new ArgumentException("Inventory: Error finding workload in manager database");
                }
                MRPCredentialType _credential = _workload.credential;
                if (_credential == null)
                {
                    throw new ArgumentException(String.Format("Error finding credentials for workload {0} {1}", _workload.id, _workload.hostname));
                }
                _password = _credential.encrypted_password;

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


                KeyboardInteractiveAuthenticationMethod _keyboard_authentication = new KeyboardInteractiveAuthenticationMethod(_credential.username);
                _keyboard_authentication.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);
                PasswordAuthenticationMethod _password_authentication = new PasswordAuthenticationMethod(_credential.username, _password);
                ConnectionInfo ConnNfo = new ConnectionInfo(workload_ip, 22, _credential.username, new AuthenticationMethod[] { _keyboard_authentication, _password_authentication });


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
                                            _workload_netstats.Add(new NetworkFlowType()
                                            {
                                                id = Objects.RamdomGuid(),
                                                source_address = IPSplit.Parse(tokens[4]).Address.ToString(),
                                                source_port = IPSplit.Parse(tokens[4]).Port,
                                                target_address = IPSplit.Parse(tokens[3]).Address.ToString(),
                                                target_port = IPSplit.Parse(tokens[3]).Port,
                                                timestamp = DateTime.UtcNow,
                                                process = _processes.FirstOrDefault(x => x.pid == _pid).name,
                                                pid = _pid
                                            });
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
                await NetstatUpload.Upload(_workload_netstats, _workload);
                Logger.log(String.Format("Inventory: Completed netstat collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);

            });
        }
    }
}