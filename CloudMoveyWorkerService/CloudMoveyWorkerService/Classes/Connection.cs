using CloudMoveyWorkerService.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Classes.Static_Classes
{
    class Connection
    {
        public static string find_working_ip_string(string iplist)
        {
            foreach (string ip in iplist.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                Ping pinger = new Ping();
                try
                {
                    PingReply reply = pinger.Send(ip);
                    if (reply.Status == IPStatus.Success)
                    {
                        return ip;
                    }
                }
                catch (PingException)
                {

                }
            }
            return null;
        }
        public static string find_working_ip_workload_literal(Workload _workload)
        {
            String ipaddresslist = _workload.iplist;
            String workingip = null;
            Ping testPing = new Ping();
            foreach (string ip in ipaddresslist.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                PingReply reply = testPing.Send(ip, 1000);
                if (reply != null)
                {
                    workingip = ip;
                    break;
                }
            }
            testPing.Dispose();

            //check for IPv6 address
            IPAddress _check_ip = IPAddress.Parse(workingip);
            if (_check_ip.AddressFamily.ToString() == System.Net.Sockets.AddressFamily.InterNetworkV6.ToString())
            {
                String _workingip = workingip;
                _workingip = _workingip.Replace(":", "-");
                _workingip = _workingip.Replace("%", "s");
                _workingip = _workingip + ".ipv6-literal.net";
                workingip = _workingip;
            }
            return workingip;
        }
        public static string find_working_ip_workload_normal(Workload _workload)
        {
            String ipaddresslist = _workload.iplist;
            String workingip = null;
            Ping testPing = new Ping();
            foreach (string ip in ipaddresslist.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                PingReply reply = testPing.Send(ip, 1000);
                if (reply != null)
                {
                    workingip = ip;
                    break;
                }
            }
            testPing.Dispose();
            return workingip;
        }
    }
}
