using MRPService.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Netstat
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create connection object to remote machine
            ConnectionOptions connOptions = new ConnectionOptions();
            connOptions.Impersonation = ImpersonationLevel.Impersonate;
            connOptions.Authentication = AuthenticationLevel.Default;
            connOptions.EnablePrivileges = true;
            //connOptions.Username = @".\Administrator";
            //connOptions.Password = @"Zaq1@wsx";

            ManagementScope scope = new ManagementScope(@"\\192.168.0.11\root\CIMV2", connOptions);
            scope.Connect();

            string remoteFile = @"\\192.168.0.11\c$\netstat.out";
            string netstatCmd = @"cmd.exe /c netstat -a -n -o > C:\netstat.out";
            Dictionary<string, string> netstatCmdParams = new Dictionary<string, string>();
            netstatCmdParams["CommandLine"] = netstatCmd;


            ManagementPath wmiObjectPath = new ManagementPath("Win32_Process");
            ObjectGetOptions ogo = new ObjectGetOptions();
            ManagementBaseObject returnValue;

            using (ManagementClass mc = new ManagementClass(scope, wmiObjectPath, ogo))
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
            foreach (PropertyData p in returnValue.Properties)
                Console.WriteLine(p.Name + " : " + p.Value);

            int processId = 0;
            if (returnValue != null)
            {
                processId = Convert.ToInt32(returnValue.Properties["ProcessId"].Value);
            }

            if (processId == 0)
            {
                return;
            }
            //Wait for the process to complete
            Process process = new Process();

            while (true)
            {
                try
                {
                    process = Process.GetProcessById(processId, "127.0.0.1");
                }
                catch (Exception)
                {
                    break;
                }
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }

            List<ProcessInfo> _processes = new List<ProcessInfo>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM Win32_Process"));

            foreach (ManagementObject queryObj in searcher.Get())
            {
                try
                {
                    Console.WriteLine(queryObj["Name"].ToString());
                    Console.WriteLine(queryObj["CommandLine"].ToString());
                    Console.WriteLine(queryObj["ProcessId"].ToString());

                    _processes.Add(new ProcessInfo() { name = queryObj["Name"].ToString(), command = queryObj["CommandLine"].ToString(), pid = Int16.Parse(queryObj["ProcessId"].ToString()) });
                }
                catch (Exception ex) { }

            }



            List<String> netstatList;
            using (new Impersonator("administrator", ".", "Zaq1@wsx"))
            {
                var logFile = File.ReadAllLines(remoteFile);
                netstatList = new List<String>(logFile);

            }
            var _netstatinfo = new List<NetstatInfo>();

            foreach (string row in netstatList)
            {
                //Split it baby
                string[] tokens = row.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 4 && (tokens[0].Equals("UDP") || tokens[0].Equals("TCP")))
                {
                    if (tokens[3] == "ESTABLISHED" || tokens[3] == "CLOSE_WAIT")
                    {

                        int _pid = tokens[1] == "UDP" ? Int16.Parse(tokens[3]) : Int16.Parse(tokens[4]);
                        if (_processes.FirstOrDefault(x => x.pid == _pid) != null && tokens[2].Split(':')[0] != "0.0.0.0" && tokens[2].Split(':')[0].ToString() != "127.0.0.1")
                        {
                            _netstatinfo.Add(new NetstatInfo()
                            {
                                proto = tokens[0],
                                pid = _pid,
                                process = _processes.FirstOrDefault(x => x.pid == _pid).name,
                                source_ip = IPSplit.Parse(tokens[1]).Address.ToString(),
                                source_port = IPSplit.Parse(tokens[1]).Port.ToString(),
                                target_ip = IPSplit.Parse(tokens[2]).Address.ToString(),
                                target_port = IPSplit.Parse(tokens[2]).Port.ToString(),
                                state = tokens[3]

                            });
                        }
                    }
                }
            }
            Console.ReadKey();
            //for (int i = 4; i < netstatList.Count; i++)
            //{
            //    string list = netstatList[i];
            //    string[] seperated = list.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            //    if (seperated.ToList().Count() == 5)
            //    {
            //        string command = netstatList[i + 1];
            //        NetstatInfo _netstat = new NetstatInfo(seperated, command);
            //    }
            //}

        }
    }
    public class NetstatInfo
    {
        public string proto { get; set; }
        public string source_ip { get; set; }
        public string target_ip { get; set; }
        public string source_port { get; set; }
        public string target_port { get; set; }
        public string state { get; set; }
        public int pid { get; set; }
        public string process { get; set; }
    }
    public class Port
    {
        public string name
        {
            get
            {
                return string.Format("{0} ({1} port {2})", this.process_name, this.protocol, this.port_number);
            }
            set { }
        }
        public string port_number { get; set; }
        public string process_name { get; set; }
        public string protocol { get; set; }
    }
    public class ProcessInfo
    {
        public int pid { get; set; }
        public string command { get; set; }
        public string name { get; set; }
    }


}
