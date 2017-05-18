using MRMPService.MRMPService.Log;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace MRMPService.Utilities
{
    class Connection : IDisposable
    {
        Ping testPing = new Ping();
        public string FindConnection(string iplist, bool literal = false, AddressFamily[] __ip_type = null)
        {
            String workingip = null;
            AddressFamily[] _ip_type = __ip_type == null ? (new AddressFamily[] { AddressFamily.InterNetwork, AddressFamily.InterNetworkV6 }) : __ip_type;
            String[] _iplist = iplist.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            Array.Reverse(_iplist);
            foreach (string ip in _iplist)
            {
                PingReply reply = null;
                if (_ip_type.Contains(IPAddress.Parse(ip).AddressFamily))
                {
                    int retry = 3;
                    while (retry > 0)
                    {
                        retry--;
                        try
                        {
                            reply = testPing.Send(ip);
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException(String.Format("Ping Error: {0}", ex.GetBaseException().Message));
                        }
                        if (reply != null)
                        {
                            if (reply.Status == IPStatus.Success)
                            {
                                workingip = ip;
                                break;
                            }
                        }
                    }
                    if (reply.Status == IPStatus.Success)
                    {
                        break;
                    }
                }

            }

            if (literal == true && workingip != null)
            {
                IPAddress _check_ip;
                if (IPAddress.TryParse(workingip, out _check_ip))
                {
                    if (_check_ip.AddressFamily.ToString() == AddressFamily.InterNetworkV6.ToString())
                    {
                        String _workingip = workingip;
                        _workingip = _workingip.Replace(":", "-");
                        _workingip = _workingip.Replace("%", "s");
                        _workingip = _workingip + ".ipv6-literal.net";
                        workingip = _workingip;
                    }

                }
            }
            return workingip;

        }
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    testPing.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
    }
}
