using MRMPService.Utilities;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace linux_code
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionInfo ConnNfo = new ConnectionInfo("192.168.0.100", 22, "root",
                new AuthenticationMethod[] { new PasswordAuthenticationMethod("root", "c0mp2q") }
            );

            string localpath = @"C:\Users\phillip.spies\Documents\MRMP\Scripts";
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
                    sftp.CreateDirectory(remotepath);
                    sftp.ChangeDirectory(remotepath);
                }
                if (!remotepath_exist)
                {
                    foreach (String _file in Directory.GetFiles(localpath))
                    {
                        var fileStream = new FileStream(_file, FileMode.Open);
                        if (fileStream != null)
                        {
                            //If you have a folder located at sftp://ftp.example.com/share
                            //then you can add this like:
                            sftp.UploadFile(fileStream, Path.GetFileName(_file), null);
                        }
                    }
                }
                else
                {
                    if (sftp.Exists("output"))
                    {
                        foreach (SftpFile _file in sftp.ListDirectory("output"))
                        {
                            if (_file.Name.Contains("Perf_"))
                            {
                                //get OS edition name
                                List<String> _perf_file = sftp.ReadAllLines(_file.FullName).ToList();

                                //Process Disks
                                for (var i = 0; i < _perf_file.Count; i++)
                                {
                                    if (_perf_file[i] == "<PERF>")
                                    {
                                        bool _valid_perf = false;
                                        while (_perf_file[i] != "</PERF>")
                                        {
                                            i++;
                                            if (_perf_file[i].Contains("PERS_ClassName"))
                                            {
                                                string _class = _perf_file[i].Split('=').Last();
                                                _valid_perf = (_class == "Process") ? false : true;
                                            }
                                            if (_perf_file[i].Contains("PERS_InstanceName"))
                                            {
                                                string _instance = _perf_file[i].Split('=').Last();
                                            }
                                            if (_perf_file[i].Contains("PERS_MetricName"))
                                            {
                                                string _metric = _perf_file[i].Split('=').Last();
                                            }
                                            if (_perf_file[i].Contains("PERD_CounterTime"))
                                            {
                                                string _countertime = _perf_file[i].Split('=').Last();
                                            }
                                            if (_perf_file[i].Contains("PERD_CounterMax"))
                                            {
                                                decimal _value = Decimal.Parse(_perf_file[i].Split('=').Last());
                                            }
                                        }
                                        if (_valid_perf)
                                        {
                                            
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                sftp.Disconnect();
            }


            // Execute a (SHELL) Command - prepare upload directory
            using (var sshclient = new SshClient(ConnNfo))
            {
                sshclient.Connect();
                using (var cmd = sshclient.CreateCommand(String.Format("ls -al %1/Perf*", Path.Combine(remotepath, "output"))))
                {
                    cmd.Execute();
                    Console.WriteLine("Command>" + cmd.Result);
                    Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                    Console.ReadKey();

                }
                sshclient.Disconnect();
            }
        }
    }

}
