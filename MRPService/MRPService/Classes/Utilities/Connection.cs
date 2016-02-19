using MRPService.MRPService.Log;
using MRPService.LocalDatabase;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace MRPService.Utilities
{
    class Connection
    {
        public static string find_working_ip(string iplist, bool literal = false)
        {
            String ipaddresslist = iplist;
            String workingip = null;
            Ping testPing = new Ping();
            foreach (string ip in ipaddresslist.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                PingReply reply = null;
                try {
                    reply = testPing.Send(ip, 1000);
                }
                catch (Exception ex) { }
                if (reply != null)
                {
                    workingip = ip;
                    break;
                }
            }
            testPing.Dispose();

            if (literal == true)
            {
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
            }
            return workingip;
        }
        public static string find_working_ip(Workload _workload, bool literal=false)
        {
            String ipaddresslist = _workload.iplist;
            String workingip = null;
            bool foundip = false;
            Ping testPing = new Ping();
            foreach (string ip in ipaddresslist.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                while (true)
                {
                    try {
                        PingReply reply = testPing.Send(ip, 1000);
                        if (reply != null)
                        {
                            workingip = ip;
                            foundip = true;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.log(ex.Message, Logger.Severity.Error);
                        Thread.Sleep(new TimeSpan(0, 0, 30));
                    }
                }
                if (foundip == true)
                {
                    break;
                }
            }
            testPing.Dispose();

            if (literal == true)
            {
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
            }
            return workingip;
        }
    }
}
