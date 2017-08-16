using MRMPService.MRMPService.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using MRMPService.Utilities;
using MRMPService.Modules.MRMPPortal.Contracts;
using Renci.SshNet.Common;
using Renci.SshNet;
using System.IO;
using Renci.SshNet.Sftp;
using MRMPService.LocalDatabase;
using System.Globalization;

namespace MRMPService.Scheduler.PerformanceCollection
{
    partial class WorkloadPerformance
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
        public static void WorkloadPerformanceUnixDo(MRMPWorkloadBaseType _workload)
        {
            List<PerformanceType> _workload_counters = new List<PerformanceType>();

            #region load and check workload information
            //check for credentials
            MRPCredentialType _credential = _workload.GetCredentials();
            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Error finding credentials"));
            }
            _password = _credential.decrypted_password;

            //check for working IP
            string workload_ip = _workload.GetContactibleIP(true);
            #endregion

            Logger.log(String.Format("Performance: Start Performance collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
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
                    Logger.log(String.Format("MRMP scripts are not present on workload {0} using {1}. Inventory process needs to setup scripts first!", _workload.hostname, workload_ip), Logger.Severity.Warn);
                    throw new Exception(String.Format("MRMP scripts are not present on workload {0} using {1}. Inventory process needs to setup scripts first!", _workload.hostname, workload_ip));
                }
                if (remotepath_exist)
                {
                    if (sftp.Exists("output"))
                    {
                        int _file_count = 0;
                        foreach (SftpFile _file in sftp.ListDirectory("output"))
                        {
                            if (_file.Name.StartsWith("Perf_"))
                            {
                                _file_count += 1;
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
                                        Performancecounter _perf = new Performancecounter();
                                        bool _valid_perf = false;
                                        while (_perf_file[i] != "</PERF>")
                                        {
                                            i++;
                                            if (_perf_file[i].Contains("PERS_ClassName"))
                                            {
                                                _perf.category = _perf_file[i].Split('=').Last();
                                                _valid_perf = (_perf.category == "Process") ? false : true;
                                            }
                                            if (_perf_file[i].Contains("PERS_InstanceName"))
                                            {
                                                _perf.instance = _perf_file[i].Split('=').Last();
                                            }
                                            if (_perf_file[i].Contains("PERS_MetricName"))
                                            {
                                                _perf.counter = _perf_file[i].Split('=').Last();
                                            }
                                            if (_perf_file[i].Contains("PERD_CounterTime"))
                                            {
                                                DateTime _timestamp = _utc_time.ToUniversalTime();
                                                _perf.timestamp = new DateTime(_utc_time.Year, _timestamp.Month, _timestamp.Day, _timestamp.Hour, 0, 0);
                                            }
                                            if (_perf_file[i].Contains("PERD_CounterAvg"))
                                            {
                                                Double _temp_value;
                                                string _value_string = _perf_file[i].Split('=').Last().ToString();
                                                if (Double.TryParse(_value_string,NumberStyles.Float, CultureInfo.InvariantCulture, out _temp_value))
                                                {
                                                    _perf.value = _temp_value;
                                                }
                                                else
                                                {
                                                    Logger.log(String.Format("Cannot parse {0} for classname {1} and instance {2}", _value_string, _perf.category, _perf.instance), Logger.Severity.Warn);
                                                    _perf.value = 0.0;
                                                }
                                            }
                                        }
                                        if (_valid_perf)
                                        {
                                            _perf.workload_id = _workload.id;
                                            _perf.id = Objects.RamdomGuid();
                                            if (_perf.counter == "Disk Bytes Out/sec")
                                            {
                                                _perf.counter = "Disk Write Bytes/sec";
                                            }
                                            if (_perf.counter == "Disk Bytes In/sec")
                                            {
                                                _perf.counter = "Disk Read Bytes/sec";
                                            }
                                            using (PerformancecounterSet _perf_db = new PerformancecounterSet())
                                            {
                                                _perf_db.ModelRepository.Insert(_perf);
                                            }

                                            if (_perf.category == "Memory" && _perf.counter == "Available Bytes")
                                            {
                                                if (_workload.vmemory != null && _workload.vmemory != 0)
                                                {
                                                    long _workload_total_memory = (Convert.ToInt64(_workload.vmemory) * 1024 * 1024 * 1024);
                                                    DateTime _timestamp = new DateTime(_utc_time.ToUniversalTime().Year, _utc_time.ToUniversalTime().Month, _utc_time.ToUniversalTime().Day, _utc_time.ToUniversalTime().Hour, 0, 0);

                                                    //memory: Used Bytes
                                                    Double _memory_used_bytes = _workload_total_memory - _perf.value;
                                                    String _memory_used_counter = "Used Bytes";

                                                    _perf = new Performancecounter();
                                                    _perf.workload_id = _workload.id;
                                                    _perf.timestamp = _timestamp;
                                                    _perf.category = "Memory";
                                                    _perf.counter = _memory_used_counter;
                                                    _perf.instance = "_Total";
                                                    _perf.value = _memory_used_bytes;
                                                    _perf.id = Objects.RamdomGuid();
                                                    using (PerformancecounterSet _perf_db = new PerformancecounterSet())
                                                    {
                                                        _perf_db.ModelRepository.Insert(_perf);
                                                    }
                                                    //memory: % used
                                                    Double _percentage_memory_used = ((Convert.ToDouble(_memory_used_bytes) / Convert.ToDouble(_workload_total_memory)) * 100);
                                                    String _memory_counter = "% Used";

                                                    _perf = new Performancecounter();
                                                    _perf.workload_id = _workload.id;
                                                    _perf.timestamp = _timestamp;
                                                    _perf.category = "Memory";
                                                    _perf.counter = _memory_counter;
                                                    _perf.instance = "_Total";
                                                    _perf.value = _percentage_memory_used;
                                                    _perf.id = Objects.RamdomGuid();
                                                    using (PerformancecounterSet _perf_db = new PerformancecounterSet())
                                                    {
                                                        _perf_db.ModelRepository.Insert(_perf);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //network totals
                                foreach (var _counter in new string[] { "Bytes Received/sec", "Bytes Sent/sec" })
                                {
                                    Performancecounter _perf = new Performancecounter();
                                    _perf.workload_id = _workload.id;
                                    _perf.timestamp = _utc_time;
                                    _perf.category = "Network Interface";
                                    _perf.counter = _counter;
                                    _perf.instance = "_Total";
                                    _perf.id = Objects.RamdomGuid();
                                    using (PerformancecounterSet _perf_db = new PerformancecounterSet())
                                    {
                                        _perf.value = _perf_db.ModelRepository.Get(x => x.category == "Network Interface" && x.counter == _counter && x.instance != "_Total" && x.instance != "lo" && x.timestamp == _utc_time).AsEnumerable().Sum(x => x.value);
                                        _perf_db.ModelRepository.Insert(_perf);
                                    }
                                }
                            }

                        }
                        if (_file_count == 0)
                        {
                            throw new Exception(String.Format("No performance files found on {0} {1}.", _workload.hostname, workload_ip));
                        }
                    }
                    else
                    {
                        throw new Exception(String.Format("Output folder not found on {0} {1}.", _workload.hostname, workload_ip));
                    }
                }

            }
            Logger.log(String.Format("Performance: Completed PerformanceType collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}
