using System;
using System.Net;
using System.Net.NetworkInformation;

namespace MRMPService.Utilities
{
    class Connection : IDisposable
    {
        Ping testPing = new Ping();
        public string FindConnection(string iplist, bool literal = false)
        {
            String ipaddresslist = iplist;
            String workingip = null;
            String[] _iplist = ipaddresslist.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            Array.Reverse(_iplist);
            foreach (string ip in _iplist)
            {
                int retry = 3;
                PingReply reply = null;
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

            if (literal == true && workingip != null)
            {
                //check for IPv6 address
                //Check if the string is a hostname or a IP Address

                IPAddress _check_ip;
                if (IPAddress.TryParse(workingip, out _check_ip))
                {
                    if (_check_ip.AddressFamily.ToString() == System.Net.Sockets.AddressFamily.InterNetworkV6.ToString())
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
