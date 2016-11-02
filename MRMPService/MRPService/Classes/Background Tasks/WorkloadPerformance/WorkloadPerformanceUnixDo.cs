using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using MRMPService.Utilities;
using MRMPService.MRMPAPI.Types.API;
using Renci.SshNet.Common;
using Renci.SshNet;
using System.IO;
using Renci.SshNet.Sftp;

namespace MRMPService.PerformanceCollection
{
    partial class WorkloadPerformance : IDisposable
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
        public static void WorkloadPerformanceUnixDo(MRPWorkloadType _workload)
        {
            #region load and check workload information
            //check for credentials
            MRPCredentialType _credential = _workload.credential;
            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Error finding credentials"));
            }
            _password = _credential.encrypted_password;

            //check for working IP
            string workload_ip = null;
            using (Connection _connection = new Connection())
            {
                workload_ip = _connection.FindConnection(_workload.iplist, false);
            }
            if (workload_ip == null)
            {
                throw new ArgumentException(String.Format("Error contacting workload"));
            }
            #endregion

            Logger.log(String.Format("Performance: Start performance collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
            KeyboardInteractiveAuthenticationMethod _keyboard_authentication = new KeyboardInteractiveAuthenticationMethod(_credential.username);
            _keyboard_authentication.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);
            PasswordAuthenticationMethod _password_authentication = new PasswordAuthenticationMethod(_credential.username, _password);
            ConnectionInfo ConnNfo = new ConnectionInfo(workload_ip, 22, _credential.username, new AuthenticationMethod[] { _keyboard_authentication, _password_authentication });

            string locationlocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string localscripts = Path.Combine(locationlocation, "Scripts");

            string remotepath = @"mrmp";
            bool remotepath_exist = false;

            using (var sftp = new SftpClient(ConnNfo))
            {
                sftp.Connect();
                try
                {
                    sftp.ChangeDirectory(remotepath);
                    remotepath_exist = true;
                }
                catch (SftpPathNotFoundException)
                {
                    Logger.log(String.Format("MRMP scripts are not present on workload {0} {1}. Inventory process needs to setup scripts first!", _workload.hostname, workload_ip), Logger.Severity.Info);
                }
                if (remotepath_exist)
                {
                    if (sftp.Exists("output"))
                    {
                        foreach (SftpFile _file in sftp.ListDirectory("output"))
                        {
                            if (_file.Name.StartsWith("Perf_"))
                            {
                                //get OS edition name
                                List<String> _perf_file = sftp.ReadAllLines(_file.FullName).ToList();

                                DateTime _utc_time = new DateTime();
                                String _datetime_string = _perf_file.FirstOrDefault(x => x.Contains("DATE=")).Split('=').Last();
                                DateTime.TryParse(_datetime_string, out _utc_time);

                                //Remove Inventory file from server
                                sftp.DeleteFile(_file.FullName);

                                //Process Disks
                                for (var i = 0; i < _perf_file.Count; i++)
                                {
                                    if (_perf_file[i] == "<PERF>")
                                    {
                                        Performance _perf = new Performance();
                                        bool _valid_perf = false;
                                        while (_perf_file[i] != "</PERF>")
                                        {
                                            i++;
                                            if (_perf_file[i].Contains("PERS_ClassName"))
                                            {
                                                _perf.category_name = _perf_file[i].Split('=').Last();
                                                _valid_perf = (_perf.category_name == "Process") ? false : true;
                                            }
                                            if (_perf_file[i].Contains("PERS_InstanceName"))
                                            {
                                                _perf.instance = _perf_file[i].Split('=').Last();
                                            }
                                            if (_perf_file[i].Contains("PERS_MetricName"))
                                            {
                                                _perf.counter_name = _perf_file[i].Split('=').Last();
                                            }
                                            if (_perf_file[i].Contains("PERD_CounterTime"))
                                            {
                                                DateTime _timestamp = _utc_time.ToUniversalTime();
                                                _perf.timestamp = new DateTime(_utc_time.Year, _timestamp.Month, _timestamp.Day, _timestamp.Hour, 0, 0);
                                            }
                                            if (_perf_file[i].Contains("PERD_CounterAvg"))
                                            {
                                                _perf.value = Double.Parse(_perf_file[i].Split('=').Last());
                                            }
                                        }
                                        if (_valid_perf)
                                        {
                                            _perf.workload_id = _workload.id;
                                            _perf.id = Objects.RamdomGuid();
                                            if (_perf.counter_name == "Disk Bytes Out/sec")
                                            {
                                                _perf.counter_name = "Disk Write Bytes/sec";
                                            }
                                            if (_perf.counter_name == "Disk Bytes In/sec")
                                            {
                                                _perf.counter_name = "Disk Read Bytes/sec";
                                            }
                                            using (PerformanceSet performance_db_set = new PerformanceSet())
                                            {
                                                performance_db_set.ModelRepository.Insert(_perf);
                                            }
                                            if (_perf.category_name == "Memory" && _perf.counter_name == "Available Bytes")
                                            {
                                                if (_workload.vmemory != null && _workload.vmemory != 0)
                                                {
                                                    long _workload_total_memory = (Convert.ToInt64(_workload.vmemory) * 1024 * 1024 * 1024);
                                                    DateTime _timestamp = new DateTime(_utc_time.ToUniversalTime().Year, _utc_time.ToUniversalTime().Month, _utc_time.ToUniversalTime().Day, _utc_time.ToUniversalTime().Hour, 0, 0);

                                                    //memory: Used Bytes
                                                    Double _memory_used_bytes = _workload_total_memory - _perf.value;
                                                    String _memory_used_counter_name = "Used Bytes";

                                                    _perf = new Performance();
                                                    _perf.workload_id = _workload.id;
                                                    _perf.timestamp = _timestamp;
                                                    _perf.category_name = "Memory";
                                                    _perf.counter_name = _memory_used_counter_name;
                                                    _perf.instance = "_Total";
                                                    _perf.value = _memory_used_bytes;
                                                    _perf.id = Objects.RamdomGuid();
                                                    using (PerformanceSet performance_db_set = new PerformanceSet())
                                                    {
                                                        performance_db_set.ModelRepository.Insert(_perf);
                                                    }
                                                    //memory: % used
                                                    Double _percentage_memory_used = ((Convert.ToDouble(_memory_used_bytes) / Convert.ToDouble(_workload_total_memory)) * 100);
                                                    String _memory_counter_name = "% Used";

                                                    _perf = new Performance();
                                                    _perf.workload_id = _workload.id;
                                                    _perf.timestamp = _timestamp;
                                                    _perf.category_name = "Memory";
                                                    _perf.counter_name = _memory_counter_name;
                                                    _perf.instance = "_Total";
                                                    _perf.value = _percentage_memory_used;
                                                    _perf.id = Objects.RamdomGuid();
                                                    using (PerformanceSet performance_db_set = new PerformanceSet())
                                                    {
                                                        performance_db_set.ModelRepository.Insert(_perf);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Logger.log(String.Format("Performance: Completed performance collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}
