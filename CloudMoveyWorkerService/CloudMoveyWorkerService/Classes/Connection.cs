using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Classes.Static_Classes
{
    class Connection
    {
        public static string find_working_ip(string iplist)
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
    }
}
