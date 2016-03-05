using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetFlowv9;
using System.Net.Sockets;
using System.Net;

namespace MRPService.MRPService.Classes.Background_Classes
{
    class NetflowWorkerv9
    {
        static public void Start()
        {

            TemplatesV9 _templates = new TemplatesV9();

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 9991);
            sock.Bind(iep);
            EndPoint ep = (EndPoint)iep;

            byte[] data = new byte[2048];

            while (true)
            {
                int recv = sock.ReceiveFrom(data, ref ep);
                byte[] bytes = new byte[recv];

                for (int i = 0; i < recv; i++)
                    bytes[i] = data[i];

                Packet packet = new Packet(bytes, _templates);

            }
        }
    }
}
