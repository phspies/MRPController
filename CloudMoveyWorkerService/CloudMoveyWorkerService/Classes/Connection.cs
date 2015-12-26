﻿using CloudMoveyWorkerService.LocalDatabase;
using System;
using System.Net;
using System.Net.NetworkInformation;

namespace CloudMoveyWorkerService.CloudMovey.Classes.Static_Classes
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
                PingReply reply = testPing.Send(ip, 1000);
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
